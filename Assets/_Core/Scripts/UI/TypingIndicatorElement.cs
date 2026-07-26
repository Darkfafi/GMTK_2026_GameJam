using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public sealed class TypingIndicatorElement : VisualElement
	{
		private readonly VisualElement[] _dots = new VisualElement[3];
		private float _time;

		public TypingIndicatorElement()
		{
			AddToClassList("typing-bubble");
			pickingMode = PickingMode.Ignore;

			for (int i = 0; i < _dots.Length; i++)
			{
				VisualElement dot = new VisualElement { pickingMode = PickingMode.Ignore };
				dot.AddToClassList("typing-bubble__dot");
				_dots[i] = dot;
				Add(dot);
			}

			schedule.Execute(Tick).Every(50);
		}

		private void Tick()
		{
			if (resolvedStyle.display == DisplayStyle.None)
			{
				return;
			}

			_time += 0.05f;
			for (int i = 0; i < _dots.Length; i++)
			{
				float wave = Mathf.Sin((_time - i * 0.2f) * 5.2f);
				float lift = Mathf.Max(0f, wave) * 5f;
				_dots[i].style.translate = new Translate(0f, -lift);
				_dots[i].style.opacity = 0.4f + Mathf.Max(0f, wave) * 0.6f;
			}
		}
	}
}
