using System;

namespace GMTK_2026
{
	public sealed class CelestialBodyAspect : EntityAspect
	{
		private CelestialBodyAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly CelestialBodyAspect Star = Build("Star", "Blazing stellar body.", c =>
		{
			c.Provides.Add(EnvironmentTag.Heat);
			c.Provides.Add(EnvironmentTag.Radiation);
		});

		public static readonly CelestialBodyAspect GasGiant = Build("Gas Giant", "Crushing gaseous world.", c =>
		{
			c.Provides.Add(EnvironmentTag.Pressure);
		});

		private static CelestialBodyAspect Build(string name, string description, Action<CelestialBodyAspect> configure)
		{
			CelestialBodyAspect body = new CelestialBodyAspect(name, description);
			configure(body);
			return body;
		}
	}
}
