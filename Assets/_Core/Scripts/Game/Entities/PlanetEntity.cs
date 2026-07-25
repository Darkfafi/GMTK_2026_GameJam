using System.Collections.Generic;

namespace GMTK_2026
{
	public class PlanetEntity : GameEntityBase
	{
		public HashSet<TagBase> Provides => Profile.Provides;

		public PlanetEntity(string name, params TagBase[] provides)
			: base(name)
		{
			Profile.Provides.UnionWith(provides);
		}
	}
}
