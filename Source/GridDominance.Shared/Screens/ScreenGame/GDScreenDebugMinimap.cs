using MonoSAMFramework.Portable.DebugTools;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDScreenDebugMinimap : DebugMinimap
	{
		public GDScreenDebugMinimap(GDGameScreen scrn) : base(scrn)
		{
		}
		
		protected override float MaxSize => 192;
		protected override float Padding => 32;
	}
}
