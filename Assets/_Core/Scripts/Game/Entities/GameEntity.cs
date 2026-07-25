using System.Collections.Generic;

namespace GMTK_2026
{
	public abstract class GameEntityBase
	{
		private readonly List<EntityAspect> _aspects = new List<EntityAspect>();

		public string Name
		{
			get; private set;
		}

		public TagProfile Profile { get; } = new TagProfile();

		public IReadOnlyList<EntityAspect> Aspects => _aspects;

		protected GameEntityBase(string name)
		{
			Name = name;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public void ApplyAspect(EntityAspect aspect)
		{
			if (aspect == null)
			{
				return;
			}

			_aspects.Add(aspect);
			aspect.ApplyTo(Profile);
		}
	}
}
