using System;
using System.Collections.Generic;
using System.Linq;
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

		private static readonly string[] CONFIGURATIONS =
		{
			"01L100L313S121O120J432J1",
			"01T200Z114I212T120T331T4",
			"02J200L214I211O131J130J3",
			"01Z200L213J223J421L420S1",
			"02T400T210J421I133O130J1",
			"03T301L400O123T120J431S2",
			"01J100J313S120L132J130O1",
			"02T400J310T422Z232J130O1",
			"01J300I213O112J420L432J1",
			"01J300I212L123Z122J420L4",
			"02S100J313T110S132T430Z2",
			"02O100O113T120T232S230L3",
			"03Z101S100L222S232L330O1",
			"01J100J314I222L420L130O1",
			"02S201Z100J423J231J130J3",
			"01T200I214I213I211Z130T4",
			"02L200O113L223L420S230L3",
			"02T100T213L410L332L330O1",
			"01Z200L212Z223L420S131Z2",
			"02T100T214I210L331L130L3",
			"02T400T210I223L422S121L2",
			"02S200S210Z123J222Z130L3",
			"02S200O112J423J220L231L3",
			"03Z101O100I223Z122J420L4",
		};

		public List<TetroAnimationElement> _animation = new List<TetroAnimationElement>();

		public TetrisFillOperation(float duration) : base(duration)
		{

		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			_animation.Clear();

			var config = CONFIGURATIONS[FloatMath.Random.Next(CONFIGURATIONS.Length)];

			int off = 0;
			var c = Enumerable.Range(0, COLORS.Length).Shuffle().ToList();
			foreach (var i in Enumerable.Range(0, 6).Shuffle())
			{
				var x  = config[4*i+0] - '0';
				var y  = config[4*i+1] - '0';
				var id = config[4*i+2] + "" + config[4*i+3];

				_animation.Add(new TetroAnimationElement
				{
					Offset = new DPoint(x, y),
					Scale = 0,
					Tetromino = TetroPieces.PIECES[id],
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
