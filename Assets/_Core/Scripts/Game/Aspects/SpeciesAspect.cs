using System;

namespace GMTK_2026
{
	public sealed class SpeciesAspect : EntityAspect
	{
		private SpeciesAspect(string name, string description = "")
			: base(name, description)
		{
		}

		public static readonly SpeciesAspect Human = Build("Human", "Carbon-based humanoid.", s =>
		{
			s.Requires.Add(EnvironmentTag.Oxygen);
			s.Intolerances.Add(EnvironmentTag.Chlorine);
		});

		public static readonly SpeciesAspect Silathi = Build("Silathi", "Silicon-based; breathes chlorine.", s =>
		{
			s.Requires.Add(EnvironmentTag.Chlorine);
			s.Intolerances.Add(EnvironmentTag.Oxygen);
		});

		private static SpeciesAspect Build(string name, string description, Action<SpeciesAspect> configure)
		{
			SpeciesAspect species = new SpeciesAspect(name, description);
			configure(species);
			return species;
		}
	}
}
