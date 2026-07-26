using System.Collections.Generic;

namespace GMTK_2026
{
	public class SurvivalEnvelope
	{
		public FloatRange? Pressure { get; set; }
		public FloatRange? Gravity { get; set; }
		public FloatRange? Temperature { get; set; }
		public HashSet<TagBase> Requirements { get; } = new HashSet<TagBase>();
	}
}
