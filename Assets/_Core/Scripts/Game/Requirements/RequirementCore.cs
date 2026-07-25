namespace GMTK_2026
{
	public abstract class Requirement
	{
		public abstract string Name { get; }

		public abstract RequirementResult Evaluate(PilotRequestBase request);
	}

	public readonly struct RequirementResult
	{
		public bool IsMet { get; }
		public string Reason { get; }

		public RequirementResult(bool fulfilled, string reason)
		{
			IsMet = fulfilled;
			Reason = reason;
		}

		public static RequirementResult Pass(string reason = "") => new RequirementResult(true, reason);
		public static RequirementResult Fail(string reason) => new RequirementResult(false, reason);
	}
}
