using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MonoSAMFramework.Portable.Sound
{
	public abstract class SAMSoundPlayer
	{
		public bool IsMuted = false;

		protected SoundEffect ButtonClickEffect = null;
		protected SoundEffect ButtonKeyboardClickEffect = null;

		protected void PlaySoundeffect(SoundEffect e)
		{
			if (IsMuted) return;

			e.Play();
		}

		public abstract void Initialize(ContentManager content);

		public void TryPlayButtonClickEffect()
		{
			if (ButtonClickEffect != null) PlaySoundeffect(ButtonClickEffect);
		}

		public void TryPlayButtonKeyboardClickEffect()
		{
			if (ButtonKeyboardClickEffect != null) PlaySoundeffect(ButtonKeyboardClickEffect);
		}
	}
}
