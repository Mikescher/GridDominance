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

		private bool _isDragging = false;
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

			if (_gdScreen.Mode == LevelEditorMode.AddCannon)
			{
				if (istate.IsExclusiveJustDown)
				{
					istate.Swallow(InputConsumer.GameBackground);

					var stub = _gdScreen.TryInsertCannonStub(new FPoint(x, y));
					if (stub != null) { _preview = stub; _isDragging = true; }
				}
				else if (istate.IsRealDown && _isDragging)
				{
					if (_preview != null) { _preview.Kill(); _preview = null; }

					var stub = _gdScreen.TryInsertCannonStub(new FPoint(x, y));
					if (stub != null) _preview = stub;
				}
				else if (istate.IsExclusiveJustUp && _isDragging)
				{
					istate.Swallow(InputConsumer.GameBackground);

					if (_preview != null)
					{
						_gdScreen.SetMode(LevelEditorMode.Mouse);
						_gdScreen.SelectStub(_preview);
					}

					_preview = null;
					_isDragging = false;
				}
				else
				{
					_isDragging = false;
				}

			}
		}
	}
}
