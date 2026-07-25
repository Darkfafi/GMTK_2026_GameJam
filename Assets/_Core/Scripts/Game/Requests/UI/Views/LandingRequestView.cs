namespace GMTK_2026
{
	public sealed class LandingRequestView : RequestViewBase
	{
		private readonly CreatureView _pilot = new CreatureView();
		private readonly PlanetView _target = new PlanetView();
		private readonly ShipView _ship = new ShipView();

		public LandingRequestView()
		{
			AddToClassList("request-view--landing");
			Add(_pilot);
			Add(_target);
			Add(_ship);
		}

		public override void Bind(PilotRequestBase request)
		{
			LandingPilotRequest landing = request as LandingPilotRequest;
			_pilot.Bind(landing?.Pilot);
			_target.Bind(landing?.Target);
			_ship.Bind(landing?.Ship);
		}
	}
}
