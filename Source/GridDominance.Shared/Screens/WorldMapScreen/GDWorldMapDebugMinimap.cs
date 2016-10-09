using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapDebugMinimap : DebugMinimap
	{
		public GDWorldMapDebugMinimap(GDWorldMapScreen scrn) : base(scrn)
		{
		}

		protected override FRectangle Boundings => new FRectangle(0, 0, GDWorldMapScreen.VIEW_WIDTH, GDWorldMapScreen.VIEW_HEIGHT);
		protected override float MaxSize => 192;
		protected override float Padding => 32;
	}
}
