using System;
using System.Collections.Generic;

namespace GMTK_2026
{
	public sealed class HullIntegrityRequirement : Requirement
	{
		public override string Name => "Hull Integrity";

		public override RequirementResult Evaluate(PilotRequestBase request)
		{
			ShipEntity ship = request.GetDependency<ShipEntity>(DependencyKeys.Ship);
			PlanetEntity planet = request.GetDependency<PlanetEntity>(DependencyKeys.Target);

			ShipAspect shipClass = ship?.Class;
			EnvironmentProfile environment = planet?.Environment;

			if (ship == null || planet == null || shipClass == null || environment == null)
			{
				return RequirementResult.Fail("Insufficient data to verify hull integrity.");
			}

			List<string> problems = new List<string>();

			CheckAxis(problems, "Pressure", environment.Pressure, "atm", shipClass.Hull.Pressure, shipClass.Name);
			CheckAxis(problems, "Gravity", environment.Gravity, "m/s²", shipClass.Hull.Gravity, shipClass.Name);
			CheckAxis(problems, "Temperature", environment.AverageTemperature, "°C", shipClass.Hull.Temperature, shipClass.Name);

			if (problems.Count == 0)
			{
				return RequirementResult.Pass($"{shipClass.Name} hull is rated for {planet.Name}.");
			}

			return RequirementResult.Fail(string.Join("; ", problems) + ".");
		}

		private static void CheckAxis(List<string> problems, string axis, float value, string unit,
			FloatRange? rating, string className)
		{
			if (rating.HasValue && rating.Value.Contains(value))
			{
				return;
			}

			string rated = rating.HasValue ? rating.Value.Describe(unit) : "unrated";
			problems.Add($"{axis} {value.ToString("0.######")} {unit} exceeds the {className} hull rating ({rated})");
		}
	}
}
