using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class StandardDebugMinimapImplementation : DebugMinimap
	{
		protected override float MaxSize { get; }
		protected override float Padding { get; }

		public StandardDebugMinimapImplementation(GameScreen scrn, float size, float pad) : base(scrn)
		{
			MaxSize = size;
			Padding = pad;
		}
	}
}
