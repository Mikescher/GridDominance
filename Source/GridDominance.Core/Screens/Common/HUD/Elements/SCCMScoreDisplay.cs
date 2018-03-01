using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Elements;
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
		private readonly HUDImage _img;
		private readonly HUDTetroAnimation _ani;

		private readonly DeltaLimitedFloat _value = new DeltaLimitedFloat(0, 35);

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

			_img = new HUDImage
			{
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(10, 0),
				Size = new FSize(40, 40),

				Image = Textures.TexCircle,
				Color = FlatColors.WetAsphalt * 0.4f,
			};

			_ani = new HUDTetroAnimation
			{
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(10, 0),
				Size = new FSize(40, 40),

				Foreground = FlatColors.Alizarin,
			};

			Alignment = HUDAlignment.TOPRIGHT;
			RelativePosition = new FPoint(10, 10);
			Size = new FSize(250, 60);

			IsVisible = (MainGame.Inst.Profile.ScoreSCCM > 0);

			UpdateRelativePosition();
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			//
		}

		public override void OnInitialize()
		{
			AddElement(_text);
			
			AddElement(_img);
			
			AddElement(_ani);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{

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
