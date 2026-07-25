using System.Collections;
using System.Collections.Generic;

namespace GMTK_2026
{
	public class RaFolder : RaFileSystemItemBase, IList<RaFileSystemItemBase>
	{
		private List<RaFileSystemItemBase> _children;

		public IReadOnlyList<RaFileSystemItemBase> Children => _children;

		public int Count => _children.Count;
		public bool IsReadOnly => false;

		public RaFileSystemItemBase this[int index] { get => _children[index]; set => _children[index] = value; }

		public RaFolder(string name, params RaFileSystemItemBase[] children)
			: base(name)
		{
			_children = new List<RaFileSystemItemBase>(children);
		}

		public int IndexOf(RaFileSystemItemBase item)
		{
			return _children.IndexOf(item);
		}

		public void Insert(int index, RaFileSystemItemBase item)
		{
			_children.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_children.RemoveAt(index);
		}

		public void Add(RaFileSystemItemBase item)
		{
			_children.Add(item);
		}

		public void Clear()
		{
			_children.Clear();
		}

		public bool Contains(RaFileSystemItemBase item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(RaFileSystemItemBase[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public bool Remove(RaFileSystemItemBase item)
		{
			return _children.Remove(item);
		}

		public IEnumerator<RaFileSystemItemBase> GetEnumerator() => _children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class RaFile : RaFileSystemItemBase
	{
		public RaFile(string name)
			: base(name)
		{
		}
	}

	public abstract class RaFileSystemItemBase
	{
		public string Name
		{
			get; private set;
		}

		public RaFileSystemItemBase(string name)
		{
			Name = name;
		}

		public void SetName(string name)
		{
			Name = name;
		}
	}
}