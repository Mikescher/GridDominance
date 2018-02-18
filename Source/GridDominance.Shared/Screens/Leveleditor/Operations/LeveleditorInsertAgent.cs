using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor;
using GridDominance.Shared.Screens.Leveleditor.Entities;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
	class LeveleditorInsertAgent : SAMUpdateOp<LevelEditorScreen>
	{
		private LevelEditorScreen _gdScreen;

		public override string Name => "LeveleditorInsertAgent";

		private ILeveleditorStub _preview = null;

		public LeveleditorInsertAgent()
		{
			//
		}

		protected override void OnInit(LevelEditorScreen screen)
		{
			_gdScreen = screen;
		}

		protected override void OnUpdate(LevelEditorScreen screen, SAMTime gameTime, InputState istate)
		{
			const float raster = (GDConstants.TILE_WIDTH / 2f);
			var x = raster * FloatMath.Round(istate.GamePointerPositionOnMap.X / raster);
			var y = raster * FloatMath.Round(istate.GamePointerPositionOnMap.Y / raster);

			if (_gdScreen.Mode == LevelEditorMode.AddCannon && istate.IsExclusiveJustDown)
			{
				istate.Swallow(InputConsumer.GameBackground);

				var stub = _gdScreen.CanInsertCannonStub(new FPoint(x, y), null);
				if (stub != null)
				{
					_preview = stub;
					_gdScreen.Entities.AddEntity(stub);
					_gdScreen.SelectStub(_preview);
					_gdScreen.DragAgent.ManualStartCannonMove(istate);
				}
			}

			if (_gdScreen.Mode == LevelEditorMode.AddObstacle && istate.IsExclusiveJustDown)
			{
				istate.Swallow(InputConsumer.GameBackground);

				var stub = _gdScreen.CanInsertObstacleStub(new FPoint(x, y), null);
				if (stub != null)
				{
					_preview = stub;
					_gdScreen.Entities.AddEntity(stub);
					_gdScreen.SelectStub(_preview);
					_gdScreen.DragAgent.ManualStartObstacleMove(istate);
				}
			}
		}
	}
}
