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
		private readonly Label _size;

		public RaFileSystemItemBase Entity { get; private set; }

		public RaEntityElement()
		{
			AddToClassList(UssClass);

			_icon = new Label { pickingMode = PickingMode.Ignore };
			_icon.AddToClassList("ra-entity__icon");

			_label = new Label { pickingMode = PickingMode.Ignore };
			_label.AddToClassList("ra-entity__label");

			_size = new Label { pickingMode = PickingMode.Ignore };
			_size.AddToClassList("ra-entity__size");

			Add(_icon);
			Add(_label);
			Add(_size);

			RegisterCallback<ClickEvent>(OnClickedEvent);
		}

		public void Bind(RaFileSystemItemBase entity)
		{
			Entity = entity;
			_label.text = entity?.Name ?? string.Empty;

			bool isFolder = entity is RaFolder;
			bool isImage = !isFolder && IsImage(entity?.Name);

			_icon.text = isFolder ? "📁" : (isImage ? "🖼" : "📄");
			_icon.EnableInClassList("ra-entity__icon--folder", isFolder);
			_icon.EnableInClassList("ra-entity__icon--image", isImage);
			_icon.EnableInClassList("ra-entity__icon--file", !isFolder && !isImage);

			_size.text = entity is RaFile file ? file.Size : string.Empty;

			EnableInClassList(FolderModifier, isFolder);
			SetSelected(false);
		}

		private static bool IsImage(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			return name.EndsWith(".jpg") || name.EndsWith(".jpeg") || name.EndsWith(".png");
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
