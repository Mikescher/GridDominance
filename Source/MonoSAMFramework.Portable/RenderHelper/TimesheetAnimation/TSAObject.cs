using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation
{
	public abstract class TSAObject
	{
		public readonly int Order;

		protected TSAObject(int o)
		{
			Order = o;
		}

		public abstract void Draw(IBatchRenderer sbatch, FPoint offset, float scale, float time);
	}
}
