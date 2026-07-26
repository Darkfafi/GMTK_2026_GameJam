using System.Collections.Generic;

namespace GMTK_2026
{
	public class ShipEntity : GameEntityBase
	{
		public HashSet<TagBase> LifeSupport => Profile.Provides;

		public ShipAspect Class => GetAspect<ShipAspect>();

		public ShipEntity(string name, params TagBase[] lifeSupport)
			: base(name)
		{
			Profile.Provides.UnionWith(lifeSupport);
		}
	}
}
