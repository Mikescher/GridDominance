using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class ParticleBanner
	{
		public const float LINE_SPACING              = 0.30f;
		public const float CHAR_SPACING              = 0.25f;
		public const float ANIMATION_LINEBREAK_PAUSE = 0.45f;

		private readonly GameScreen _screen;
		private readonly TextureRegion2D[] _tex;
		private readonly int _order;

		private readonly List<IParticleEmitter> _childEmitter = new List<IParticleEmitter>();

		public FRectangle TargetRect;
		public string Text;
		public bool UseCPUParticles = false;
		public float AnimationTime = 0f;

		public ParticleBanner(GameScreen screen, TextureRegion2D[] particleArray, int order)
		{
			_screen = screen;
			_tex = particleArray;
			_order = order;
		}

		public void CreateEntities(ParticleEmitterConfig.ParticleEmitterConfigBuilder cfg)
		{
			RemoveEntities();

			var lines = Regex.Split(Text, @"\r?\n");

			float usHeight = lines.Length + (lines.Length - 1) * LINE_SPACING;
			float[] usWidths = lines.Select(LineLength).ToArray();
			float usWidth = usWidths.Max();

			var rect = TargetRect.SetRatioUnderfitKeepCenter(usWidth / usHeight);
			var scale = rect.Height / usHeight;

			var animLen = (AnimationTime - ANIMATION_LINEBREAK_PAUSE * (lines.Length - 1)) / (lines.Sum(l => l.Length)/2f);

			animLen = FloatMath.Max(0, animLen);

			int idx = 0;
			for (int y = 0; y < lines.Length; y++)
			{
				var py = rect.Top + y * (scale + scale * LINE_SPACING);

				var px = rect.Left + scale * (usWidth - usWidths[y]) / 2;

				for (int x = 0; x < lines[y].Length; x++)
				{
					var chr = PathPresets.LETTERS[lines[y][x]];

					var emitterConfig = cfg.Build(_tex, scale, chr.Length);

					AddLetter(chr, emitterConfig, px + scale * chr.Boundings.Width / 2f, py + scale/2f, scale, animLen * (idx/2f) + y * ANIMATION_LINEBREAK_PAUSE, animLen);
					idx++;

					px += scale * (chr.Boundings.Width + CHAR_SPACING);
				}
			}
		}

		public void RemoveEntities()
		{
			if (_childEmitter.Any())
			{
				foreach (var pemi in _childEmitter) pemi.Alive = false;
				_childEmitter.Clear();
			}
		}

		private float LineLength(string str)
		{
			float x = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (i > 0) x += CHAR_SPACING;
				x += PathPresets.LETTERS[str[i]].Boundings.Width;
			}
			return x;
		}

		private void AddLetter(VectorPath chr, ParticleEmitterConfig cfg, float px, float py, float scale, float animOffset, float animLen)
		{
			if (UseCPUParticles)
			{
				var emt = new AnimatedPathCPUParticleEmitter(_screen, new Vector2(px, py), chr.AsScaled(scale), cfg, animOffset, animLen, _order);
				_childEmitter.Add(emt);
				_screen.Entities.AddEntity(emt);
			}
			else
			{
				var emt = new AnimatedPathGPUParticleEmitter(_screen, new Vector2(px, py), chr.AsScaled(scale), cfg, animOffset, animLen, _order);
				_childEmitter.Add(emt);
				_screen.Entities.AddEntity(emt);
			}
		}
	}
}
