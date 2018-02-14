using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.GlobalAgents
{
	class HighscoreAgent : CyclicOperation<MainGame>
	{
		private const float INTERVAL    = 10 * 60;

		public override string Name => "HighscoreAgent";

		public HighscoreAgent() : base(INTERVAL, false) { }

		protected override void OnCycle(MainGame element, int counter)
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
