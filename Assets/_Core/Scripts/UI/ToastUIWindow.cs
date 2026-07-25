using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public enum ToastType
	{
		Ok,
		Error,
		Info,
	}

	public class ToastUIWindow : MonoBehaviour
	{
		[SerializeField]
		private UIDocument _document = null;

		[SerializeField]
		private float _lingerSeconds = 2.5f;

		private VisualElement _box;

		private void OnEnable()
		{
			_box = _document.rootVisualElement.Q<VisualElement>("toast-box");
		}

		public void Show(string message, ToastType type)
		{
			if (_box == null)
			{
				return;
			}

			Label toast = new Label(message) { pickingMode = PickingMode.Ignore };
			toast.AddToClassList("toast");
			toast.AddToClassList(TypeClass(type));
			_box.Add(toast);

			toast.schedule.Execute(() => toast.AddToClassList("toast--in")).ExecuteLater(16);
			long lingerMs = (long)(_lingerSeconds * 1000f);
			toast.schedule.Execute(() => toast.AddToClassList("toast--out")).ExecuteLater(lingerMs);
			toast.schedule.Execute(toast.RemoveFromHierarchy).ExecuteLater(lingerMs + 350);
		}

		private static string TypeClass(ToastType type)
		{
			switch (type)
			{
				case ToastType.Ok: return "toast--ok";
				case ToastType.Error: return "toast--err";
				default: return "toast--nfo";
			}
		}
	}
}
