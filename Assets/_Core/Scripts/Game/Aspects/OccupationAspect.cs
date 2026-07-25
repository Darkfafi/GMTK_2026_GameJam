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

		public static readonly OccupationAspect HazmatSpecialist = Build("Hazmat Specialist", "Trained in toxic environments.", o =>
		{
			o.Removes.Add(EnvironmentTag.Radiation);
		});

		public static readonly OccupationAspect DeepMiner = Build("Deep Miner", "Used to extreme pressure.", o =>
		{
			o.Removes.Add(EnvironmentTag.Pressure);
		});

		private static OccupationAspect Build(string name, string description, Action<OccupationAspect> configure)
		{
			OccupationAspect occupation = new OccupationAspect(name, description);
			configure(occupation);
			return occupation;
		}
	}
}
