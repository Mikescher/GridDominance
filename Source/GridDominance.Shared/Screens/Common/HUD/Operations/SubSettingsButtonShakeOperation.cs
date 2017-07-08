using System;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class SubSettingsButtonShakeOperation : HUDIntervalElementOperation<SubSettingButton>
	{
		private const float DELAY    = 1.40f;
		private const float CYCLE    = 0.50f;
		private const int   SHAKES   = 4;
		private const float ROTATION = FloatMath.RAD_POS_015;

		public override string Name => "SubSettingsButtonShake";

		public SubSettingsButtonShakeOperation() : base(DELAY, CYCLE, DELAY)
		{
		}
		
		protected override void OnStart(SubSettingButton element)
		{
			//
		}

		protected override void OnEnd(SubSettingButton element)
		{
			//
		}

		protected override void OnCycleStart(SubSettingButton entity, SAMTime gameTime, InputState istate)
		{
			entity.IconRotation = 0f;
		}

		protected override void OnCycleProgress(SubSettingButton entity, float progress, SAMTime gameTime, InputState istate)
		{
			entity.IconRotation = ROTATION * FloatMath.Sin(progress * FloatMath.TAU * SHAKES);
		}

		protected override void OnCycleEnd(SubSettingButton entity, SAMTime gameTime, InputState istate)
		{
			entity.IconRotation = 0f;
		}
	}
}
