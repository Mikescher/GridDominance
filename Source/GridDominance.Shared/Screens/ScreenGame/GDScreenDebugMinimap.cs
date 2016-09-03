using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDScreenDebugMinimap : DebugMinimap
	{
		public GDScreenDebugMinimap(GDGameScreen scrn) : base(scrn)
		{
		}

		protected override FRectangle Boundings => new FRectangle(0, 0, GDGameScreen.VIEW_WIDTH, GDGameScreen.VIEW_HEIGHT);
		protected override float MaxSize => 192;
		protected override float Padding => 32;
	}
}
