using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDLambdaLabel : HUDLabel
	{
		public Func<string> Lambda = null;

		public HUDLambdaLabel(int depth = 0) : base(depth)
		{

		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			if (Lambda != null) Text = Lambda();
		}
	}
}
