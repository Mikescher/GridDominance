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
	public class MultiplayerScoreDisplay : HUDContainer
	{
		public override int Depth => 0;

		private readonly ScoreDisplay _ref;

		private readonly HUDImage icon;
		private readonly HUDRawText text;

		public MultiplayerScoreDisplay(ScoreDisplay reference)
		{
			_ref = reference;

			text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Text = MainGame.Inst.Profile.TotalPoints.ToString(),
				TextColor = FlatColors.TextHUD,
				FontSize = 60f,
				RelativePosition = new FPoint(10 + 40 + 30, 0),
			};

			icon = new HUDImage
			{
				Image = Textures.TexIconMPScore,
				Color = FlatColors.Amethyst,
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(10, 0),
				Size = new FSize(40, 40),
				RotationSpeed = 0.05f,
			};

			Alignment = HUDAlignment.TOPRIGHT;
			RelativePosition = new FPoint(10, 10);
			Size = new FSize(250, 60);

			IsVisible = MainGame.Inst.Profile.MultiplayerPoints != 0;
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
			text.Text = MainGame.Inst.Profile.MultiplayerPoints.ToString();

			icon.RenderScaleOverride = 1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * 2) * 0.05f;

			var rp = new FPoint(_ref.RelativePosition.X, _ref.RelativePosition.Y + _ref.Height + 10);

			if (rp != RelativePosition)
			{
				RelativePosition = rp;
				Revalidate();
			}
		}
	}
}
