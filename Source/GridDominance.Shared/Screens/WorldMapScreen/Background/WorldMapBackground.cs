using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;

namespace GridDominance.Shared.Screens.WorldMapScreen.Background
{
	public class WorldMapBackground : GameBackground
	{
		private const int TILE_WIDTH = GDConstants.TILE_WIDTH;

		private const int NODE_SPREAD = 9;
		private const int GRADIENT_RESOLUTION = 12;

		public float GridLineAlpha = 1.0f;
		public float BackgroundPercentageOverride = 1.0f;

		private readonly Dictionary<int, float> _tileValues = new Dictionary<int, float>();
		private readonly List<Tuple<Color, FRectangle>> partitions = new List<Tuple<Color, FRectangle>>();

		public readonly GDWorldMapScreen GDScreen;

		public WorldMapBackground(GDWorldMapScreen scrn) : base(scrn)
		{
			GDScreen = scrn;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			//
		}

		public void InitBackground(List<LevelNode> nodes, Rectangle bounds)
		{
			CreateTiles(nodes);

#if DEBUG
			var sw = Stopwatch.StartNew();
			CreatePartitions(bounds);
			SAMLog.Debug($"Time for partition algorithm: {sw.ElapsedMilliseconds}ms");
			SAMLog.Debug($"PartitionCount: {partitions.Count} / {bounds.Width*bounds.Height}");
#else
			CreatePartitions(bounds);
#endif

		}

		private void CreatePartitions(Rectangle bounds)
		{
			bool[,] finished = new bool[bounds.Width, bounds.Height];

			int GridGet(int x, int y)
			{
				float ggftype = 0f;
				if (_tileValues.TryGetValue(100000 * (x + bounds.Left) + (y + bounds.Top), out _)) return (int) (ggftype * GRADIENT_RESOLUTION);
				return 0;
			}

			var pointQueue = new Queue<Tuple<int, int>>();
			pointQueue.Enqueue(Tuple.Create(0, 0));

			int counter = 0;

			while (pointQueue.Any())
			{
				if (counter > 75000) { SAMLog.Error("WMBG:P_TO", $"Ran out of steps in partitioning algorithm ({bounds})"); return; }

				var tl = pointQueue.Dequeue();
				var tl_x = tl.Item1;
				var tl_y = tl.Item2;
				if (finished[tl_x, tl_y]) continue;

				if (tl_x > 0 && !finished[tl_x - 1, tl_y]) { pointQueue.Enqueue(Tuple.Create(tl_x, tl_y)); continue; }
				if (tl_y > 0 && !finished[tl_x, tl_y - 1]) { pointQueue.Enqueue(Tuple.Create(tl_x, tl_y)); continue; }

				int partitionvalue = GridGet(tl_x, tl_y);

				int s = 1;
				int w = 1;
				int h = 1;

				for (int i = 1; tl_x + i <= bounds.Width; i++)
				{
					if (GridGet(tl_x + i - 1, tl_y) == partitionvalue && !finished[tl_x + i - 1, tl_y]) s = w = i;
					else break;
				}

				int idx = w;
				for (int idy = h; tl_y + idy <= bounds.Height; idy++)
				{
					int nidx = 0;
					while (nidx < idx && GridGet(tl_x + nidx, tl_y + idy - 1) == partitionvalue && !finished[tl_x + nidx, tl_y + idy - 1]) nidx++;
					idx = nidx;
					if (idx <= 0) break;
					if (idx * idy > s) s = (w = idx) * (h = idy);
				}

#if DEBUG
				for (int iw = 0; iw < w; iw++)
				{
					for (int ih = 0; ih < h; ih++)
					{
						if (finished[tl_x + iw, tl_y + ih])
							SAMLog.Error("WMBG:Partition", $"finished[{tl_x}+{iw}, {tl_y}+{ih}] ({w}|{h})");
					}
				}
#endif

				for (int iw = 0; iw < w; iw++)
				{
					for (int ih = 0; ih < h; ih++)
					{
						finished[tl_x + iw, tl_y + ih] = true;
					}
				}

				var c = ColorMath.Blend(FlatColors.Background, FlatColors.BackgroundGreen, partitionvalue / (GRADIENT_RESOLUTION*1f));
				var r = new FRectangle((tl_x+bounds.Left) * GDConstants.TILE_WIDTH, (tl_y+bounds.Top) * GDConstants.TILE_WIDTH, w * GDConstants.TILE_WIDTH, h * GDConstants.TILE_WIDTH);

				partitions.Add(Tuple.Create(c, r));

				if (tl_x + w < bounds.Width) pointQueue.Enqueue(Tuple.Create(tl_x + w, tl_y));
				if (tl_y + h < bounds.Height) pointQueue.Enqueue(Tuple.Create(tl_x, tl_y + h));
			}
		}

		private void CreateTiles(List<LevelNode> nodes)
		{
			foreach (LevelNode node in nodes)
			{
				int pow = (int) (node.LevelData.CompletionCount * 1.5f);

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
			{
				if (GridLineAlpha < 0.5f)
				{
					DrawColoredPartitions(sbatch);
				}
				else
				{
					DrawColoredNormal(sbatch);
				}
			}
			else
			{
				DrawSimple(sbatch);
			}
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

			if (GridLineAlpha >= 1)
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

		private void DrawColoredNormal(IBatchRenderer sbatch)
		{
			sbatch.FillRectangle(VAdapter.VirtualTotalBoundingBox, FlatColors.Background);

			int offX = (int) (Owner.MapOffsetX / TILE_WIDTH);
			int offY = (int) (Owner.MapOffsetY / TILE_WIDTH);

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
					if (_tileValues.TryGetValue(100000 * rx + ry, out perc)) color = ColorMath.Blend(FlatColors.Background, FlatColors.BackgroundGreen, (((int)(perc* GRADIENT_RESOLUTION)*1f)/ GRADIENT_RESOLUTION) * BackgroundPercentageOverride);

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

		private void DrawColoredPartitions(IBatchRenderer sbatch)
		{
			sbatch.FillRectangle(VAdapter.VirtualTotalBoundingBox.RelativeTo(Owner.MapOffset), FlatColors.Background);

			if (FloatMath.IsEpsilonOne(BackgroundPercentageOverride))
			{
				foreach (var pt in partitions)
				{
					sbatch.DrawStretched(Textures.TexPixel, pt.Item2, pt.Item1);
				}
			}
			else
			{
				foreach (var pt in partitions)
				{
					var color = ColorMath.Blend(FlatColors.Background, pt.Item1, BackgroundPercentageOverride);

					sbatch.DrawStretched(Textures.TexPixel, pt.Item2, color);
				}
			}
			
			if (GridLineAlpha > 0)
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
						var rx = x - offX;
						var ry = y - offY;

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

#if DEBUG
			if (DebugSettings.Get("DebugBackground"))
			{
				foreach (var pt in partitions)
				{
					sbatch.FillRectangle(pt.Item2.AsDeflated(GDConstants.TILE_WIDTH/6, GDConstants.TILE_WIDTH/6), Color.Blue * 0.1f);
					sbatch.DrawRectangle(pt.Item2.AsDeflated(GDConstants.TILE_WIDTH/6, GDConstants.TILE_WIDTH/6), Color.Blue, Owner.PixelWidth);
				}
			}

#endif

		}
	}
}