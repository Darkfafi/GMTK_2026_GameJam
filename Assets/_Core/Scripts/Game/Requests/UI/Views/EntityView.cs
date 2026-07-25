using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public abstract class EntityView : VisualElement
	{
		public const string UssClass = "entity-view";

		private readonly Label _name;
		private readonly VisualElement _body;

		protected EntityView(string kind)
		{
			AddToClassList(UssClass);

			Label kindLabel = new Label(kind) { pickingMode = PickingMode.Ignore };
			kindLabel.AddToClassList("entity-view__kind");

			_name = new Label { pickingMode = PickingMode.Ignore };
			_name.AddToClassList("entity-view__name");

			_body = new VisualElement { pickingMode = PickingMode.Ignore };
			_body.AddToClassList("entity-view__body");

			Add(kindLabel);
			Add(_name);
			Add(_body);
		}

		protected void SetDisplayName(string name) => _name.text = string.IsNullOrEmpty(name) ? "—" : name;

		protected void ClearBody() => _body.Clear();

		protected void AddChips(string label, IEnumerable<string> values)
		{
			List<string> items = values?.Where(v => !string.IsNullOrEmpty(v)).ToList() ?? new List<string>();
			if (items.Count == 0)
			{
				return;
			}

			VisualElement row = new VisualElement { pickingMode = PickingMode.Ignore };
			row.AddToClassList("entity-view__row");

			Label rowLabel = new Label(label) { pickingMode = PickingMode.Ignore };
			rowLabel.AddToClassList("entity-view__row-label");
			row.Add(rowLabel);

			VisualElement chips = new VisualElement { pickingMode = PickingMode.Ignore };
			chips.AddToClassList("entity-view__chips");
			foreach (string item in items)
			{
				Label chip = new Label(item) { pickingMode = PickingMode.Ignore };
				chip.AddToClassList("tag-chip");
				chips.Add(chip);
			}
			row.Add(chips);

			_body.Add(row);
		}
	}

	public sealed class CreatureView : EntityView
	{
		public CreatureView() : base("Pilot") { }

		public void Bind(CreatureEntity creature)
		{
			SetDisplayName(creature?.Name);
			ClearBody();
			if (creature == null)
			{
				return;
			}

			AddChips("Is", creature.Aspects.Select(a => a.Name));
			AddChips("Needs", creature.Profile.Requires.Select(t => t.Name));
			AddChips("Avoids", creature.Profile.Intolerances.Select(t => t.Name));
		}
	}

	public sealed class PlanetView : EntityView
	{
		public PlanetView() : base("Planet") { }

		public void Bind(PlanetEntity planet)
		{
			SetDisplayName(planet?.Name);
			ClearBody();
			if (planet == null)
			{
				return;
			}

			AddChips("Is", planet.Aspects.Select(a => a.Name));
			AddChips("Provides", planet.Profile.Provides.Select(t => t.Name));
		}
	}

	public sealed class ShipView : EntityView
	{
		public ShipView() : base("Ship") { }

		public void Bind(ShipEntity ship)
		{
			SetDisplayName(ship?.Name);
			ClearBody();
			if (ship == null)
			{
				return;
			}

			AddChips("Is", ship.Aspects.Select(a => a.Name));
			AddChips("Life Support", ship.LifeSupport.Select(t => t.Name));
		}
	}
}
