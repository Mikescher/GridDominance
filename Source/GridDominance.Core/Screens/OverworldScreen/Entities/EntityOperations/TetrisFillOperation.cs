using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Tetromino;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class TetrisFillOperation : FixTimeOperation<OverworldNode_SCCM>
	{
		public override string Name => "Tetris::Fill";

		public class TetroAnimationElement
		{
			public TetroPiece Tetromino;
			public DPoint Offset;
			public float Scale;
			public int Delay;
			public Color Color;
			public MutableTuple<FRectangle, Color> Target1 = null;
			public MutableTuple<FRectangle, Color> Target2 = null;
			public MutableTuple<FRectangle, Color> Target3 = null;
			public MutableTuple<FRectangle, Color> Target4 = null;
		}

		private static readonly Color[] COLORS =
		{
			FlatColors.GreenSea,
			FlatColors.Emerald,
			FlatColors.PeterRiver,
			FlatColors.Wisteria,
			FlatColors.SunFlower,
			FlatColors.Orange,
			FlatColors.Pumpkin,
			FlatColors.Alizarin,
		};

		public List<TetroAnimationElement> _animation = new List<TetroAnimationElement>();

		public TetrisFillOperation(float duration) : base(duration)
		{

		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			_animation.Clear();

			var config = TetroConfigurations.CONFIGURATIONS[FloatMath.Random.Next(TetroConfigurations.CONFIGURATIONS.Length)];

			int off = 0;
			var c = Enumerable.Range(0, COLORS.Length).Shuffle().ToList();
			foreach (var i in Enumerable.Range(0, 6).Shuffle())
			{
				var tx = (int)(((config >> (i * 10 + 0)) & 0x1F) % 5);
				var ty = (int)(((config >> (i * 10 + 0)) & 0x1F) / 5);
				var ti = (int)(((config >> (i * 10 + 5)) & 0x1F));

				_animation.Add(new TetroAnimationElement
				{
					Offset = new DPoint(tx, ty),
					Scale = 0,
					Tetromino = TetroPieces.ALPHABET[ti],
					Delay = off,
					Color = COLORS[c[i]],
				});

				off++;
			}

			node.Blocks.Clear();
			node.Blocks.Add(new MutableTuple<FRectangle, Color>(new FRectangle(
				node.NonTranslatedBounds.X,
				node.NonTranslatedBounds.Bottom - (node.NonTranslatedBounds.Width / 5f),
				node.NonTranslatedBounds.Width / 5f,
				node.NonTranslatedBounds.Width / 5f
				), Color.White));
		}
		
		protected override void OnProgress(OverworldNode_SCCM node, float progress, SAMTime gameTime, InputState istate)
		{
			var innerBounds = node.NonTranslatedBounds;
			var tetroRectSize = innerBounds.Width / 5f;

			foreach (var anim in _animation)
			{
				var t0 = anim.Delay / 7f;
				var t1 = (anim.Delay+2) / 7f;

				var newscale = anim.Scale;

				if (progress < t0)
				{
					newscale = 0;
				}
				else if (progress > t1)
				{
					newscale = 1f;
				}
				else
				{
					newscale = (progress - t0) / (t1 - t0);
				}

				newscale *= 1.0005f; // prevent rounding error overlap bullshit

				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (newscale > 0 && anim.Scale != newscale)
				{
					anim.Scale = FloatMath.FunctionEaseInOutQuart(newscale);

					if (anim.Target1 == null)
					{
						anim.Target1 = new MutableTuple<FRectangle, Color>(new FRectangle(), anim.Color);
						anim.Target2 = new MutableTuple<FRectangle, Color>(new FRectangle(), anim.Color);
						anim.Target3 = new MutableTuple<FRectangle, Color>(new FRectangle(), anim.Color);
						anim.Target4 = new MutableTuple<FRectangle, Color>(new FRectangle(), anim.Color);

						node.Blocks.Add(anim.Target1);
						node.Blocks.Add(anim.Target2);
						node.Blocks.Add(anim.Target3);
						node.Blocks.Add(anim.Target4);
					}
					
					var tl1 = (anim.Tetromino.Center.WithOrigin(anim.Offset) + ((anim.Tetromino.P1 - anim.Tetromino.Center) * anim.Scale));
					anim.Target1.Item1 = new FRectangle(
						innerBounds.X + tetroRectSize * tl1.X,
						innerBounds.Y + tetroRectSize * tl1.Y,
						tetroRectSize * anim.Scale,
						tetroRectSize * anim.Scale);

					var tl2 = (anim.Tetromino.Center.WithOrigin(anim.Offset) + ((anim.Tetromino.P2 - anim.Tetromino.Center) * anim.Scale));
					anim.Target2.Item1 = new FRectangle(
						innerBounds.X + tetroRectSize * tl2.X,
						innerBounds.Y + tetroRectSize * tl2.Y,
						tetroRectSize * anim.Scale,
						tetroRectSize * anim.Scale);

					var tl3 = (anim.Tetromino.Center.WithOrigin(anim.Offset) + ((anim.Tetromino.P3 - anim.Tetromino.Center) * anim.Scale));
					anim.Target3.Item1 = new FRectangle(
						innerBounds.X + tetroRectSize * tl3.X,
						innerBounds.Y + tetroRectSize * tl3.Y,
						tetroRectSize * anim.Scale,
						tetroRectSize * anim.Scale);

					var tl4 = (anim.Tetromino.Center.WithOrigin(anim.Offset) + ((anim.Tetromino.P4 - anim.Tetromino.Center) * anim.Scale));
					anim.Target4.Item1 = new FRectangle(
						innerBounds.X + tetroRectSize * tl4.X,
						innerBounds.Y + tetroRectSize * tl4.Y,
						tetroRectSize * anim.Scale,
						tetroRectSize * anim.Scale);
				}
			}
		}

		protected override void OnEnd(OverworldNode_SCCM node)
		{
			//
		}

		protected override void OnAbort(OverworldNode_SCCM node)
		{
			OnEnd(node);
		}
	}
}
