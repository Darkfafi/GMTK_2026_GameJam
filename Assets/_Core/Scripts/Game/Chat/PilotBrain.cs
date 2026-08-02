using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK_2026
{
	public class PilotBrain
	{
		// Knowledge / Personality
		private readonly PilotPersona _persona;

		// Clarification Required, If any of the subjects is given, we know the Topic
		// For Example: You asked about my Kind, but I don't know if its my kind of ship, or my kind of species..
		// Subject map: Pilot -> Species or Ship -> Ship Type
		private Dictionary<Subject, ChatTopic> _pendingMap;

		// What is the Ship Type? -> And the Name? 
		// Remembers you were talking about the Ship
		// 1 Subject at a time
		private Subject? _lastSubject;

		// Consequtive Fails -> Used to give tips after x tries
		private int _fails;

		public PilotBrain(PilotPersona persona)
		{
			_persona = persona;
		}

		public string GetIntroLine()
		{
			var pilot = _persona.Request.Pilot;
			var planet = _persona.Request.Target;
			var ship = _persona.Request.Ship;

			return Pick(
				$"Station Alpha, this is {pilot}. Requesting landing clearance on {planet}.",
				$"...hello? This is {pilot}. I need to land. {planet}, if possible.",
				$"Station Alpha, {pilot} here, flying the {ship}. Requesting permission to land on {planet}.",
				$"{pilot} to Station Alpha. Landing request for {planet}. Standing by.");
		}

		public string GetUnpromptedLine()
		{
			List<string> _unprompted = new List<string>
			{
				"Operator? Are you still there?",
				"I really need to get down there soon."
			};

			if (_persona.Nervousness > 0.5f)
			{
				_unprompted.Add("...");
				_unprompted.Add("Please, I just need to land.");
			}
			if (_persona.Cooperation < 0.5f)
			{
				_unprompted.Add("I don't have all day, operator.");
				_unprompted.Add("Is this going to take much longer?");
			}
			else
			{
				_unprompted.Add("Standing by for clearance.");
				_unprompted.Add("Let me know if you need anything else from me.");
			}

			return _unprompted[Random.Range(0, _unprompted.Count)];
		}

		public BrainResult Interpret(string message)
		{
			// Clean-up
			string normalizedMessage = message.ToLowerInvariant().Trim();
			Dictionary<Subject, ChatTopic> pending = _pendingMap;
			_pendingMap = null;

			BrainResult result = new BrainResult();

			// Score Topics
			List<(ChatTopic topic, int score)> sorted = PilotBrainTopic.ScoreTopics(normalizedMessage);

			// Pending Topic (What is the Name?) -> (Get Subject out of follow-up)
			if (pending != null && sorted.Count == 0)
			{
				Subject? subject = PilotBrainSubject.DetectSubject(normalizedMessage);
				if (subject.HasValue && pending.TryGetValue(subject.Value, out ChatTopic pendingTopic))
				{
					return AppendAnswer(result, pendingTopic);
				}
			}

			// If we detected Topics
			if (sorted.Count > 0)
			{
				// There was no Subject found while multiple Topics are found, we might need clarification
				if (sorted.Count > 1 && PilotBrainSubject.DetectSubject(normalizedMessage) == null)
				{
					// If Top 2 Topics are Ambiguous, then ask for Clarification, but only if
					// The 2nd Topic is at least 60% certainty score compared to the first
					// Hello Species, what is your name (what is your name > 60% than Species)
					// So then we skip clarification. We understand the main topic is the top one
					ChatAttribute? chatAttribute = PilotBrainAttribute.GetAmbiguousChatTopicAttribute(sorted[0].topic, sorted[1].topic);
					if (chatAttribute.HasValue && sorted[1].score >= sorted[0].score * 0.6f)
					{
						_pendingMap = PilotBrainAttribute.ChatAttributeToSubjectTopicPairMap[chatAttribute.Value];
						result.Kind = BrainResult.ResultKind.Clarify;
						_fails = 0;
						return result;
					}
				}

				// Answer Original Question
				AppendAnswer(result, sorted[0].topic);

				// Answer Second Non-ambiguous topic within the same sentence
				// Only if the second topic's certainty is scored at least 40% to the main topic.
				if (sorted.Count > 1 && sorted[1].score >= sorted[0].score * 0.4f && PilotBrainAttribute.GetAmbiguousChatTopicAttribute(sorted[0].topic, sorted[1].topic) == null)
				{
					AppendAnswer(result, sorted[1].topic);
				}
				return result;
			}

			// If no clear Topic was given in this message, we will use chat attributes
			foreach (KeyValuePair<ChatAttribute, string[]> attrToWordsPair in PilotBrainAttribute.ChatAttributeNeutralWordsMap)
			{
				// Was this Attribute used within the message?
				if (!attrToWordsPair.Value.Any(normalizedMessage.Contains))
				{
					continue;
				}

				// We try to determine the Subject in the message, or the subject last discussed
				// Last Prompt: What Ship do you have?
				// This Prompt: And Type?
				// (Knows you are asking about the Type of the Ship you just asked the name for due to _lastSubject) 
				// This also covers when the message is just 'Subject Attribute'
				Subject? subject = PilotBrainSubject.DetectSubject(normalizedMessage) ?? _lastSubject;
				if (subject.HasValue && PilotBrainAttribute.ChatAttributeToSubjectTopicPairMap[attrToWordsPair.Key].TryGetValue(subject.Value, out ChatTopic topic))
				{
					return AppendAnswer(result, topic);
				}

				// If no answer given, get Subject Topic Map for clarification s
				_pendingMap = PilotBrainAttribute.ChatAttributeToSubjectTopicPairMap[attrToWordsPair.Key];
				result.Kind = BrainResult.ResultKind.Clarify;
				_fails = 0;
				return result;
			}

			// Lol I have no idea what you are talking about dude.. x_x
			_fails++;
			result.Kind = BrainResult.ResultKind.Fallback;
			return result;
		}

		private BrainResult AppendAnswer(BrainResult result, ChatTopic topic)
		{
			result.Kind = BrainResult.ResultKind.Answer;
			result.Topics.Add(topic);
			if (PilotBrainTopic.TopicSubjectMap.TryGetValue(topic, out Subject subject))
			{
				_lastSubject = subject;
			}
			if (_persona.GetKnowledge(topic) <= 0f && !PilotBrainTopic.IsDirect(topic))
			{
				result.AsksAboutUnknown = true;
			}
			_fails = 0;
			return result;
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
					"I'm not following. Ask what I am, where I'm from, what gear I have, what I'm flying, or where I'm headed.",
					"I don't understand. Try: my species, my home world, my equipment, my ship class, or my destination.");
			}
			return Pick("I'm not sure what you're asking.", "Could you rephrase that?", "I don't follow.", "Hmm? What do you mean?", "Can you be more specific?");
		}

		public string GetAnswerLine(ChatTopic topic)
		{
			PilotRequestBase request = _persona.Request;

			switch (topic)
			{
				case ChatTopic.Greeting:
					if (_persona.Nervousness > 0.6f)
					{
						return Pick("...hello. Hello, yes.", "Hi. Hi, sorry — I'm a little on edge.", "Oh — hello, operator.");
					}
					return _persona.Cooperation > 0.5f
						? Pick("Hello, operator.", "Hi there.", "Greetings, Station Alpha.", "Hey. Good to hear a voice.")
						: Pick("Yeah. Hello.", "Hello. Can we get on with it?", "Greetings. Now, about my landing.");
				case ChatTopic.Rude:
					return _persona.Cooperation > 0.6f
						? Pick("I understand you're busy, but...", "There's no need for that.", "I'm trying to cooperate here.")
						: Pick("Excuse me?", "I don't appreciate that tone.", "...fine.", "Maybe I should hail a different station.");
				case ChatTopic.Hurry:
					return Pick("I'm going as fast as I can.", "I understand the urgency.", "I'm trying...");
				case ChatTopic.Goodbye:
					return Pick("Wait — can I land or not?", "So... what's the decision?", "I still need an answer.", "Don't leave me hanging.");
				case ChatTopic.Health:
					if (_persona.Nervousness > 0.6f)
					{
						return Pick(
							"Not great, honestly. I just want to be on the ground.",
							"I've been up here too long. I'm tired.",
							"Shaken. It's been a rough crossing.");
					}
					return _persona.Cooperation > 0.5f
						? Pick("I'm fine, just need to land.", "All systems nominal, thanks for asking.", "I'm well. The crossing was long, though.")
						: Pick("I'm fine. Can we move on?", "Does it matter how I am?", "Alive. Which is why I'd like to land.");
				case ChatTopic.Purpose:
					return _persona.Cooperation > 0.5f
						? Pick("Routine run. Nothing special.", "Just work. Deliveries, surveys, that kind of thing.", "I have business at the colony.")
						: Pick("That's my concern.", "Does it matter?", "I have my reasons.");

				case ChatTopic.Name:
					return GetAnswerLine(topic, request.Pilot?.Name,
						fallback: "I... don't remember my name right now.");
				case ChatTopic.ShipName:
					return GetAnswerLine(topic, request.Ship?.Name,
						fallback: "I never caught the vessel's name. It's just... my ship.");

				case ChatTopic.ShipClass:
					ShipAspect shipClass = request.Ship?.Class;
					string shipClassName = shipClass?.Name;
					return GetAnswerLine(topic, shipClassName,
						fallback: GetDescribeClueLine(shipClass?.Description,
							"No idea what class it is. It was the only one on the pad."));
				case ChatTopic.Destination:
					return GetAnswerLine(topic, request.Target?.Name,
						fallback: "The nav computer knows. I don't, honestly.");

				case ChatTopic.Species:
					SpeciesAspect species = request.Pilot?.Species;
					string speciesName = species?.Name;
					// A pilot who can't name their species still knows where they're from —
					// the planetary index lists each world's natives.
					return GetAnswerLine(topic, speciesName,
						fallback: species != null
							? Pick(
								$"I don't know what your records call us. I'm from {species.Origin}, if that helps.",
								$"Couldn't tell you the official term. My people are native to {species.Origin}.",
								$"No idea. Look up {species.Origin} — that's where I'm from.")
							: "I'm... not sure what you'd call me.");

				case ChatTopic.Origin:
					string originName = request.Pilot?.Species?.Origin;
					return GetAnswerLine(topic, originName,
						fallback: "I've been out here so long I couldn't tell you where I started.");

				case ChatTopic.Equipment:
					return GetEquipmentAnswerLine();

				case ChatTopic.Body:
					CelestialBodyAspect body = request.Target?.Body;
					return GetAnswerLine(topic, body?.Name,
						fallback: GetDescribeClueLine(body?.Description, "I can't name the type, but I can see it from here."));
				case ChatTopic.Occupation:
					OccupationAspect occupation = FindAspect<OccupationAspect>(request.Pilot);
					return GetAnswerLine(topic, occupation?.Name,
						fallback: GetDescribeClueLine(occupation?.Description, "I just... fly. That's all I know."));

				case ChatTopic.Needs:
					return GetNeedsAnswerLine();

				default:
					return "...";
			}
		}

		public string GetDecisionLine(PlayerChoice choice)
		{
			switch (choice)
			{
				case PlayerChoice.None:
					return Pick("...?", "Did you just...?", "Hey! I'm still here!", "No! Wait—");
				case PlayerChoice.Approved:
					return Pick("Thank you, operator.", "Copy that. Proceeding.", "Acknowledged. Good to go.", "Finally. Thank you.");
				case PlayerChoice.Denied:
					return Pick("...understood.", "Are you sure about that?", "You can't be serious.", "Fine. I'll find another way.");
			}

			return string.Empty;
		}

		private string GetEquipmentAnswerLine()
		{
			List<EquipmentAspect> carried = _persona.Request.Pilot?.Equipment;
			if (carried == null || carried.Count == 0)
			{
				return Pick(
					"Nothing. Just me and the ship.",
					"No gear. I wasn't expecting to need any.",
					"I'm travelling bare — no suit, no rig.");
			}

			string names = string.Join(" and ", carried.Select(e => e.Name));

			if (_persona.GetKnowledge(ChatTopic.Equipment) >= 1f)
			{
				return Pick(
					$"I have a {names}.",
					$"I'm carrying a {names}.",
					$"{names}. That's everything I've got aboard.");
			}

			// Vague: they can describe the thing but not name it.
			string described = string.Join(" and ", carried
				.Select(e => e.Description.ToLowerInvariant().TrimEnd('.')));
			return Pick(
				$"There's a rig in the locker — {described}. I don't know what it's called.",
				$"Something like this: {described}. Never read the label.");
		}

		private string GetNeedsAnswerLine()
		{
			CreatureEntity pilot = _persona.Request.Pilot;
			if (pilot == null)
			{
				return "...";
			}

			float knowledge = _persona.GetKnowledge(ChatTopic.Needs);
			string requires = string.Join(", ", pilot.Requires.Select(t => t.Name));

			if (knowledge >= 1f)
			{
				// They know what they consume, not the exact pressure/gravity/heat bands —
				// those live in the species registry, which keeps the research loop intact.
				string answer = requires.Length > 0
					? $"I need {requires} to survive."
					: "I don't need much to survive.";
				return answer + Pick(
					" The exact limits are in your registry, not my head.",
					" Beyond that, look my species up — I don't carry the numbers around.",
					string.Empty);
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

		private string GetAnswerLine(ChatTopic topic, string value, string fallback)
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
			return fallback;
		}

		private static string GetDescribeClueLine(string description, string fallback)
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

		// Helper Methods

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


	public class BrainResult
	{
		/// <summary>
		/// Answer: Topic + Persona == Answer
		/// Clarify: Can be about different Topics.. What do you want to know? 
		/// Fallback: No clue or Tip to User
		/// </summary>
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
}
