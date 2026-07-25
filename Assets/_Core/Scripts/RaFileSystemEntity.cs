using System.Collections;
using System.Collections.Generic;

namespace GMTK_2026
{
	public class RaFolder : RaFileSystemEntity, IList<RaFileSystemEntity>
	{
		private List<RaFileSystemEntity> _children;

		public IReadOnlyList<RaFileSystemEntity> Children => _children;

		public int Count => _children.Count;
		public bool IsReadOnly => false;

		public RaFileSystemEntity this[int index] { get => _children[index]; set => _children[index] = value; }

		public RaFolder(string name, params RaFileSystemEntity[] children)
			: base(name)
		{
			_children = new List<RaFileSystemEntity>(children);
		}

		public int IndexOf(RaFileSystemEntity item)
		{
			return _children.IndexOf(item);
		}

		public void Insert(int index, RaFileSystemEntity item)
		{
			_children.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_children.RemoveAt(index);
		}

		public void Add(RaFileSystemEntity item)
		{
			_children.Add(item);
		}

		public void Clear()
		{
			_children.Clear();
		}

		public bool Contains(RaFileSystemEntity item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(RaFileSystemEntity[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public bool Remove(RaFileSystemEntity item)
		{
			return _children.Remove(item);
		}

		public IEnumerator<RaFileSystemEntity> GetEnumerator() => _children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class RaFile : RaFileSystemEntity
	{
		public RaFile(string name)
			: base(name)
		{
		}
	}

	public abstract class RaFileSystemEntity
	{
		public string Name
		{
			get; private set;
		}

		public RaFileSystemEntity(string name)
		{
			Name = name;
		}

		public void SetName(string name)
		{
			Name = name;
		}
	}
}