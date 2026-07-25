using System;
using System.Collections.Generic;
using RaDataHolder;
using UnityEngine;

namespace GMTK_2026
{
	public class RequestsController : RaMonoDataHolderBase<RequestsController.CoreData>
	{
		public event Action<PilotRequestBase, PlayerChoice> RequestSubmittedEvent;
		public event Action<PilotRequestBase> RequestExpiredEvent;

		[SerializeField]
		private RequestsUIWindow _uiWindow = null;

		private readonly List<PilotRequestBase> _activeRequests = new List<PilotRequestBase>();
		private readonly List<PilotRequestBase> _expiredThisTick = new List<PilotRequestBase>();

		public IReadOnlyList<PilotRequestBase> ActiveRequests => _activeRequests;

		protected override void OnSetData()
		{
			_uiWindow.RequestApprovedEvent += OnRequestApproved;
			_uiWindow.RequestDeniedEvent += OnRequestDenied;

			if (Data.Request != null)
			{
				AddRequest(Data.Request);
			}
		}

		protected override void OnClearData()
		{
			_uiWindow.RequestApprovedEvent -= OnRequestApproved;
			_uiWindow.RequestDeniedEvent -= OnRequestDenied;

			for (int i = _activeRequests.Count - 1; i >= 0; i--)
			{
				_uiWindow.RemoveRequest(_activeRequests[i]);
			}
			_activeRequests.Clear();
		}

		public void AddRequest(PilotRequestBase request)
		{
			if (request == null || _activeRequests.Contains(request))
			{
				return;
			}

			_activeRequests.Add(request);
			_uiWindow.AddRequest(request);
		}

		public void RemoveRequest(PilotRequestBase request)
		{
			if (request == null || !_activeRequests.Remove(request))
			{
				return;
			}

			_uiWindow.RemoveRequest(request);
		}

		private void Update()
		{
			if (!HasData)
			{
				return;
			}

			float deltaTime = Time.deltaTime;

			_expiredThisTick.Clear();
			for (int i = 0; i < _activeRequests.Count; i++)
			{
				PilotRequestBase request = _activeRequests[i];
				bool justExpired = request.TickTime(deltaTime);
				_uiWindow.SetTimeNormalized(request, request.TimeNormalized);

				if (justExpired)
				{
					_expiredThisTick.Add(request);
				}
			}

			for (int i = 0; i < _expiredThisTick.Count; i++)
			{
				RequestExpiredEvent?.Invoke(_expiredThisTick[i]);
			}
		}

		private void OnRequestApproved(PilotRequestBase request) => RequestSubmittedEvent?.Invoke(request, PlayerChoice.Approved);
		private void OnRequestDenied(PilotRequestBase request) => RequestSubmittedEvent?.Invoke(request, PlayerChoice.Denied);

		public struct CoreData
		{
			public PilotRequestBase Request;
		}
	}
}
