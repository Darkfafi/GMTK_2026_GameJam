using System.Collections.Generic;

namespace GMTK_2026
{
	public abstract class PilotRequestBase
	{
		private readonly Dictionary<string, GameEntityBase> _dependencies = new Dictionary<string, GameEntityBase>();
		private readonly List<Requirement> _requirements = new List<Requirement>();

		public abstract string RequestType { get; }
		public abstract string RequestTitle { get; }
		public abstract string RequestDescription { get; }

		public IReadOnlyList<Requirement> Requirements => _requirements;

		public CreatureEntity Pilot => GetDependency<CreatureEntity>(DependencyKeys.Pilot);

		public float TimeLimit { get; }
		public float TimeRemaining { get; private set; }

		public float TimeNormalized
		{
			get
			{
				if (TimeLimit <= 0f)
				{
					return 0f;
				}
				float fraction = TimeRemaining / TimeLimit;
				return fraction < 0f ? 0f : (fraction > 1f ? 1f : fraction);
			}
		}

		public bool IsExpired => TimeRemaining <= 0f;

		public bool IsResolved { get; private set; }

		public void Resolve() => IsResolved = true;

		protected PilotRequestBase(CreatureEntity pilot, float timeLimit = 20f)
		{
			SetDependency(DependencyKeys.Pilot, pilot);
			TimeLimit = timeLimit;
			TimeRemaining = timeLimit;
		}

		public bool TickTime(float deltaTime)
		{
			if (TimeRemaining <= 0f)
			{
				return false;
			}

			TimeRemaining -= deltaTime;
			if (TimeRemaining <= 0f)
			{
				TimeRemaining = 0f;
				return true;
			}
			return false;
		}

		protected void SetDependency(string key, GameEntityBase entity)
		{
			_dependencies[key] = entity;
		}

		protected void AddRequirement(Requirement requirement)
		{
			_requirements.Add(requirement);
		}

		public T GetDependency<T>(string key) where T : GameEntityBase
		{
			return _dependencies.TryGetValue(key, out GameEntityBase entity) ? entity as T : null;
		}

		public RequestVerdict Evaluate()
		{
			List<string> reasons = new List<string>();
			bool approved = true;

			foreach (Requirement requirement in _requirements)
			{
				RequirementResult result = requirement.Evaluate(this);
				if (!result.IsMet)
				{
					approved = false;
					reasons.Add($"[{requirement.Name}] {result.Reason}");
				}
			}

			return new RequestVerdict(approved, reasons);
		}
	}

	public class RequestVerdict
	{
		public bool IsApproved { get; }
		public IReadOnlyList<string> Reasons { get; }

		public RequestVerdict(bool approved, IReadOnlyList<string> reasons)
		{
			IsApproved = approved;
			Reasons = reasons;
		}
	}
}
