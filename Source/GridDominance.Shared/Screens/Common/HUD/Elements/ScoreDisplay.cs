using System;
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
	public class ScoreDisplay : HUDContainer
	{
		public override int Depth => 0;

		private readonly HUDImage _icon;
		private readonly HUDRawText _text;

		private readonly DeltaLimitedFloat _offset = new DeltaLimitedFloat(0, 43);
		private readonly DeltaLimitedFloat _value = new DeltaLimitedFloat(0, 537);

		public ScoreDisplay(bool count)
		{
			_value.Set(MainGame.Inst.Profile.TotalPoints);
			_value.SetDelta(FloatMath.Max(537, MainGame.Inst.Profile.TotalPoints / 4f));
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
			var whud = HUD as GDWorldHUD;

			_text.Text = MainGame.Inst.Profile.TotalPoints.ToString();

			_icon.RenderScaleOverride = 1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * 2) * 0.05f;

			if (whud != null)
				_offset.Set(whud.TopLevelDisplay.RelativeBottom);
			else
				_offset.Set(0);

			_offset.Update(gameTime);
			if (_offset.ActualValue < _offset.TargetValue) _offset.SetForce(_offset.TargetValue);

			if (FloatMath.FloatInequals(_value.ActualValue, _value.TargetValue))
			{
				_value.Update(gameTime);
				_text.Text = ((int)_value.ActualValue).ToString();
			}

			var rp = new FPoint(10, _offset.ActualValue + 10);
			if (rp != RelativePosition)
			{
				RelativePosition = rp;
				Revalidate();
			}
		}

		public void FinishCounter()
		{
			_value.Finish();
			_text.Text = ((int)_value.ActualValue).ToString();
		}
	}
}
