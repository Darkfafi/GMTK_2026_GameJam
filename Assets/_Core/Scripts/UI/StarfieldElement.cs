using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public sealed class StarfieldElement : VisualElement
	{
		private struct Star { public float X, Y, R, A, Speed, Phase, Twinkle; }

		private const int Count = 140;

		private readonly List<Star> _stars = new List<Star>();
		private readonly System.Random _rng = new System.Random(20260725);
		private float _time;
		private bool _seeded;

		public StarfieldElement()
		{
			pickingMode = PickingMode.Ignore;
			generateVisualContent += OnGenerate;
			schedule.Execute(Tick).Every(33);
			RegisterCallback<GeometryChangedEvent>(_ => Seed());
		}

		private void Seed()
		{
			float w = resolvedStyle.width;
			float h = resolvedStyle.height;
			if (w <= 0f || h <= 0f)
			{
				return;
			}

			_stars.Clear();
			for (int i = 0; i < Count; i++)
			{
				_stars.Add(new Star
				{
					X = (float)_rng.NextDouble() * w,
					Y = (float)_rng.NextDouble() * h,
					R = (float)_rng.NextDouble() * 1.4f + 0.3f,
					A = (float)_rng.NextDouble() * 0.7f + 0.2f,
					Speed = (float)_rng.NextDouble() * 0.25f + 0.04f,
					Phase = (float)_rng.NextDouble() * 6.28f,
					Twinkle = (float)_rng.NextDouble() * 0.9f + 0.3f,
				});
			}
			_seeded = true;
		}

		private void Tick()
		{
			if (!_seeded)
			{
				Seed();
				return;
			}

			float w = resolvedStyle.width;
			float h = resolvedStyle.height;
			_time += 0.033f;

			for (int i = 0; i < _stars.Count; i++)
			{
				Star s = _stars[i];
				s.Y += s.Speed;
				if (s.Y > h + 4f)
				{
					s.Y = -4f;
					s.X = (float)_rng.NextDouble() * w;
				}
				_stars[i] = s;
			}
			MarkDirtyRepaint();
		}

		private void OnGenerate(MeshGenerationContext ctx)
		{
			if (!_seeded)
			{
				return;
			}

			Painter2D p = ctx.painter2D;
			for (int i = 0; i < _stars.Count; i++)
			{
				Star s = _stars[i];
				float alpha = s.A * (0.5f + 0.5f * Mathf.Sin(_time * s.Twinkle + s.Phase));
				p.fillColor = new Color(0.75f, 0.82f, 1f, Mathf.Clamp01(alpha));
				p.BeginPath();
				p.Arc(new Vector2(s.X, s.Y), Mathf.Max(0.1f, s.R), Angle.Degrees(0f), Angle.Degrees(360f));
				p.Fill();
			}
		}
	}
}
