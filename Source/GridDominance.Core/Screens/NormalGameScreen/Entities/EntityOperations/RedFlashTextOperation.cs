using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations
{
	class RedFlashTextOperation : SAMUpdateOp<BackgroundText>
	{
		private const float CYCLE_TIME = 6.0f;

		public override string Name => "RedFlashText";

		public RedFlashTextOperation()
		{
		}
		
		protected override void OnUpdate(BackgroundText entity, SAMTime gameTime, InputState istate)
		{
			var modtime = Lifetime % CYCLE_TIME;

			entity.Color = ColorMath.Blend(FlatColors.Foreground, FlatColors.Pomegranate, FloatMath.PercSin(FloatMath.TAU * modtime / CYCLE_TIME) * 0.3f);
		}
	}
}
