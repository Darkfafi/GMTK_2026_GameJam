using UnityEngine.UIElements;

namespace GMTK_2026
{
	public abstract class RequestViewBase : VisualElement
	{
		public const string UssClass = "request-view";

		protected RequestViewBase()
		{
			AddToClassList(UssClass);
		}

		public abstract void Bind(PilotRequestBase request);
	}
}
