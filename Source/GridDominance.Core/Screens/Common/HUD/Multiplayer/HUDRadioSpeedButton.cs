using System;
using System.Collections.Generic;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.Common.HUD.Multiplayer
{
	class HUDRadioSpeedButton : HUDButton
	{
		public override int Depth => 0;

		public bool Selected = false;
		public GameSpeedModes Speed;

		public List<HUDRadioSpeedButton> RadioGroup = new List<HUDRadioSpeedButton>();

		protected override void DoDrawBackground(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(Textures.TexHUDButtonBase, Center, 1f, Selected ? FlatColors.Emerald : FlatColors.Flamingo, 0f);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(GetTexture(), Center, 1f, FlatColors.Clouds, 0f);
		}

		private TextureRegion2D GetTexture()
		{
			switch (Speed)
			{
				case GameSpeedModes.SUPERSLOW:
					return Textures.TexHUDButtonSpeedSet0;
				case GameSpeedModes.SLOW:
					return Textures.TexHUDButtonSpeedSet1;
				case GameSpeedModes.NORMAL:
					return Textures.TexHUDButtonSpeedSet2;
				case GameSpeedModes.FAST:
					return Textures.TexHUDButtonSpeedSet3;
				case GameSpeedModes.SUPERFAST:
					return Textures.TexHUDButtonSpeedSet4;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnPress(InputState istate)
		{
			foreach (var b in RadioGroup) b.Selected = false;
			Selected = true;
		}

		protected override void OnDoublePress(InputState istate)
		{
			//
		}

		protected override void OnTriplePress(InputState istate)
		{
			//
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			//
		}
	}
}
