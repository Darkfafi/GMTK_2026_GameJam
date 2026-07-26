using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace GMTK_2026
{
	public class RaFileSystemUIWindow : MonoBehaviour
	{
		public event Action<RaFileSystemItemBase> FileOpenedEvent;
		public event Action<RaFolder> FolderChangedEvent;

		[SerializeField]
		private UIDocument _document;

		private Button _backButton;
		private VisualElement _breadcrumb;
		private ScrollView _contents;
		private Label _statusLabel;
		private VisualElement _fileContent;
		private Label _fcText;
		private bool _isFileContentOpen;
		private RaFile _openFile;
		private readonly List<(List<RaFolder> Path, RaFile File)> _linkHistory = new List<(List<RaFolder>, RaFile)>();

		private readonly List<RaFolder> _path = new List<RaFolder>();
		private readonly List<RaEntityElement> _rowPool = new List<RaEntityElement>();

		private RaEntityElement _selected;

		public RaFolder CurrentFolder => _path.Count > 0 ? _path[^1] : null;

		private void OnEnable()
		{
			VisualElement root = _document.rootVisualElement;

			_backButton = root.Q<Button>("ra-back-button");
			_breadcrumb = root.Q<VisualElement>("ra-breadcrumb");
			_contents = root.Q<ScrollView>("ra-contents");
			_statusLabel = root.Q<Label>("ra-status-label");
			_fileContent = root.Q<VisualElement>("ra-file-content");
			_fcText = root.Q<Label>("fc-text");

			if (_backButton != null)
			{
				_backButton.clicked += OnBackClicked;
			}

			if (_fcText != null)
			{
				_fcText.RegisterCallback<PointerUpLinkTagEvent>(OnLinkClicked);
			}

			CloseFileContent();

			if (_path.Count > 0)
			{
				Render();
			}
		}

		private void OnDisable()
		{
			if (_backButton != null)
				_backButton.clicked -= OnBackClicked;

			if (_fcText != null)
				_fcText.UnregisterCallback<PointerUpLinkTagEvent>(OnLinkClicked);
		}

		private void OnLinkClicked(PointerUpLinkTagEvent evt)
		{
			OpenLink(evt.linkID);
		}

		public void OpenLink(string target)
		{
			if (string.IsNullOrEmpty(target) || _path.Count == 0)
			{
				return;
			}

			List<RaFolder> chain = new List<RaFolder> { _path[0] };
			RaFile file = FindFile(_path[0], target, chain);
			if (file == null)
			{
				return;
			}

			_linkHistory.Add((new List<RaFolder>(_path), _openFile));

			_path.Clear();
			_path.AddRange(chain);
			_selected = null;
			Render();
			OpenFileContent(file);
			FolderChangedEvent?.Invoke(CurrentFolder);
		}

		private static RaFile FindFile(RaFolder folder, string target, List<RaFolder> chain)
		{
			int slash = target.IndexOf('/');
			if (slash >= 0)
			{
				string head = target.Substring(0, slash);
				string rest = target.Substring(slash + 1);
				for (int i = 0; i < folder.Children.Count; i++)
				{
					if (folder.Children[i] is RaFolder sub &&
						string.Equals(sub.Name, head, StringComparison.OrdinalIgnoreCase))
					{
						chain.Add(sub);
						RaFile viaPath = FindFile(sub, rest, chain);
						if (viaPath != null)
						{
							return viaPath;
						}
						chain.RemoveAt(chain.Count - 1);
					}
				}
				return null;
			}

			for (int i = 0; i < folder.Children.Count; i++)
			{
				if (folder.Children[i] is RaFile file &&
					string.Equals(file.Name, target, StringComparison.OrdinalIgnoreCase))
				{
					return file;
				}
			}

			for (int i = 0; i < folder.Children.Count; i++)
			{
				if (folder.Children[i] is RaFolder sub)
				{
					chain.Add(sub);
					RaFile found = FindFile(sub, target, chain);
					if (found != null)
					{
						return found;
					}
					chain.RemoveAt(chain.Count - 1);
				}
			}

			return null;
		}

		private void OnBackClicked()
		{
			if (_linkHistory.Count > 0)
			{
				(List<RaFolder> path, RaFile file) = _linkHistory[^1];
				_linkHistory.RemoveAt(_linkHistory.Count - 1);

				_path.Clear();
				_path.AddRange(path);
				_selected = null;
				Render();
				if (file != null)
				{
					OpenFileContent(file);
				}
				FolderChangedEvent?.Invoke(CurrentFolder);
				return;
			}

			if (_isFileContentOpen)
			{
				CloseFileContent();
			}
			else
			{
				NavigateUp();
			}
		}

		public void OpenFileContent(RaFile file)
		{
			if (_fileContent == null || file == null)
			{
				return;
			}

			_isFileContentOpen = true;
			_openFile = file;
			_fcText.text = TerminalMarkdown.ToRichText(file.Content);
			_fileContent.style.display = DisplayStyle.Flex;
			_contents.style.display = DisplayStyle.None;
			if (_statusLabel != null)
			{
				_statusLabel.parent.style.display = DisplayStyle.None;
			}
			RenderBreadcrumb();
			RefreshBackButton();
		}

		public void CloseFileContent()
		{
			if (_fileContent == null)
			{
				return;
			}

			bool wasOpen = _isFileContentOpen;
			_isFileContentOpen = false;
			_openFile = null;
			_fileContent.style.display = DisplayStyle.None;
			_contents.style.display = DisplayStyle.Flex;
			if (_statusLabel != null)
			{
				_statusLabel.parent.style.display = DisplayStyle.Flex;
			}
			if (wasOpen)
			{
				RenderBreadcrumb();
			}
			RefreshBackButton();
		}

		private void RefreshBackButton()
		{
			if (_backButton != null)
			{
				_backButton.SetEnabled(_path.Count > 1 || _isFileContentOpen || _linkHistory.Count > 0);
			}
		}

		public void SetRootFolder(RaFolder rootFolder)
		{
			_linkHistory.Clear();
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

			_linkHistory.Clear();
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

			_linkHistory.Clear();
			_path.RemoveAt(_path.Count - 1);
			_selected = null;
			Render();
			FolderChangedEvent?.Invoke(CurrentFolder);
		}

		public void NavigateToDepth(int depth)
		{
			if (depth < 0 || depth >= _path.Count - 1)
				return;

			_linkHistory.Clear();
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

			CloseFileContent();
			RenderBreadcrumb();
			RenderContents();
			RefreshBackButton();
		}

		private void RenderBreadcrumb()
		{
			_breadcrumb.Clear();

			for (int i = 0; i < _path.Count; i++)
			{
				bool isCurrent = i == _path.Count - 1 && _openFile == null;

				var crumb = new Button { text = _path[i].Name };
				crumb.AddToClassList("ra-crumb");
				if (isCurrent)
				{
					crumb.AddToClassList("ra-crumb--current");
				}
				else if (_openFile != null && i == _path.Count - 1)
				{
					crumb.clicked += () =>
					{
						_linkHistory.Clear();
						CloseFileContent();
					};
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

			if (_openFile != null)
			{
				var fileCrumb = new Button { text = _openFile.Name };
				fileCrumb.AddToClassList("ra-crumb");
				fileCrumb.AddToClassList("ra-crumb--current");
				_breadcrumb.Add(fileCrumb);
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
			OnRowActivated(row);
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
				if (entity is RaFile raFile)
				{
					OpenFileContent(raFile);
				}
				FileOpenedEvent?.Invoke(entity);
			}
		}
	}
}
