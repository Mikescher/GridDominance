using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	class HUDKeyboardButton : HUDElement
	{
		public override int Depth { get; } = 0;

		public float CornerRadius = 0f;
		public float TextSize = 1f;
		public float TextSizeAlt = 1f;
		public float TextAltPadding = 1f;
		public string Text = "";
		public string TextAlt = "";

		public float IconPadding = 0f;
		public TextureRegion2D Icon = null;

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, HUDKeyboard.COLOR_KEY, CornerRadius);
			
			if (Text != "")
			{
				FontRenderHelper.DrawTextCentered(sbatch, HUD.DefaultFont, TextSize, Text, HUDKeyboard.COLOR_TEXT, bounds.VecCenter);
			}
			if (TextAlt != "") 
			{
				FontRenderHelper.DrawTextTopRight(sbatch, HUD.DefaultFont, TextSizeAlt, TextAlt, HUDKeyboard.COLOR_ALT, bounds.TopRight + new Vector2(-TextAltPadding, 0));
			}
			if (Icon != null)
			{
				var b = Icon.Size().Underfit(bounds.Size, IconPadding);
				sbatch.DrawCentered(Icon, bounds.Center, b.Width, b.Height, HUDKeyboard.COLOR_TEXT);
			}
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;

		public override void OnInitialize()
		{
			//
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
