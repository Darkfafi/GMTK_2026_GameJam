using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	[RequireComponent(typeof(UIDocument))]
	public class RaFileSystemWindow : MonoBehaviour
	{
		public event Action<RaFileSystemItemBase> FileOpenedEvent;
		public event Action<RaFolder> FolderChangedEvent;

		private UIDocument _document;

		private Button _backButton;
		private VisualElement _breadcrumb;
		private ScrollView _contents;
		private Label _statusLabel;

		private readonly List<RaFolder> _path = new List<RaFolder>();
		private readonly List<RaEntityElement> _rowPool = new List<RaEntityElement>();

		private RaEntityElement _selected;

		public RaFolder CurrentFolder => _path.Count > 0 ? _path[^1] : null;

		private void OnEnable()
		{
			_document = GetComponent<UIDocument>();
			VisualElement root = _document.rootVisualElement;

			_backButton = root.Q<Button>("ra-back-button");
			_breadcrumb = root.Q<VisualElement>("ra-breadcrumb");
			_contents = root.Q<ScrollView>("ra-contents");
			_statusLabel = root.Q<Label>("ra-status-label");

			if (_backButton != null)
			{
				_backButton.clicked += NavigateUp;
			}

			if (_path.Count > 0)
			{
				Render();
			}
		}

		private void OnDisable()
		{
			if (_backButton != null)
				_backButton.clicked -= NavigateUp;
		}

		public void SetRootFolder(RaFolder rootFolder)
		{
			_path.Clear();
			if (rootFolder != null)
			{
				_path.Add(rootFolder);
			}
			_selected = null;
			Render();
		}

		public void NavigateInto(RaFolder folder)
		{
			if (folder == null)
			{
				return;
			}

			_path.Add(folder);
			_selected = null;
			Render();
			FolderChangedEvent?.Invoke(folder);
		}

		public void NavigateUp()
		{
			if (_path.Count <= 1)
			{
				return;
			}

			_path.RemoveAt(_path.Count - 1);
			_selected = null;
			Render();
			FolderChangedEvent?.Invoke(CurrentFolder);
		}

		public void NavigateToDepth(int depth)
		{
			if (depth < 0 || depth >= _path.Count - 1)
				return;

			_path.RemoveRange(depth + 1, _path.Count - depth - 1);
			_selected = null;
			Render();
			FolderChangedEvent?.Invoke(CurrentFolder);
		}

		private void Render()
		{
			if (_contents == null)
			{
				return;
			}

			RenderBreadcrumb();
			RenderContents();

			if (_backButton != null)
			{
				_backButton.SetEnabled(_path.Count > 1);
			}
		}

		private void RenderBreadcrumb()
		{
			_breadcrumb.Clear();

			for (int i = 0; i < _path.Count; i++)
			{
				bool isCurrent = i == _path.Count - 1;

				var crumb = new Button { text = _path[i].Name };
				crumb.AddToClassList("ra-crumb");
				if (isCurrent)
				{
					crumb.AddToClassList("ra-crumb--current");
				}
				else
				{
					int depth = i;
					crumb.clicked += () => NavigateToDepth(depth);
				}

				_breadcrumb.Add(crumb);

				if (!isCurrent)
				{
					var sep = new Label("›");
					sep.AddToClassList("ra-crumb-separator");
					_breadcrumb.Add(sep);
				}
			}
		}

		private void RenderContents()
		{
			_contents.Clear();

			IReadOnlyList<RaFileSystemItemBase> children = CurrentFolder?.Children;
			int count = children?.Count ?? 0;

			if (count == 0)
			{
				var empty = new Label("This folder is empty");
				empty.AddToClassList("ra-empty-label");
				_contents.Add(empty);
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					RaEntityElement row = GetRow(i);
					row.Bind(children[i]);
					_contents.Add(row);
				}
			}

			if (_statusLabel != null)
			{
				_statusLabel.text = count == 1 ? "1 item" : $"{count} items";
			}
		}

		private RaEntityElement GetRow(int index)
		{
			while (_rowPool.Count <= index)
			{
				var row = new RaEntityElement();
				row.ClickedEvent += OnRowClicked;
				row.ActivatedEvent += OnRowActivated;
				_rowPool.Add(row);
			}
			return _rowPool[index];
		}

		private void OnRowClicked(RaEntityElement row)
		{
			_selected?.SetSelected(false);
			_selected = row;
			row.SetSelected(true);
		}

		private void OnRowActivated(RaEntityElement row)
		{
			RaFileSystemItemBase entity = row.Entity;
			if (entity == null)
				return;

			if (entity is RaFolder raFolder)
			{
				NavigateInto(raFolder);
			}
			else
			{
				FileOpenedEvent?.Invoke(entity);
			}
		}
	}
}
