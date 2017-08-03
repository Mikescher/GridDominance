using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using System;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public abstract class OverworldNode : GameEntity
	{
		public const float SIZE = 3 * GDConstants.TILE_WIDTH;
		public const float INNERSIZE = 2 * GDConstants.TILE_WIDTH;

		public const float COLLAPSE_TIME = 5f;

#if DEBUG
		public FPoint TargetNodePos;
#endif

		public FPoint NodePos;
		public override FPoint Position => NodePos;
		public override FSize DrawingBoundingBox { get; } = new FSize(SIZE, SIZE);
		public override Color DebugIdentColor { get; } = Color.Blue;
		public readonly Guid ContentID;

		protected readonly int _l10ndescription;
		protected readonly GameEntityMouseArea clickArea;

		public float AlphaOverride = 1f;

		public float FlickerTime = 0f;

		public abstract bool IsNodeEnabled { get; }
		
		protected OverworldNode(GDOverworldScreen scrn, FPoint pos, int l10ntext, Guid id) : base(scrn, GDConstants.ORDER_WORLD_NODE)
		{
			_l10ndescription = l10ntext;
			NodePos = pos;
			ContentID = id;

			clickArea = AddClickMouseArea(FRectangle.CreateByCenter(FPoint.Zero, new FSize(SIZE, SIZE)), OnClick);
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			if(!MonoSAMGame.IsInitializationLag) FlickerTime += gameTime.ElapsedSeconds;
		}

		public void CancelClick()
		{
			clickArea.CancelClick();
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.FillRectangle(TargetNodePos.AsTranslated(-DrawingBoundingBox.Width * 0.5f, -DrawingBoundingBox.Height * 0.5f), DrawingBoundingBox, Color.Firebrick * 0.2f);
			sbatch.DrawRectangle(TargetNodePos.AsTranslated(-DrawingBoundingBox.Width * 0.5f, -DrawingBoundingBox.Height * 0.5f), DrawingBoundingBox, Color.Firebrick, 2);

		}

		protected abstract void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate);
	}
}
