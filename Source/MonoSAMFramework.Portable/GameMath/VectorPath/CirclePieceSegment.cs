using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.FloatClasses;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class CirclePieceSegment : VectorPathSegment
	{
		private readonly FPoint center;
		private readonly float radius;

		private readonly float angleStart;
		private readonly float angleEnd;

		private readonly Vector2 directionZero;

		public override float Length { get; }
		public override FRectangle Boundings { get; }

		public CirclePieceSegment(FPoint center, float radius, float aStart = 0, float aEnd = FloatMath.TAU)
		{
			this.center = center;
			this.radius = radius;

			angleStart = aStart;
			angleEnd = aEnd;

			Length = (FloatMath.PI * radius * radius) * (angleEnd - angleStart)/FloatMath.TAU;
			directionZero = new Vector2(0, radius);

			Boundings = new FRectangle(center.X - radius, center.Y - radius, 2*radius, 2*radius); // TODO This is wrong - use correct calculations: http://mathoverflow.net/questions/93659
		}

		public override FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			return directionZero.Rotate(angleStart + (angleEnd - angleStart) * (len / Length)).ToFPoint();
		}
	}
}
