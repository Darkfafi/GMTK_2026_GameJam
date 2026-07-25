using System;
using System.Collections.Generic;

namespace GMTK_2026
{
	public class CreatureEntity : GameEntityBase
	{
		public HashSet<TagBase> Requires => Profile.Requires;
		public HashSet<TagBase> Intolerances => Profile.Intolerances;

		public CreatureEntity(string name)
			: base(name)
		{
		}
	}
}
