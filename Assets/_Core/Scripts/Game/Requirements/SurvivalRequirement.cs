using System;
using System.Collections.Generic;
using System.Linq;

namespace GMTK_2026
{
	public sealed class SurvivalRequirement : Requirement
	{
		public override string Name => "Survival";

		public override RequirementResult Evaluate(PilotRequestBase request)
		{
			CreatureEntity pilot = request.GetDependency<CreatureEntity>(DependencyKeys.Pilot);
			PlanetEntity planet = request.GetDependency<PlanetEntity>(DependencyKeys.Target);

			SpeciesAspect species = pilot?.Species;
			EnvironmentProfile environment = planet?.Environment;

			if (pilot == null || planet == null || species == null || environment == null)
			{
				return RequirementResult.Fail("Insufficient data to verify survival.");
			}

			List<EquipmentAspect> carried = pilot.Equipment;
			List<EquipmentAspect> usable = carried.Where(e => e.CanBeEquippedBy(species)).ToList();
			List<EquipmentAspect> unusable = carried.Where(e => !e.CanBeEquippedBy(species)).ToList();

			List<string> problems = new List<string>();

			CheckAxis(problems, "Pressure", environment.Pressure, "atm",
				species.Envelope.Pressure, usable, e => e.Envelope.Pressure, species.Name);

			CheckAxis(problems, "Gravity", environment.Gravity, "m/s²",
				species.Envelope.Gravity, usable, e => e.Envelope.Gravity, species.Name);

			CheckAxis(problems, "Temperature", environment.AverageTemperature, "°C",
				species.Envelope.Temperature, usable, e => e.Envelope.Temperature, species.Name);

			HashSet<TagBase> available = new HashSet<TagBase>(environment.Composition);
			foreach (EquipmentAspect equipment in usable)
			{
				available.UnionWith(equipment.Envelope.Requirements);
			}

			List<string> missing = species.Envelope.Requirements
				.Where(requirement => !available.Contains(requirement))
				.Select(requirement => requirement.Name)
				.ToList();

			if (missing.Count > 0)
			{
				problems.Add($"{planet.Name} cannot supply {string.Join(" or ", missing)}, which {species.Name} require");
			}

			if (problems.Count == 0)
			{
				string aided = usable.Count > 0
					? $" (aided by {string.Join(", ", usable.Select(e => e.Name))})"
					: " unaided";
				return RequirementResult.Pass($"{species.Name} can survive on {planet.Name}{aided}.");
			}

			if (unusable.Count > 0)
			{
				problems.Add($"note: {string.Join(", ", unusable.Select(e => e.Name))} is not rated for {species.Name} and provides nothing");
			}

			return RequirementResult.Fail(string.Join("; ", problems) + ".");
		}

		private static void CheckAxis(List<string> problems, string axis, float value, string unit,
			FloatRange? speciesRange, List<EquipmentAspect> usable,
			Func<EquipmentAspect, FloatRange?> equipmentRange, string speciesName)
		{
			if (speciesRange.HasValue && speciesRange.Value.Contains(value))
			{
				return;
			}

			foreach (EquipmentAspect equipment in usable)
			{
				FloatRange? rating = equipmentRange(equipment);
				if (rating.HasValue && rating.Value.Contains(value))
				{
					return;
				}
			}

			string tolerated = speciesRange.HasValue ? speciesRange.Value.Describe(unit) : "unknown";
			problems.Add($"{axis} {value.ToString("0.######")} {unit} is outside {speciesName} tolerance ({tolerated}) and unprotected");
		}
	}
}
