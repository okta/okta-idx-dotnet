using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IInteractionHandleResponse : IResource
    {
        string InteractionHandle { get;  }
    }
}