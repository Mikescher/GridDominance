using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
	class WorldPreviewPanel : HUDRoundedPanel
	{
		public const float WIDTH =  0.8f * GDConstants.VIEW_WIDTH;
		public const float HEIGHT = 0.8f * GDConstants.VIEW_HEIGHT;

		public const float INNER_WIDTH  = 0.7f * GDConstants.VIEW_WIDTH;
		public const float INNER_HEIGHT = 0.7f * GDConstants.VIEW_HEIGHT;

		public override int Depth => 0;

		private readonly LevelBlueprint[] _blueprints;

		public WorldPreviewPanel(LevelBlueprint[] bps)
		{
			_blueprints = bps;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			var prev = new GDGameScreen_Preview(MainGame.Inst, MainGame.Inst.Graphics, _blueprints, 0);

			AddElement(new HUDSubScreenProxyRenderer(prev)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = FPoint.Zero,
				Size = new FSize(INNER_WIDTH, INNER_HEIGHT),
			});
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
	}
}