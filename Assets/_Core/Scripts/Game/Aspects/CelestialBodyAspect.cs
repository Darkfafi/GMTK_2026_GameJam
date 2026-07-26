namespace GMTK_2026
{
	public enum BodyCategory
	{
		Terrestrial,
		GasOrIceGiant,
		OtherBody,
	}

	public sealed class CelestialBodyAspect : EntityAspect
	{
		public EnvironmentProfile Environment { get; } = new EnvironmentProfile();
		public BodyCategory Category { get; private set; }

		private CelestialBodyAspect(string name, string description = "")
			: base(name, description)
		{
		}

		// --- Terrestrial ---
		public static readonly CelestialBodyAspect Mercury = Build("Mercury", "Scorched, airless rock closest to the star.", BodyCategory.Terrestrial,
			pressure: 12f, gravity: 3.7f, low: -180f, high: 430f, average: 167f,
			composition: new[] { EnvironmentTag.Sodium, EnvironmentTag.Oxygen, EnvironmentTag.Hydrogen, EnvironmentTag.SolarRadiation });

		public static readonly CelestialBodyAspect Venus = Build("Venus", "Crushing greenhouse furnace under sulfuric cloud.", BodyCategory.Terrestrial,
			pressure: 90f, gravity: 8.8f, low: 438f, high: 482f, average: 464f,
			composition: new[] { EnvironmentTag.CarbonDioxide, EnvironmentTag.Sulfur });

		public static readonly CelestialBodyAspect Earth = Build("Earth", "Temperate ocean world. Station Alpha's home system anchor.", BodyCategory.Terrestrial,
			pressure: 1f, gravity: 9.8f, low: -89.2f, high: 56.7f, average: 15f,
			composition: new[] { EnvironmentTag.Nitrogen, EnvironmentTag.Oxygen, EnvironmentTag.Water });

		public static readonly CelestialBodyAspect Mars = Build("Mars", "Cold iron desert with a whisper of atmosphere.", BodyCategory.Terrestrial,
			pressure: 0.0006f, gravity: 3.7f, low: -143f, high: 35f, average: -62f,
			composition: new[] { EnvironmentTag.CarbonDioxide, EnvironmentTag.SubsurfaceIron });

		// --- Gas and Ice Giants ---
		public static readonly CelestialBodyAspect Jupiter = Build("Jupiter", "Immense hydrogen giant. No surface, only depth.", BodyCategory.GasOrIceGiant,
			pressure: 1200f, gravity: 24.8f, low: -145f, high: -145f, average: -110f,
			composition: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium });

		public static readonly CelestialBodyAspect Saturn = Build("Saturn", "Ringed giant scoured by the fastest winds known.", BodyCategory.GasOrIceGiant,
			pressure: 1100f, gravity: 10.45f, low: -180f, high: -180f, average: -140f,
			composition: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.KineticWindEnergy });

		public static readonly CelestialBodyAspect Uranus = Build("Uranus", "Tilted ice giant, blue with methane.", BodyCategory.GasOrIceGiant,
			pressure: 1000f, gravity: 8.7f, low: -224f, high: -224f, average: -195f,
			composition: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium, EnvironmentTag.MethaneIce });

		public static readonly CelestialBodyAspect Neptune = Build("Neptune", "Storm-locked ice giant in permanent electrical fury.", BodyCategory.GasOrIceGiant,
			pressure: 1050f, gravity: 11.15f, low: -218f, high: -218f, average: -200f,
			composition: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium, EnvironmentTag.AtmosphericStatic });

		// --- Other Bodies ---
		public static readonly CelestialBodyAspect Titan = Build("Titan", "Hazy moon with rivers and lakes of methane.", BodyCategory.OtherBody,
			pressure: 1.45f, gravity: 1.3f, low: -180f, high: -179f, average: -179.6f,
			composition: new[] { EnvironmentTag.Nitrogen, EnvironmentTag.LiquidHydrocarbons });

		public static readonly CelestialBodyAspect Pluto = Build("Pluto", "Distant dwarf world with a geologically warm heart.", BodyCategory.OtherBody,
			pressure: 0.000003f, gravity: 0.6f, low: -240f, high: -218f, average: -232f,
			composition: new[] { EnvironmentTag.Nitrogen, EnvironmentTag.GeothermalHeat });

		private static CelestialBodyAspect Build(string name, string description, BodyCategory category,
			float pressure, float gravity, float low, float high, float average, TagBase[] composition)
		{
			CelestialBodyAspect body = new CelestialBodyAspect(name, description);
			body.Category = category;
			body.Environment.Pressure = pressure;
			body.Environment.Gravity = gravity;
			body.Environment.LowestTemperature = low;
			body.Environment.HighestTemperature = high;
			body.Environment.AverageTemperature = average;

			foreach (TagBase tag in composition)
			{
				body.Environment.Composition.Add(tag);
				body.Provides.Add(tag);
			}
			return body;
		}
	}
}
