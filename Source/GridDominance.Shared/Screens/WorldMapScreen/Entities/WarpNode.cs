using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens.Entities.Operation;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class WarpNode : GameEntity, IWorldNode
	{
		public const float DIAMETER = 3f * GDConstants.TILE_WIDTH;
		public const float INNER_DIAMETER = 2f * GDConstants.TILE_WIDTH;

		private GDWorldMapScreen GDOwner => (GDWorldMapScreen) Owner;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.DarkOrange;
		IEnumerable<IWorldNode> IWorldNode.NextLinkedNodes => Enumerable.Empty<IWorldNode>();
		public Guid ConnectionID => Blueprint.TargetWorld;
		public float ColorOverdraw = 0f;

		public readonly WarpNodeBlueprint Blueprint;
		public readonly GraphBlueprint Target;

		private GameEntityMouseArea clickAreaThis;

		private PointCPUParticleEmitter emitter;

		public bool NodeEnabled { get; set; } = false;

		public WarpNode(GDWorldMapScreen scrn, WarpNodeBlueprint bp) : base(scrn, GDConstants.ORDER_MAP_NODE)
		{
			Position = new FPoint(bp.X, bp.Y);
			DrawingBoundingBox = new FSize(DIAMETER, DIAMETER);
			Blueprint = bp;
			Target = Levels.WORLDS[bp.TargetWorld];
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(FRectangle.CreateByCenter(0, 0, DIAMETER, DIAMETER), OnClick);

			var cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				TextureIndex = 14,
				SpawnRate = 32,
				ParticleLifetimeMin = 2.3f,
				ParticleLifetimeMax = 2.3f,
				ParticleVelocityMin = 12f,
				ParticleVelocityMax = 24f,
				ParticleSizeInitial = 0,
				ParticleSizeFinal = 48,
				ParticleAlphaInitial =0f,
				ParticleAlphaFinal = 1f,
				Color = FlatColors.SunFlower,
			}.Build(Textures.TexParticle);

			emitter = new PointCPUParticleEmitter(Owner, Position, cfg, GDConstants.ORDER_MAP_NODEPARTICLES);

			manager.AddEntity(emitter);
		}

		private void OnClick(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			if (GDOwner.ZoomState != BistateProgress.Normal) return;

#if DEBUG
			if (!NodeEnabled && DebugSettings.Get("UnlockNode"))
			{
				NodeEnabled = true;
			}
#endif
			if (!NodeEnabled)
			{
				MainGame.Inst.GDSound.PlayEffectError();

				AddEntityOperation(new ScreenShakeAndCenterOperation2(this, GDOwner));

				Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_GLOB_WORLDLOCK), 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);

				return;
			}

			if (GDConstants.USE_IAB)
			{
				if (MainGame.Inst.Profile.PurchasedWorlds.Contains(Blueprint.TargetWorld))
				{
					Owner.AddAgent(new LeaveTransitionWorldMapAgent(GDOwner, this, Target));
					MainGame.Inst.GDSound.PlayEffectZoomOut();
				}
				else
				{
					Owner.AddAgent(new LeaveTransitionOverworldAgent(GDOwner));
					MainGame.Inst.GDSound.PlayEffectZoomOut();
				}
			}
			else
			{
				Owner.AddAgent(new LeaveTransitionWorldMapAgent(GDOwner, this, Target));
				MainGame.Inst.GDSound.PlayEffectZoomOut();
			}
			
		}

		public void CreatePipe(IWorldNode target, PipeBlueprint.Orientation orientation)
		{
			throw new NotSupportedException();
		}

		public override void OnRemove()
		{

		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			emitter.IsEnabled = MainGame.Inst.Profile.EffectsEnabled && NodeEnabled;
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, FRectangle.CreateByCenter(Position, DIAMETER, DIAMETER), clickAreaThis.IsMouseDown() ? FlatColors.WetAsphalt : FlatColors.Asbestos);
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, FRectangle.CreateByCenter(Position, DIAMETER, DIAMETER), 2, FlatColors.MidnightBlue);

			SimpleRenderHelper.DrawRoundedRect(sbatch, FRectangle.CreateByCenter(Position, INNER_DIAMETER, INNER_DIAMETER), NodeEnabled ? ColorMath.Blend(Color.Black, FlatColors.Background, ColorOverdraw) : FlatColors.Asbestos);

			FontRenderHelper.DrawTextCenteredWithBackground(
				sbatch, 
				Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, 
				L10N.T(Levels.WORLD_NAMES[Target.ID]), 
				FlatColors.TextHUD, 
				Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH),
				FlatColors.BackgroundHUD2 * 0.5f);

			if (!NodeEnabled)
			{
				var scale = 1 + FloatMath.Sin(Lifetime) * 0.05f;
				sbatch.DrawCentered(Textures.TexIconLock, Position, INNER_DIAMETER * scale, INNER_DIAMETER * scale, Color.Black);
			}
		}

		public bool HasAnyCompleted()
		{
			return false;
		}
	}
}
