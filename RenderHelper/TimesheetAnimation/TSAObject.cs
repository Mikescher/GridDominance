using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Interfaces;

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
