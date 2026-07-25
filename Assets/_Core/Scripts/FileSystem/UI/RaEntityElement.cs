using System;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public class RaEntityElement : VisualElement
	{
		public const string UssClass = "ra-entity";
		public const string FolderModifier = "ra-entity--folder";
		public const string SelectedModifier = "ra-entity--selected";

		public event Action<RaEntityElement> ClickedEvent;
		public event Action<RaEntityElement> ActivatedEvent;

		private readonly Label _icon;
		private readonly Label _label;

		public RaFileSystemEntity Entity { get; private set; }

		public RaEntityElement()
		{
			AddToClassList(UssClass);

			_icon = new Label { pickingMode = PickingMode.Ignore };
			_icon.AddToClassList("ra-entity__icon");

			_label = new Label { pickingMode = PickingMode.Ignore };
			_label.AddToClassList("ra-entity__label");

			Add(_icon);
			Add(_label);

			RegisterCallback<ClickEvent>(OnClickedEvent);
		}

		public void Bind(RaFileSystemEntity entity)
		{
			Entity = entity;
			_label.text = entity?.Name ?? string.Empty;
			bool isFolder = entity is RaFolder;
			_icon.text = isFolder ? "📁" : "📄";
			EnableInClassList(FolderModifier, isFolder);
			SetSelected(false);
		}

		public void SetSelected(bool selected) => EnableInClassList(SelectedModifier, selected);

		private void OnClickedEvent(ClickEvent evt)
		{
			if (Entity == null)
			{
				return;
			}

			if (evt.clickCount >= 2)
			{
				ActivatedEvent?.Invoke(this);
			}
			else
			{
				ClickedEvent?.Invoke(this);
			}
		}
	}
}
