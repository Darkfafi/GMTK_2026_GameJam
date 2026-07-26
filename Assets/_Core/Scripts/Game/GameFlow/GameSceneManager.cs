using UnityEngine;
using RaFSM;

namespace GMTK_2026
{
	public class GameSceneManager : MonoBehaviour, IRaFSMCycler
	{
		[SerializeField]
		private Transform _fsmRoot = null;

		[field: SerializeField]
		public RaFileSystemController FileSystemController
		{
			get; private set;
		}

		[field: SerializeField]
		public RequestsController RequestsController
		{
			get; private set;
		}

		[field: SerializeField]
		public LogController LogController
		{
			get; private set;
		}

		[field: SerializeField]
		public SessionStatsController StatsController
		{
			get; private set;
		}

		[field: SerializeField]
		public ChatController ChatController
		{
			get; private set;
		}

		[field: SerializeField]
		public ToastUIWindow ToastWindow
		{
			get; private set;
		}

		private RaGOFiniteStateMachine _fsm = null;

		protected void Awake()
		{
			_fsm = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(_fsmRoot));
		}

		protected void Start()
		{
			_fsm.SwitchState(0);
		}

		protected void OnDestroy()
		{
			_fsm.Dispose();
			_fsm = null;
		}

		public void GoToNextState()
		{
			_fsm.GoToNextState(wrap: false);
		}
	}
}