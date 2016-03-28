using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen.hud
{
	class GDGameHUD
	{
		private readonly GameScreen owner;

		public GDGameHUD(GameScreen scrn)
		{
			owner = scrn;

			OnInitialize();
		}

		private void OnInitialize()
		{
//			throw new NotImplementedException();
		}

		public void Update(GameTime gameTime, InputState istate)
		{
//			throw new NotImplementedException();
		}

		public void Draw(SpriteBatch sbatch)
		{
//			throw new NotImplementedException();
		}
	}
}
