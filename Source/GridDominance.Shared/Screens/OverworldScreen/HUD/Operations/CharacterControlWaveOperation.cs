
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.Operations
{
	class CharacterControlWaveOperation : HUDIntervalElementOperation<HUDContainer>
	{
		public const float INITIAL_DELAY = 1f;
		public const float ANIMATION_TIME = 4f;
		public const float SLEEP_TIME = 15f;

		public const float OFFSET = 24f;

		private readonly FPoint[] _originalCoords;
		private readonly HUDCharacterControl[] _controls;

		public override string Name => "CharacterControlWave";

		public CharacterControlWaveOperation(HUDCharacterControl[] controls) : base(INITIAL_DELAY, ANIMATION_TIME, SLEEP_TIME)
		{
			_controls = controls;
			_originalCoords = new FPoint[_controls.Length];
		}

		protected override void OnStart(HUDContainer element)
		{
			//
		}

		protected override void OnEnd(HUDContainer element)
		{
			//
		}

		protected override void OnCycleStart(HUDContainer entity, SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _controls.Length; i++) _originalCoords[i] = _controls[i].RelativePosition;
		}

		protected override void OnCycleProgress(HUDContainer entity, float progress, SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _controls.Length; i++)
			{
				var cp = i * 1f / (_controls.Length - 1);

				var pos = 3 - progress * 4 + cp;

				if (pos < 0 || pos > 2)
				{
					_controls[i].RelativePosition = _originalCoords[i];
				}
				else
				{
					_controls[i].RelativePosition = _originalCoords[i] + new Vector2(0, FloatMath.Sin(pos*FloatMath.TAU)*OFFSET);
				}
			}
		}

		protected override void OnCycleEnd(HUDContainer entity, SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _controls.Length; i++) _controls[i].RelativePosition = _originalCoords[i];
		}
	}
}
