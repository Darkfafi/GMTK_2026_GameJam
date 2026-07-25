using RaDataHolder;
using UnityEngine;

namespace GMTK_2026
{
	public class RaFileSystemController : RaMonoDataHolderBase<RaFolder>
	{
		[SerializeField]
		private RaFileSystemUIWindow _uiWindow = null;

		public RaFolder RootFolder => Data;

		protected override void OnSetData()
		{
			_uiWindow.FileOpenedEvent += OnFileOpenedEvent;
			_uiWindow.SetRootFolder(Data);
		}

		protected override void OnClearData()
		{
			_uiWindow.FileOpenedEvent -= OnFileOpenedEvent;
		}

		private void OnFileOpenedEvent(RaFileSystemItemBase item)
		{
			Debug.Log("Action Performed on: " + item.Name);
		}
	}
}
