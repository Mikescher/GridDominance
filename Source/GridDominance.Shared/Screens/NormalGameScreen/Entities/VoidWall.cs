using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class VoidWall : GameEntity
	{
		public const float WIDTH      = VoidWallBlueprint.DEFAULT_WIDTH;
		public const float MARGIN_TEX = 8f;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		private readonly float _rotation;
		private readonly float _length;
		private readonly int _renderPartCount;
		private readonly FRectangle[] _rectsUnrotated;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;
		
		public VoidWall(GDGameScreen scrn, VoidWallBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			var pos   = new Vector2(blueprint.X, blueprint.Y);

			_rotation = FloatMath.DegreesToRadians * blueprint.Rotation;
			_length = blueprint.Length;
			_renderPartCount = FloatMath.Round(_length / GDConstants.TILE_WIDTH);
			_rectsUnrotated = CreateRenderRects(pos, _length, _renderPartCount);

			Position = pos;

			DrawingBoundingBox = new Vector2(_length, 0).Rotate(_rotation).ToAbsSize().AtLeast(WIDTH, WIDTH);

			this.GDOwner().GDBackground.RegisterBlockedLine(pos - Vector2.UnitX.RotateWithLength(_rotation, _length/2f), pos + Vector2.UnitX.RotateWithLength(_rotation, _length / 2f));
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Position), 0, BodyType.Static);
			PhysicsFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(_length), ConvertUnits.ToSimUnits(WIDTH), 1, Vector2.Zero, PhysicsBody, this);

			PhysicsBody.Rotation = _rotation;
		}

		private FRectangle[] CreateRenderRects(Vector2 pos, float len, int pc)
		{
			if (pc <= 2)
			{
				return new[]
				{
					FRectangle.CreateByCenter(pos, MARGIN_TEX + len + MARGIN_TEX, MARGIN_TEX + WIDTH + MARGIN_TEX)
				};
			}
			else
			{
				var partlen = len / pc;

				var dir = Vector2.UnitX.RotateWithLength(_rotation, 1);

				var result = new FRectangle[pc];
				for (int i = 0; i < pc; i++)
				{
					var di = i - (pc - 1) / 2f;


					if (i == 0)
					{
						var off = dir * (di * partlen - MARGIN_TEX/2f);
						result[i] = FRectangle.CreateByCenter(pos + off, MARGIN_TEX + partlen, MARGIN_TEX + WIDTH + MARGIN_TEX);
					}
					else if (i + 1 < pc)
					{
						var off = dir * di * partlen;
						result[i] = FRectangle.CreateByCenter(pos + off, partlen, MARGIN_TEX + WIDTH + MARGIN_TEX);
					}
					else
					{
						var off = dir * (di * partlen + MARGIN_TEX / 2f);
						result[i] = FRectangle.CreateByCenter(pos + off, partlen + MARGIN_TEX, MARGIN_TEX + WIDTH + MARGIN_TEX);
					}
				}
				return result;
			}
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{

			if (_renderPartCount <= 1)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_BG_L1, _rectsUnrotated[0], Color.White, _rotation);
			}
			else if (_renderPartCount == 2)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_BG_L2, _rectsUnrotated[0], Color.White, _rotation);
			}
			else
			{
				for (int i = 0; i < _renderPartCount; i++)
				{
					if (i == 0)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_End, _rectsUnrotated[i], Color.White, _rotation + FloatMath.RAD_POS_180);
					}
					else if (i + 1 < _renderPartCount)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_Middle, _rectsUnrotated[i], Color.White, _rotation);
					}
					else
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_End, _rectsUnrotated[i], Color.White, _rotation);
					}
				}
			}

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				foreach (var r in _rectsUnrotated)
				{
					sbatch.DrawRectangleRot(r, Color.Cyan, _rotation);
				}
			}
#endif
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			if (_renderPartCount <= 1)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_FG_L1, _rectsUnrotated[0], Color.White, _rotation);
			}
			else if (_renderPartCount == 2)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_FG_L2, _rectsUnrotated[0], Color.White, _rotation);
			}
			else
			{
				for (int i = 0; i < _renderPartCount; i++)
				{
					if (i == 0)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_End, _rectsUnrotated[i], Color.White, _rotation + FloatMath.RAD_POS_180);
					}
					else if (i + 1 < _renderPartCount)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_Middle, _rectsUnrotated[i], Color.White, _rotation);
					}
					else
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_End, _rectsUnrotated[i], Color.White, _rotation);
					}
				}
			}
		}
	}
}
