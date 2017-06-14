using System;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameAgents;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.GlobalAgents
{
	class HighscoreAgent : GameIntervalAgent
	{
		private const float DELAY_START = 0;
		private const float INTERVAL    = 10 * 60;

		public HighscoreAgent() : base("HighscoreAgent", DELAY_START, INTERVAL) { }

		protected override void OnEvent(SAMTime gameTime)
		{
			var profile = MainGame.Inst.Profile;
			
			if (profile.OnlineUserID >= 0)
			{
				MainGame.Inst.Backend.Ping(profile).ContinueWith(t => MainGame.Inst.Backend.DownloadHighscores(profile)).EnsureNoError();
			}
			else
			{
				MainGame.Inst.Backend.CreateUser(profile).ContinueWith(t => MainGame.Inst.Backend.DownloadHighscores(profile)).EnsureNoError();
			}
		}
	}
}
