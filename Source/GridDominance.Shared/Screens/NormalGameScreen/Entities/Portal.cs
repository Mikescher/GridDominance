using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Particles;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class Portal : GameEntity
	{
		public const           float   WIDTH  = PortalBlueprint.DEFAULT_WIDTH;
		public static readonly Color[] COLORS = { Color.Magenta, FlatColors.SunFlower, FlatColors.Alizarin, FlatColors.Wisteria, FlatColors.Emerald, FlatColors.PeterRiver, FlatColors.Nephritis, FlatColors.Turquoise, FlatColors.Silver };

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		public  readonly float Normal;
		public  readonly float Length;
		public  readonly Vector2 VecNormal;
		public  readonly Vector2 VecDirection;
		public readonly Color Color;
		private readonly int _group;
		private readonly bool _side;

		private readonly FRectangle _rectFull;
		private readonly FRectangle _rectHorizon;
		private readonly FRectangle _rectGradient;
		private readonly FRectangle _rectDropTop;
		private readonly FRectangle _rectDropMid;
		private readonly FRectangle _rectDropBot;

		public List<Portal> Links;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;
		
		public Portal(GDGameScreen scrn, PortalBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_PORTAL)
		{
			var pos   = new FPoint(blueprint.X, blueprint.Y);

			Normal       = FloatMath.DegreesToRadians * blueprint.Normal;
			VecNormal    = Vector2.UnitX.Rotate(Normal);
			Length       = blueprint.Length;
			VecDirection = VecNormal.RotateWithLength(FloatMath.RAD_POS_090, blueprint.Length / 2f);
			Color        = COLORS[blueprint.Group];
			_group       = blueprint.Group;
			_side        = blueprint.Side;

			Position = pos;

			DrawingBoundingBox = new Vector2(Length, 0).Rotate(Normal + FloatMath.RAD_POS_090).ToAbsFSize().AtLeast(WIDTH, WIDTH);

			this.GDOwner().GDBackground.RegisterBlockedLine(pos - Vector2.UnitX.RotateWithLength(Normal + FloatMath.RAD_POS_090, Length/2f), pos + Vector2.UnitX.RotateWithLength(Normal + FloatMath.RAD_POS_090, Length / 2f));

			_rectFull     = FRectangle.CreateByCenter(pos, Length, WIDTH);
			_rectHorizon  = FRectangle.CreateByCenter(pos + VecNormal.WithLength(WIDTH / 4f), Length, WIDTH / 2f);
			_rectGradient = FRectangle.CreateByCenter(pos + VecNormal.WithLength(WIDTH / 2f), Length, WIDTH);
			_rectDropTop  = FRectangle.CreateByCenter(pos - VecDirection.WithLength(Length / 2f), 24, 16);
			_rectDropMid  = FRectangle.CreateByCenter(pos, 24, Length - 16);
			_rectDropBot  = FRectangle.CreateByCenter(pos + VecDirection.WithLength(Length / 2f), 24, 16);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), 0, BodyType.Static);
			PhysicsFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(Length), ConvertUnits.ToSimUnits(WIDTH), 1, Vector2.Zero, PhysicsBody, this);

			PhysicsBody.Rotation = Normal + FloatMath.RAD_POS_090;

			Owner.Entities.AddEntity(new PortalParticleEmitter(this));
		}

		public void OnAfterLevelLoad(List<Portal> portals)
		{
			Links = portals.Where(p => p._group == _group && p._side != _side).ToList();
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
			sbatch.DrawStretched(Textures.TexPortalDropEnd1, _rectDropTop, Color.White, Normal);
			sbatch.DrawStretched(Textures.TexPortalDropMid, _rectDropMid, Color.White,  Normal);
			sbatch.DrawStretched(Textures.TexPortalDropEnd2, _rectDropBot, Color.White, Normal);

			sbatch.DrawStretched(Textures.TexGradient, _rectGradient, Color,           Normal - FloatMath.RAD_POS_090);
			sbatch.DrawStretched(Textures.TexPixel,    _rectFull, FlatColors.Clouds,   Normal - FloatMath.RAD_POS_090);
			sbatch.DrawStretched(Textures.TexPixel,    _rectHorizon, Color,            Normal - FloatMath.RAD_POS_090);

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				sbatch.FillCircle(Position, 4, 16, Color.Turquoise);
				sbatch.DrawLine(Position, Position + Vector2.UnitX.RotateWithLength(Normal, 32), Color.Turquoise, 2);
			}
#endif
		}
	}
}
