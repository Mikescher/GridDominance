using GridDominance.Shared.Screens.Common.HUD.Elements;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD
{
	public class SubSettingClickZone : HUDElement
	{
		public override int Depth => 1;

		private readonly SubSettingButton _master;

		public SubSettingClickZone(SubSettingButton master)
		{
			_master = master;

			Size = new FSize(0, 0);
			Alignment = HUDAlignment.ABSOLUTE_VERTCENTERED;
		}

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (!IsEnabled) return;
			HUD.Screen.Game.Sound.TryPlayButtonClickEffect();
			_master.SlavePress(istate);
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			//
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
	}
}
