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
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class RootNode : GameEntity, IWorldNode
	{
		public const float DIAMETER = 3f * GDConstants.TILE_WIDTH;
		public const float INNER_DIAMETER = 2f * GDConstants.TILE_WIDTH;

		private GDWorldMapScreen GDOwner => (GDWorldMapScreen) Owner;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;
		IEnumerable<IWorldNode> IWorldNode.NextLinkedNodes => NextLinkedNodes;
		public Guid ConnectionID => Blueprint.WorldID;

		public readonly RootNodeBlueprint Blueprint;

		private GameEntityMouseArea clickAreaThis;

		public readonly List<IWorldNode> NextLinkedNodes = new List<IWorldNode>(); // ordered by pipe priority
		public readonly List<LevelNodePipe> OutgoingPipes = new List<LevelNodePipe>();

		private PointCPUParticleEmitter emitter;

		public bool NodeEnabled { get { return true; } set {} }

		private readonly string _text = L10N.T(L10NImpl.STR_GLOB_OVERWORLD);

		public RootNode(GDWorldMapScreen scrn, RootNodeBlueprint bp) : base(scrn, GDConstants.ORDER_MAP_NODE)
		{
			Position = new Vector2(bp.X, bp.Y);
			DrawingBoundingBox = new FSize(DIAMETER, DIAMETER);
			Blueprint = bp;

			AddEntityOperation(new CyclicGameEntityOperation<RootNode>("LevelNode::OrbSpawn", LevelNode.ORB_SPAWN_TIME, false, SpawnOrb));
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(FRectangle.CreateByCenter(0, 0, DIAMETER, DIAMETER), OnClick);

			var cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// red fire
				TextureIndex = 12,
				SpawnRate = 20,
				ParticleLifetimeMin = 1.0f,
				ParticleLifetimeMax = 2.5f,
				ParticleVelocityMin = 24f,
				ParticleVelocityMax = 32f,
				ParticleSizeInitial = 64,
				ParticleSizeFinalMin = 32,
				ParticleSizeFinalMax = 48,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkOrange,
				ColorFinal = Color.DarkRed,
			}.Build(Textures.TexParticle);

			emitter = new PointCPUParticleEmitter(Owner, Position, cfg, GDConstants.ORDER_MAP_NODEPARTICLES);

			manager.AddEntity(emitter);
		}

		private void OnClick(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			if (GDOwner.ZoomState != BistateProgress.Normal) return;

			Owner.AddAgent(new LeaveTransitionOverworldAgent(GDOwner));
			MainGame.Inst.GDSound.PlayEffectZoomOut();
		}

		public void CreatePipe(IWorldNode target, PipeBlueprint.Orientation orientation)
		{
			NextLinkedNodes.Add(target);

			var p = new LevelNodePipe(Owner, this, target, orientation);
			OutgoingPipes.Add(p);
			Manager.AddEntity(p);
		}

		private void SpawnOrb(RootNode me, int cycle)
		{
			if (!NextLinkedNodes.Any()) return;
			if (!MainGame.Inst.Profile.EffectsEnabled) return;

			FractionDifficulty d = FractionDifficulty.NEUTRAL;
			switch (cycle % 4)
			{
				case 0: d = FractionDifficulty.DIFF_0; break;
				case 1: d = FractionDifficulty.DIFF_1; break;
				case 2: d = FractionDifficulty.DIFF_2; break;
				case 3: d = FractionDifficulty.DIFF_3; break;
			}

			foreach (var t in OutgoingPipes)
			{
				if (!((GameEntity)t.NodeSource).IsInViewport && !((GameEntity)t.NodeSink).IsInViewport) return;

				var orb = new ConnectionOrb(Owner, t, d);

				Manager.AddEntity(orb);
			}
		}

		public override void OnRemove()
		{

		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			emitter.IsEnabled = MainGame.Inst.Profile.EffectsEnabled;
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, FRectangle.CreateByCenter(Position, DIAMETER, DIAMETER), clickAreaThis.IsMouseDown() ? FlatColors.WetAsphalt : FlatColors.Asbestos);
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, FRectangle.CreateByCenter(Position, DIAMETER, DIAMETER), 2, FlatColors.MidnightBlue);

			SimpleRenderHelper.DrawRoundedRect(sbatch, FRectangle.CreateByCenter(Position, INNER_DIAMETER, INNER_DIAMETER), Color.Black);

			FontRenderHelper.DrawTextCenteredWithBackground(
				sbatch,
				Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH,
				_text,
				FlatColors.TextHUD,
				Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH),
				FlatColors.BackgroundHUD2 * 0.5f);
		}

		public bool HasAnyCompleted()
		{
			return true;
		}
	}
}
