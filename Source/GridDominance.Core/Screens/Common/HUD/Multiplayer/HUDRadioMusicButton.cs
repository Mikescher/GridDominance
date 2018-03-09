using System.Collections.Generic;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.Common.HUD.Multiplayer
{
	class HUDRadioMusicButton : HUDButton
	{
		public override int Depth => 0;

		public bool Selected = false;
		public int MusicIndex;

		public List<HUDRadioMusicButton> RadioGroup = new List<HUDRadioMusicButton>();

		protected override void DoDrawBackground(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(Textures.TexHUDButtonBase, Center, 1f, Selected ? FlatColors.Emerald : FlatColors.Flamingo, 0f);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(Textures.TexHUDButtonIconMusicOn, Center, 1f, FlatColors.Clouds, 0f);
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

			MainGame.Inst.GDSound.PlayMusicLevel(MusicIndex);
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
