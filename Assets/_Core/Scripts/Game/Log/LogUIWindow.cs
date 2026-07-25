using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public enum LogLevel
	{
		Normal,
		Accent,
		Danger,
		Warning,
		Info,
	}

	public class LogUIWindow : MonoBehaviour
	{
		[SerializeField]
		private UIDocument _document = null;

		[SerializeField]
		private int _maxEntries = 100;

		private ScrollView _body;

		private void OnEnable()
		{
			_body = _document.rootVisualElement.Q<ScrollView>("log-body");
		}

		public void AddEntry(string message, LogLevel level)
		{
			if (_body == null)
			{
				return;
			}

			VisualElement entry = new VisualElement { pickingMode = PickingMode.Ignore };
			entry.AddToClassList("log-entry");

			Label time = new Label(System.DateTime.Now.ToString("HH:mm:ss")) { pickingMode = PickingMode.Ignore };
			time.AddToClassList("log-t");

			Label msg = new Label(message) { pickingMode = PickingMode.Ignore };
			msg.AddToClassList("log-m");
			string modifier = LevelClass(level);
			if (modifier != null)
			{
				msg.AddToClassList(modifier);
			}

			entry.Add(time);
			entry.Add(msg);
			_body.Add(entry);

			// Slide in Effect
			entry.schedule.Execute(() => entry.AddToClassList("log-entry--in")).ExecuteLater(16);

			VisualElement content = _body.contentContainer;
			while (content.childCount > _maxEntries)
			{
				content.RemoveAt(0);
			}

			_body.schedule.Execute(() =>
			{
				if (_body.verticalScroller != null)
				{
					_body.verticalScroller.value = _body.verticalScroller.highValue;
				}
			}).ExecuteLater(32);
		}

		public void Clear() => _body?.Clear();

		private static string LevelClass(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Accent: return "log-m--accent";
				case LogLevel.Danger: return "log-m--danger";
				case LogLevel.Warning: return "log-m--warning";
				case LogLevel.Info: return "log-m--info";
				default: return null;
			}
		}
	}
}
