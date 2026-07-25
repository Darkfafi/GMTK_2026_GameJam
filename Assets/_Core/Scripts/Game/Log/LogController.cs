using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2026
{
	public class LogController : MonoBehaviour
	{
		[SerializeField]
		private LogUIWindow _uiWindow = null;

		private readonly List<string> _entries = new List<string>();

		public IReadOnlyList<string> Entries => _entries;

		public void Log(string message, LogLevel level = LogLevel.Normal)
		{
			_entries.Add(message);
			_uiWindow.AddEntry(message, level);
		}

		public void Clear()
		{
			_entries.Clear();
			_uiWindow.Clear();
		}
	}
}
