using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	public abstract class SCCMListElement : HUDContainer
	{
		public override int Depth => 2;

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
