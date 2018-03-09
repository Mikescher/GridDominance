using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	public class MultiplayerScoreDisplay : HUDContainer
	{
		public override int Depth => 0;

		public FPoint RefPosition;

		private readonly HUDImage _icon;
		private readonly HUDRawText _text;

		private readonly DeltaLimitedFloat _value = new DeltaLimitedFloat(0, 37);

		public MultiplayerScoreDisplay(bool count)
		{
			_value.Set(MainGame.Inst.Profile.MultiplayerPoints);
			_value.SetDelta(FloatMath.Max(37, MainGame.Inst.Profile.MultiplayerPoints / 4f));
			if (!count) _value.Finish();

			_text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Text = ((int)_value.ActualValue).ToString(),
				TextColor = FlatColors.TextHUD,
				FontSize = 60f,
				RelativePosition = new FPoint(10 + 40 + 30, 0),
			};

			_icon = new HUDImage
			{
				Image = Textures.TexIconMPScore,
				Color = FlatColors.Amethyst,
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(10, 0),
				Size = new FSize(40, 40),
			};

			Alignment = HUDAlignment.TOPRIGHT;
			RelativePosition = new FPoint(10, 10);
			Size = new FSize(250, 60);

			IsVisible = MainGame.Inst.Profile.HasMultiplayerGames;

			UpdateRelativePosition();
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.6f, 8);
			sbatch.DrawCentered(Textures.TexCircle, _icon.Center, 50, 50, FlatColors.WetAsphalt * 0.4f);
		}

		public override void OnInitialize()
		{
			AddElement(_text);
			AddElement(_icon);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_icon.RenderScaleOverride = 1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * 2) * 0.05f;
			_icon.Rotation = 0.05f * gameTime.TotalElapsedSeconds * FloatMath.TAU;

			if (FloatMath.FloatInequals(_value.ActualValue, _value.TargetValue))
			{
				_value.Update(gameTime);
				_text.Text = ((int)_value.ActualValue).ToString();
			}

			UpdateRelativePosition();
		}

		private void UpdateRelativePosition()
		{
			if (RefPosition != RelativePosition)
			{
				RelativePosition = RefPosition;
				Revalidate();
			}
		}
		
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			HUD.ShowToast(null, L10N.T(L10NImpl.STR_SCOREMAN_INFO_MPSCORE), 32, FlatColors.Silver, FlatColors.Foreground, 2f);
		}

		public void FinishCounter()
		{
			_value.Finish();
			_text.Text = ((int)_value.ActualValue).ToString();
		}
	}
}
