using System.Collections.Generic;

namespace GMTK_2026
{
	public abstract class GameEntityBase
	{
		private readonly List<EntityAspect> _aspects = new List<EntityAspect>();

		public string Name
		{
			get; private set;
		}

		public TagProfile Profile { get; } = new TagProfile();

		public IReadOnlyList<EntityAspect> Aspects => _aspects;

		protected GameEntityBase(string name)
		{
			Name = name;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public void ApplyAspect(EntityAspect aspect)
		{
			if (aspect == null)
			{
				return;
			}

			_aspects.Add(aspect);
			aspect.ApplyTo(Profile);
		}
	}

	public class PlanetEntity : GameEntityBase
	{
		public HashSet<TagBase> Provides => Profile.Provides;

		public PlanetEntity(string name, params TagBase[] provides)
			: base(name)
		{
			Profile.Provides.UnionWith(provides);
		}
	}

	public class CreatureEntity : GameEntityBase
	{
		public HashSet<TagBase> Requires => Profile.Requires;
		public HashSet<TagBase> Intolerances => Profile.Intolerances;

		public CreatureEntity(string name)
			: base(name)
		{
		}
	}

	public class ShipEntity : GameEntityBase
	{
		public HashSet<TagBase> LifeSupport => Profile.Provides;

		public ShipEntity(string name, params TagBase[] lifeSupport)
			: base(name)
		{
			Profile.Provides.UnionWith(lifeSupport);
		}
	}
}
