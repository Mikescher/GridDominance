using GridDominance.Shared.Screens.NormalGameScreen.Entities;

namespace GridDominance.Shared.Network.Multiplayer
{
	public struct RemoteBulletHint
	{
		public Bullet Bullet;
		public RemoteBullet.RemoteBulletState State;

		public int RemainingPostDeathTransmitions;
	}
}
