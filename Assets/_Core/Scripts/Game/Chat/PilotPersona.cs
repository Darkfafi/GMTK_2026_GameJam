using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2026
{
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

		// How directly the pilot answers (low = vague phrasing)
		public float Clarity { get; private set; }

		// Willingness to answer non-critical questions (low = evasive)
		public float Cooperation { get; private set; }

		/// Nervousness (high = on edge)
		public float Nervousness { get; private set; }

		// What they want to talk to you about
		public PilotRequestBase Request { get; private set; }

		private readonly Dictionary<ChatTopic, float> _knowledge = new Dictionary<ChatTopic, float>();

		public PilotPersona(PilotRequestBase request, float? clarity, float? cooperation, float? nervousness, Dictionary<ChatTopic, float> customKnowledge)
		{
			Request = request;
			Clarity = clarity.HasValue ? clarity.Value : Random.Range(0.35f, 0.95f);
			Cooperation = cooperation.HasValue ? cooperation.Value : Random.Range(0.3f, 0.95f);
			Nervousness = nervousness.HasValue ? nervousness.Value : Random.Range(0f, 0.8f);
			_knowledge = new Dictionary<ChatTopic, float>(customKnowledge);
			Roll();
		}

		public float GetKnowledge(ChatTopic topic)
			=> _knowledge.TryGetValue(topic, out float value) ? value : 1f;

		private void Roll()
		{
			void SetKnowledge(ChatTopic topic, float value)
			{
				if (!_knowledge.ContainsKey(topic))
				{
					_knowledge[topic] = value;
				}
			}

			// Always Known
			SetKnowledge(ChatTopic.Name, 1f);
			SetKnowledge(ChatTopic.Destination, 1f);
			SetKnowledge(ChatTopic.Origin, 1f);
			SetKnowledge(ChatTopic.ShipName, RollKnowledge(0.85f, 0.05f));

			// Pilots can see the gear they packed
			SetKnowledge(ChatTopic.Equipment, RollKnowledge(0.85f, 0.15f));

			// Not everyone knows what class of hull they were handed.
			SetKnowledge(ChatTopic.ShipClass, RollKnowledge(0.7f, 0.15f));

			// The research-relevant topics: often known, sometimes vague, sometimes
			// only describable - forcing the operator to match the description
			// against the registry files.
			SetKnowledge(ChatTopic.Species, RollKnowledge(0.6f, 0.15f));
			SetKnowledge(ChatTopic.Body, RollKnowledge(0.6f, 0.15f));
			SetKnowledge(ChatTopic.Occupation, RollKnowledge(0.7f, 0.1f));
			SetKnowledge(ChatTopic.Needs, RollKnowledge(0.5f, 0.2f));
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
	}
}
