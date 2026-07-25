using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public sealed class CountdownRingElement : VisualElement
	{
		private static readonly Color BackColor = new Color(0.10f, 0.14f, 0.21f, 1f);
		private static readonly Color OkColor = new Color(0f, 1f, 0.53f, 1f);
		private static readonly Color WarnColor = new Color(1f, 0.62f, 0.27f, 1f);
		private static readonly Color CritColor = new Color(1f, 0.28f, 0.34f, 1f);

		private readonly Label _num;
		private float _progress = 1f;
		private Color _color = OkColor;

		public CountdownRingElement()
		{
			AddToClassList("cd-ring");
			pickingMode = PickingMode.Ignore;
			generateVisualContent += OnGenerate;
			RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());

			_num = new Label("0") { pickingMode = PickingMode.Ignore };
			_num.AddToClassList("cd-num");
			Add(_num);
		}

		public void SetProgress(float normalized)
		{
			_progress = Mathf.Clamp01(normalized);
			_color = _progress <= 0.15f ? CritColor : (_progress <= 0.30f ? WarnColor : OkColor);
			MarkDirtyRepaint();
		}

		public void SetSeconds(int seconds) => _num.text = seconds.ToString();

		private void OnGenerate(MeshGenerationContext ctx)
		{
			Rect r = contentRect;
			if (r.width <= 0f || r.height <= 0f)
			{
				return;
			}

			Vector2 center = r.center;
			float radius = Mathf.Min(r.width, r.height) * 0.5f - 4f;
			if (radius <= 0f)
			{
				return;
			}

			Painter2D p = ctx.painter2D;
			p.lineWidth = 4f;
			p.lineCap = LineCap.Round;

			// background ring
			p.strokeColor = BackColor;
			p.BeginPath();
			p.Arc(center, radius, Angle.Degrees(0f), Angle.Degrees(360f));
			p.Stroke();

			// Starts at Top
			if (_progress > 0f)
			{
				p.strokeColor = _color;
				p.BeginPath();
				p.Arc(center, radius, Angle.Degrees(-90f), Angle.Degrees(-90f + 360f * _progress));
				p.Stroke();
			}
		}
	}
}
