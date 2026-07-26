namespace GMTK_2026
{
	public class IntroState : GameSceneStateBase
	{
		protected override void OnInit()
		{
			Dependency.FileSystemController.SetData(GameFilesBuilder.BuildRoot());
		}

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.ChatController.ShiftStartRequestedEvent += OnShiftStartRequested;
			Dependency.ChatController.ShowBriefing();

			Dependency.LogController.Log("Terminal initialized. Station Alpha File System loaded.", LogLevel.Info);
			Dependency.LogController.Log("Browse the records. Start your shift when ready.", LogLevel.Info);
		}

		protected override void OnExit(bool isSwitch)
		{
			Dependency.ChatController.ShiftStartRequestedEvent -= OnShiftStartRequested;
			Dependency.ChatController.HideBriefing();
		}

		private void OnShiftStartRequested()
		{
			Dependency.LogController.Log("Shift initiated. Good luck, Operator.", LogLevel.Accent);
			Dependency.LogController.Log("Incoming requests queued. Awaiting first contact.", LogLevel.Info);
			FSM_GoToNextState();
		}
	}
}
