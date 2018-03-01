using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	public class AchievementPopup : HUDElement
	{
		public const float FONTSIZE              = 96;
		public const float ROTATION_SPEED        = 0.2f;
		public const float PAD                   = 16;
		public const float CORNER_RADIUS         = 16;

		public const float TRANSLATION_SPEED     = 112;   // STEP 1 | 1.0s
		public const float COG_TRANSLATION_SPEED = 1.25f; // STEP 2 | 0.8s
		public const float BASE_LIFETIME         = 5.00f; // STEP 3 | 5.0s
		public const float FADEOUT_SPEED         = 1.00f; // STEP 4 | 1.0s

		public const int MIN_PARTICLE_COUNT = 64;
		public const int MAX_PARTICLE_COUNT = 384;

		public const float PARTICLE_SPAWN_DELAY_MIN = 0.002f;
		public const float PARTICLE_SPAWN_DELAY_MAX = 0.010f;

		public const float PARTICLE_LIFETIME_FADE = 0.50f;
		public const float PARTICLE_LIFETIME_MIN  = 1.25f;
		public const float PARTICLE_LIFETIME_MAX  = 2.50f; // max fadeout-speed
		
		public const float PARTICLE_SIDELEN_MIN = 2f;
		public const float PARTICLE_SIDELEN_MAX = 6f;

		public const float PARTICLE_ROTSPEED_MIN = -FloatMath.TAU/4f;
		public const float PARTICLE_ROTSPEED_MAX = +FloatMath.TAU/4f;

		public const float PARTICLE_SPEED_X_MAX = 24f;
		public const float PARTICLE_SPEED_Y_MIN = 64f;
		public const float PARTICLE_SPEED_Y_MAX = 128f;

		public static readonly Color[] PARTICLE_COLORS = 
		{
			Color.Red, Color.DeepPink, Color.Orange, Color.OrangeRed, 
			Color.Yellow, 
			Color.Green, Color.GreenYellow, Color.Aquamarine, Color.Teal, 
			Color.Blue, Color.Cyan, Color.Navy,
			Color.Purple, Color.Fuchsia, Color.Indigo,
		};

		public static readonly Vector2 PAD_VEC = new Vector2(PAD,PAD);
		public static readonly Vector2 RAD_INSET = new Vector2(6.6f, 6.6f); // 6.6 = sqrt(CORNER_RADIUS*CORNER_RADIUS + CORNER_RADIUS*CORNER_RADIUS) - CORNER_RADIUS

		private struct AchievementParticle
		{
			public FPoint Position;
			public Vector2 Vector;
			public FSize Size;
			public float Rotation;
			public float RotationSpeed;
			public Color Color;
			public float RemainingLifetime;
		}

		public Color Background = FlatColors.PeterRiver;
		public Color Foreground = FlatColors.MidnightBlue;

		private SpriteFont _font;
		private float _width;
		private float _height;
		private float _rotation = 0f;
		private string _text;
		private float _cogMovement = 0;
		private float _remainingLifetime = BASE_LIFETIME;
		private float _fadeOutAlpha = 1;

		public override int Depth => 99999999;

		private float _nextParticleSpawn = 0f;
		private readonly List<AchievementParticle> _particles = new List<AchievementParticle>();

		private AchievementPopup(SpriteFont font, string text)
		{
			_font = font;
			_height = PAD + FONTSIZE + PAD;
			_width = PAD + FontRenderHelper.MeasureStringCached(font, text, FONTSIZE).Width + PAD;
			_text = text;
		}

		public static void Show(string text) //TODO evtl sound
		{
			var hud = MainGame.Inst.GetCurrentScreen().HUD;
			if (hud == null) return;

			hud.AddElement(new AchievementPopup(Textures.HUDFontBold, text));
		}

		public override void OnInitialize()
		{
			Alignment = HUDAlignment.TOPLEFT;
			RelativePosition  = new FPoint(PAD, -_height - PAD - PAD);
			Size = new FSize(_width, _height);

			IsVisible = !HUD.Enumerate().OfType<AchievementPopup>().Any(p => p.IsVisible); // only show one achivement at once
		}

		public override void OnRemove()
		{
			//
		}
		
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			Remove();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_rotation += gameTime.ElapsedSeconds * ROTATION_SPEED * FloatMath.TAU;

			int mode = 0;

			if (!IsVisible)
			{
				// wait for other to dissapear
				IsVisible = !HUD.Enumerate().OfType<AchievementPopup>().Any(p => p.IsVisible);
				mode = 0;
			}
			else if (RelativePosition.Y < 0)
			{
				var y = RelativePosition.Y + gameTime.ElapsedSeconds * TRANSLATION_SPEED;
				if (y >= 0) y = 0;
				RelativePosition = new FPoint(RelativePosition.X, y);
				mode = 1;
			}
			else if (_cogMovement < 1)
			{
				_cogMovement += gameTime.ElapsedSeconds * COG_TRANSLATION_SPEED;
				if (_cogMovement >= 1) _cogMovement = 1;
				mode = 2;
			}
			else if (_remainingLifetime > 0)
			{
				_remainingLifetime -= gameTime.ElapsedSeconds;
				mode = 3;
			}
			else if (_fadeOutAlpha > 0)
			{
				_fadeOutAlpha -= gameTime.ElapsedSeconds * FADEOUT_SPEED;
				mode = 4;
			}
			else if (_particles.Any())
			{
				mode = 5;
			}
			else
			{
				Remove();
				mode = 6;
			}

			UpdateParticles(gameTime, mode>0 && mode < 4);
		}

		private void UpdateParticles(SAMTime gameTime, bool spawn)
		{
			if (spawn)
			{
				while (_particles.Count < MIN_PARTICLE_COUNT) SpawnNewParticle();
				_nextParticleSpawn -= gameTime.ElapsedSeconds;
				if (_nextParticleSpawn < 0 && _particles.Count < MAX_PARTICLE_COUNT) SpawnNewParticle();
			}

			for (int i = _particles.Count-1; i >= 0; i--)
			{
				var p =_particles[i];

				p.RemainingLifetime -= gameTime.ElapsedSeconds;
				p.Rotation += p.RotationSpeed * gameTime.ElapsedSeconds;
				p.Position += p.Vector * gameTime.ElapsedSeconds;

				if (p.RemainingLifetime < 0)
					_particles.RemoveAt(i);
				else
					_particles[i] = p;
			}
		}

		private void SpawnNewParticle()
		{
			_particles.Add(new AchievementParticle
			{
				Position = new FPoint(FloatMath.GetRangedRandom(Left, Right), HUD.Top),
				Vector = new Vector2(FloatMath.GetRangedRandom(-PARTICLE_SPEED_X_MAX, +PARTICLE_SPEED_X_MAX), FloatMath.GetRangedRandom(PARTICLE_SPEED_Y_MIN, PARTICLE_SPEED_Y_MAX)),
				Size = new FSize(FloatMath.GetRangedRandom(PARTICLE_SIDELEN_MIN, PARTICLE_SIDELEN_MAX), FloatMath.GetRangedRandom(PARTICLE_SIDELEN_MIN, PARTICLE_SIDELEN_MAX)),
				Rotation = FloatMath.GetRangedRandom(0, FloatMath.TAU),
				RemainingLifetime = FloatMath.GetRangedRandom(PARTICLE_LIFETIME_MIN, PARTICLE_LIFETIME_MAX),
				Color = PARTICLE_COLORS[FloatMath.GetRangedIntRandom(PARTICLE_COLORS.Length)],
				RotationSpeed = FloatMath.GetRangedRandom(PARTICLE_ROTSPEED_MIN, PARTICLE_ROTSPEED_MAX),
			});
			_nextParticleSpawn = FloatMath.GetRangedRandom(PARTICLE_SPAWN_DELAY_MIN, PARTICLE_SPAWN_DELAY_MAX);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bgrect = bounds.ToSubRectangleSouth(Height*2);

			if (_cogMovement>0)
			{
				sbatch.DrawCentered(Textures.CannonCogBig, bounds.BottomRight - RAD_INSET - PAD_VEC*(1.5f*(1-_cogMovement)), FONTSIZE, FONTSIZE, FlatColors.Clouds * _fadeOutAlpha*_fadeOutAlpha*_fadeOutAlpha, _rotation);
			}

			SimpleRenderHelper.DrawRoundedRect(sbatch, bgrect.AsDeflated(HUD.PixelWidth*1), Background * _fadeOutAlpha, CORNER_RADIUS);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, bgrect, Foreground * _fadeOutAlpha, 12, HUD.PixelWidth*2, CORNER_RADIUS);

			FontRenderHelper.DrawTextCentered(sbatch, _font, FONTSIZE, _text, Foreground * _fadeOutAlpha, bounds.Center);
			
			foreach (var particle in _particles)
			{
				if (particle.RemainingLifetime < PARTICLE_LIFETIME_FADE)
					sbatch.DrawCentered(Textures.TexPixel, particle.Position, particle.Size.Width, particle.Size.Height, particle.Color * (particle.RemainingLifetime / PARTICLE_LIFETIME_FADE), particle.Rotation);
				else
					sbatch.DrawCentered(Textures.TexPixel, particle.Position, particle.Size.Width, particle.Size.Height, particle.Color, particle.Rotation);
			}

		}
	}
}
