using MonoSAMFramework.Portable.GameMath.FloatClasses;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public abstract class VectorPathSegment
	{
		public abstract float Length { get; }
		public abstract FRectangle Boundings { get; }


		public abstract FPoint Get(float len);
		public abstract VectorPathSegment AsScaled(float scale);


		public FPoint GetStart()
		{
			return Get(0);
		}


		public FPoint GetEnd()
		{
			return Get(Length);
		}
	}
}
