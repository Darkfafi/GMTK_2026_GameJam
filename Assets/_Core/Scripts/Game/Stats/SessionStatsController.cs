using UnityEngine;

namespace GMTK_2026
{
	public class SessionStatsController : MonoBehaviour
	{
		[SerializeField]
		private StatsUIWindow _uiWindow = null;

		public int Correct { get; private set; }
		public int Incorrect { get; private set; }

		private void Start() => Refresh();

		public void RegisterCorrect()
		{
			Correct++;
			Refresh();
		}

		public void RegisterIncorrect()
		{
			Incorrect++;
			Refresh();
		}

		public void ResetStats()
		{
			Correct = 0;
			Incorrect = 0;
			Refresh();
		}

		private void Refresh()
		{
			_uiWindow.SetCorrect(Correct);
			_uiWindow.SetIncorrect(Incorrect);
		}
	}
}
