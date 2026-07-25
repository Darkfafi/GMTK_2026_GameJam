using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public static class RequestViewFactory
	{
		public static RequestViewBase Create(PilotRequestBase request)
		{
			switch (request)
			{
				case LandingPilotRequest:
					return new LandingRequestView();
				default:
					return null;
			}
		}
	}

	public class RequestCardElement : VisualElement
	{
		public const string UssClass = "req-card";

		private readonly Label _title;
		private readonly Label _badge;
		private readonly Label _description;
		private readonly CountdownRingElement _ring;
		private readonly Label _cdStatus;
		private readonly VisualElement _body;
		private readonly VisualElement _resultArea;
		private readonly Button _approveButton;
		private readonly Button _denyButton;

		public PilotRequestBase Request { get; private set; }

		public event Action<RequestCardElement> ApprovedEvent;
		public event Action<RequestCardElement> DeniedEvent;

		public RequestCardElement()
		{
			AddToClassList(UssClass);

			// Title row
			VisualElement titleRow = new VisualElement { pickingMode = PickingMode.Ignore };
			titleRow.AddToClassList("req-card__titlerow");

			_title = new Label { pickingMode = PickingMode.Ignore };
			_title.AddToClassList("req-card__title");

			_badge = new Label { pickingMode = PickingMode.Ignore };
			_badge.AddToClassList("req-card__badge");

			titleRow.Add(_title);
			titleRow.Add(_badge);

			_description = new Label { pickingMode = PickingMode.Ignore };
			_description.AddToClassList("req-card__desc");

			// Countdown box.
			VisualElement cdBox = new VisualElement { pickingMode = PickingMode.Ignore };
			cdBox.AddToClassList("cd-box");

			_ring = new CountdownRingElement();

			VisualElement cdInfo = new VisualElement { pickingMode = PickingMode.Ignore };
			cdInfo.AddToClassList("cd-info");

			Label cdLabel = new Label("RESPONSE TIMER") { pickingMode = PickingMode.Ignore };
			cdLabel.AddToClassList("cd-label");

			_cdStatus = new Label("Awaiting operator decision") { pickingMode = PickingMode.Ignore };
			_cdStatus.AddToClassList("cd-status");

			cdInfo.Add(cdLabel);
			cdInfo.Add(_cdStatus);
			cdBox.Add(_ring);
			cdBox.Add(cdInfo);

			// Detail body
			_body = new VisualElement { pickingMode = PickingMode.Ignore };
			_body.AddToClassList("req-card__body");

			// Buttons
			VisualElement actions = new VisualElement();
			actions.AddToClassList("req-card__actions");

			_approveButton = new Button(() => ApprovedEvent?.Invoke(this)) { text = "ACCESS" };
			_approveButton.AddToClassList("req-btn");
			_approveButton.AddToClassList("req-btn--approve");

			_denyButton = new Button(() => DeniedEvent?.Invoke(this)) { text = "DECLINE" };
			_denyButton.AddToClassList("req-btn");
			_denyButton.AddToClassList("req-btn--deny");

			actions.Add(_approveButton);
			actions.Add(_denyButton);

			// Result
			_resultArea = new VisualElement { pickingMode = PickingMode.Ignore };

			Add(titleRow);
			Add(_description);
			Add(cdBox);
			Add(_body);
			Add(actions);
			Add(_resultArea);
		}

		public void ShowResult(bool ok, string message)
		{
			_resultArea.Clear();

			Label bar = new Label(message) { pickingMode = PickingMode.Ignore };
			bar.AddToClassList("result-bar");
			bar.AddToClassList(ok ? "result-bar--ok" : "result-bar--err");
			_resultArea.Add(bar);
		}

		public void SetInteractable(bool interactable)
		{
			_approveButton.SetEnabled(interactable);
			_denyButton.SetEnabled(interactable);
		}

		public void Bind(PilotRequestBase request)
		{
			Request = request;
			_title.text = request?.RequestTitle ?? string.Empty;
			_badge.text = request?.RequestType ?? string.Empty;
			_description.text = request?.RequestDescription ?? string.Empty;
			_resultArea.Clear();
			SetInteractable(true);
			SetTimeRemaining(1f);
		}

		public void SetTimeRemaining(float normalized)
		{
			_ring.SetProgress(normalized);
			_ring.SetSeconds(Request != null ? Mathf.CeilToInt(Request.TimeRemaining) : 0);

			_cdStatus.RemoveFromClassList("cd-status--warn");
			_cdStatus.RemoveFromClassList("cd-status--crit");

			if (normalized <= 0.15f)
			{
				_cdStatus.AddToClassList("cd-status--crit");
				_cdStatus.text = "CRITICAL — respond now";
			}
			else if (normalized <= 0.30f)
			{
				_cdStatus.AddToClassList("cd-status--warn");
				_cdStatus.text = "Warning — time running low";
			}
			else
			{
				_cdStatus.text = "Awaiting operator decision";
			}
		}

		public void SetBody(VisualElement body)
		{
			_body.Clear();
			if (body != null)
			{
				_body.Add(body);
			}
		}
	}
}
