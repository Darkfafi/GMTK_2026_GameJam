using System.Collections.Generic;

namespace GMTK_2026
{
	public abstract class GameEntityBase
	{
		public string Name
		{
			get; private set;
		}

		protected GameEntityBase(string name)
		{
			Name = name;
		}

		public void SetName(string name)
		{
			Name = name;
		}
	}

	public class PlanetEntity : GameEntityBase
	{
		public HashSet<TagBase> Provides { get; } = new HashSet<TagBase>();

		public PlanetEntity(string name, params TagBase[] provides)
			: base(name)
		{
			Provides.UnionWith(provides);
		}
	}

	public class CreatureEntity : GameEntityBase
	{
		public HashSet<TagBase> Requires { get; } = new HashSet<TagBase>();
		public HashSet<TagBase> Intolerances { get; } = new HashSet<TagBase>();

		public CreatureEntity(string name)
			: base(name)
		{
		}
	}

	public class ShipEntity : GameEntityBase
	{
		public HashSet<TagBase> LifeSupport { get; } = new HashSet<TagBase>();

		public ShipEntity(string name, params TagBase[] lifeSupport)
			: base(name)
		{
			LifeSupport.UnionWith(lifeSupport);
		}
	}
}
