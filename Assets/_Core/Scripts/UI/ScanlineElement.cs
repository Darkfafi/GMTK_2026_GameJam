using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public sealed class ScanlineElement : VisualElement
	{
		public ScanlineElement()
		{
			pickingMode = PickingMode.Ignore;
			generateVisualContent += OnGenerate;
			RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
		}

		private void OnGenerate(MeshGenerationContext ctx)
		{
			Rect r = contentRect;
			if (r.width <= 0f || r.height <= 0f)
			{
				return;
			}

			Painter2D p = ctx.painter2D;
			p.strokeColor = new Color(0f, 1f, 0.53f, 0.04f);
			p.lineWidth = 1f;

			for (float y = 0f; y < r.height; y += 4f)
			{
				p.BeginPath();
				p.MoveTo(new Vector2(0f, y));
				p.LineTo(new Vector2(r.width, y));
				p.Stroke();
			}
		}
	}
}
