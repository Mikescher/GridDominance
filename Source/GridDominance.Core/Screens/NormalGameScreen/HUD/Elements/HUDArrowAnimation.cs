using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class HUDArrowAnimation : HUDElement
	{
		public override int Depth { get; } = 0;

		private readonly FPoint _p1;
		private readonly FPoint _p2;
		private readonly float  _rotation;

		private float _progress = 0;

		public HUDArrowAnimation(FPoint position, float rotation)
		{
			Alignment = HUDAlignment.ABSOLUTE_BOTHCENTERED;
			Size = new FSize(64, 64);

			_rotation = rotation;

			_p1 = position + Vector2.UnitX.RotateWithLength(rotation, +16);
			_p2 = position + Vector2.UnitX.RotateWithLength(rotation, -16);

			RelativePosition = _p1;
		}


		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawStretched(Textures.TexHUDIconArrow, bounds, Color.White, _rotation + FloatMath.RAD_POS_090);
		}

		public override void OnInitialize()
		{

		}

		public override void OnRemove()
		{

		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_progress += gameTime.ElapsedSeconds;
			RelativePosition = FPoint.Lerp(_p1, _p2, FloatMath.PercSin(_progress/0.5f));
		}
	}
}
