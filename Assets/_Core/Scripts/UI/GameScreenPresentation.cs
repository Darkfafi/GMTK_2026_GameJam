using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public class GameScreenPresentation : MonoBehaviour
	{
		[SerializeField]
		private UIDocument _document = null;

		private Label _clock;
		private VisualElement _logo;
		private VisualElement _dot;

		private void Start()
		{
			if (_document == null)
			{
				_document = GetComponent<UIDocument>();
			}

			VisualElement root = _document != null ? _document.rootVisualElement : null;
			if (root == null)
			{
				Debug.LogWarning("[GameScreenPresentation] No UIDocument found — effects disabled.", this);
				return;
			}

			VisualElement app = root.Q<VisualElement>("game-root") ?? root;
			StarfieldElement starfield = new StarfieldElement();
			starfield.AddToClassList("fx-starfield");
			app.Insert(0, starfield);

			VisualElement terminal = root.Q<VisualElement>("terminal-panel");
			if (terminal != null)
			{
				ScanlineElement scan = new ScanlineElement();
				scan.AddToClassList("fx-scanlines");
				terminal.Add(scan);
			}

			_clock = root.Q<Label>("clock");
			_logo = root.Q<VisualElement>("station-logo");
			_dot = root.Q<VisualElement>("sb-dot");
		}

		private void Update()
		{
			if (_clock != null)
			{
				_clock.text = DateTime.Now.ToString("HH:mm:ss");
			}

			// Slow pulse (glow stand-in).
			float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 2f);
			if (_logo != null)
			{
				_logo.style.opacity = 0.6f + 0.4f * pulse;
			}
			if (_dot != null)
			{
				_dot.style.opacity = 0.25f + 0.75f * pulse;
			}
		}
	}
}
