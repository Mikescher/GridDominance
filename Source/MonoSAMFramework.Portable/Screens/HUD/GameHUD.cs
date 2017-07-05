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
using System;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		public const int MAX_TOAST_COUNT = 4;

		public readonly GameScreen Screen;
		protected readonly HUDRootContainer root;
		public readonly SpriteFont DefaultFont;

		private HUDKeyboard _keyboard = null;
		private List<HUDToast> _toasts = new List<HUDToast>();

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

			UpdateToasts(gameTime);
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

		public void AddModal(HUDElement e, bool closeOnOutOfBoundsOrBackKey, float dim = 0f, float dimTime = 0f)
		{
			root.AddElement(new HUDModalDialog(root.ChildrenMaxDepth + 1, e, dim, dimTime, closeOnOutOfBoundsOrBackKey));
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

		private void UpdateToasts(SAMTime gameTime)
		{
			for (int i = _toasts.Count - 1; i >= 0; i--)
			{
				if (!_toasts[i].Alive) _toasts.RemoveAt(i);
			}

			float px = HUDToast.PAD_BOTTOM;
			foreach (var xtoast in _toasts)
			{
				xtoast.PositionY.Set(px);
				px += xtoast.Height + HUDToast.PAD_VERT;
			}
		}

		public HUDToast ShowToast(string text, int size, Color background, Color foreground, float lifetime)
		{
			while (_toasts.Count >= MAX_TOAST_COUNT)
			{
				_toasts[0].Alive = false;
				_toasts.RemoveAt(0);
			}

			float px = HUDToast.PAD_BOTTOM;
			foreach (var xtoast in _toasts)
			{
				xtoast.PositionY.SetForce(px);
				px += xtoast.Height + HUDToast.PAD_VERT;
			}

			var toast = new HUDToast(lifetime, px);

			toast.Text = text;
			toast.Alignment = HUDAlignment.BOTTOMCENTER;
			toast.RelativePosition = new FPoint(0, px);
			toast.FontSize = size;
			toast.Font = DefaultFont;
			toast.TextColor = foreground;
			toast.ColorBackground = background;
			toast.TextPadding = new FSize(size / 5f, size / 5f);
			toast.BackgroundType = HUDBackgroundType.SimpleBlur;
			toast.MaxWidth = Width * 0.8f;
			toast.BackgoundCornerSize = size/4f;
			toast.WordWrap = HUDWordWrap.WrapByWordTrusted;

			AddElement(toast);
			_toasts.Add(toast);
			return toast;
		}

		public void Validate()
		{
			root.ValidateRecursive();
		}

		public void CopyToast(GameHUD phud)
		{
			bool postreset = _toasts.Any();

			foreach (var t in phud._toasts)
			{
				var copy = HUDToast.Copy(t);
				AddElement(copy);
				_toasts.Add(copy);
			}

			if (postreset)
			{
				float px = HUDToast.PAD_BOTTOM;
				foreach (var xtoast in _toasts)
				{
					xtoast.PositionY.SetForce(px);
					px += xtoast.Height + HUDToast.PAD_VERT;
				}
			}
		}
	}
}
