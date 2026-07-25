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
		private readonly Label _description;
		private readonly VisualElement _timerFill;
		private readonly VisualElement _body;

		public PilotRequestBase Request { get; private set; }

		public event Action<RequestCardElement> ApprovedEvent;
		public event Action<RequestCardElement> DeniedEvent;

		public RequestCardElement()
		{
			AddToClassList(UssClass);

			_title = new Label { pickingMode = PickingMode.Ignore };
			_title.AddToClassList("req-card__title");

			_description = new Label { pickingMode = PickingMode.Ignore };
			_description.AddToClassList("req-card__desc");

			VisualElement timerBar = new VisualElement { pickingMode = PickingMode.Ignore };
			timerBar.AddToClassList("req-card__timerbar");
			_timerFill = new VisualElement { pickingMode = PickingMode.Ignore };
			_timerFill.AddToClassList("req-card__timerfill");
			timerBar.Add(_timerFill);

			VisualElement actions = new VisualElement();
			actions.AddToClassList("req-card__actions");

			Button deny = new Button(() => DeniedEvent?.Invoke(this)) { text = "Deny" };
			deny.AddToClassList("req-btn");
			deny.AddToClassList("req-btn--deny");

			Button approve = new Button(() => ApprovedEvent?.Invoke(this)) { text = "Approve" };
			approve.AddToClassList("req-btn");
			approve.AddToClassList("req-btn--approve");

			actions.Add(deny);
			actions.Add(approve);

			_body = new VisualElement { pickingMode = PickingMode.Ignore };
			_body.AddToClassList("req-card__body");

			Add(_title);
			Add(_description);
			Add(_body);
			Add(timerBar);
			Add(actions);
		}

		public void Bind(PilotRequestBase request)
		{
			Request = request;
			_title.text = request?.RequestTitle ?? string.Empty;
			_description.text = request?.RequestDescription ?? string.Empty;
			SetTimeRemaining(1f);
		}

		public void SetTimeRemaining(float normalized)
		{
			_timerFill.style.width = Length.Percent(Mathf.Clamp01(normalized) * 100f);
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
