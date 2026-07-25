using System.Collections.Generic;

namespace GMTK_2026
{
	public class TagProfile
	{
		public HashSet<TagBase> Requires { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Intolerances { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Provides { get; } = new HashSet<TagBase>();
	}

	public abstract class EntityAspect
	{
		public string Name { get; }
		public string Description { get; }

		public HashSet<TagBase> Requires { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Intolerances { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Provides { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Removes { get; } = new HashSet<TagBase>();

		protected EntityAspect(string name, string description = "")
		{
			Name = name;
			Description = description;
		}

		public void ApplyTo(TagProfile profile)
		{
			profile.Requires.UnionWith(Requires);
			profile.Intolerances.UnionWith(Intolerances);
			profile.Provides.UnionWith(Provides);

			foreach (TagBase tag in Removes)
			{
				profile.Requires.Remove(tag);
				profile.Intolerances.Remove(tag);
				profile.Provides.Remove(tag);
			}
		}
	}

	public static class GameEntityExtensions
	{
		public static T Apply<T>(this T entity, EntityAspect aspect)
			where T : GameEntityBase
		{
			entity.ApplyAspect(aspect);
			return entity;
		}
	}
}
