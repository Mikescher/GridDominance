using MonoSAMFramework.Portable.Screens.Entities;
using System;
using System.Collections.Generic;
using System.Text;
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
		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.Transparent;

		private readonly LaserNetwork _network;

		public LaserRenderer(GDGameScreen scrn, LaserNetwork nw, LevelBlueprint bp) : base(scrn, GDConstants.ORDER_GAME_LASER)
		{
			_network = nw;

			Position = new Vector2(bp.LevelWidth / 2f, bp.LevelHeight / 2f);
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
			_network.Draw(sbatch);

#if DEBUG
			if (DebugSettings.Get("DebugLaserNetwork"))
			{
				_network.DrawDebug(sbatch);
			}
#endif
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			_network.Update(gameTime, istate);
		}
	}
}
