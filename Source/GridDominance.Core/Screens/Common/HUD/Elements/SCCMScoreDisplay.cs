using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Operations;
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

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public class SCCMScoreDisplay : HUDContainer
	{
		public override int Depth => 0;

		public FPoint RefPosition;

		private readonly HUDRawText _text;

		private readonly DeltaLimitedFloat _value = new DeltaLimitedFloat(0, 35);

		private float _iconrotation = 0;
		public FPoint[] TetroCenters = new[]{ FPoint.Zero, FPoint.Zero, FPoint.Zero, FPoint.Zero };

		public SCCMScoreDisplay(bool count)
		{
			_value.Set(MainGame.Inst.Profile.ScoreStars);
			_value.SetDelta(FloatMath.Max(37, MainGame.Inst.Profile.ScoreSCCM / 4f));
			if (!count) _value.Finish();

			_text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Text = ((int)_value.ActualValue).ToString(),
				TextColor = FlatColors.TextHUD,
				FontSize = 60f,
				RelativePosition = new FPoint(10 + 40 + 30, 0),
			};

			Alignment = HUDAlignment.TOPRIGHT;
			RelativePosition = new FPoint(10, 10);
			Size = new FSize(250, 60);

			IsVisible = (MainGame.Inst.Profile.ScoreSCCM > 0);

			UpdateRelativePosition();

			AddOperation(new SCCMDisplayAnimationOperation());
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var iconcenter = new FPoint(bounds.Left + 10 + 20, bounds.CenterY);

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.6f, 8);
			sbatch.DrawCentered(Textures.TexCircle, iconcenter, 50, 50, FlatColors.WetAsphalt * 0.4f);

			for (int i = 0; i < 4; i++)
			{
				var pos = TetroCenters[i].AsScaled(10).WithOrigin(iconcenter).RotateAround(iconcenter, _iconrotation);

				sbatch.DrawCentered(Textures.TexPixel, pos, 10, 10, FlatColors.Alizarin, _iconrotation);
			}
		}

		public override void OnInitialize()
		{
			AddElement(_text);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_iconrotation = 0.05f * gameTime.TotalElapsedSeconds * FloatMath.TAU;

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
		
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			HUD.ShowToast(null, L10N.T(L10NImpl.STR_SCOREMAN_INFO_SCCMSCORE), 32, FlatColors.Silver, FlatColors.Foreground, 2f);
		}

		public void FinishCounter()
		{
			_value.Finish();
			_text.Text = ((int)_value.ActualValue).ToString();
		}
	}
}
