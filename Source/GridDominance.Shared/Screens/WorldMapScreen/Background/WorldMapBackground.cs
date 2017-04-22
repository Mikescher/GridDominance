using System.Collections.Generic;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;

namespace GridDominance.Shared.Screens.WorldMapScreen.Background
{
	public class WorldMapBackground : GameBackground
	{
		private const int TILE_WIDTH = GDConstants.TILE_WIDTH;

		private const int NODE_SPREAD = 7;

		public float GridLineAlpha = 1.0f;
		public float BackgroundPercentageOverride = 1.0f;
		private readonly Dictionary<int, float> _tileValues = new Dictionary<int, float>();

		public WorldMapBackground(GameScreen scrn) : base(scrn)
		{
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			//
		}

		public void InitBackground(List<LevelNode> nodes)
		{
			foreach (LevelNode node in nodes)
			{
				int pow = (int)(node.LevelData.CompletionCount * 1.5f);

				if (pow == 0) continue;

				for (int dx = -(6 + NODE_SPREAD); dx <= 6 + NODE_SPREAD; dx++)
				{
					for (int dy = -(6 + NODE_SPREAD); dy <= 6 + NODE_SPREAD; dy++)
					{
						float dist = new Vector2(dx, dy).Length();
						int x = (int) (node.Position.X / TILE_WIDTH) + dx;
						int y = (int) (node.Position.Y / TILE_WIDTH) + dy;
						int key = 100000 * x + y;

						float v = 0;
						if (!_tileValues.TryGetValue(key, out v)) v = 0;

						if (dist <= pow)
						{
							v = 1;
						}
						else
						{
							var nv = (NODE_SPREAD - (dist - pow)) / (1f * NODE_SPREAD);
							if (nv > v) v = nv;
						}

						if (v > 0) _tileValues[key] = v;
					}
				}
			}
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			if (MainGame.Inst.Profile.EffectsEnabled)
				DrawColored(sbatch);
			else
				DrawSimple(sbatch);
		}

		private void DrawSimple(IBatchRenderer sbatch)
		{
			int offX = TILE_WIDTH * (int)(Owner.MapOffsetX / TILE_WIDTH);
			int offY = TILE_WIDTH * (int)(Owner.MapOffsetY / TILE_WIDTH);

			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);

			sbatch.DrawStretched(
				Textures.TexPixel, 
				new FRectangle(
					-extensionX * TILE_WIDTH - offX, 
					-extensionY * TILE_WIDTH - offY, 
					(countX + 2*extensionX) * TILE_WIDTH, 
					(countY + 2 * extensionY) * TILE_WIDTH), 
				FlatColors.Background);

			if (GridLineAlpha > 0)
			{
				for (int x = -extensionX; x < countX + extensionX; x++)
				{
					for (int y = -extensionY; y < countY + extensionY; y++)
					{
						sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * TILE_WIDTH - offX, y * TILE_WIDTH - offY, TILE_WIDTH, TILE_WIDTH), Color.White * GridLineAlpha);
					}
				}
			}
		}

		private void DrawColored(IBatchRenderer sbatch)
		{
			int offX = (int)(Owner.MapOffsetX / TILE_WIDTH);
			int offY = (int)(Owner.MapOffsetY / TILE_WIDTH);

			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);
			
			for (int x = -extensionX; x < countX + extensionX; x++)
			{
				for (int y = -extensionY; y < countY + extensionY; y++)
				{
					var color = FlatColors.Background;

					var rx = x - offX;
					var ry = y - offY;

					float perc;
					if (_tileValues.TryGetValue(100000 * rx + ry, out perc)) color = ColorMath.Blend(FlatColors.Background, FlatColors.BackgroundGreen, perc * BackgroundPercentageOverride);

					sbatch.DrawStretched(
						Textures.TexPixel, 
						new FRectangle(
							rx * GDConstants.TILE_WIDTH,
							ry * GDConstants.TILE_WIDTH,
							GDConstants.TILE_WIDTH, 
							GDConstants.TILE_WIDTH), 
						color);

					if (GridLineAlpha > 0)
					{
						sbatch.DrawStretched(
							Textures.TexTileBorder,
							new FRectangle(
								rx * GDConstants.TILE_WIDTH,
								ry * GDConstants.TILE_WIDTH,
								GDConstants.TILE_WIDTH,
								GDConstants.TILE_WIDTH),
							Color.White * GridLineAlpha);
					}
				}
			}
		}
	}
}