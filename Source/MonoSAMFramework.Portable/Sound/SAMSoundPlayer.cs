using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Sound
{
	public abstract class SAMSoundPlayer
	{
		private enum MPState { Stopped, Play, TransitionOut, TransitionIn }

		public bool IsEffectsMuted = false;
		public bool IsMusicMuted = false;

		protected SoundEffect ButtonClickEffect = null;
		protected SoundEffect ButtonKeyboardClickEffect = null;


		private MPState _state = MPState.Stopped;
		private Song[] _currentSet = null;
		private Song[] _nextSet = null;
		private bool _loopSet = true;

		private float _fadeIn = 0f;
		private float _fadeOut = 0f;
		private float _fadeTime = 0f;

		private int _playIndex = 0;

		protected SAMSoundPlayer()
		{
			MediaPlayer.IsRepeating = false;
			MediaPlayer.IsShuffled = false;
			MediaPlayer.Volume = 1f;
		}

		protected void PlaySoundeffect(SoundEffect e)
		{
			if (IsEffectsMuted) return;

			e.Play();
		}

		protected void PlaySong(IEnumerable<Song> s, float fadeOut, float fadeIn, bool loop = true) => PlaySong(s.ToArray(), fadeOut, fadeIn, loop);
		protected void PlaySong(Song s, float fadeOut, float fadeIn, bool loop = true) => PlaySong(new[]{s}, fadeOut, fadeIn, loop);

		protected void PlaySong(Song[] s, float fadeIn, float fadeOut, bool loop = true)
		{
			_fadeIn = fadeIn;
			_fadeOut = fadeOut;

			if (_state == MPState.Play)
			{
				_state = MPState.TransitionOut;
				_fadeTime = 0f;
				_nextSet = s;
			}
			else if (s.Any())
			{
				MediaPlayer.Volume = 0f;
				MediaPlayer.Play(s[0]);
				_state = MPState.TransitionIn;
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
				case MPState.TransitionIn:
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
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void NextSongDirect()
		{
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
				MediaPlayer.Volume = 0;
				MediaPlayer.Stop();
				MediaPlayer.Play(_currentSet[_playIndex]);
				_fadeTime = 0f;
				_state = MPState.TransitionIn;
			}
			else
			{
				_state = MPState.Stopped;
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

		public string GetStringState()
		{
			var perc = 0d;
			var song = "NONE";
			if (_currentSet != null)
			{
				perc = MediaPlayer.PlayPosition.TotalSeconds * 100f / _currentSet[_playIndex].Duration.TotalSeconds;
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
					return $"Play[{_playIndex} : {song}] ({perc:00.00}%) {add}";
				case MPState.TransitionOut:
					return $"TransitionOut[{song}] ({_fadeTime * 100f / _fadeOut:00.00}%) {add}";
				case MPState.TransitionIn:
					return $"TransitionIn[{song}] ({_fadeTime * 100f / _fadeIn:00.00}%) {add}";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
