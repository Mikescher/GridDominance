namespace MonoSAMFramework.Portable.BatchRenderer
{
	public interface ITranslateBatchRenderer : IBatchRenderer
	{
		float VirtualOffsetX { get; set; }
		float VirtualOffsetY { get; set; }
	}
}
