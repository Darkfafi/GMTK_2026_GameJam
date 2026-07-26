namespace GMTK_2026
{
	public sealed class ShipAspect : EntityAspect
	{
		public SurvivalEnvelope Hull { get; } = new SurvivalEnvelope();

		private ShipAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly ShipAspect LightLander = Build("Light Lander",
			"Cheap short-hop shuttle. Thin hull, minimal shielding.",
			pressure: new FloatRange(0f, 3f), gravity: new FloatRange(0f, 15f), temperature: new FloatRange(-150f, 80f));

		public static readonly ShipAspect HeavyFreighter = Build("Heavy Freighter",
			"Bulk cargo hauler. Reinforced for mass, not for extremes.",
			pressure: new FloatRange(0f, 8f), gravity: new FloatRange(0f, 15f), temperature: new FloatRange(-180f, 120f));

		public static readonly ShipAspect LongRangeExplorer = Build("Long-Range Explorer",
			"Survey vessel built for unfamiliar conditions. Broadly capable.",
			pressure: new FloatRange(0f, 20f), gravity: new FloatRange(0f, 20f), temperature: new FloatRange(-240f, 150f));

		public static readonly ShipAspect ThermalShieldedLander = Build("Thermal-Shielded Lander",
			"Ablative furnace-diver rated for greenhouse and dayside descents.",
			pressure: new FloatRange(0f, 120f), gravity: new FloatRange(0f, 15f), temperature: new FloatRange(-100f, 500f));

		public static readonly ShipAspect IceRunner = Build("Ice Runner",
			"Cryogenic hull for the outer system. Useless anywhere warm.",
			pressure: new FloatRange(0f, 10f), gravity: new FloatRange(0f, 13f), temperature: new FloatRange(-250f, 40f));

		public static readonly ShipAspect DeepPressureDescender = Build("Deep-Pressure Descender",
			"Giant-diving bathyscaphe. The only hull rated for the deep cloud decks.",
			pressure: new FloatRange(0f, 1500f), gravity: new FloatRange(0f, 30f), temperature: new FloatRange(-250f, 100f));

		private static ShipAspect Build(string name, string description,
			FloatRange pressure, FloatRange gravity, FloatRange temperature)
		{
			ShipAspect ship = new ShipAspect(name, description);
			ship.Hull.Pressure = pressure;
			ship.Hull.Gravity = gravity;
			ship.Hull.Temperature = temperature;
			return ship;
		}
	}
}
