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
			ShipEntity ship = request.GetDependency<ShipEntity>(DependencyKeys.Ship);

			if (pilot == null || planet == null)
			{
				return RequirementResult.Fail("Insufficient data to verify survival.");
			}

			HashSet<TagBase> available = new HashSet<TagBase>(planet.Provides);
			if (ship != null)
			{
				available.UnionWith(ship.LifeSupport);
			}

			// Environmental Surival Check
			foreach (EnvironmentTag need in pilot.Requires.OfType<EnvironmentTag>())
			{
				if (!available.Contains(need))
				{
					return RequirementResult.Fail($"{pilot.Name} needs {need}, unavailable on {planet.Name}.");
				}
			}

			// Hazards Check
			foreach (EnvironmentTag condition in planet.Provides.OfType<EnvironmentTag>())
			{
				if (pilot.Intolerances.Contains(condition))
				{
					return RequirementResult.Fail($"{planet.Name} exposes {pilot.Name} to {condition}, which is hazardous.");
				}
			}

			return RequirementResult.Pass($"{planet.Name} is survivable for {pilot.Name}.");
		}
	}
}
