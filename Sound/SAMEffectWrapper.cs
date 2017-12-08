﻿using System;
using Microsoft.Xna.Framework.Audio;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Sound
{
	public class SAMEffectWrapper
	{
		public readonly SAMSoundPlayer Player;
		public readonly ILifetimeObject Owner;
		public readonly SoundEffectInstance Instance;

		public SAMEffectWrapper(SAMSoundPlayer player, ILifetimeObject owner, SoundEffect effect)
		{
			Player = player;
			Owner = owner;
			Instance = effect.CreateInstance();
		}

		public bool IsLooped { get => Instance.IsLooped; set => Instance.IsLooped = value; }

		public bool IsPlaying => Instance.State == SoundState.Playing;
		
		public void Play()
		{
			if (Player.IsEffectsMuted) return;

			try
			{
				Instance.Play();
			}
			catch (Exception ex)
			{
				if (ex.GetType().FullName == @"Microsoft.Xna.Framework.Audio.InstancePlayLimitException")
				{
					//ignore
					SAMLog.Warning("SSP::IPLE", "InstancePlayLimitException");
				}
				else
				{
					throw;
				}
			}
		}

		public void Stop()
		{
			Instance.Stop();
		}

		public void TryDispose()
		{
			if (!Instance.IsDisposed)
			{
				if (Instance.State == SoundState.Playing) Instance.Stop();
				Instance.Dispose();
			}
		}
	}
}
