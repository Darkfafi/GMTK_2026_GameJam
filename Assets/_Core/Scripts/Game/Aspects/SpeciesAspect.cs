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

		public static readonly SpeciesAspect Aquatoid = Build("Aquatoid", "Amphibious, water-dependent.", s =>
		{
			s.Requires.Add(EnvironmentTag.Water);
			s.Intolerances.Add(EnvironmentTag.Heat);
		});

		public static readonly SpeciesAspect Volcan = Build("Volcan", "Magma-dwelling entity.", s =>
		{
			s.Requires.Add(EnvironmentTag.Heat);
			s.Intolerances.Add(EnvironmentTag.Cold);
			s.Intolerances.Add(EnvironmentTag.Water);
		});

		private static SpeciesAspect Build(string name, string description, Action<SpeciesAspect> configure)
		{
			SpeciesAspect species = new SpeciesAspect(name, description);
			configure(species);
			return species;
		}
	}
}
