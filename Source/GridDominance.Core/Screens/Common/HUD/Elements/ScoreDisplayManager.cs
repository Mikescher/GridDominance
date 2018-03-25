using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	public class ScoreDisplayManager
	{
		public readonly ScoreDisplay ScoreDisplay;
		public readonly MultiplayerScoreDisplay MPScoreDisplay;
		public readonly StarsScoreDisplay StarsScoreDisplay;
		public readonly SCCMScoreDisplay SCCMScoreDisplay;

		public ScoreDisplayManager(GameHUD hud, bool firstShow)
		{
			hud.AddElement(ScoreDisplay      = new ScoreDisplay(firstShow));
			hud.AddElement(MPScoreDisplay    = new MultiplayerScoreDisplay(firstShow));
			hud.AddElement(StarsScoreDisplay = new StarsScoreDisplay(firstShow));
			hud.AddElement(SCCMScoreDisplay  = new SCCMScoreDisplay(firstShow));

			FakeFirstUpdate();
		}

		private void FakeFirstUpdate()
		{
			Update();
			ScoreDisplay.Update2(MonoSAMGame.CurrentTime);
			MPScoreDisplay.Update2(MonoSAMGame.CurrentTime);
			StarsScoreDisplay.Update2(MonoSAMGame.CurrentTime);
			SCCMScoreDisplay.Update2(MonoSAMGame.CurrentTime);
		}

		public void FinishCounter()
		{
			ScoreDisplay.FinishCounter();
			MPScoreDisplay.FinishCounter();
			StarsScoreDisplay.FinishCounter();
			SCCMScoreDisplay.FinishCounter();
		}

		public void Update()
		{
			var refpos = new FPoint(ScoreDisplay.RelativePosition.X, ScoreDisplay.RelativePosition.Y + ScoreDisplay.Height + 10);

			MPScoreDisplay.RefPosition = refpos;
			if (MPScoreDisplay.IsVisible) refpos += new Vector2(0, MPScoreDisplay.Height + 10);

			StarsScoreDisplay.RefPosition = refpos;
			if (StarsScoreDisplay.IsVisible) refpos += new Vector2(0, StarsScoreDisplay.Height + 10);

			SCCMScoreDisplay.RefPosition = refpos;
			if (SCCMScoreDisplay.IsVisible) refpos += new Vector2(0, SCCMScoreDisplay.Height + 10);
		}
	}
}
