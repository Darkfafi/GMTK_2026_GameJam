namespace GMTK_2026
{
	public sealed class EnvironmentTag : TagBase
	{
		private EnvironmentTag(string displayName, string description = "")
			: base(displayName, description)
		{
		}

		public static readonly EnvironmentTag Oxygen = new EnvironmentTag("Oxygen", "Breathable oxygen.");
		public static readonly EnvironmentTag Nitrogen = new EnvironmentTag("Nitrogen");
		public static readonly EnvironmentTag Chlorine = new EnvironmentTag("Chlorine", "Toxic to most carbon-based life.");
		public static readonly EnvironmentTag Water = new EnvironmentTag("Water");
		public static readonly EnvironmentTag Heat = new EnvironmentTag("Extreme Heat");
		public static readonly EnvironmentTag Cold = new EnvironmentTag("Extreme Cold");
		public static readonly EnvironmentTag Radiation = new EnvironmentTag("Radiation");
		public static readonly EnvironmentTag Vacuum = new EnvironmentTag("Vacuum");
		public static readonly EnvironmentTag Pressure = new EnvironmentTag("High Pressure");
	}
}
