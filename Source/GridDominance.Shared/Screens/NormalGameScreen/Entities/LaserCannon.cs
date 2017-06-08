using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class LaserCannon : Cannon //TODO Laser
	{
		private readonly LaserCannonBlueprint Blueprint;
		private readonly LaserSource _laserSource;

		public LaserCannon(GDGameScreen scrn, LaserCannonBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;

			_laserSource = scrn.LaserNetwork.AddSource(this);
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			bool active = CannonHealth.TargetValue >= 1 && controller.DoBarrelRecharge();
			_laserSource.SetState(active, Fraction, Rotation.ActualValue);
		}

		public override void ResetChargeAndBooster()
		{
			FinishAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			//
		}
	}
}
