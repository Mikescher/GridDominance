using GridDominance.Shared.Screens.GameScreen;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame
{
	abstract class GDEntity : GameEntity
	{
		public GDEntityManager GDManager => (GDEntityManager) Manager;
		public GDGameScreen GDOwner => (GDGameScreen) Owner;

		protected GDEntity(MonoSAMFramework.Portable.Screens.GameScreen scrn) : base(scrn) { }
	}
}
