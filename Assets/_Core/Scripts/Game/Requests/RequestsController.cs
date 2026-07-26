using System;
using System.Collections.Generic;
using RaDataHolder;
using UnityEngine;

namespace GMTK_2026
{
	public class RequestsController : RaMonoDataHolderBase<RequestsController.CoreData>
	{
		public event Action<PilotRequestBase> RequestAddedEvent;
		public event Action<PilotRequestBase> RequestRemovedEvent;
		public event Action<PilotRequestBase> RequestExpiredEvent;

		private readonly List<PilotRequestBase> _activeRequests = new List<PilotRequestBase>();
		private readonly List<PilotRequestBase> _expiredThisTick = new List<PilotRequestBase>();

		public IReadOnlyList<PilotRequestBase> ActiveRequests => _activeRequests;

		protected override void OnSetData()
		{
			if (Data.Request != null)
			{
				AddRequest(Data.Request);
			}
		}

		protected override void OnClearData()
		{
			for (int i = _activeRequests.Count - 1; i >= 0; i--)
			{
				RemoveRequest(_activeRequests[i]);
			}
		}

		public void AddRequest(PilotRequestBase request)
		{
			if (request == null || _activeRequests.Contains(request))
			{
				return;
			}

			_activeRequests.Add(request);
			RequestAddedEvent?.Invoke(request);
		}

		public void RemoveRequest(PilotRequestBase request)
		{
			if (request == null || !_activeRequests.Remove(request))
			{
				return;
			}

			RequestRemovedEvent?.Invoke(request);
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
				if (request.IsResolved)
				{
					continue;
				}

				if (request.TickTime(deltaTime))
				{
					_expiredThisTick.Add(request);
				}
			}

			for (int i = 0; i < _expiredThisTick.Count; i++)
			{
				RequestExpiredEvent?.Invoke(_expiredThisTick[i]);
			}
		}

		public struct CoreData
		{
			public PilotRequestBase Request;
		}
	}
}
