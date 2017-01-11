using MonoSAMFramework.Portable.DebugTools;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapDebugMinimap : DebugMinimap
	{
		public GDWorldMapDebugMinimap(GDWorldMapScreen scrn) : base(scrn)
		{
		}

		protected override float MaxSize => 192;
		protected override float Padding => 32;
	}
}
