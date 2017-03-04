using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDFadeOutInfoBox : HUDMessageBox, IHUDModalChild
	{
		private readonly float boxLifetime;
		private readonly float boxNormalFadeTime;
		private readonly float boxFastFadeTime;

		private float totalLifetime = 0;
		private float totalFastFadetime = 0;
		private bool fastFade = false;

		public HUDFadeOutInfoBox(float lifetime, float fadeTime, float forceFadeoutTime)
		{
			boxLifetime = lifetime;
			boxNormalFadeTime = fadeTime;
			boxFastFadeTime = forceFadeoutTime;
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			totalLifetime += gameTime.ElapsedSeconds;

			float alpha = 1;

			if (totalLifetime > boxLifetime) alpha = 1 - (totalLifetime - boxLifetime) / boxNormalFadeTime;
			
			if (fastFade)
			{
				totalFastFadetime += gameTime.ElapsedSeconds;

				alpha = FloatMath.Min(alpha, 1 - totalFastFadetime / boxFastFadeTime);
			}

			if (alpha <= 0) Remove();

			this.Alpha = alpha;
		}

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (CloseOnClick) fastFade = true;
		}

		public void OnOutOfBoundsClick()
		{
			if (CloseOnClick) fastFade = true;
		}
	}
}
