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

		protected abstract float MaxSize { get; }
		protected abstract float Padding { get; }

		protected DebugMinimap(GameScreen scrn)
		{
			Owner = scrn;
		}

		public void Draw(IBatchRenderer sbatch)
		{
			if (!DebugSettings.Get("ShowDebugMiniMap")) return;

			var rectBoundings = Owner.MapFullBounds;
			var scale = FloatMath.Min(MaxSize / rectBoundings.Width, MaxSize / rectBoundings.Height);

			var sizeOuter = rectBoundings.AsScaled(scale).Size;
			var sizeView = Owner.GuaranteedMapViewport.AsScaled(scale).Size;
			var sizeView2 = Owner.CompleteMapViewport.AsScaled(scale).Size;
			var posView = (Owner.GuaranteedMapViewport.VectorTopLeft - rectBoundings.VectorTopLeft) * scale;
			var posView2 = (Owner.CompleteMapViewport.VectorTopLeft - rectBoundings.VectorTopLeft) * scale;

			var offset = new Vector2(Owner.VAdapterHUD.VirtualTotalWidth - Padding - sizeOuter.Width, Padding) - Owner.VAdapterHUD.VirtualGuaranteedBoundingsOffset;
			var offsetZero = offset - rectBoundings.VectorTopLeft * scale;

			sbatch.FillRectangle(offset, sizeOuter, Color.Red * 0.25f);
			sbatch.DrawRectangle(offset, sizeOuter, Color.Red);

			sbatch.FillRectangle(offset + posView, sizeView, Color.Black * 0.1f);
			sbatch.DrawRectangle(offset + posView, sizeView, Color.Black, 2);
			sbatch.DrawRectangle(offset + posView2, sizeView2, Color.Black * 0.666f, 1);

			sbatch.DrawLine(offsetZero.X, offset.Y, offsetZero.X, offset.Y + sizeOuter.Height, Color.Black * 0.5f);
			sbatch.DrawLine(offset.X, offsetZero.Y, offset.X + sizeOuter.Width, offsetZero.Y, Color.Black * 0.5f);
			sbatch.FillRectangle(FRectangle.CreateByCenter(offsetZero, 0, 0, 5, 5), Color.Black * 0.5f);

			foreach (var entity in Owner.GetAllEntities())
			{
				float radius = scale * (entity.DrawingBoundingBox.Width + entity.DrawingBoundingBox.Height) / 4;

				if (radius < 0.5f) continue;

				if (entity.IsInViewport)
				{
					if (entity.DebugIdentColor.A > 0) sbatch.FillCircle(offsetZero + entity.Position * scale, radius, 8, entity.DebugIdentColor * 0.85f);
				}
				else
				{
					if (entity.DebugIdentColor.A > 0) sbatch.FillCircle(offsetZero + entity.Position * scale, radius, 8, entity.DebugIdentColor * 0.25f);
				}
			}
		}
	}
}
