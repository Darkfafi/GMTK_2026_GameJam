using UnityEngine;

namespace GMTK_2026
{
	[RequireComponent(typeof(RaFileSystemWindow))]
	public class RaFileSystemDemo : MonoBehaviour
	{
		private void Start()
		{
			var window = GetComponent<RaFileSystemWindow>();

			var root = new RaFolder("Home",
				new RaFolder("Documents",
					new RaFile("design_notes.txt"),
					new RaFolder("Ideas",
						new RaFile("gmtk_theme.txt"),
						new RaFile("mechanics.txt")
					)
				),
				new RaFolder("Photos",
					new RaFile("screenshot_01.png"),
					new RaFile("screenshot_02.png")
				),
				new RaFile("readme.md")
			);

			window.FileOpenedEvent += e => Debug.Log($"Opened file: {e.Name}");
			window.SetRootFolder(root);
		}
	}
}
