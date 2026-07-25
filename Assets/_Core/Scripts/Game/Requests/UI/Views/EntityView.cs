using UnityEngine.UIElements;

namespace GMTK_2026
{
	public abstract class EntityView : VisualElement
	{
		public const string UssClass = "entity-view";

		private readonly VisualElement _body;

		protected EntityView(string kind, string icon, string iconModifier)
		{
			AddToClassList(UssClass);

			VisualElement head = new VisualElement { pickingMode = PickingMode.Ignore };
			head.AddToClassList("entity-view__head");

			Label iconLabel = new Label(icon) { pickingMode = PickingMode.Ignore };
			iconLabel.AddToClassList("entity-view__ico");
			iconLabel.AddToClassList(iconModifier);

			Label kindLabel = new Label(kind) { pickingMode = PickingMode.Ignore };
			kindLabel.AddToClassList("entity-view__kind");

			head.Add(iconLabel);
			head.Add(kindLabel);

			_body = new VisualElement { pickingMode = PickingMode.Ignore };
			_body.AddToClassList("entity-view__body");

			Add(head);
			Add(_body);
		}

		protected void ClearBody() => _body.Clear();

		protected void AddRow(string key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			VisualElement row = new VisualElement { pickingMode = PickingMode.Ignore };
			row.AddToClassList("entity-view__row");

			Label keyLabel = new Label(key) { pickingMode = PickingMode.Ignore };
			keyLabel.AddToClassList("entity-view__row-key");

			Label valueLabel = new Label(value) { pickingMode = PickingMode.Ignore };
			valueLabel.AddToClassList("entity-view__row-val");

			row.Add(keyLabel);
			row.Add(valueLabel);
			_body.Add(row);
		}

		protected static T FindAspect<T>(GameEntityBase entity) where T : EntityAspect
		{
			if (entity == null)
			{
				return null;
			}

			for (int i = 0; i < entity.Aspects.Count; i++)
			{
				if (entity.Aspects[i] is T match)
				{
					return match;
				}
			}
			return null;
		}
	}

	public sealed class CreatureView : EntityView
	{
		public CreatureView() : base("PILOT", "🧑", "entity-view__ico--pilot") { }

		public void Bind(CreatureEntity creature)
		{
			ClearBody();
			if (creature == null)
			{
				return;
			}

			AddRow("Name", creature.Name);
			AddRow("Species", FindAspect<SpeciesAspect>(creature)?.Name);
			AddRow("Occupation", FindAspect<OccupationAspect>(creature)?.Name);
		}
	}

	public sealed class PlanetView : EntityView
	{
		public PlanetView() : base("PLANET", "🪐", "entity-view__ico--planet") { }

		public void Bind(PlanetEntity planet)
		{
			ClearBody();
			if (planet == null)
			{
				return;
			}

			AddRow("Name", planet.Name);
			AddRow("Body", FindAspect<CelestialBodyAspect>(planet)?.Name);
		}
	}

	public sealed class ShipView : EntityView
	{
		public ShipView() : base("SHIP", "🚀", "entity-view__ico--ship") { }

		public void Bind(ShipEntity ship)
		{
			ClearBody();
			if (ship == null)
			{
				return;
			}

			AddRow("Name", ship.Name);
		}
	}
}
