using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Sound;

namespace GridDominance.Shared.Resources
{
	public class GDSounds : SAMSoundPlayer
	{
		private const float MUSIC_LEVEL_FADEIN = 1.0f;
		private const float MUSIC_FADEOUT      = 0.5f;
		private const float MUSIC_FADENEXT     = 0.0f;
		
		private const float MUSIC_BACKGROUND_FADEIN = 2.5f;

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
		private SoundEffect effectZoomIn;
		private SoundEffect effectZoomOut;
		private SoundEffect effectError;
		private SoundEffect effectReflect;
		private SoundEffect effectLaser;
		private SoundEffect effectGlass;

		private Song music_background;
		private Song music_tutorial;
		private Song[] music_level;

		public Song[] LevelMusic => music_level;

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
			effectZoomIn        = content.Load<SoundEffect>("sounds/zoomin");
			effectZoomOut       = content.Load<SoundEffect>("sounds/zoomout");
			effectError         = content.Load<SoundEffect>("sounds/error");
			effectReflect       = content.Load<SoundEffect>("sounds/reflect");
			effectLaser         = content.Load<SoundEffect>("sounds/laser");
			effectGlass         = content.Load<SoundEffect>("sounds/glass");

			music_background = content.Load<Song>("music/spirit-forge");
			music_tutorial = content.Load<Song>("music/macaron-island");
			music_level = new[]
			{
				content.Load<Song>("music/spinning-gears"),
				content.Load<Song>("music/cyber-factory"),
				content.Load<Song>("music/tekno-labs"),
				content.Load<Song>("music/mr-krabs"),
				music_tutorial,
				music_background,
			};

			ButtonClickEffect         = effectButton;
			ButtonKeyboardClickEffect = effectKeyboardClick;
		}

		public void PlayEffectOpen()      => PlaySoundeffect(effectOpen);
		public void PlayEffectClose()     => PlaySoundeffect(effectClose);
		public void PlayEffectShoot()     => PlaySoundeffect(effectShoot);
		public void PlayEffectCollision() => PlaySoundeffect(effectCollision);
		public void PlayEffectHit()       => PlaySoundeffect(effectHit);
		public void PlayEffectBoost()     => PlaySoundeffect(effectBoost);
		public void PlayEffectGameWon()   => PlaySoundeffect(effectGameWon);
		public void PlayEffectGameOver()  => PlaySoundeffect(effectGameOver);
		public void PlayEffectZoomIn()    => PlaySoundeffect(effectZoomIn);
		public void PlayEffectZoomOut()   => PlaySoundeffect(effectZoomOut);
		public void PlayEffectError()     => PlaySoundeffect(effectError);
		public void PlayEffectReflect()   => PlaySoundeffect(effectReflect);
		public void PlayEffectGlass()     => PlaySoundeffect(effectGlass);
		
		public SAMEffectWrapper GetEffectLaser(ILifetimeObject owner) => CreateEffect(owner, effectLaser);

		public void PlayMusicTutorial() => PlaySong(music_tutorial, MUSIC_LEVEL_FADEIN, MUSIC_FADEOUT, MUSIC_FADENEXT);
		public void PlayMusicBackground() => PlaySong(music_background, MUSIC_BACKGROUND_FADEIN, MUSIC_FADEOUT, MUSIC_FADENEXT, true, true);

		public void PlayMusicLevel(int i)
		{
			if (i >= 0 && i < music_level.Length) PlaySong(music_level[i], MUSIC_LEVEL_FADEIN, MUSIC_FADEOUT, MUSIC_FADENEXT, true, true);
		}

		protected override void OnEffectError()
		{
			IsEffectsMuted = true;
			MainGame.Inst.Profile.SoundsEnabled = false;
			SAMLog.Warning("SSP::DISABLE_1", "Disable Music due to error");

			MainGame.Inst.DispatchBeginInvoke(() =>
			{
				MainGame.Inst.ShowToast("SSP::ERR_EFFECTS", L10N.T(L10NImpl.STR_ERR_SOUNDPLAYBACK), 32, FlatColors.Flamingo, FlatColors.Foreground, 8f);
			});
		}

		protected override void OnSongError()
		{
			IsMusicMuted = true;
			MainGame.Inst.Profile.MusicEnabled = false;
			SAMLog.Warning("SSP::DISABLE_2", "Disable Sounds due to error");

			MainGame.Inst.DispatchBeginInvoke(() =>
			{
				MainGame.Inst.ShowToast("SSP::ERR_SONG", L10N.T(L10NImpl.STR_ERR_MUSICPLAYBACK), 32, FlatColors.Flamingo, FlatColors.Foreground, 8f);
			});
		}
	}
}
