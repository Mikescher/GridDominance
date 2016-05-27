using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public class HUDLabel : HUDContainer
	{
		public override int Depth { get; }

		protected readonly HUDRawText internalText;

		public HUDAlignment TextAlignment
		{
			get { return internalText.Alignment; }
			set { internalText.Alignment = value; }
		}

		public string Text
		{
			get { return internalText.Text; }
			set { internalText.Text = value; }
		}

		public Color TextColor
		{
			get { return internalText.TextColor; }
			set { internalText.TextColor = value; }
		}

		public Color BackgroundColor = Color.Transparent;

		public SpriteFont Font
		{
			get { return internalText.Font; }
			set { internalText.Font = value; }
		}

		public float FontSize
		{
			get { return internalText.FontSize; }
			set { internalText.FontSize = value; }
		}

		public FSize InnerLabelSize => internalText.Size;

		public HUDLabel(int depth = 0)
		{
			Depth = depth;

			internalText = new HUDRawText{ Alignment = HUDAlignment.BOTTOMLEFT };
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (BackgroundColor != Color.Transparent)
			{
				SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, BackgroundColor);
			}
		}

		public override void OnInitialize()
		{
			AddElement(internalText);
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			// NOP
		}
	}
}
