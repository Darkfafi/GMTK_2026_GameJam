using UnityEngine;

namespace GMTK_2026
{
	public readonly struct FloatRange
	{
		public float Min { get; }
		public float Max { get; }

		public FloatRange(float min, float max)
		{
			Min = Mathf.Min(min, max);
			Max = Mathf.Max(min, max);
		}

		public bool Contains(float value) => value >= Min && value <= Max;

		public string Describe(string unit, string format = "0.######")
			=> $"{Min.ToString(format)} – {Max.ToString(format)} {unit}";

		public override string ToString() => $"{Min} – {Max}";
	}
}
