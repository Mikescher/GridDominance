using MonoSAMFramework.Portable.BatchRenderer;

namespace MonoSAMFramework.Portable.Interfaces
{
	public interface ISAMLayeredDrawable
	{
		void DrawBackground(IBatchRenderer sbatch);
		void DrawForeground(IBatchRenderer sbatch);
	}
}
