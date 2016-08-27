using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame
{
	static class EntityHelper
	{
		public static GDEntityManager GDManager(this GameEntity e) => (GDEntityManager) e.Manager;
		public static GDGameScreen GDOwner(this GameEntity e) => (GDGameScreen)e.Owner;
	}
}
