using RaFSM;

namespace GMTK_2026
{
	public class GameplayFlowState : GameSceneStateBase, IRaFSMCycler
	{
		private RaGOFiniteStateMachine _fsm = null;

		protected override void OnInit()
		{
			_fsm = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(transform));
		}

		protected override void OnDeinit()
		{
			_fsm.Dispose();
		}

		protected override void OnEnter()
		{
			_fsm.SwitchState(0);
		}

		protected override void OnExit(bool isSwitch)
		{
			_fsm.SwitchState(null);	
		}

		public void GoToNextState()
		{
			_fsm.GoToNextState(wrap: true);
		}
	}
}