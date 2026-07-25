using System;

namespace GMTK_2026
{
	public sealed class OccupationAspect : EntityAspect
	{
		private OccupationAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly OccupationAspect Warrior = Build("Warrior", "Trained combatant.", _ => { });

		public static readonly OccupationAspect VacuumWorker = Build("Vacuum Worker", "Hardened for exposure.", o =>
		{
			o.Removes.Add(EnvironmentTag.Vacuum);
		});

		private static OccupationAspect Build(string name, string description, Action<OccupationAspect> configure)
		{
			OccupationAspect occupation = new OccupationAspect(name, description);
			configure(occupation);
			return occupation;
		}
	}
}
