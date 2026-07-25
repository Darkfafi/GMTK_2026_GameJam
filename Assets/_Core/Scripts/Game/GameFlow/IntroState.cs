namespace GMTK_2026
{
	public class IntroState : GameSceneStateBase
	{
		protected override void OnInit()
		{
			// Create World
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

			Dependency.FileSystemController.SetData(root);
		}

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{
			FSM_GoToNextState();
		}

		protected override void OnExit(bool isSwitch)
		{

		}
	}
}