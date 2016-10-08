using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.DebugTools
{
	public abstract class DebugMinimap : ISAMDrawable
	{
		protected readonly GameScreen Owner;

		protected abstract FRectangle Boundings { get; }
		protected abstract float MaxSize { get; }
		protected abstract float Padding { get; }

		protected DebugMinimap(GameScreen scrn)
		{
			Owner = scrn;
		}

		public void Draw(IBatchRenderer sbatch)
		{
			if (!DebugSettings.Get("ShowDebugMiniMap")) return;

			var rectBoundings = Boundings;
			var scale = FloatMath.Min(MaxSize / rectBoundings.Width, MaxSize / rectBoundings.Height);

			var sizeOuter = rectBoundings.AsScaled(scale).Size;
			var sizeView = Owner.GuaranteedMapViewport.AsScaled(scale).Size;
			var sizeView2 = Owner.CompleteMapViewport.AsScaled(scale).Size;
			var posView = (Owner.GuaranteedMapViewport.VectorTopLeft - Boundings.VectorTopLeft) * scale;
			var posView2 = (Owner.CompleteMapViewport.VectorTopLeft - Boundings.VectorTopLeft) * scale;

			var offset = new Vector2(Owner.VAdapter.VirtualTotalWidth - Padding - sizeOuter.Width, Padding) - Owner.VAdapter.VirtualGuaranteedBoundingsOffset;
			var offsetZero = offset - Boundings.VectorTopLeft *scale;

			sbatch.FillRectangle(offset, sizeOuter, Color.Red * 0.25f);
			sbatch.DrawRectangle(offset, sizeOuter, Color.Red);

			sbatch.FillRectangle(offset + posView, sizeView, Color.Black * 0.1f);
			sbatch.DrawRectangle(offset + posView, sizeView, Color.Black, 2);
			sbatch.DrawRectangle(offset + posView2, sizeView2, Color.Black * 0.666f, 1);

			foreach (var entity in Owner.GetAllEntities())
			{
				float radius = scale * (entity.DrawingBoundingBox.Width + entity.DrawingBoundingBox.Height) / 4;

				if (radius < 0.5f) continue;

				if (entity.IsInViewport)
				{
					sbatch.FillCircle(offsetZero + entity.Position * scale, radius, 8, entity.DebugIdentColor * 0.85f);
				}
				else
				{
					sbatch.FillCircle(offsetZero + entity.Position * scale, radius, 8, entity.DebugIdentColor * 0.25f);
				}
			}
		}
	}
}
