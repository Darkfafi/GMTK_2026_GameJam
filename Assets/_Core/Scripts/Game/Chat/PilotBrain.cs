using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK_2026
{
	public class BrainResult
	{
		public enum ResultKind
		{
			Answer,
			Clarify,
			Fallback,
		}

		public ResultKind Kind;
		public readonly List<ChatTopic> Topics = new List<ChatTopic>();
		public bool AsksAboutUnknown;
	}

	public class PilotBrain
	{
		private enum Subject { Pilot, Ship, Planet }
		private enum Attr { Kind, Name }

		private readonly PilotPersona _persona;
		private Dictionary<Subject, ChatTopic> _pendingMap;
		private Subject? _lastSubject;
		private int _fails;

		public PilotBrain(PilotPersona persona)
		{
			_persona = persona;
		}

		private static readonly Dictionary<ChatTopic, string[]> Phrases = new Dictionary<ChatTopic, string[]>
		{
			{ ChatTopic.Name, new[] { "your name", "who are you", "whats your name", "what's your name", "what is your name", "identify yourself", "call you" } },
			{ ChatTopic.Species, new[] { "species", "what are you", "your kind", "your race", "are you human", "are you alien", "life form", "lifeform", "what creature", "biological" } },
			{ ChatTopic.Occupation, new[] { "what do you do", "your job", "occupation", "profession", "work as", "your role", "your position" } },
			{ ChatTopic.Destination, new[] { "where are you going", "your destination", "what planet", "which planet", "where to", "landing on", "going to", "heading", "headed", "destination", "land where", "where do you need" } },
			{ ChatTopic.Body, new[] { "kind of planet", "type of planet", "what body", "body type", "kind of world", "type of world", "describe the planet", "planet like", "what is the planet", "whats the planet", "what's the planet", "celestial" } },
			{ ChatTopic.ShipName, new[] { "ship name", "vessel name", "ship called", "name of your ship", "what ship", "what vessel", "callsign", "call sign" } },
			{ ChatTopic.Needs, new[] { "what do you need", "need to survive", "need to live", "breathe", "breath", "oxygen", "atmosphere do you", "survive on", "requirements", "what keeps you alive", "environment do you need" } },
			{ ChatTopic.Health, new[] { "how are you", "are you okay", "are you alright", "your condition", "are you hurt", "injured", "how do you feel" } },
			{ ChatTopic.Purpose, new[] { "why do you need", "purpose", "reason", "what for", "why are you going", "what brings you", "business", "why this planet" } },
			{ ChatTopic.Greeting, new[] { "hello", "hi ", "hey", "greetings", "good morning", "good evening", "howdy" } },
			{ ChatTopic.Rude, new[] { "shut up", "stupid", "idiot", "moron", "dumb", "useless" } },
			{ ChatTopic.Hurry, new[] { "hurry", "quick", "be fast", "speed up", "faster", "time is running" } },
			{ ChatTopic.Goodbye, new[] { "bye", "goodbye", "done", "thats all", "that's all", "nothing else", "were done", "we're done" } },
		};

		private static readonly Dictionary<Subject, string[]> SubjectWords = new Dictionary<Subject, string[]>
		{
			{ Subject.Ship, new[] { "ship", "ships", "vessel", "vessels", "craft", "freighter", "shuttle", "boat" } },
			{ Subject.Planet, new[] { "planet", "world", "destination", "moon", "body", "there" } },
			{ Subject.Pilot, new[] { "you", "your", "yourself", "u", "pilot", "captain" } },
		};

		private static readonly Dictionary<Attr, Dictionary<Subject, ChatTopic>> AttrMap = new Dictionary<Attr, Dictionary<Subject, ChatTopic>>
		{
			{ Attr.Kind, new Dictionary<Subject, ChatTopic> { { Subject.Pilot, ChatTopic.Species }, { Subject.Planet, ChatTopic.Body }, { Subject.Ship, ChatTopic.ShipName } } },
			{ Attr.Name, new Dictionary<Subject, ChatTopic> { { Subject.Pilot, ChatTopic.Name }, { Subject.Planet, ChatTopic.Destination }, { Subject.Ship, ChatTopic.ShipName } } },
		};

		private static readonly Dictionary<Attr, string[]> AttrNeutralWords = new Dictionary<Attr, string[]>
		{
			{ Attr.Kind, new[] { "kind", "type", "class", "sort" } },
			{ Attr.Name, new[] { "name", "called", "call" } },
		};

		private static readonly Dictionary<ChatTopic, Subject> TopicSubject = new Dictionary<ChatTopic, Subject>
		{
			{ ChatTopic.Name, Subject.Pilot }, { ChatTopic.Species, Subject.Pilot }, { ChatTopic.Occupation, Subject.Pilot },
			{ ChatTopic.Needs, Subject.Pilot }, { ChatTopic.Health, Subject.Pilot },
			{ ChatTopic.ShipName, Subject.Ship },
			{ ChatTopic.Destination, Subject.Planet }, { ChatTopic.Body, Subject.Planet },
		};

		public BrainResult Interpret(string message)
		{
			string low = message.ToLowerInvariant().Trim();
			Dictionary<Subject, ChatTopic> pending = _pendingMap;
			_pendingMap = null;

			BrainResult result = new BrainResult();
			List<(ChatTopic topic, int score)> sorted = ScoreTopics(low);

			if (pending != null && sorted.Count == 0)
			{
				Subject? subject = DetectSubject(low);
				if (subject.HasValue && pending.TryGetValue(subject.Value, out ChatTopic pendingTopic))
				{
					return Answer(result, pendingTopic);
				}
			}

			if (sorted.Count > 0)
			{
				if (sorted.Count > 1 && DetectSubject(low) == null)
				{
					Attr? attr = AmbiguousPair(sorted[0].topic, sorted[1].topic);
					if (attr.HasValue && sorted[1].score >= sorted[0].score * 0.6f)
					{
						_pendingMap = AttrMap[attr.Value];
						result.Kind = BrainResult.ResultKind.Clarify;
						_fails = 0;
						return result;
					}
				}

				Answer(result, sorted[0].topic);
				if (sorted.Count > 1 && sorted[1].score >= sorted[0].score * 0.4f && AmbiguousPair(sorted[0].topic, sorted[1].topic) == null)
				{
					Answer(result, sorted[1].topic);
				}
				return result;
			}

			foreach (KeyValuePair<Attr, string[]> pair in AttrNeutralWords)
			{
				if (!pair.Value.Any(low.Contains))
				{
					continue;
				}

				Subject? subject = DetectSubject(low) ?? _lastSubject;
				if (subject.HasValue && AttrMap[pair.Key].TryGetValue(subject.Value, out ChatTopic topic))
				{
					return Answer(result, topic);
				}

				_pendingMap = AttrMap[pair.Key];
				result.Kind = BrainResult.ResultKind.Clarify;
				_fails = 0;
				return result;
			}

			_fails++;
			result.Kind = BrainResult.ResultKind.Fallback;
			return result;
		}

		private BrainResult Answer(BrainResult result, ChatTopic topic)
		{
			result.Kind = BrainResult.ResultKind.Answer;
			result.Topics.Add(topic);
			if (TopicSubject.TryGetValue(topic, out Subject subject))
			{
				_lastSubject = subject;
			}
			if (_persona.GetKnowledge(topic) <= 0f && !IsDirect(topic))
			{
				result.AsksAboutUnknown = true;
			}
			_fails = 0;
			return result;
		}

		private static List<(ChatTopic, int)> ScoreTopics(string low)
		{
			var scores = new List<(ChatTopic, int)>();
			int words = low.Split(' ').Count(w => w.Length > 0);
			if (words < 2 && !low.EndsWith("?"))
			{
				return scores;
			}

			foreach (KeyValuePair<ChatTopic, string[]> pair in Phrases)
			{
				int score = pair.Value.Where(low.Contains).Sum(p => p.Length);
				if (score > 0)
				{
					scores.Add((pair.Key, score));
				}
			}
			return scores.OrderByDescending(s => s.Item2).ToList();
		}

		private static Subject? DetectSubject(string low)
		{
			string[] words = new string(low.Select(c => char.IsLetterOrDigit(c) ? c : ' ').ToArray())
				.Split(' ').Where(w => w.Length > 0).ToArray();

			// Ship/planet win over pilot: "your ship" contains both.
			if (words.Any(w => SubjectWords[Subject.Ship].Contains(w))) return Subject.Ship;
			if (words.Any(w => SubjectWords[Subject.Planet].Contains(w))) return Subject.Planet;
			if (words.Any(w => SubjectWords[Subject.Pilot].Contains(w))) return Subject.Pilot;
			return null;
		}

		private static Attr? AmbiguousPair(ChatTopic a, ChatTopic b)
		{
			bool Has(ChatTopic x, ChatTopic y) => (a == x && b == y) || (a == y && b == x);
			if (Has(ChatTopic.Species, ChatTopic.Body)) return Attr.Kind;
			if (Has(ChatTopic.Name, ChatTopic.ShipName) || Has(ChatTopic.Name, ChatTopic.Destination)) return Attr.Name;
			return null;
		}

		public string GetClarifyLine()
			=> Pick(
				"I don't follow — are you asking about me, my ship, or the planet?",
				"You mean me, the vessel, or where I'm headed?",
				"Which one — me, the ship, or the planet?");

		public string GetFallbackLine()
		{
			if (_fails >= 2)
			{
				return Pick(
					"I'm not following. You can ask about me, my ship, or where I'm headed.",
					"I don't understand. Try asking who I am, what I am, or what kind of place I'm landing on.");
			}
			return Pick("I'm not sure what you're asking.", "Could you rephrase that?", "I don't follow.", "Hmm? What do you mean?", "Can you be more specific?");
		}

		public string GenerateResponse(ChatTopic topic)
		{
			LandingPilotRequest request = _persona.Request;

			switch (topic)
			{
				case ChatTopic.Greeting:
					return Pick("Hello, operator.", "Hi there.", "Greetings.", "...hello.");
				case ChatTopic.Rude:
					return _persona.Cooperation > 0.6f
						? Pick("I understand you're busy, but...", "There's no need for that.", "I'm trying to cooperate here.")
						: Pick("Excuse me?", "I don't appreciate that tone.", "...fine.", "Maybe I should hail a different station.");
				case ChatTopic.Hurry:
					return Pick("I'm going as fast as I can.", "I understand the urgency.", "I'm trying...");
				case ChatTopic.Goodbye:
					return Pick("Wait — can I land or not?", "So... what's the decision?", "I still need an answer.", "Don't leave me hanging.");
				case ChatTopic.Health:
					return Pick("I'm fine, just need to land.", "All systems nominal.", "I'm okay. The journey was long.");
				case ChatTopic.Purpose:
					return _persona.Cooperation > 0.5f
						? Pick("Routine run. Nothing special.", "Just work. Deliveries, surveys, that kind of thing.", "I have business at the colony.")
						: Pick("That's my concern.", "Does it matter?", "I have my reasons.");

				case ChatTopic.Name:
					return ValueAnswer(topic, request.Pilot?.Name,
						dont: "I... don't remember my name right now.");
				case ChatTopic.ShipName:
					return ValueAnswer(topic, request.Ship?.Name,
						dont: "I never caught the vessel's name. It's just... my ship.");
				case ChatTopic.Destination:
					return ValueAnswer(topic, request.Target?.Name,
						dont: "The nav computer knows. I don't, honestly.");

				case ChatTopic.Species:
					SpeciesAspect species = FindAspect<SpeciesAspect>(request.Pilot);
					return ValueAnswer(topic, species?.Name,
						dont: DescribeClue(species?.Description, "I'm... not sure what you'd call me."));
				case ChatTopic.Body:
					CelestialBodyAspect body = FindAspect<CelestialBodyAspect>(request.Target);
					return ValueAnswer(topic, body?.Name,
						dont: DescribeClue(body?.Description, "I can't name the type, but I can see it from here."));
				case ChatTopic.Occupation:
					OccupationAspect occupation = FindAspect<OccupationAspect>(request.Pilot);
					return ValueAnswer(topic, occupation?.Name,
						dont: DescribeClue(occupation?.Description, "I just... fly. That's all I know."));

				case ChatTopic.Needs:
					return NeedsAnswer(request);

				default:
					return "...";
			}
		}

		private string NeedsAnswer(LandingPilotRequest request)
		{
			CreatureEntity pilot = request.Pilot;
			if (pilot == null)
			{
				return "...";
			}

			float knowledge = _persona.GetKnowledge(ChatTopic.Needs);
			string requires = string.Join(", ", pilot.Requires.Select(t => t.Name));
			string fatal = string.Join(", ", pilot.Intolerances.Select(t => t.Name));

			if (knowledge >= 1f)
			{
				string answer = requires.Length > 0 ? $"I need {requires} to survive." : "I don't need much to survive.";
				if (fatal.Length > 0 && Random.value < _persona.Cooperation)
				{
					answer += $" And keep me away from {fatal} — that kills my kind.";
				}
				return answer;
			}
			if (knowledge >= 0.5f)
			{
				return requires.Length > 0
					? $"{requires}... I think? I've never had to think about it."
					: "I honestly don't know what keeps me alive.";
			}
			return Pick(
				"I don't know the specifics of my own biology. Check my species in your records.",
				"Couldn't tell you. Whatever my kind needs, I need.");
		}

		private string ValueAnswer(ChatTopic topic, string value, string dont)
		{
			float knowledge = _persona.GetKnowledge(topic);
			bool critical = PilotPersona.CriticalTopics.Contains(topic);

			if (knowledge >= 1f && !string.IsNullOrEmpty(value))
			{
				if (Random.value < _persona.Clarity)
				{
					return Pick($"{value}.", $"It's {value}.", $"{value}. That's the one.", $"You can put down {value}.");
				}
				return Pick($"I think it's {value}...", $"{value}? Yes. {value}.", $"...{value}. Pretty sure.");
			}

			if (knowledge >= 0.5f && !string.IsNullOrEmpty(value))
			{
				return Pick($"I think... {value}? Yeah.", $"Let me think... {value}, I believe.", $"Hmm, probably {value}.");
			}

			if (!critical && Random.value > _persona.Cooperation)
			{
				return Pick("Why do you ask?", "Does it matter?", "I'd rather not discuss that.", "Next question.");
			}
			return dont;
		}

		private static string DescribeClue(string description, string fallback)
		{
			if (string.IsNullOrEmpty(description))
			{
				return fallback;
			}
			return Pick(
				$"Hard to say. All I can tell you is this: {description.ToLowerInvariant().TrimEnd('.')}.",
				$"I can't give you the official term. {description}",
				$"No idea what your records call it. {description}");
		}

		private static bool IsDirect(ChatTopic topic)
			=> topic == ChatTopic.Greeting || topic == ChatTopic.Rude || topic == ChatTopic.Hurry
			|| topic == ChatTopic.Goodbye || topic == ChatTopic.Health || topic == ChatTopic.Purpose;

		private static T FindAspect<T>(GameEntityBase entity) where T : EntityAspect
		{
			if (entity == null)
			{
				return null;
			}
			for (int i = 0; i < entity.Aspects.Count; i++)
			{
				if (entity.Aspects[i] is T match)
				{
					return match;
				}
			}
			return null;
		}

		private static string Pick(params string[] options)
			=> options[Random.Range(0, options.Length)];
	}
}
