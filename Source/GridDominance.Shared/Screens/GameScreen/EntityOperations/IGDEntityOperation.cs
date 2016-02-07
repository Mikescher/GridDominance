using GridDominance.Shared.Framework;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen
{
	interface IGDEntityOperation
	{
		bool Update(GDEntity entity, GameTime gameTime, InputState istate);
	}
}