using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDRotatingImage : HUDImage
	{
		public float RotationSpeed = 1f; //Rot per sec

		public HUDRotatingImage(int depth = 0) : base(depth)
		{
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			Rotation = FloatMath.IncModulo(Rotation, RotationSpeed * FloatMath.TAU * gameTime.ElapsedSeconds, FloatMath.TAU);
		}
	}
}
