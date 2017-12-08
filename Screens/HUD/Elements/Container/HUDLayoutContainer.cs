using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public class HUDLayoutContainer : HUDContainer
	{
		public override int Depth { get; }

		public HUDLayoutContainer(int depth = 0)
		{
			Depth = depth;
		}
		
		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds) { }

		public override void OnInitialize() { }

		public override void OnRemove() { }

		protected override void DoUpdate(SAMTime gameTime, InputState istate) { }
	}
}
