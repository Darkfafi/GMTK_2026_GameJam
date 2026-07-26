using System.Collections.Generic;

namespace GMTK_2026
{
	public class EnvironmentProfile
	{
		public float Pressure { get; set; }
		public float Gravity { get; set; }
		public float LowestTemperature { get; set; }
		public float HighestTemperature { get; set; }
		public float AverageTemperature { get; set; }
		public HashSet<TagBase> Composition { get; } = new HashSet<TagBase>();
	}
}
