using System.Collections.Generic;
using System.Linq;

namespace GMTK_2026
{
	/// <summary>
	/// Topic - 3/3 - The Final Question that will be answered
	/// </summary>
	public static class PilotBrainTopic
	{
		public static readonly Dictionary<ChatTopic, string[]> TopicToPhrasesMap = new Dictionary<ChatTopic, string[]>
		{
			{ ChatTopic.Name, new[] { "your name", "who are you", "whats your name", "what's your name", "what is your name", "identify yourself", "call you" } },
			{ ChatTopic.Species, new[] { "species", "what are you", "your kind", "your race", "are you human", "are you alien", "life form", "lifeform", "what creature", "biological" } },
			{ ChatTopic.Origin, new[] { "where are you from", "where do you come from", "your home", "homeworld", "home world", "your origin", "originate", "native to", "born" } },
			{ ChatTopic.Equipment, new[] { "equipment", "your gear", "any gear", "a suit", "your suit", "wearing", "carrying", "protection", "protective", "rig", "do you have gear", "what do you have with you", "life support" } },
			{ ChatTopic.Occupation, new[] { "what do you do", "your job", "occupation", "profession", "work as", "your role", "your position" } },
			{ ChatTopic.Destination, new[] { "where are you going", "your destination", "what planet", "which planet", "where to", "landing on", "going to", "heading", "headed", "destination", "land where", "where do you need" } },
			{ ChatTopic.Body, new[] { "kind of planet", "type of planet", "what body", "body type", "kind of world", "type of world", "describe the planet", "planet like", "what is the planet", "whats the planet", "what's the planet", "celestial" } },
			{ ChatTopic.ShipName, new[] { "ship name", "vessel name", "ship called", "name of your ship", "what ship", "what vessel", "callsign", "call sign" } },
			{ ChatTopic.ShipClass, new[] { "ship class", "class of ship", "what kind of ship", "what type of ship", "kind of vessel", "type of vessel", "vessel class", "hull", "what are you flying", "rated for", "how tough" } },
			{ ChatTopic.Needs, new[] { "what do you need", "need to survive", "need to live", "breathe", "breath", "oxygen", "atmosphere do you", "survive on", "requirements", "what keeps you alive", "environment do you need" } },
			{ ChatTopic.Health, new[] { "how are you", "are you okay", "are you ok", "you alright", "are you alright", "your condition", "are you hurt", "injured", "how do you feel", "how are things", "hows it going", "how's it going", "you good", "all good", "doing okay", "doing well", "everything alright" } },
			{ ChatTopic.Purpose, new[] { "why do you need", "purpose", "reason", "what for", "why are you going", "what brings you", "business", "why this planet" } },
			{ ChatTopic.Greeting, new[] { "hello", "hi", "hey", "yo", "greetings", "good morning", "good evening", "good day", "howdy", "salutations" } },
			{ ChatTopic.Rude, new[] { "shut up", "stupid", "idiot", "moron", "dumb", "useless" } },
			{ ChatTopic.Hurry, new[] { "hurry", "quick", "be fast", "speed up", "faster", "time is running" } },
			{ ChatTopic.Goodbye, new[] { "bye", "goodbye", "done", "thats all", "that's all", "nothing else", "were done", "we're done" } },
		};
		
		public static readonly Dictionary<ChatTopic, Subject> TopicSubjectMap = new Dictionary<ChatTopic, Subject>
		{
			{ ChatTopic.Name, Subject.Pilot }, { ChatTopic.Species, Subject.Pilot }, { ChatTopic.Occupation, Subject.Pilot },
			{ ChatTopic.Needs, Subject.Pilot }, { ChatTopic.Health, Subject.Pilot },
			{ ChatTopic.Origin, Subject.Pilot }, { ChatTopic.Equipment, Subject.Pilot },
			{ ChatTopic.ShipName, Subject.Ship }, { ChatTopic.ShipClass, Subject.Ship },
			{ ChatTopic.Destination, Subject.Planet }, { ChatTopic.Body, Subject.Planet },
		};

		public static bool IsDirect(ChatTopic topic)
			=> topic == ChatTopic.Greeting || topic == ChatTopic.Rude || topic == ChatTopic.Hurry
			|| topic == ChatTopic.Goodbye || topic == ChatTopic.Health || topic == ChatTopic.Purpose;

		public static List<(ChatTopic, int)> ScoreTopics(string low)
		{
			var scores = new List<(ChatTopic, int)>();
			string[] words = Tokenize(low);

			foreach (KeyValuePair<ChatTopic, string[]> pair in TopicToPhrasesMap)
			{
				int score = pair.Value.Where(pattern => MatchesPattern(low, words, pattern)).Sum(p => p.Length);
				if (score > 0)
				{
					scores.Add((pair.Key, score));
				}
			}
			return scores.OrderByDescending(s => s.Item2).ToList();
		}

		private static bool MatchesPattern(string low, string[] words, string pattern)
		{
			if (pattern.Length <= 4 && !pattern.Contains(' '))
			{
				return words.Contains(pattern);
			}
			return low.Contains(pattern);
		}

		private static string[] Tokenize(string low)
			=> new string(low.Select(c => char.IsLetterOrDigit(c) || c == '\'' ? c : ' ').ToArray())
				.Split(' ').Where(w => w.Length > 0).ToArray();
	}

	public enum ChatTopic
	{
		Name,
		Species,
		Origin,
		Equipment,
		Occupation,
		Destination,
		Body,
		ShipName,
		ShipClass,
		Needs,
		Health,
		Purpose,
		Greeting,
		Rude,
		Hurry,
		Goodbye,
	}
}
