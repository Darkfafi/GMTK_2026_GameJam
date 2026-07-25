using System.Text;

namespace GMTK_2026
{
	public static class TerminalMarkdown
	{
		private const string Accent = "#00ff88";
		private const string Warning = "#ff9f43";
		private const string Info = "#00d2ff";
		private const string Danger = "#ff4757";

		public static string ToRichText(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return string.Empty;
			}

			string[] lines = source.Replace("\r\n", "\n").Split('\n');
			StringBuilder sb = new StringBuilder(source.Length + 256);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];

				if (line.StartsWith("### "))
				{
					sb.Append($"<color={Info}>{Keywords(line.Substring(4))}</color>");
				}
				else if (line.StartsWith("## "))
				{
					sb.Append($"<color={Warning}><b>{Keywords(line.Substring(3))}</b></color>");
				}
				else if (line.StartsWith("# "))
				{
					sb.Append($"<size=16><color={Accent}><b>{Keywords(line.Substring(2))}</b></color></size>");
				}
				else if (line.StartsWith("- "))
				{
					sb.Append($"  <color={Accent}>•</color> {Keywords(line.Substring(2))}");
				}
				else
				{
					sb.Append(Keywords(line));
				}

				if (i < lines.Length - 1)
				{
					sb.Append('\n');
				}
			}

			return sb.ToString();
		}

		private static string Keywords(string line)
		{
			return line
				.Replace("BANNED", $"<color={Danger}><b>BANNED</b></color>")
				.Replace("DENIED", $"<color={Danger}><b>DENIED</b></color>")
				.Replace("FATAL", $"<color={Danger}><b>FATAL</b></color>")
				.Replace("PERMITTED", $"<color={Accent}><b>PERMITTED</b></color>")
				.Replace("REQUIRED", $"<color={Warning}><b>REQUIRED</b></color>");
		}
	}
}
