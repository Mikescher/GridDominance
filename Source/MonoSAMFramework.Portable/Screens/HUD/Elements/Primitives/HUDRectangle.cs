using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDRectangle : HUDElement
	{
		public override int Depth { get; }

		public HUDBackgroundDefinition Definition = HUDBackgroundDefinition.DUMMY;

		public HUDRectangle(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			HUDRenderHelper.DrawBackground(sbatch, bounds, Definition);
		}

		public override void OnInitialize()
		{
			//
		}

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
