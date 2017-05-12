using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.BatchRenderer.GraphicsWrapper
{
	class DummyGraphicsDeviceWrapper : ISecureGraphicsDeviceWrapper
	{
		public Viewport Viewport { get; set; }

		public DummyGraphicsDeviceWrapper()
		{
			Viewport = new Viewport(0, 0, 800, 600);
		}
	}
}
