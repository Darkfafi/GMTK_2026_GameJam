using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public class StatsUIWindow : MonoBehaviour
	{
		[SerializeField]
		private UIDocument _document = null;

		private Label _correct;
		private Label _wrong;

		private void OnEnable()
		{
			VisualElement root = _document.rootVisualElement;
			_correct = root.Q<Label>("stat-correct");
			_wrong = root.Q<Label>("stat-wrong");
		}

		public void SetCorrect(int value)
		{
			if (_correct != null)
			{
				_correct.text = value.ToString();
			}
		}

		public void SetIncorrect(int value)
		{
			if (_wrong != null)
			{
				_wrong.text = value.ToString();
			}
		}
	}
}
