namespace GMTK_2026
{
	public sealed class EnvironmentTag : TagBase
	{
		private EnvironmentTag(string displayName, string description = "")
			: base(displayName, description)
		{
		}

		// --- gases & matter ---
		public static readonly EnvironmentTag Oxygen = new EnvironmentTag("Oxygen", "Free breathable oxygen.");
		public static readonly EnvironmentTag Nitrogen = new EnvironmentTag("Nitrogen", "Inert nitrogen atmosphere.");
		public static readonly EnvironmentTag CarbonDioxide = new EnvironmentTag("Carbon Dioxide", "Dense carbon dioxide atmosphere.");
		public static readonly EnvironmentTag Hydrogen = new EnvironmentTag("Hydrogen", "Molecular hydrogen.");
		public static readonly EnvironmentTag Helium = new EnvironmentTag("Helium", "Inert helium.");
		public static readonly EnvironmentTag Water = new EnvironmentTag("Water", "Liquid water or accessible ice.");
		public static readonly EnvironmentTag Sodium = new EnvironmentTag("Sodium", "Sodium vapour traces.");
		public static readonly EnvironmentTag Sulfur = new EnvironmentTag("Sulfur", "Sulfuric compounds.");
		public static readonly EnvironmentTag MethaneIce = new EnvironmentTag("Methane Ice", "Frozen methane deposits.");
		public static readonly EnvironmentTag LiquidHydrocarbons = new EnvironmentTag("Liquid Hydrocarbons", "Methane and ethane lakes.");
		public static readonly EnvironmentTag SubsurfaceIron = new EnvironmentTag("Subsurface Iron", "Iron-rich regolith.");

		// --- energy sources ---
		public static readonly EnvironmentTag SolarRadiation = new EnvironmentTag("Solar Radiation", "Intense unfiltered starlight.");
		public static readonly EnvironmentTag GeothermalHeat = new EnvironmentTag("Geothermal Heat", "Internal heat bleeding from the core.");
		public static readonly EnvironmentTag KineticWindEnergy = new EnvironmentTag("Kinetic Wind Energy", "Sustained supersonic winds.");
		public static readonly EnvironmentTag AtmosphericStatic = new EnvironmentTag("Atmospheric Static", "Permanent electrical storm activity.");
	}
}
