using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Channels;

namespace MonoSAMFramework.Portable.Font
{
    public abstract class SAMFont
    {
        public abstract void Draw(SpriteBatch batch, string text, Vector2 position, Color color);
        public abstract void Draw(SpriteBatch batch, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
        public abstract Vector2 MeasureString(string text);
        public abstract float GetVCenterOffset(float boundsHeight);
        public abstract bool Contains(char c);
        public abstract float LineSpacing();
        public abstract float Spacing();
        public abstract float ExtraScaleFactor();
    }

    public class MonoGameSpriteFont : SAMFont
    {
        public readonly SpriteFont Font;

        public MonoGameSpriteFont(SpriteFont font)
        {
            Font = font;
        }

        public override void Draw(SpriteBatch batch, string text, Vector2 position, Color color)
        {
            batch.DrawString(Font, text, position, color);
        }

        public override void Draw(SpriteBatch batch, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            batch.DrawString(Font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public override Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }

        public override float GetVCenterOffset(float boundsHeight)
        {
            var glyph = Font.GetGlyphs()['M'];
            var top = glyph.Cropping.Y;
            var bot = Font.LineSpacing - glyph.BoundsInTexture.Height - top;

            var offset = (bot - top) / 2f;

            return offset;
        }

        public override bool Contains(char c)
        {
            if (Font.Characters == null) return false;

            return Font.Characters.Contains(c);
        }

        public override float LineSpacing()
        {
            return Font.LineSpacing;
        }

        public override float Spacing()
        {
            return Font.Spacing;
        }

        public override float ExtraScaleFactor()
        {
            return 1.0f;
        }
    }

    public class FontStashFont : SAMFont
    {
        public readonly SpriteFontBase Font;

        public FontStashFont(SpriteFontBase font)
        {
            Font = font;
        }

        public override bool Contains(char c)
        {
            return true;
        }

        public override void Draw(SpriteBatch batch, string text, Vector2 position, Color color)
        {
            Font.DrawText(batch, text, position, color);
        }

        public override void Draw(SpriteBatch batch, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Font.DrawText(batch: batch, 
                          text: text, 
                          position: position, 
                          color: color, 
                          scale: new Vector2(scale, scale),
                          origin: origin,
                          rotation: rotation,
                          layerDepth: layerDepth);
        }

        public override float GetVCenterOffset(float boundsHeight)
        {
            var glyph = Font.GetGlyphs("M", Vector2.Zero)[0];

            var top = glyph.Bounds.Top;
            var bot = glyph.Bounds.Bottom;

            var offsetReal = top + (bot - top) / 2f;
            var offsetCenter = boundsHeight / 2f;

            return offsetCenter - offsetReal;
        }

        public override float LineSpacing()
        {
            return Font.LineHeight;
        }

        public override Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }

        public override float Spacing()
        {
            return 0;
        }

        public override float ExtraScaleFactor()
        {
            return 0.9f;
        }
    }
}
