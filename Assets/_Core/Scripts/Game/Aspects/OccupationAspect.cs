namespace GMTK_2026
{
	public sealed class OccupationAspect : EntityAspect
	{
		private OccupationAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly OccupationAspect FreightPilot = new OccupationAspect("Freight Pilot", "Hauls bulk cargo between colonies.");
		public static readonly OccupationAspect Surveyor = new OccupationAspect("Surveyor", "Maps unclaimed terrain for the registry.");
		public static readonly OccupationAspect Diplomat = new OccupationAspect("Diplomat", "Travels on behalf of a homeworld government.");
		public static readonly OccupationAspect Miner = new OccupationAspect("Miner", "Extracts ore and volatiles from hostile worlds.");
		public static readonly OccupationAspect Researcher = new OccupationAspect("Researcher", "Studies atmospheric and geological phenomena.");
		public static readonly OccupationAspect Medic = new OccupationAspect("Medic", "Runs emergency response between outposts.");
		public static readonly OccupationAspect Trader = new OccupationAspect("Trader", "Independent merchant working the outer routes.");
	}
}
