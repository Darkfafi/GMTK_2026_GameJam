namespace GMTK_2026
{
	public static class GameCatalog
	{
		public static readonly SpeciesAspect[] Species =
		{
			SpeciesAspect.Human,
			SpeciesAspect.Silathi,
			SpeciesAspect.Aquatoid,
			SpeciesAspect.Volcan,
		};

		public static readonly OccupationAspect[] Occupations =
		{
			OccupationAspect.Warrior,
			OccupationAspect.VacuumWorker,
			OccupationAspect.HazmatSpecialist,
			OccupationAspect.DeepMiner,
		};

		public static readonly CelestialBodyAspect[] CelestialBodies =
		{
			CelestialBodyAspect.Star,
			CelestialBodyAspect.GasGiant,
			CelestialBodyAspect.IceDwarf,
			CelestialBodyAspect.OceanWorld,
		};
	}
}
