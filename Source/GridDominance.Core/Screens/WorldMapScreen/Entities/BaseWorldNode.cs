using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public abstract class BaseWorldNode : GameEntity
	{
		protected BaseWorldNode(GameScreen scrn, int order) : base(scrn, order)
		{
		}
	}
}
