using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Levelfileformat.Blueprint;
using MonoSAMFramework.Portable.DebugTools;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	class LaserRenderer : GameEntity
	{
		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.Transparent;

		private readonly LaserNetwork _network;

		public LaserRenderer(GDGameScreen scrn, LaserNetwork nw, LevelBlueprint bp) : base(scrn, GDConstants.ORDER_GAME_LASER)
		{
			_network = nw;

			Position = new FPoint(bp.LevelWidth / 2f, bp.LevelHeight / 2f);
			DrawingBoundingBox = new FSize(bp.LevelWidth, bp.LevelHeight);
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			DrawNetwork(sbatch);

#if DEBUG
			if (DebugSettings.Get("DebugLaserNetwork"))
			{
				DrawNetworkDebug(sbatch);
			}
#endif
		}

		private void DrawNetwork(IBatchRenderer sbatch)
		{
			// TODO Draw Laz0rs
		}

		private void DrawNetworkDebug(IBatchRenderer sbatch)
		{
			foreach (var src in _network.Sources)
			{
				foreach (var ray in src.Lasers)
				{
					sbatch.DrawLine(ray.Start, ray.End, Color.LimeGreen, 4);

					if (ray.Terminator == LaserRayTerminator.LaserDoubleTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.Salmon);
					if (ray.Terminator == LaserRayTerminator.LaserSelfTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.CornflowerBlue);
					if (ray.Terminator == LaserRayTerminator.LaserFaultTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.Magenta);
				}
			}
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			_network.Update(gameTime, istate);
		}
	}
}
