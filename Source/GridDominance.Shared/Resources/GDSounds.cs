using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoSAMFramework.Portable.Sound;

namespace GridDominance.Shared.Resources
{
	public class GDSounds : SAMSoundPlayer
	{
		private SoundEffect effectButton;
		private SoundEffect effectKeyboardClick;
		private SoundEffect effectOpen;
		private SoundEffect effectClose;
		private SoundEffect effectShoot;
		private SoundEffect effectCollision;
		private SoundEffect effectHit;
		private SoundEffect effectBoost;
		private SoundEffect effectGameWon;
		private SoundEffect effectGameOver;

		public override void Initialize(ContentManager content)
		{
			effectButton        = content.Load<SoundEffect>("sounds/button");
			effectKeyboardClick = content.Load<SoundEffect>("sounds/click");
			effectOpen          = content.Load<SoundEffect>("sounds/open");
			effectClose         = content.Load<SoundEffect>("sounds/close");
			effectShoot         = content.Load<SoundEffect>("sounds/shoot");
			effectCollision     = content.Load<SoundEffect>("sounds/collision");
			effectHit           = content.Load<SoundEffect>("sounds/hit");
			effectBoost         = content.Load<SoundEffect>("sounds/boost");
			effectGameWon       = content.Load<SoundEffect>("sounds/gamewon");
			effectGameOver      = content.Load<SoundEffect>("sounds/gameover");

			this.ButtonClickEffect         = effectButton;
			this.ButtonKeyboardClickEffect = effectKeyboardClick;

		}

		public void PlayEffectButton() => PlaySoundeffect(effectButton);
		public void PlayEffectClick() => PlaySoundeffect(effectKeyboardClick);
		public void PlayEffectOpen() => PlaySoundeffect(effectOpen);
		public void PlayEffectClose() => PlaySoundeffect(effectClose);
		public void PlayEffectShoot() => PlaySoundeffect(effectShoot);
		public void PlayEffectCollision() => PlaySoundeffect(effectCollision);
		public void PlayEffectHit() => PlaySoundeffect(effectHit);
		public void PlayEffectBoost() => PlaySoundeffect(effectBoost);
		public void PlayEffectGameWon() => PlaySoundeffect(effectGameWon);
		public void PlayEffectGameOver() => PlaySoundeffect(effectGameOver);
	}
}
