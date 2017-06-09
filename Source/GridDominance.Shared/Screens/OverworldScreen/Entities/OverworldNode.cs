using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using System;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public abstract class OverworldNode : GameEntity
	{
		public const float SIZE = 3 * GDConstants.TILE_WIDTH;
		public const float INNERSIZE = 2 * GDConstants.TILE_WIDTH;

		public const float COLLAPSE_TIME = 5f;

		public Vector2 NodePos;
		public override Vector2 Position => NodePos;
		public override FSize DrawingBoundingBox { get; } = new FSize(SIZE, SIZE);
		public override Color DebugIdentColor { get; } = Color.Blue;
		public readonly Guid ContentID;

		protected readonly int _l10ndescription;
		protected readonly GameEntityMouseArea clickArea;

		public float AlphaOverride = 1f;

		public float FlickerTime = 0f;

		public OverworldNode(GDOverworldScreen scrn, Vector2 pos, int l10ntext, Guid id) : base(scrn, GDConstants.ORDER_WORLD_NODE)
		{
			_l10ndescription = l10ntext;
			NodePos = pos;
			ContentID = id;

			clickArea = AddClickMouseArea(FRectangle.CreateByCenter(Vector2.Zero, new FSize(SIZE, SIZE)), OnClick);
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
			FlickerTime += gameTime.ElapsedSeconds;
		}

		public void CancelClick()
		{
			clickArea.CancelClick();
		}

		protected abstract void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate);
	}
}
