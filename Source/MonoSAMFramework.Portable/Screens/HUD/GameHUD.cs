using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		public readonly GameScreen Screen;
		protected readonly HUDRootContainer root;
		public readonly SpriteFont DefaultFont;

		private HUDKeyboard _keyboard = null;
		private HUDToast _toast = null;

		protected GameHUD(GameScreen scrn, SpriteFont font)
		{
			Screen = scrn;
			DefaultFont = font;

			root = new HUDRootContainer { HUD = this };
			root.Initialize();
		}

		public float Left => -Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetX;
		public float Top => -Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetY;

		public float Right => Screen.VAdapterHUD.VirtualTotalWidth - Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetX;
		public float Bottom => Screen.VAdapterHUD.VirtualTotalHeight - Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetY;

		public float Width => Right - Left;
		public float Height => Bottom - Top;

		public float CenterX => Left + Width/2;
		public float CenterY => Top + Height / 2;

		public float PixelWidth => Width * 1f / Screen.VAdapterHUD.RealTotalWidth;

		private HUDElement _focusedElement = null;
		public HUDElement FocusedElement
		{
			get { return _focusedElement; }
			set
			{
				if (_focusedElement == value) return;

				_focusedElement?.FocusLoose();
				value?.FocusGain();

				_focusedElement = value;
			}
		}

		public IEnumerable<HUDElement> Enumerate() => Enumerable.Repeat(root, 1).Concat(root.EnumerateElements());

		public void Update(SAMTime gameTime, InputState istate)
		{
			root.Update(gameTime, istate);

			OnUpdate(gameTime, istate);

			if (FocusedElement != null && !FocusedElement.Alive) FocusedElement = null;
		}

		protected virtual void OnUpdate(SAMTime gameTime, InputState istate)
		{
			// override me
		}

		public void Draw(IBatchRenderer sbatch)
		{
			if (root.IsVisible)
			{
				root.DrawBackground(sbatch);
				root.DrawForeground(sbatch);
			}
		}

		public void AddModal(HUDElement e, bool closeOnOOB, float dim = 0f)
		{
			root.AddElement(new HUDModalDialog(root.ChildrenMaxDepth + 1, e, dim, closeOnOOB));
		}

		public void AddElement(HUDElement e)
		{
			root.AddElement(e);
		}

		public void AddElements(IEnumerable<HUDElement> es)
		{
			root.AddElements(es);
		}

		public void RecalculateAllElementPositions()
		{
			root.InvalidatePosition();
		}

		public int FlatCount()
		{
			return root.ChildrenCount;
		}

		public int DeepCount()
		{
			return root.DeepInclusiveCount;
		}

		public void HideKeyboard()
		{
			_keyboard?.Remove();
			_keyboard = null;
		}

		public HUDKeyboard ShowKeyboard(IKeyboardListener reciever)
		{
			if (_keyboard != null) HideKeyboard();

			_keyboard = new HUDKeyboard(reciever);

			AddModal(_keyboard, true);

			return _keyboard;
		}

		public HUDToast ShowToast(string text, int size, Color background, Color foreground, float lifetime)
		{
			if (_toast != null && _toast.Alive)
			{
				_toast.Remove();
			}

			_toast = new HUDToast(lifetime);

			_toast.Text = text;
			_toast.Alignment = HUDAlignment.BOTTOMCENTER;
			_toast.RelativePosition = new FPoint(0, size);
			_toast.FontSize = size;
			_toast.Font = DefaultFont;
			_toast.TextColor = foreground;
			_toast.ColorBackground = background;
			_toast.TextPadding = new FSize(size / 5f, size / 5f);
			_toast.BackgroundType = HUDBackgroundType.SimpleBlur;
			_toast.MaxWidth = Width * 0.8f;
			_toast.BackgoundCornerSize = size/4f;
			_toast.WordWrap = HUDWordWrap.WrapByWordTrusted;

			AddElement(_toast);
			return _toast;
		}

		public void Validate()
		{
			root.ValidateRecursive();
		}
	}
}
