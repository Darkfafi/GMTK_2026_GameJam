using System;
using System.Collections.Generic;

namespace GMTK_2026
{

	public abstract class TagBase : IEquatable<TagBase>
	{
		public string Id { get; }
		public string Name { get; }
		public string Description { get; }

		protected TagBase(string displayName, string description = "")
		{
			Id = Guid.NewGuid().ToString();
			Name = displayName;
			Description = description;
		}

		public bool Equals(TagBase other)
			=> other != null && GetType() == other.GetType() && Id == other.Id;

		public override bool Equals(object obj) => Equals(obj as TagBase);
		public override int GetHashCode() => (GetType().FullName + "::" + Id).GetHashCode();
		public override string ToString() => Name;
	}

	public static class TagExtensions
	{
		public static bool Has<T>(this IEnumerable<TagBase> tags)
			where T : TagBase
		{
			foreach (TagBase tag in tags)
			{
				if (tag is T)
					return true;
			}
			return false;
		}
	}
}
