using System.Collections.Generic;
using System.Linq;

namespace GMTK_2026
{
	/// <summary>
	/// Subject - 1/3 - The Object we want to talk about 
	/// </summary>
	public static class PilotBrainSubject
	{
		private static readonly Dictionary<Subject, string[]> SubjectWords = new Dictionary<Subject, string[]>
		{
			{ Subject.Ship, new[] { "ship", "ships", "vessel", "vessels", "craft", "freighter", "shuttle", "boat" } },
			{ Subject.Planet, new[] { "planet", "world", "destination", "moon", "body", "there" } },
			{ Subject.Pilot, new[] { "you", "your", "yourself", "u", "pilot", "captain" } },
		};

		public static Subject? DetectSubject(string low)
		{
			string[] words = new string(low.Select(c => char.IsLetterOrDigit(c) ? c : ' ').ToArray())
				.Split(' ').Where(w => w.Length > 0).ToArray();

			// Ship/planet win over pilot: "your ship" contains both.
			if (words.Any(w => SubjectWords[Subject.Ship].Contains(w))) return Subject.Ship;
			if (words.Any(w => SubjectWords[Subject.Planet].Contains(w))) return Subject.Planet;
			if (words.Any(w => SubjectWords[Subject.Pilot].Contains(w))) return Subject.Pilot;
			return null;
		}
	}

	public enum Subject { Pilot, Ship, Planet }
}
