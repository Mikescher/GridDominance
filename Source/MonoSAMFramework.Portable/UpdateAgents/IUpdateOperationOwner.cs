namespace MonoSAMFramework.Portable.UpdateAgents
{
	public interface IUpdateOperationOwner
	{
		void AddOperation(IUpdateOperation op);
	}
}
