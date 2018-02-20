using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
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

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class BackgroundText : GameEntity
	{
		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		public readonly BackgroundTextBlueprint Blueprint;
		private readonly float _rotation;
		
		private readonly FRotatedRectangle _bounds;

		private readonly string _text;
		private readonly float _scale;

		public float SuperRotation = 0f;
		public Color Color = FlatColors.Foreground;
		
		public BackgroundText(GDGameScreen scrn, BackgroundTextBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_BACKGROUNDTEXT)
		{
			var pos = new FPoint(blueprint.X, blueprint.Y);

			Blueprint = blueprint;

			_rotation = FloatMath.ToRadians(blueprint.Rotation);
			_bounds = new FRotatedRectangle(pos, blueprint.Width, blueprint.Height, _rotation);

			Position = pos;

			DrawingBoundingBox = _bounds.OuterSize;

			_text = FontRenderHelper.MakeTextSafe(Textures.LevelBackgroundFont, L10N.T(blueprint.L10NText), '_');
			_scale = FontRenderHelper.GetFontScale(Textures.LevelBackgroundFont, _text, _bounds.Size);
			
			if ((Blueprint.Config & BackgroundTextBlueprint.CONFIG_SHAKE) == BackgroundTextBlueprint.CONFIG_SHAKE)
			{
				AddOperation(new ShakeTextOperation());
			}

			if ((Blueprint.Config & BackgroundTextBlueprint.CONFIG_ONLYD1) == BackgroundTextBlueprint.CONFIG_ONLYD1)
			{
				if (scrn.Difficulty != FractionDifficulty.DIFF_0) Alive = false;
			}

			if ((Blueprint.Config & BackgroundTextBlueprint.CONFIG_ONLY_UNCLEARED) == BackgroundTextBlueprint.CONFIG_ONLY_UNCLEARED)
			{
				if (MainGame.Inst.Profile.GetLevelData(scrn.Blueprint).HasCompletedOrBetter(scrn.Difficulty)) Alive = false;
			}

			if ((Blueprint.Config & BackgroundTextBlueprint.CONFIG_REDFLASH) == BackgroundTextBlueprint.CONFIG_REDFLASH)
			{
				AddOperation(new RedFlashTextOperation());
			}

			if (scrn.IsPreview) Alive = false;
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
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (!Alive) return;
			
			FontRenderHelper.DrawTextCenteredWithScale(sbatch, Textures.LevelBackgroundFont, _scale, _text, Color, Position, _rotation + SuperRotation);
		}
	}
}
