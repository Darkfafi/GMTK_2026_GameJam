using System.Collections.Generic;

namespace GMTK_2026
{
	/// <summary>
	/// Attribute - 2/3 - Bridge between Subject and Topic
	/// </summary>
	public static class PilotBrainAttribute
	{
		public static readonly Dictionary<ChatAttribute, Dictionary<Subject, ChatTopic>> ChatAttributeToSubjectTopicPairMap = new Dictionary<ChatAttribute, Dictionary<Subject, ChatTopic>>
		{
			{ ChatAttribute.Kind, new Dictionary<Subject, ChatTopic> { { Subject.Pilot, ChatTopic.Species }, { Subject.Planet, ChatTopic.Body }, { Subject.Ship, ChatTopic.ShipClass } } },
			{ ChatAttribute.Name, new Dictionary<Subject, ChatTopic> { { Subject.Pilot, ChatTopic.Name }, { Subject.Planet, ChatTopic.Destination }, { Subject.Ship, ChatTopic.ShipName } } },
		};

		public static readonly Dictionary<ChatAttribute, string[]> ChatAttributeNeutralWordsMap = new Dictionary<ChatAttribute, string[]>
		{
			{ ChatAttribute.Kind, new[] { "kind", "type", "class", "sort" } },
			{ ChatAttribute.Name, new[] { "name", "called", "call" } },
		};

		public static ChatAttribute? GetAmbiguousChatTopicAttribute(ChatTopic a, ChatTopic b)
		{
			// If we have 2 Topics and no Subject
			if (Has(ChatTopic.Species, ChatTopic.Body) ||
				Has(ChatTopic.Species, ChatTopic.ShipClass) ||
				Has(ChatTopic.Body, ChatTopic.ShipClass))
			{
				return ChatAttribute.Kind;
			}

			if (Has(ChatTopic.Name, ChatTopic.ShipName) ||
				Has(ChatTopic.Name, ChatTopic.Destination))
			{
				return ChatAttribute.Name;
			}

			return null;

			bool Has(ChatTopic x, ChatTopic y) => (a == x && b == y) || (a == y && b == x);
		}
	}

	public enum ChatAttribute { Kind, Name }
}
