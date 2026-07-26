namespace GMTK_2026
{
	public static class GameCatalog
	{
		public static readonly SpeciesAspect[] Species =
		{
			SpeciesAspect.Humans,
			SpeciesAspect.Solarians,
			SpeciesAspect.Lucifers,
			SpeciesAspect.Dustcrawls,
			SpeciesAspect.Jovians,
			SpeciesAspect.Nyxs,
			SpeciesAspect.Buttcolds,
			SpeciesAspect.Olympians,
			SpeciesAspect.Titans,
			SpeciesAspect.Plutonians,
		};

		public static readonly CelestialBodyAspect[] CelestialBodies =
		{
			CelestialBodyAspect.Mercury,
			CelestialBodyAspect.Venus,
			CelestialBodyAspect.Earth,
			CelestialBodyAspect.Mars,
			CelestialBodyAspect.Jupiter,
			CelestialBodyAspect.Saturn,
			CelestialBodyAspect.Uranus,
			CelestialBodyAspect.Neptune,
			CelestialBodyAspect.Titan,
			CelestialBodyAspect.Pluto,
		};

		public static readonly EquipmentAspect[] Equipment =
		{
			EquipmentAspect.EvaSuit,
			EquipmentAspect.ExtremeHazardSuit,
			EquipmentAspect.Rebreather,
			EquipmentAspect.ContainmentRig,
			EquipmentAspect.CompressionShell,
			EquipmentAspect.ThermalExoskeleton,
			EquipmentAspect.CryoRecirculator,
			EquipmentAspect.DynamoEngine,
			EquipmentAspect.VapourRebreather,
		};

		public static readonly ShipAspect[] ShipClasses =
		{
			ShipAspect.LightLander,
			ShipAspect.HeavyFreighter,
			ShipAspect.LongRangeExplorer,
			ShipAspect.ThermalShieldedLander,
			ShipAspect.IceRunner,
			ShipAspect.DeepPressureDescender,
		};

		public static readonly OccupationAspect[] Occupations =
		{
			OccupationAspect.FreightPilot,
			OccupationAspect.Surveyor,
			OccupationAspect.Diplomat,
			OccupationAspect.Miner,
			OccupationAspect.Researcher,
			OccupationAspect.Medic,
			OccupationAspect.Trader,
		};

		public static CelestialBodyAspect FindBody(string name)
		{
			for (int i = 0; i < CelestialBodies.Length; i++)
			{
				if (CelestialBodies[i].Name == name)
				{
					return CelestialBodies[i];
				}
			}
			return null;
		}
	}
}
