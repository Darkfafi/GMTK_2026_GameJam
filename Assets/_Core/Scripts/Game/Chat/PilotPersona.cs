using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2026
{
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

	public class PilotPersona
	{
		public static readonly HashSet<ChatTopic> CriticalTopics = new HashSet<ChatTopic>
		{
			ChatTopic.Species,
			ChatTopic.Origin,
			ChatTopic.Equipment,
			ChatTopic.Destination,
			ChatTopic.ShipClass,
		};

		public LandingPilotRequest Request { get; }

		// How directly the pilot answers (low = vague phrasing)
		public float Clarity { get; private set; }

		// Willingness to answer non-critical questions (low = evasive)
		public float Cooperation { get; private set; }

		/// Delay
		public float Nervousness { get; private set; }

		public string Hail { get; private set; }
		public IReadOnlyList<string> UnpromptedLines => _unprompted;

		private readonly Dictionary<ChatTopic, float> _knowledge = new Dictionary<ChatTopic, float>();
		private readonly List<string> _unprompted = new List<string>();

		public PilotPersona(LandingPilotRequest request)
		{
			Request = request;
			Roll();
		}

		public float GetKnowledge(ChatTopic topic)
			=> _knowledge.TryGetValue(topic, out float value) ? value : 1f;

		private void Roll()
		{
			Clarity = Random.Range(0.35f, 0.95f);
			Cooperation = Random.Range(0.3f, 0.95f);
			Nervousness = Random.Range(0f, 0.8f);

			// Always Known
			_knowledge[ChatTopic.Name] = 1f;
			_knowledge[ChatTopic.Destination] = 1f;
			_knowledge[ChatTopic.Origin] = 1f;
			_knowledge[ChatTopic.ShipName] = RollKnowledge(0.85f, 0.05f);

			// Pilots can see the gear they packed
			_knowledge[ChatTopic.Equipment] = RollKnowledge(0.85f, 0.15f);

			// Not everyone knows what class of hull they were handed.
			_knowledge[ChatTopic.ShipClass] = RollKnowledge(0.7f, 0.15f);

			// The research-relevant topics: often known, sometimes vague, sometimes
			// only describable - forcing the operator to match the description
			// against the registry files.
			_knowledge[ChatTopic.Species] = RollKnowledge(0.6f, 0.15f);
			_knowledge[ChatTopic.Body] = RollKnowledge(0.6f, 0.15f);
			_knowledge[ChatTopic.Occupation] = RollKnowledge(0.7f, 0.1f);
			_knowledge[ChatTopic.Needs] = RollKnowledge(0.5f, 0.2f);

			string pilot = Request.Pilot?.Name;
			string planet = Request.Target?.Name;
			string ship = Request.Ship?.Name;

			Hail = Pick(
				$"Station Alpha, this is {pilot}. Requesting landing clearance on {planet}.",
				$"...hello? This is {pilot}. I need to land. {planet}, if possible.",
				$"Station Alpha, {pilot} here, flying the {ship}. Requesting permission to land on {planet}.",
				$"{pilot} to Station Alpha. Landing request for {planet}. Standing by.");

			_unprompted.Add("Operator? Are you still there?");
			_unprompted.Add("I really need to get down there soon.");
			if (Nervousness > 0.5f)
			{
				_unprompted.Add("...");
				_unprompted.Add("Please, I just need to land.");
			}
			if (Cooperation < 0.5f)
			{
				_unprompted.Add("I don't have all day, operator.");
				_unprompted.Add("Is this going to take much longer?");
			}
			else
			{
				_unprompted.Add("Standing by for clearance.");
				_unprompted.Add("Let me know if you need anything else from me.");
			}
		}

		private static float RollKnowledge(float knowChance, float vagueChance)
		{
			float roll = Random.value;
			if (roll < knowChance)
			{
				return 1f;
			}
			return roll < knowChance + vagueChance ? 0.5f : 0f;
		}

		private static string Pick(params string[] options)
			=> options[Random.Range(0, options.Length)];
	}
}
