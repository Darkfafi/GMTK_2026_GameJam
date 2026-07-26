using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMTK_2026
{
	public static class GameFilesBuilder
	{
		public static RaFolder BuildRoot()
		{
			List<RaFileSystemItemBase> speciesFiles = GameCatalog.Species
				.Select(species => MakeFile(Slug(species.Name), SpeciesPage(species)))
				.Cast<RaFileSystemItemBase>()
				.ToList();

			List<RaFileSystemItemBase> planetFiles = GameCatalog.CelestialBodies
				.Select(body => MakeFile(Slug(body.Name), PlanetPage(body)))
				.Cast<RaFileSystemItemBase>()
				.ToList();

			List<RaFileSystemItemBase> equipmentFiles = GameCatalog.Equipment
				.Select(equipment => MakeFile(Slug(equipment.Name), EquipmentPage(equipment)))
				.Cast<RaFileSystemItemBase>()
				.ToList();

			List<RaFileSystemItemBase> shipFiles = GameCatalog.ShipClasses
				.Select(shipClass => MakeFile(Slug(shipClass.Name), ShipPage(shipClass)))
				.Cast<RaFileSystemItemBase>()
				.ToList();

			return new RaFolder("Home",
				new RaFolder("Documents",
					MakeFile("landing_protocols.md", LandingProtocols()),
					MakeFile("species_registry.md", SpeciesIndex()),
					MakeFile("planetary_index.md", PlanetIndex()),
					MakeFile("equipment_registry.md", EquipmentIndex()),
					MakeFile("ship_registry.md", ShipIndex()),
					new RaFolder("Species", speciesFiles.ToArray()),
					new RaFolder("Planets", planetFiles.ToArray()),
					new RaFolder("Equipment", equipmentFiles.ToArray()),
					new RaFolder("Ships", shipFiles.ToArray())
				),
				new RaFolder("Photos",
					MakeFile("station_alpha.jpg", "[IMAGE: Orbital Station Alpha — modular ring structure, 12 docking bays]"),
					MakeFile("crew_2187.jpg", "[IMAGE: Station crew portrait — 12 members in uniform, dated 2187.01.15]")
				),
				MakeFile("readme.md", Readme())
			);
		}

		private static string LandingProtocols()
		{
			return
@"# Landing Protocols

A landing is cleared only when the pilot can survive the destination.
Four conditions are checked. ALL must pass.

## The Four Axes
- **Pressure** (atm) — the world's value must fall inside a range the pilot tolerates
- **Gravity** (m/s²) — same
- **Temperature** (°C) — judged on the world's AVERAGE temperature
- **Composition** — every substance the species REQUIRED must be present

## Procedure
- Identify the pilot's species in the [species registry](species_registry.md)
- Identify the destination in the [planetary index](planetary_index.md)
- Compare each axis against the species' natural tolerance
- If an axis falls outside it, check the pilot's gear in the [equipment registry](equipment_registry.md)
- Finally check the vessel against the [ship registry](ship_registry.md)

## Equipment Rules
- Gear covers an axis only if the world's value falls inside the GEAR's rating
- Gear listing no rating for an axis does not help there — the species' own limit applies
- Gear only works for the species it is certified for. Uncertified gear is DENIED as protection
- Gear may also supply missing composition requirements

## Hull Rules
- The ship must survive the descent on pressure, gravity and temperature
- Equipment does not protect a hull. You cannot put a suit on a freighter
- A surviving pilot in an under-rated hull is still DENIED

## Rulings
- Pilot survives all four axes AND the hull is rated: landing PERMITTED
- Anything uncovered: landing DENIED

A species can always survive its own origin world unaided — its ship may not.";
		}

		private static string SpeciesIndex()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Species Registry");
			sb.AppendLine();
			sb.AppendLine("Registered spacefaring species and their natural survival envelopes.");
			sb.AppendLine();

			foreach (SpeciesAspect species in GameCatalog.Species)
			{
				sb.AppendLine($"## [{species.Name}]({Slug(species.Name)})");
				sb.AppendLine($"- Origin: [{species.Origin}]({Slug(species.Origin)})");
				sb.AppendLine($"- Pressure: {Describe(species.Envelope.Pressure, "atm")}");
				sb.AppendLine($"- Gravity: {Describe(species.Envelope.Gravity, "m/s²")}");
				sb.AppendLine($"- Temperature: {Describe(species.Envelope.Temperature, "°C")}");
				sb.AppendLine($"- REQUIRED: {Tags(species.Envelope.Requirements)}");
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private static string PlanetIndex()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Planetary Index");
			sb.AppendLine();
			sb.AppendLine("Measured surface conditions for every registered body.");
			sb.AppendLine();

			foreach (BodyCategory category in new[] { BodyCategory.Terrestrial, BodyCategory.GasOrIceGiant, BodyCategory.OtherBody })
			{
				sb.AppendLine($"## {CategoryName(category)}");
				sb.AppendLine();

				foreach (CelestialBodyAspect body in GameCatalog.CelestialBodies.Where(b => b.Category == category))
				{
					sb.AppendLine($"### [{body.Name}]({Slug(body.Name)})");
					sb.AppendLine($"- Pressure: {Number(body.Environment.Pressure)} atm");
					sb.AppendLine($"- Gravity: {Number(body.Environment.Gravity)} m/s²");
					sb.AppendLine($"- Average Temp: {Number(body.Environment.AverageTemperature)} °C");
					sb.AppendLine($"- Composition: {Tags(body.Environment.Composition)}");
					sb.AppendLine();
				}
			}

			return sb.ToString().TrimEnd();
		}

		private static string EquipmentIndex()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Equipment Registry");
			sb.AppendLine();
			sb.AppendLine("Certified survival gear. Gear is only valid for the species it is rated for.");
			sb.AppendLine();

			foreach (EquipmentAspect equipment in GameCatalog.Equipment)
			{
				sb.AppendLine($"## [{equipment.Name}]({Slug(equipment.Name)})");
				sb.AppendLine($"- Equippable by: {equipment.DescribeEquippableBy()}");
				sb.AppendLine($"- Pressure: {Describe(equipment.Envelope.Pressure, "atm")}");
				sb.AppendLine($"- Gravity: {Describe(equipment.Envelope.Gravity, "m/s²")}");
				sb.AppendLine($"- Temperature: {Describe(equipment.Envelope.Temperature, "°C")}");
				sb.AppendLine($"- Supplies: {Tags(equipment.Envelope.Requirements)}");
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private static string ShipIndex()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Ship Registry");
			sb.AppendLine();
			sb.AppendLine("Certified hull ratings. A hull must survive the descent independently");
			sb.AppendLine("of its occupant — equipment cannot protect a ship.");
			sb.AppendLine();

			foreach (ShipAspect shipClass in GameCatalog.ShipClasses)
			{
				sb.AppendLine($"## [{shipClass.Name}]({Slug(shipClass.Name)})");
				sb.AppendLine($"- Pressure: {Describe(shipClass.Hull.Pressure, "atm")}");
				sb.AppendLine($"- Gravity: {Describe(shipClass.Hull.Gravity, "m/s²")}");
				sb.AppendLine($"- Temperature: {Describe(shipClass.Hull.Temperature, "°C")}");
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private static string ShipPage(ShipAspect shipClass)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {shipClass.Name}");
			sb.AppendLine(shipClass.Description);
			sb.AppendLine();
			sb.AppendLine("## Hull Ratings");
			sb.AppendLine($"- Pressure: {Describe(shipClass.Hull.Pressure, "atm")}");
			sb.AppendLine($"- Gravity: {Describe(shipClass.Hull.Gravity, "m/s²")}");
			sb.AppendLine($"- Temperature: {Describe(shipClass.Hull.Temperature, "°C")}");
			sb.AppendLine();

			List<CelestialBodyAspect> rated = GameCatalog.CelestialBodies.Where(body =>
				Covers(shipClass.Hull.Pressure, body.Environment.Pressure) &&
				Covers(shipClass.Hull.Gravity, body.Environment.Gravity) &&
				Covers(shipClass.Hull.Temperature, body.Environment.AverageTemperature)).ToList();

			sb.AppendLine("## Cleared For Descent");
			if (rated.Count == 0)
			{
				sb.AppendLine("- No registered body within this hull's ratings");
			}
			else
			{
				foreach (CelestialBodyAspect body in rated)
				{
					sb.AppendLine($"- [{body.Name}]({Slug(body.Name)})");
				}
			}

			sb.AppendLine();
			sb.AppendLine("_The hull surviving does not mean the occupant will._");
			sb.AppendLine($"_Check the [species registry](species_registry.md) as well._");
			return sb.ToString().TrimEnd();
		}

		private static bool Covers(FloatRange? range, float value)
			=> range.HasValue && range.Value.Contains(value);

		private static bool CanSurviveUnaided(SpeciesAspect species, CelestialBodyAspect body)
		{
			// Check pressure
			if (species.Envelope.Pressure.HasValue && !species.Envelope.Pressure.Value.Contains(body.Environment.Pressure))
				return false;
			// Check gravity
			if (species.Envelope.Gravity.HasValue && !species.Envelope.Gravity.Value.Contains(body.Environment.Gravity))
				return false;
			// Check temperature
			if (species.Envelope.Temperature.HasValue && !species.Envelope.Temperature.Value.Contains(body.Environment.AverageTemperature))
				return false;
			// Check composition requirements
			foreach (var req in species.Envelope.Requirements)
			{
				if (!body.Environment.Composition.Contains(req))
					return false;
			}
			return true;
		}

		private static string SpeciesPage(SpeciesAspect species)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {species.Name}");
			sb.AppendLine(species.Description);
			sb.AppendLine();
			sb.AppendLine("## Origin");
			sb.AppendLine($"- Home world: [{species.Origin}]({Slug(species.Origin)})");
			sb.AppendLine();
			sb.AppendLine("## Natural Survival Envelope");
			sb.AppendLine($"- Pressure: {Describe(species.Envelope.Pressure, "atm")}");
			sb.AppendLine($"- Gravity: {Describe(species.Envelope.Gravity, "m/s²")}");
			sb.AppendLine($"- Temperature: {Describe(species.Envelope.Temperature, "°C")}");
			sb.AppendLine($"- REQUIRED: {Tags(species.Envelope.Requirements)}");
			sb.AppendLine();

			sb.AppendLine("## Unaided Survival Clearance");
			List<CelestialBodyAspect> survivable = GameCatalog.CelestialBodies
				.Where(body => CanSurviveUnaided(species, body)).ToList();
			if (survivable.Count == 0)
			{
				sb.AppendLine("- No registered body can support this species unaided");
			}
			else
			{
				foreach (CelestialBodyAspect body in survivable)
				{
					sb.AppendLine($"- [{body.Name}]({Slug(body.Name)})");
				}
			}
			sb.AppendLine();

			List<EquipmentAspect> certified = GameCatalog.Equipment
				.Where(e => e.CanBeEquippedBy(species)).ToList();

			sb.AppendLine("## Certified Equipment");
			if (certified.Count == 0)
			{
				sb.AppendLine("- None registered");
			}
			else
			{
				foreach (EquipmentAspect equipment in certified)
				{
					sb.AppendLine($"- [{equipment.Name}]({Slug(equipment.Name)})");
				}
			}

			sb.AppendLine();
			sb.AppendLine($"See also: [landing protocols](landing_protocols.md)");
			return sb.ToString().TrimEnd();
		}

		private static string PlanetPage(CelestialBodyAspect body)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {body.Name}");
			sb.AppendLine(body.Description);
			sb.AppendLine();
			sb.AppendLine($"## Classification");
			sb.AppendLine($"- Category: {CategoryName(body.Category)}");
			sb.AppendLine();
			sb.AppendLine("## Measured Conditions");
			sb.AppendLine($"- Pressure: {Number(body.Environment.Pressure)} atm");
			sb.AppendLine($"- Gravity: {Number(body.Environment.Gravity)} m/s²");
			sb.AppendLine($"- Average Temp: {Number(body.Environment.AverageTemperature)} °C");
			sb.AppendLine($"- Lowest Temp: {Number(body.Environment.LowestTemperature)} °C");
			sb.AppendLine($"- Highest Temp: {Number(body.Environment.HighestTemperature)} °C");
			sb.AppendLine($"- Composition: {Tags(body.Environment.Composition)}");
			sb.AppendLine();
			sb.AppendLine("_Clearance is judged on the average temperature._");
			sb.AppendLine();

			List<SpeciesAspect> natives = GameCatalog.Species
				.Where(s => s.Origin == body.Name).ToList();

			sb.AppendLine("## Native Species");
			if (natives.Count == 0)
			{
				sb.AppendLine("- No native spacefaring species");
			}
			else
			{
				foreach (SpeciesAspect species in natives)
				{
					sb.AppendLine($"- [{species.Name}]({Slug(species.Name)})");
				}
			}

			return sb.ToString().TrimEnd();
		}

		private static string EquipmentPage(EquipmentAspect equipment)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {equipment.Name}");
			sb.AppendLine(equipment.Description);
			sb.AppendLine();
			sb.AppendLine("## Certification");
			foreach (SpeciesAspect species in equipment.EquippableBy)
			{
				sb.AppendLine($"- [{species.Name}]({Slug(species.Name)})");
			}
			sb.AppendLine();
			sb.AppendLine("## Protection Ratings");
			sb.AppendLine($"- Pressure: {Describe(equipment.Envelope.Pressure, "atm")}");
			sb.AppendLine($"- Gravity: {Describe(equipment.Envelope.Gravity, "m/s²")}");
			sb.AppendLine($"- Temperature: {Describe(equipment.Envelope.Temperature, "°C")}");
			sb.AppendLine($"- Supplies: {Tags(equipment.Envelope.Requirements)}");
			sb.AppendLine();
			sb.AppendLine("_An axis marked \"not rated\" is unprotected — the wearer's own tolerance applies._");
			sb.AppendLine("_Worn by an uncertified species this equipment provides nothing._");
			return sb.ToString().TrimEnd();
		}

		private static string Readme()
		{
			return
@"# Station Alpha — File System

Welcome, Operator.

Pilots will hail you requesting landing clearance. They will not volunteer
everything. Interrogate them, then verify what they tell you against these records.

## Directory Structure
- Documents/ — protocols, registries and indexes
- Photos/ — reference imagery (not operationally relevant)

## Start Here
- [Landing protocols](landing_protocols.md) — how a clearance decision is made
- [Species registry](species_registry.md) — who can survive what
- [Planetary index](planetary_index.md) — measured conditions per world
- [Equipment registry](equipment_registry.md) — what gear covers, and for whom
- [Ship registry](ship_registry.md) — hull ratings per vessel class

## Your Responsibility
You are the final authority on landing approvals.
The files are the truth — the pilot may not be.";
		}

		private static RaFile MakeFile(string name, string content)
		{
			return new RaFile(name, content);
		}

		private static string Slug(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}

			StringBuilder sb = new StringBuilder(name.Length + 3);
			foreach (char c in name.ToLowerInvariant())
			{
				if (char.IsLetterOrDigit(c))
				{
					sb.Append(c);
				}
				else if (c == ' ' || c == '-' || c == '_')
				{
					sb.Append('_');
				}
			}
			return sb.Append(".md").ToString();
		}

		private static string Describe(FloatRange? range, string unit)
			=> range.HasValue ? range.Value.Describe(unit) : "not rated";

		private static string Number(float value) => value.ToString("0.######");

		private static string Tags(IEnumerable<TagBase> tags)
		{
			string joined = string.Join(", ", tags.Select(t => t.Name));
			return string.IsNullOrEmpty(joined) ? "none" : joined;
		}

		private static string CategoryName(BodyCategory category)
		{
			switch (category)
			{
				case BodyCategory.Terrestrial: return "Terrestrial";
				case BodyCategory.GasOrIceGiant: return "Gas and Ice Giant";
				default: return "Other Body";
			}
		}
	}
}
