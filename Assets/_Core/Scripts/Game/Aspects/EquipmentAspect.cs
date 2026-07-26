using System.Collections.Generic;
using System.Linq;

namespace GMTK_2026
{
	public sealed class EquipmentAspect : EntityAspect
	{
		public SurvivalEnvelope Envelope { get; } = new SurvivalEnvelope();

		public HashSet<SpeciesAspect> EquippableBy { get; } = new HashSet<SpeciesAspect>();

		private EquipmentAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public bool CanBeEquippedBy(SpeciesAspect species)
			=> species != null && EquippableBy.Contains(species);

		public static readonly EquipmentAspect EvaSuit = Build(
			"Standard EVA Space Suit", "Sealed life-support shell with mag-boots for vacuum work.",
			new[] { SpeciesAspect.Humans },
			pressure: new FloatRange(0f, 3f), gravity: new FloatRange(0f, 15f), temperature: new FloatRange(-200f, 120f),
			fulfills: new[] { EnvironmentTag.Oxygen, EnvironmentTag.Water });

		public static readonly EquipmentAspect ExtremeHazardSuit = Build(
			"Extreme Hazard Suit", "Armoured rig with toxic gas filtration for furnace worlds.",
			new[] { SpeciesAspect.Humans },
			pressure: new FloatRange(0.1f, 100f), gravity: new FloatRange(0f, 15f), temperature: new FloatRange(-100f, 500f),
			fulfills: new[] { EnvironmentTag.Oxygen, EnvironmentTag.Water });

		public static readonly EquipmentAspect Rebreather = Build(
			"Rebreather & Oxygen Mask", "Breathing apparatus only — offers no pressure or thermal protection.",
			new[] { SpeciesAspect.Humans },
			pressure: null, gravity: null, temperature: null,
			fulfills: new[] { EnvironmentTag.Oxygen });

		public static readonly EquipmentAspect ContainmentRig = Build(
			"Deep-Atmosphere Containment Rig", "Internal high-pressure vessel preventing explosive decompression.",
			new[] { SpeciesAspect.Jovians, SpeciesAspect.Nyxs, SpeciesAspect.Buttcolds, SpeciesAspect.Olympians },
			pressure: new FloatRange(0.0001f, 20f), gravity: new FloatRange(0f, 30f), temperature: new FloatRange(-250f, 100f),
			fulfills: new[] { EnvironmentTag.Hydrogen, EnvironmentTag.Helium });

		public static readonly EquipmentAspect CompressionShell = Build(
			"Low-Pressure Compression Shell", "Flexible pressurised suit that stops thin-air life from being crushed.",
			new[] { SpeciesAspect.Dustcrawls, SpeciesAspect.Plutonians },
			pressure: new FloatRange(0.1f, 5f), gravity: new FloatRange(0f, 12f), temperature: new FloatRange(-250f, 50f),
			fulfills: new[] { EnvironmentTag.CarbonDioxide, EnvironmentTag.Nitrogen });

		public static readonly EquipmentAspect ThermalExoskeleton = Build(
			"Thermal-Absorptive Exoskeleton", "Radiative frame with an internal heating array for heat-eaters in the cold.",
			new[] { SpeciesAspect.Solarians, SpeciesAspect.Lucifers },
			pressure: new FloatRange(0.5f, 100f), gravity: new FloatRange(0f, 12f), temperature: new FloatRange(-150f, 500f),
			fulfills: new[] { EnvironmentTag.GeothermalHeat, EnvironmentTag.SolarRadiation });

		public static readonly EquipmentAspect CryoRecirculator = Build(
			"Cryo-Fluid Recirculator", "Cooling and chemical feed rig for cold-world natives in warmer zones.",
			new[] { SpeciesAspect.Titans, SpeciesAspect.Buttcolds, SpeciesAspect.Plutonians },
			pressure: new FloatRange(0.000001f, 10f), gravity: new FloatRange(0f, 12f), temperature: new FloatRange(-250f, -50f),
			fulfills: new[] { EnvironmentTag.LiquidHydrocarbons, EnvironmentTag.Nitrogen, EnvironmentTag.MethaneIce });

		public static readonly EquipmentAspect DynamoEngine = Build(
			"Kinetic-Static Dynamo Engine", "Harness generating synthetic turbulence and electrical friction.",
			new[] { SpeciesAspect.Nyxs, SpeciesAspect.Olympians },
			pressure: null, gravity: null, temperature: new FloatRange(-230f, -100f),
			fulfills: new[] { EnvironmentTag.KineticWindEnergy, EnvironmentTag.AtmosphericStatic });

		public static readonly EquipmentAspect VapourRebreather = Build(
			"Multi-Gas Vapour Rebreather", "Mask that synthesises trace atmosphere into concentrated heavy gases.",
			new[] { SpeciesAspect.Humans, SpeciesAspect.Lucifers, SpeciesAspect.Dustcrawls },
			pressure: null, gravity: null, temperature: null,
			fulfills: new[] { EnvironmentTag.Oxygen, EnvironmentTag.CarbonDioxide, EnvironmentTag.Sulfur });

		private static EquipmentAspect Build(string name, string description, SpeciesAspect[] equippableBy,
			FloatRange? pressure, FloatRange? gravity, FloatRange? temperature, TagBase[] fulfills)
		{
			EquipmentAspect equipment = new EquipmentAspect(name, description);
			equipment.Envelope.Pressure = pressure;
			equipment.Envelope.Gravity = gravity;
			equipment.Envelope.Temperature = temperature;

			foreach (SpeciesAspect species in equippableBy)
			{
				equipment.EquippableBy.Add(species);
			}
			foreach (TagBase tag in fulfills)
			{
				equipment.Envelope.Requirements.Add(tag);
				equipment.Provides.Add(tag);
			}
			return equipment;
		}

		public string DescribeEquippableBy()
			=> string.Join(", ", EquippableBy.Select(s => s.Name));
	}
}
