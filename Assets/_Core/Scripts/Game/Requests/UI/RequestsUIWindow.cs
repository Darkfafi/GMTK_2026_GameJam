using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public class RequestsUIWindow : MonoBehaviour
	{
		public event Action<PilotRequestBase> RequestApprovedEvent;
		public event Action<PilotRequestBase> RequestDeniedEvent;

		[SerializeField]
		private UIDocument _document = null;

		private ScrollView _list;
		private Label _count;
		private Label _emptyLabel;

		private readonly Dictionary<PilotRequestBase, RequestCardElement> _cards = new Dictionary<PilotRequestBase, RequestCardElement>();

		private void OnEnable()
		{
			VisualElement root = _document.rootVisualElement;

			_list = root.Q<ScrollView>("req-list");
			_count = root.Q<Label>("req-count");

			RefreshEmptyState();
			RefreshCount();
		}

		public void AddRequest(PilotRequestBase request)
		{
			if (request == null || _list == null || _cards.ContainsKey(request))
			{
				return;
			}

			RequestCardElement card = new RequestCardElement();
			card.Bind(request);

			RequestViewBase view = RequestViewFactory.Create(request);
			if (view != null)
			{
				view.Bind(request);
				card.SetBody(view);
			}

			card.ApprovedEvent += OnCardApproved;
			card.DeniedEvent += OnCardDenied;

			_cards.Add(request, card);
			_list.Add(card);

			RefreshEmptyState();
			RefreshCount();
		}

		public void RemoveRequest(PilotRequestBase request)
		{
			if (request == null || !_cards.TryGetValue(request, out RequestCardElement card))
			{
				return;
			}

			card.ApprovedEvent -= OnCardApproved;
			card.DeniedEvent -= OnCardDenied;
			card.RemoveFromHierarchy();
			_cards.Remove(request);

			RefreshEmptyState();
			RefreshCount();
		}

		public void SetTimeNormalized(PilotRequestBase request, float normalized)
		{
			if (_cards.TryGetValue(request, out RequestCardElement card))
			{
				card.SetTimeRemaining(normalized);
			}
		}

		private void OnCardApproved(RequestCardElement card) => RequestApprovedEvent?.Invoke(card.Request);
		private void OnCardDenied(RequestCardElement card) => RequestDeniedEvent?.Invoke(card.Request);

		private void RefreshCount()
		{
			if (_count != null)
			{
				_count.text = _cards.Count == 1 ? "1 waiting" : $"{_cards.Count} waiting";
			}
		}

		private void RefreshEmptyState()
		{
			if (_list == null)
			{
				return;
			}

			if (_cards.Count == 0 && _emptyLabel == null)
			{
				_emptyLabel = new Label("No pending requests");
				_emptyLabel.AddToClassList("req-empty-label");
				_list.Add(_emptyLabel);
			}
			else if (_cards.Count > 0 && _emptyLabel != null)
			{
				_emptyLabel.RemoveFromHierarchy();
				_emptyLabel = null;
			}
		}
	}

	public enum PlayerChoice
	{
		Approved,
		Denied,
	}
}
