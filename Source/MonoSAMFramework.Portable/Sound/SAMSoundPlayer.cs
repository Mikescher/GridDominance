using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Sound
{
	public abstract class SAMSoundPlayer
	{
		private const int MAX_EFFECTS_PER_FRAME = 2;

		private enum MPState { Stopped, Play, TransitionOut, TransitionInFromNew, TransitionInFromSame }

		public bool IsEffectsMuted = false;
		public bool IsMusicMuted = false;

		protected SoundEffect ButtonClickEffect = null;
		protected SoundEffect ButtonKeyboardClickEffect = null;

		public bool InitErrorState = false;

		private readonly List<SAMEffectWrapper> _activeEffects = new List<SAMEffectWrapper>();

		private MPState _state = MPState.Stopped;
		private Song[] _currentSet = null;
		private Song[] _nextSet = null;
		private bool _loopSet = true;

		private float _fadeIn = 0f;
		private float _fadeOut = 0f;
		private float _fadeChange = 0f;
		private float _fadeTime = 0f;

		private int _playIndex = 0;

		private int _effectCounter = 0;

		private float _lastMediaPlayHardwarePos = 0f;
		private float _mediaPlayerStartTime = 0f;
		private float _mediaPlayerRealPlayTime = 0f;

		protected SAMSoundPlayer()
		{
			MediaPlayer.IsRepeating = false;
			MediaPlayer.IsShuffled = false;
			MediaPlayer.Volume = 1f;
		}

		protected void PlaySoundeffect(SoundEffect e)
		{
			if (IsEffectsMuted) return;
			if (InitErrorState) return;

			if (_effectCounter >= MAX_EFFECTS_PER_FRAME) return;
			try
			{
				e.Play();
			}
			catch (Exception ex)
			{
				if (ex.GetType().FullName == @"Microsoft.Xna.Framework.Audio.InstancePlayLimitException")
				{
					//ignore
					SAMLog.Warning("SSP::IPLE", "InstancePlayLimitException");
					_effectCounter = 999;
				}
				else
				{
					SAMLog.Error("SSP::PlayEffect", ex);
				}
			}

			_effectCounter++;
		}

		protected void PlaySong(IEnumerable<Song> s, float fadeOut, float fadeIn, float fadeChange, bool loop = true) => PlaySong(s.ToArray(), fadeOut, fadeIn, fadeChange, loop);

		protected void PlaySong(Song s, float fadeOut, float fadeIn, float fadeChange, bool loop = true, bool noreset = false)
		{
			if (noreset && _state == MPState.Play && _nextSet == null && _currentSet != null && _currentSet.Length == 1 && _currentSet[0] == s)
			{
				return;
			}

			if (s == null) { StopSong(); return; }

			PlaySong(new[] { s }, fadeOut, fadeIn, fadeChange, loop);
		}

		protected void PlaySong(Song[] s, float fadeIn, float fadeOut, float fadeChange, bool loop = true)
		{
			if (InitErrorState) return;

			_fadeIn = fadeIn;
			_fadeOut = fadeOut;
			_fadeChange = fadeChange;


			if (_state == MPState.Play)
			{
				_state = MPState.TransitionOut;
				_fadeTime = 0f;
				_nextSet = s;
			}
			else if (s.Any())
			{
				MediaPlayer.Volume = 0f;
				PlaySongInPlayer(s[0]);
				_state = MPState.TransitionInFromNew;
				_fadeTime = 0f;
				_playIndex = 0;
				_currentSet = s;
			}
			else
			{
				_state = MPState.Stopped;
			}
		}

		public void SkipSong()
		{
			if (_state == MPState.Stopped)
			{
				NextSongDirect();
			}
			else
			{
				_state = MPState.TransitionOut;
				_fadeTime = 0f;
			}
		}

		public void StopSong()
		{
			if (_state != MPState.Stopped)
			{
				_state = MPState.TransitionOut;
				_currentSet = null;
				_fadeTime = 0f;
			}
		}

		public abstract void Initialize(ContentManager content);

		public void Update(SAMTime gameTime)
		{
			if (InitErrorState) return;

			UpdateEffects(gameTime);
			UpdateMusic(gameTime);
			UpdateRealPlayTime();
		}

		private void UpdateEffects(SAMTime gameTime)
		{
			for (int i = _activeEffects.Count - 1; i >= 0; i--)
			{
				if (!_activeEffects[i].Owner.Alive)
				{
					_activeEffects[i].TryDispose();
					_activeEffects.RemoveAt(i);
				}
			}

			if (IsEffectsMuted)
			{
				foreach (var e in _activeEffects)
				{
					if (e.IsPlaying) e.Stop();
				}
			}

			_effectCounter = 0;
		}

		private void UpdateMusic(SAMTime gameTime)
		{
			if (MediaPlayer.IsMuted != IsMusicMuted) MediaPlayer.IsMuted = IsMusicMuted;

			switch (_state)
			{
				case MPState.Stopped:
					if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
					break;
				case MPState.Play:
					if (MediaPlayer.State != MediaState.Playing)
					{
						NextSongDirect();
					}
					else
					{
						if (MediaPlayer.State == MediaState.Playing && _currentSet != null && _fadeOut > 0)
						{
							var curr = _mediaPlayerRealPlayTime;
							var max  = _currentSet[_playIndex].Duration.TotalSeconds;

							if (max - curr < _fadeOut)
							{
								_state = MPState.TransitionOut;
								_fadeTime = 0f;
							}
						}
					}
					break;
				case MPState.TransitionOut:
					_fadeTime += gameTime.ElapsedSeconds;
					if (_fadeTime > _fadeOut)
					{
						NextSongDirect();
					}
					else
					{
						MediaPlayer.Volume = 1 - (_fadeTime / _fadeOut);
					}
					break;
				case MPState.TransitionInFromNew:
					_fadeTime += gameTime.ElapsedSeconds;
					if (_fadeTime > _fadeIn)
					{
						_state = MPState.Play;
						MediaPlayer.Volume = 1f;
					}
					else
					{
						MediaPlayer.Volume = (_fadeTime / _fadeIn);
					}
					break;
				case MPState.TransitionInFromSame:
					_fadeTime += gameTime.ElapsedSeconds;
					if (_fadeTime > _fadeChange)
					{
						_state = MPState.Play;
						MediaPlayer.Volume = 1f;
					}
					else
					{
						MediaPlayer.Volume = (_fadeTime / _fadeChange);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateRealPlayTime()
		{
			if (MediaPlayer.State == MediaState.Stopped)
			{
				_mediaPlayerRealPlayTime = 0f;
				return;
			}
			if (MediaPlayer.State == MediaState.Paused)
			{
				_mediaPlayerRealPlayTime = (float)MediaPlayer.PlayPosition.TotalSeconds;
				return;
			}

			var hardware = (float)MediaPlayer.PlayPosition.TotalSeconds;
			var now = Environment.TickCount / 1000f;

			if (_lastMediaPlayHardwarePos != hardware)
			{
				_lastMediaPlayHardwarePos = hardware;
				_mediaPlayerStartTime = (now - hardware) - 0.75f;
			}

			var estimate = now - _mediaPlayerStartTime;

			//if (FloatMath.Abs(estimate - hardware) > 2.0f) return hardware;
			_mediaPlayerRealPlayTime = estimate;
		}
		
		private void NextSongDirect()
		{
			if (InitErrorState) return;

			if (_nextSet != null)
			{
				_currentSet = _nextSet;
				_nextSet = null;
				_playIndex = 0;
			}
			else
			{
				_playIndex++;
			}

			if (_currentSet == null)
			{
				_state = MPState.Stopped;
				return;
			}

			if (_playIndex >= _currentSet.Length)
			{
				if (_loopSet)
				{
					_playIndex = 0;
				}
				else
				{
					_state = MPState.Stopped;
					return;
				}
			}

			if (_playIndex < _currentSet.Length)
			{
				if (FloatMath.IsZero(_fadeChange))
				{
					MediaPlayer.Volume = 1;
					PlaySongInPlayer(_currentSet[_playIndex]);
					_state = MPState.Play;
				}
				else
				{
					MediaPlayer.Volume = 0;
					PlaySongInPlayer(_currentSet[_playIndex]);
					_fadeTime = 0f;
					_state = MPState.TransitionInFromSame;
				}
			}
			else
			{
				_state = MPState.Stopped;
			}

		}

		private void PlaySongInPlayer(Song s)
		{
			try
			{
				MediaPlayer.Play(s);
				_mediaPlayerStartTime = Environment.TickCount / 1000f;
				_mediaPlayerRealPlayTime = 0f;

				UpdateRealPlayTime();
			}
			catch (Exception e)
			{
				SAMLog.Error("SSP::PlaySong", e);
			}
		}

		public void TryPlayButtonClickEffect()
		{
			if (ButtonClickEffect != null) PlaySoundeffect(ButtonClickEffect);
		}

		public void TryPlayButtonKeyboardClickEffect()
		{
			if (ButtonKeyboardClickEffect != null) PlaySoundeffect(ButtonKeyboardClickEffect);
		}

		public string GetEffectsStringState()
		{
			if (InitErrorState) return "ERR";

			return $"{_activeEffects.Count} active soundeffects";
		}

		public string GetMusicStringState()
		{
			if (InitErrorState) return "ERR";

			var perc1 = 0d;
			var perc2 = 0d;
			var total = 0d;
			var song = "NONE";
			if (_currentSet != null)
			{
				total = _currentSet[_playIndex].Duration.TotalSeconds;
				perc1 = MediaPlayer.PlayPosition.TotalSeconds * 100f / total;
				perc2 = _mediaPlayerRealPlayTime * 100f / total;
				song = _currentSet[_playIndex].Name;
			}

			string add = "";
			if (IsMusicMuted) add += " [S-MUTED]";
			if (IsEffectsMuted) add += " [E-MUTED]";
			if (_nextSet != null) add += " [QUEUED]";
			if (_loopSet) add += " [LOOP]";

			switch (_state)
			{
				case MPState.Stopped:
					return $"Stopped {add}";
				case MPState.Play:
					return $"Play[{_playIndex} : {song}] ({perc1:00}% | {perc2:00}%) (= {_mediaPlayerRealPlayTime:000.0}s) {add}";
				case MPState.TransitionOut:
					return $"TransitionOut[{song}] ({_fadeTime * 100f / _fadeOut:00.00}%) {add}";
				case MPState.TransitionInFromSame:
					return $"TransitionIn[{song}] ({_fadeTime * 100f / _fadeChange:00.00}%) {add}";
				case MPState.TransitionInFromNew:
					return $"TransitionIn[{song}] ({_fadeTime * 100f / _fadeIn:00.00}%) {add}";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public SAMEffectWrapper CreateEffect(ILifetimeObject owner, SoundEffect effect)
		{
			var e = new SAMEffectWrapper(this, owner, effect);
			_activeEffects.Add(e);
			return e;
		}
	}
}
