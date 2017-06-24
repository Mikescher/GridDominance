using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations
{
	class RedFlashTextOperation : GameEntityOperation<BackgroundText>
	{
		private const float CYCLE_TIME = 6.0f;

		private float _time;
		
		public RedFlashTextOperation() : base("RedFlashText", null)
		{
			_time = 0;
		}

		protected override void OnStart(BackgroundText entity)
		{
			//
		}

		protected override void OnProgress(BackgroundText entity, float progress, SAMTime gameTime, InputState istate)
		{
			_time = (_time + gameTime.ElapsedSeconds) % CYCLE_TIME;

			entity.Color = ColorMath.Blend(FlatColors.Foreground, FlatColors.Pomegranate, FloatMath.PercSin(FloatMath.TAU * _time / CYCLE_TIME) * 0.3f);
		}

		protected override void OnEnd(BackgroundText entity)
		{
			//
		}

		protected override void OnAbort(BackgroundText entity)
		{
			//
		}
	}
}
