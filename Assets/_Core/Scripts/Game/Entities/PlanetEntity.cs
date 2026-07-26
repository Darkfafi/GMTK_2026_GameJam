using System.Collections.Generic;

namespace GMTK_2026
{
	public class PlanetEntity : GameEntityBase
	{
		public HashSet<TagBase> Provides => Profile.Provides;
		public CelestialBodyAspect Body => GetAspect<CelestialBodyAspect>();
		public EnvironmentProfile Environment => Body?.Environment;

		public PlanetEntity(string name, params TagBase[] provides)
			: base(name)
		{
			Profile.Provides.UnionWith(provides);
		}
	}
}
