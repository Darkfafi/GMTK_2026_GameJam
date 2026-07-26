using System.Collections.Generic;

namespace GMTK_2026
{
	public class CreatureEntity : GameEntityBase
	{
		public HashSet<TagBase> Requires => Profile.Requires;
		public HashSet<TagBase> Intolerances => Profile.Intolerances;

		public SpeciesAspect Species => GetAspect<SpeciesAspect>();
		public List<EquipmentAspect> Equipment => GetAspects<EquipmentAspect>();

		public CreatureEntity(string name)
			: base(name)
		{
		}
	}
}
