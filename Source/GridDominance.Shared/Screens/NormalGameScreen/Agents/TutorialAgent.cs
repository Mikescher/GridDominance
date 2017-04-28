using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
	public class TutorialAgent : GameScreenAgent
	{
		public override bool Alive { get; } = true;

		private readonly GDGameScreen _screen;
		private readonly Cannon _cannon1; // bottomLeft
		private readonly Cannon _cannon2; // topLeft
		private readonly Cannon _cannon3; // topCenter
		private readonly Cannon _cannon4; // topRight
		private readonly Cannon _cannon5; // BottomRight

		public TutorialAgent(GDGameScreen scrn) : base(scrn)
		{
			_screen = scrn;

			_cannon1 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 1001);
			_cannon2 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 1002);
			_cannon3 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 3000);
			_cannon4 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 2002);
			_cannon5 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 2001);
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
