using System;

namespace GMTK_2026
{
	public sealed class SpeciesAspect : EntityAspect
	{
		public SurvivalEnvelope Envelope { get; } = new SurvivalEnvelope();

		public string Origin { get; private set; }

		private SpeciesAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly SpeciesAspect Humans = Build("Humans", "Bipedal carbon-based natives of Earth.", "Earth",
			pressure: new FloatRange(0.5f, 1.5f), gravity: new FloatRange(3.7f, 12f), temperature: new FloatRange(4f, 35f),
			requirements: new[] { EnvironmentTag.Oxygen, EnvironmentTag.Water });

		public static readonly SpeciesAspect Solarians = Build("Solarians", "Heat-eating silicate life that basks in raw starlight.", "Mercury",
			pressure: new FloatRange(8f, 15f), gravity: new FloatRange(2f, 5f), temperature: new FloatRange(-185f, 450f),
			requirements: new[] { EnvironmentTag.Sodium, EnvironmentTag.SolarRadiation });

		public static readonly SpeciesAspect Lucifers = Build("Lucifers", "Pressure-hardened acid dwellers from the cloud decks of Venus.", "Venus",
			pressure: new FloatRange(80f, 100f), gravity: new FloatRange(7f, 10f), temperature: new FloatRange(420f, 500f),
			requirements: new[] { EnvironmentTag.CarbonDioxide, EnvironmentTag.Sulfur });

		public static readonly SpeciesAspect Dustcrawls = Build("Dustcrawls", "Near-vacuum burrowers that metabolise iron dust.", "Mars",
			pressure: new FloatRange(0.0001f, 0.001f), gravity: new FloatRange(2.5f, 5f), temperature: new FloatRange(-150f, 40f),
			requirements: new[] { EnvironmentTag.CarbonDioxide, EnvironmentTag.SubsurfaceIron });

		public static readonly SpeciesAspect Jovians = Build("Jovians", "Colossal deep-cloud swimmers built for crushing pressure.", "Jupiter",
			pressure: new FloatRange(800f, 1500f), gravity: new FloatRange(20f, 30f), temperature: new FloatRange(-160f, -100f),
			requirements: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium });

		public static readonly SpeciesAspect Nyxs = Build("Nyxs", "Storm-riders that feed on wind shear.", "Saturn",
			pressure: new FloatRange(800f, 1200f), gravity: new FloatRange(8f, 12f), temperature: new FloatRange(-190f, -130f),
			requirements: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.KineticWindEnergy });

		public static readonly SpeciesAspect Buttcolds = Build("Buttcolds", "Methane-ice grazers of the frozen giants.", "Uranus",
			pressure: new FloatRange(800f, 1200f), gravity: new FloatRange(6.5f, 10.5f), temperature: new FloatRange(-230f, -180f),
			requirements: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium, EnvironmentTag.MethaneIce });

		public static readonly SpeciesAspect Olympians = Build("Olympians", "Charge-drinkers that live inside permanent lightning.", "Neptune",
			pressure: new FloatRange(800f, 1300f), gravity: new FloatRange(9f, 13f), temperature: new FloatRange(-225f, -190f),
			requirements: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.AtmosphericStatic });

		public static readonly SpeciesAspect Titans = Build("Titans", "Low-gravity waders of the hydrocarbon lakes.", "Titan",
			pressure: new FloatRange(1f, 2f), gravity: new FloatRange(0.5f, 2.5f), temperature: new FloatRange(-190f, -170f),
			requirements: new[] { EnvironmentTag.Nitrogen, EnvironmentTag.LiquidHydrocarbons });

		public static readonly SpeciesAspect Plutonians = Build("Plutonians", "Vacuum-thin crystalline life warmed from below.", "Pluto",
			pressure: new FloatRange(0.000001f, 0.00001f), gravity: new FloatRange(0.2f, 1f), temperature: new FloatRange(-240f, -210f),
			requirements: new[] { EnvironmentTag.Nitrogen, EnvironmentTag.GeothermalHeat });

		private static SpeciesAspect Build(string name, string description, string origin,
			FloatRange pressure, FloatRange gravity, FloatRange temperature, TagBase[] requirements)
		{
			SpeciesAspect species = new SpeciesAspect(name, description);
			species.Origin = origin;
			species.Envelope.Pressure = pressure;
			species.Envelope.Gravity = gravity;
			species.Envelope.Temperature = temperature;

			foreach (TagBase requirement in requirements)
			{
				species.Envelope.Requirements.Add(requirement);
				species.Requires.Add(requirement);
			}
			return species;
		}
	}
}
