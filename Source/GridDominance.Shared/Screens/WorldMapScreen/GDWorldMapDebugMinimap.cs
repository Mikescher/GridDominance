using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.FloatClasses;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapDebugMinimap : DebugMinimap
	{
		public GDWorldMapDebugMinimap(GDWorldMapScreen scrn) : base(scrn)
		{
		}

		protected override FRectangle Boundings => WorldMapDragAgent.BOUNDING;
		protected override float MaxSize => 192;
		protected override float Padding => 32;
	}
}
