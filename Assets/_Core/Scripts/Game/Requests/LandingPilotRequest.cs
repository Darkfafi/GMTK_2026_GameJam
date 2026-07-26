namespace GMTK_2026
{
	public class LandingPilotRequest : PilotRequestBase
	{
		public override string RequestType => "Landing";
		public override string RequestTitle => $"Landing clearance - {Pilot?.Name}";
		public override string RequestDescription
			=> $"{Pilot?.Name} requests permission to land {Ship?.Name} on {Target?.Name}.";

		public PlanetEntity Target => GetDependency<PlanetEntity>(DependencyKeys.Target);
		public ShipEntity Ship => GetDependency<ShipEntity>(DependencyKeys.Ship);

		public LandingPilotRequest(CreatureEntity pilot, PlanetEntity target, ShipEntity ship, float timeLimit = 20f)
			: base(pilot, timeLimit)
		{
			SetDependency(DependencyKeys.Target, target);
			SetDependency(DependencyKeys.Ship, ship);

			AddRequirement(new SurvivalRequirement());
			AddRequirement(new HullIntegrityRequirement());
		}
	}
}
