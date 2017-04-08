using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;

namespace GridDominance.Shared.Screens.OverworldScreen.Background
{
	class OverworldBackground : GameBackground
	{
		class OVBCell
		{
			public int Delta = 0;
			public float Power = 0f;
		}

		private const int TILE_COUNT_X = GDConstants.GRID_WIDTH;
		private const int TILE_COUNT_Y = GDConstants.GRID_HEIGHT;

		private const int ARRAY_EXTEND = 8;

		private const float BLINK_DELAY_MIN = 0.1f;
		private const float BLINK_DELAY_DERIVATION = 0.25f;
		private const float BLINK_HALFTIME = 2.3f;

		private const int CELL_W = TILE_COUNT_X + 2 * ARRAY_EXTEND;
		private const int CELL_H = TILE_COUNT_Y + 2 * ARRAY_EXTEND;

		private readonly OVBCell[,] _cells = new OVBCell[CELL_W, CELL_H];
		private float _blinkCountdown = 0.02f;

		public OverworldBackground(GameScreen scrn) : base(scrn)
		{
			for (int x = 0; x < CELL_W; x++)
			{
				for (int y = 0; y < CELL_H; y++)
				{
					_cells[x, y] = new OVBCell();
				}
			}
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			if (!MainGame.Inst.Profile.EffectsEnabled) return;

			for (int x = 0; x < CELL_W; x++)
			{
				for (int y = 0; y < CELL_H; y++)
				{
					var c = _cells[x, y];

					if (c.Delta == 0) continue;

					if (c.Delta == 1)
					{
						c.Power += gameTime.ElapsedSeconds / BLINK_HALFTIME;
						if (c.Power >= 1)
						{
							c.Power = 1;
							c.Delta = -1;
						}
					}
					else // if (c.Delta == -1)
					{
						c.Power -= gameTime.ElapsedSeconds / BLINK_HALFTIME;
						if (c.Power <= 0)
						{
							c.Power = 0;
							c.Delta = 0;
						}
					}
				}
			}

			_blinkCountdown -= gameTime.ElapsedSeconds;
			if (_blinkCountdown <= 0)
			{
				_blinkCountdown = BLINK_DELAY_MIN + FloatMath.GetRangedRandom(BLINK_DELAY_DERIVATION);

				int px = FloatMath.GetRangedIntRandom(0, CELL_W);
				int py = FloatMath.GetRangedIntRandom(0, CELL_H);

				var c = _cells[px, py];
				if (c.Delta == 0) c.Delta = 1;
			}
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / GDConstants.TILE_WIDTH);
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / GDConstants.TILE_WIDTH);

			if (MainGame.Inst.Profile.EffectsEnabled)
			{
				for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
				{
					for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
					{
						int cx = ARRAY_EXTEND + x;
						int cy = ARRAY_EXTEND + y;

						var color = FlatColors.Background;

						if (cx >= 0 && cy >= 0 && cx < CELL_W && cy < CELL_H)
						{
							var c = _cells[cx, cy];
							if (c.Power > 0) color = ColorMath.Blend(FlatColors.Background, FlatColors.BackgroundGreen, FloatMath.FunctionEaseInOutCubic(c.Power));
						}

						sbatch.DrawStretched(Textures.TexPixel, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), color);
						sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), Color.White);
					}
				}
			}
			else
			{
				sbatch.DrawStretched(
					Textures.TexPixel,
					new FRectangle(
						-extensionX * GDConstants.TILE_WIDTH,
						-extensionY * GDConstants.TILE_WIDTH,
						(TILE_COUNT_X + 2 * extensionX + 1) * GDConstants.TILE_WIDTH,
						(TILE_COUNT_Y + 2 * extensionY + 1) * GDConstants.TILE_WIDTH),
					FlatColors.Background);

				for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
				{
					for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
					{
						sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), Color.White);
					}
				}
			}

		}
	}
}
