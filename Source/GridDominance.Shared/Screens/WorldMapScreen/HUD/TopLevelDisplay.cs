using GridDominance.Shared.Screens.WorldMapScreen.Entities;
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
	public class TopLevelDisplay : HUDContainer
	{
		private const float SPEED_MOVE  = 2.0f;
		private const float SPEED_BLEND = 3.0f;

		public override int Depth => 0;

		private readonly HUDRawText text;

		private LevelNode displayNode = null;
		private float progressMove  = 0f;
		private float progressBlend = 0f;

		public TopLevelDisplay()
		{
			text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTER,
				Text = string.Empty,
				TextColor = FlatColors.TextHUD,
				FontSize = 30f,
			};

			Alignment = HUDAlignment.TOPCENTER;
			RelativePosition = FPoint.Zero;
			Size = new FSize(300, 35);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, Color.Black * 0.6f);
		}

		public override void OnInitialize()
		{
			AddElement(text);
		}

		public override void OnRemove()
		{
			//
		}

		// ReSharper disable RedundantCheckBeforeAssignment
		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!FloatMath.FloatEquals(Size.Width, HUD.Width)) Size = new FSize(HUD.Width, Height);

			var sel = ((GDWorldHUD) HUD).SelectedNode;

			if (sel == null && progressMove > 0)
			{
				// DISAPPEAR

				if (! (FloatMath.IsOne(progressMove) && istate.IsRealDown))
				{
					FloatMath.ProgressDec(ref progressMove, gameTime.ElapsedSeconds * SPEED_MOVE);
					FloatMath.ProgressInc(ref progressBlend, gameTime.ElapsedSeconds * SPEED_BLEND);
				}

				if (FloatMath.IsZero(progressMove))
				{
					displayNode = null;
					progressMove = 0;
					progressBlend = 1;
				}
			}
			else if (sel != null && displayNode == null)
			{
				// START APPEAR

				displayNode = sel;
				progressMove = 0;
				progressBlend = 1;

				text.Text = "Level " + displayNode.Level.Name + "  :  " + displayNode.Level.FullName;
			}
			else if (sel != null && sel != displayNode)
			{
				// BLEND AWAY FROM CURRENT

				FloatMath.ProgressInc(ref progressMove, gameTime.ElapsedSeconds * SPEED_MOVE);
				FloatMath.ProgressDec(ref progressBlend, gameTime.ElapsedSeconds * SPEED_BLEND);

				if (FloatMath.IsZero(progressBlend))
				{
					displayNode = sel;
					text.Text = "Level " + displayNode.Level.Name + "  :  " + displayNode.Level.FullName;
				}
			}
			else if (sel != null && sel == displayNode && (progressBlend < 1 || progressMove < 1))
			{
				// BLEND INTO OTHER | APPEAR

				FloatMath.ProgressInc(ref progressMove, gameTime.ElapsedSeconds * SPEED_MOVE);
				FloatMath.ProgressInc(ref progressBlend, gameTime.ElapsedSeconds * SPEED_BLEND);
			}

			var rp = new FPoint(0, (1-progressMove) * -Height);
			if (rp != RelativePosition)
			{
				RelativePosition = rp;
				Revalidate();
			}
			text.TextColor = FlatColors.TextHUD * progressBlend;

			IsVisible = FloatMath.IsNotZero(progressMove);
		}
	}
}
