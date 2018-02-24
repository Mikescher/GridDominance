using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.LevelEditorScreen.HUD.Elements
{
	class LevelEditorTestConfirmPanel : HUDContainer
	{
		public override int Depth => 999;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		private LevelBlueprint _blueprint;
		private SCCMLevelData _data;

		public LevelEditorTestConfirmPanel(LevelBlueprint level, SCCMLevelData dat) : base()
		{
			_data = dat;
			_blueprint = level;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, FlatColors.BackgroundHUD2);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(0, 0);
			Size = new FSize(14.5f * GDConstants.TILE_WIDTH, 4.0f * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTER;

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_EASY, HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartTest(FractionDifficulty.KI_EASY))
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Size = new FSize(3 * GDConstants.TILE_WIDTH, 3 * GDConstants.TILE_WIDTH),
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_NORMAL, HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartTest(FractionDifficulty.KI_NORMAL))
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Size = new FSize(3 * GDConstants.TILE_WIDTH, 3 * GDConstants.TILE_WIDTH),
				RelativePosition = new FPoint(4.0f * GDConstants.TILE_WIDTH, 0),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_HARD, HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartTest(FractionDifficulty.KI_HARD))
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Size = new FSize(3 * GDConstants.TILE_WIDTH, 3 * GDConstants.TILE_WIDTH),
				RelativePosition = new FPoint(7.5f * GDConstants.TILE_WIDTH, 0),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_IMPOSSIBLE, HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartTest(FractionDifficulty.KI_IMPOSSIBLE))
			{
				Alignment = HUDAlignment.CENTERLEFT,
				Size = new FSize(3 * GDConstants.TILE_WIDTH, 3 * GDConstants.TILE_WIDTH),
				RelativePosition = new FPoint(11.0f * GDConstants.TILE_WIDTH, 0),
			});
		}

		private void StartTest(FractionDifficulty d)
		{
			MainGame.Inst.SetEditorTestLevel(_blueprint, d, _data);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
