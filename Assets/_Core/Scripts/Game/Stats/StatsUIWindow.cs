using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public class StatsUIWindow : MonoBehaviour
	{
		[SerializeField]
		private UIDocument _document = null;

		private Label _scoreLabel;
		private Label _streakLabel;
		private VisualElement _dot1;
		private VisualElement _dot2;
		private VisualElement _dot3;

		private void OnEnable()
		{
			if (_document == null) return;
			VisualElement root = _document.rootVisualElement;
			if (root == null) return;

			_scoreLabel = root.Q<Label>("stat-score");
			_streakLabel = root.Q<Label>("stat-streak");
			_dot1 = root.Q<VisualElement>("integrity-dot-1");
			_dot2 = root.Q<VisualElement>("integrity-dot-2");
			_dot3 = root.Q<VisualElement>("integrity-dot-3");
		}

		public void SetScore(int value)
		{
			if (_scoreLabel != null)
			{
				_scoreLabel.text = value.ToString("D4");
			}
		}

		public void SetStreak(int value)
		{
			if (_streakLabel != null)
			{
				_streakLabel.text = $"x{value}";
			}
		}

		public void SetIntegrity(int mistakes)
		{
			// Set dot states based on mistakes (0, 1, 2, 3)
			SetDotState(_dot1, mistakes < 3);
			SetDotState(_dot2, mistakes < 2);
			SetDotState(_dot3, mistakes < 1);
		}

		private void SetDotState(VisualElement dot, bool isFull)
		{
			if (dot == null) return;
			if (isFull)
			{
				dot.RemoveFromClassList("integrity-dot--empty");
				dot.AddToClassList("integrity-dot--full");
			}
			else
			{
				dot.RemoveFromClassList("integrity-dot--full");
				dot.AddToClassList("integrity-dot--empty");
			}
		}
	}
}