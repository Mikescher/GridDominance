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

namespace GridDominance.Shared.Screens.EndGameScreen.Background
{
	class EndGameBackground : GameBackground
	{
		private sealed class GridElem
		{
			public FPoint Center;
			public float StartTime;
			public float Speed;
			public float Rotation;
		}

		private const int TILE_COUNT_X = GDConstants.DEFAULT_GRID_WIDTH;
		private const int TILE_COUNT_Y = GDConstants.DEFAULT_GRID_HEIGHT;
		private const int TILE_WIDTH   = GDConstants.TILE_WIDTH;

		private const float ANIMATION_DURATION = 5f;

		private float _animationTime = 0f;

		private List<GridElem> _elements = new List<GridElem>();

		public EndGameBackground(GameScreen scrn) : base(scrn)
		{
			int extensionX = 4;
			int extensionY = 4;

			int w = TILE_COUNT_X + 2 * extensionX;
			int h = TILE_COUNT_Y + 2 * extensionY;



			for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
			{
				float x = -extensionX;

				while (x < TILE_COUNT_X+ extensionX)
				{
					float width = FloatMath.GetRangedRandom(1, 5);
					float time = FloatMath.GetRangedRandom(1, 4);
					float offe = FloatMath.GetRangedRandom(0, 1);
					float offs = ANIMATION_DURATION - (time + offe);

					_elements.Add(new GridElem
					{
						Center = new FPoint((x+width/2)*TILE_WIDTH, y*TILE_WIDTH),
						StartTime = offs,
						Speed = (width * TILE_WIDTH) / time,
						Rotation = FloatMath.RAD_POS_000
					});
					x += width;
				}
			}

			for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
			{
				float y = -extensionY;

				while (y < TILE_COUNT_Y + extensionY)
				{
					float height = FloatMath.GetRangedRandom(1, 4);
					float time   = FloatMath.GetRangedRandom(1, 4);
					float offs   = ANIMATION_DURATION - time;

					_elements.Add(new GridElem
					{
						Center = new FPoint(x * TILE_WIDTH, (y + height / 2) * TILE_WIDTH),
						StartTime = offs,
						Speed = (height * TILE_WIDTH) / time,
						Rotation = FloatMath.RAD_POS_090
					});
					y += height;
				}
			}
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			_animationTime += gameTime.ElapsedSeconds;
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			int offX = TILE_WIDTH * (int)(Owner.MapOffsetX / TILE_WIDTH);
			int offY = TILE_WIDTH * (int)(Owner.MapOffsetY / TILE_WIDTH);

			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);

			var r = new FRectangle(-extensionX * TILE_WIDTH - offX, -extensionY * TILE_WIDTH - offY, (countX + 2 * extensionX) * TILE_WIDTH, (countY + 2 * extensionY) * TILE_WIDTH);

			sbatch.DrawStretched(Textures.TexPixel, r, FlatColors.Background);


			if (_animationTime < ANIMATION_DURATION+0.5f)
			{
				foreach (var e in _elements)
				{
					if (_animationTime < e.StartTime) continue;

					sbatch.DrawCentered(Textures.TexPixel, e.Center, (_animationTime - e.StartTime) * e.Speed, 2, FlatColors.BackgroundHighlight, e.Rotation);
				}
			}
			else
			{
				for (int x = -extensionX; x < countX + extensionX; x++)
				{
					sbatch.DrawCentered(Textures.TexPixel, new FPoint(x * TILE_WIDTH, r.CenterY), 2, r.Height, FlatColors.BackgroundHighlight);
				}

				for (int y = -extensionY; y < countY + extensionY; y++)
				{
					sbatch.DrawCentered(Textures.TexPixel, new FPoint(r.CenterX, y * TILE_WIDTH), r.Width, 2, FlatColors.BackgroundHighlight);
				}
			}


		}
	}
}
