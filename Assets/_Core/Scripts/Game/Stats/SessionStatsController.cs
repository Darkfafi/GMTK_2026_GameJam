using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2026
{
	public class SessionStatsController : MonoBehaviour
	{
		[SerializeField]
		private StatsUIWindow _uiWindow = null;

		public int Correct { get; private set; }
		public int Incorrect { get; private set; }
		public int Score { get; private set; }
		public int Streak { get; private set; } = 1;
		public int RequestsProcessed { get; private set; }
		public int Mistakes { get; private set; }
		public List<string> MistakeLogs { get; } = new List<string>();

		private void Start() => ResetStats();

		public void RegisterCorrect(float elapsedSeconds)
		{
			Correct++;
			RequestsProcessed++;

			int basePoints = 100;
			int speedBonus = Mathf.Max(0, Mathf.CeilToInt(30f - elapsedSeconds)) * 10;
			Score += (basePoints + speedBonus) * Streak;

			Streak++;
			Refresh();
		}

		public void RegisterCorrect()
		{
			RegisterCorrect(35f); // defaults to no speed bonus
		}

		public void RegisterIncorrect(string pilot, string decision, string why)
		{
			Incorrect++;
			RequestsProcessed++;
			Streak = 1;
			Mistakes++;

			string action = decision == "APPROVED" || decision == "ACCESS" ? "Permitted" : "Denied";
			string correctAction = decision == "APPROVED" || decision == "ACCESS" ? "declined" : "approved";
			string log = $"* Mistake #{Mistakes}: {action} {pilot}, but they should have been {correctAction}. Reason: {why}";
			MistakeLogs.Add(log);

			Refresh();
		}

		public void RegisterIncorrect()
		{
			Incorrect++;
			RequestsProcessed++;
			Streak = 1;
			Mistakes++;
			Refresh();
		}

		public void RegisterTimeout(string pilot, string correctAction)
		{
			Incorrect++;
			RequestsProcessed++;
			Streak = 1;
			Mistakes++;

			string log = $"* Mistake #{Mistakes}: Transmission with {pilot} timed out, resulting in automatic denial. Correct action was {correctAction}.";
			MistakeLogs.Add(log);

			Refresh();
		}

		public void ResetStats()
		{
			Correct = 0;
			Incorrect = 0;
			Score = 0;
			Streak = 1;
			RequestsProcessed = 0;
			Mistakes = 0;
			MistakeLogs.Clear();
			Refresh();
		}

		private void Refresh()
		{
			if (_uiWindow != null)
			{
				_uiWindow.SetScore(Score);
				_uiWindow.SetStreak(Streak);
				_uiWindow.SetIntegrity(Mistakes);
			}
		}
	}
}