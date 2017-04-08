using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class ScoreDisplay : HUDContainer
	{
		public override int Depth => 0;

		private readonly HUDImage icon;
		private readonly HUDRawText text;

		private readonly DeltaLimitedFloat offset = new DeltaLimitedFloat(0, 40f);

		public ScoreDisplay()
		{
			text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Text = "???",
				TextColor = FlatColors.TextHUD,
				FontSize = 60f,
				RelativePosition = new FPoint(10 + 40 + 30, 0),
			};

			icon = new HUDImage
			{
				Image = Textures.TexIconScore,
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(10, 0),
				Size = new FSize(40, 40)
			};

			Alignment = HUDAlignment.TOPRIGHT;
			RelativePosition = new FPoint(10, 10);
			Size = new FSize(250, 60);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.6f, 8);
			sbatch.DrawCentered(Textures.TexCircle, icon.Center, 50, 50, FlatColors.WetAsphalt * 0.4f);
		}

		public override void OnInitialize()
		{
			AddElement(text);
			AddElement(icon);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			var whud = HUD as GDWorldHUD;

			text.Text = MainGame.Inst.Profile.TotalPoints.ToString();

			icon.RenderScaleOverride = 1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * 2) * 0.05f;

			if (whud != null)
				offset.Set(whud.TopLevelDisplay.Bottom);
			else
				offset.Set(0);

			offset.Update(gameTime);
			if (offset.ActualValue < offset.TargetValue) offset.SetForce(offset.TargetValue);

			var rp = new FPoint(10, offset.ActualValue + 10);
			if (rp != RelativePosition)
			{
				RelativePosition = rp;
				Revalidate();
			}
		}
	}
}
