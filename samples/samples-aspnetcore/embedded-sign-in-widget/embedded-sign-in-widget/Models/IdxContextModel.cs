using Okta.Idx.Sdk;

namespace embedded_sign_in_widget.Models
{
    public class IdxContextModel
    {
        public static implicit operator IdxContext(IdxContextModel model)
        {
            return new IdxContext(model.CodeVerifier, model.CodeChallenge, model.CodeChallengeMethod, model.InteractionHandle, model.State);
        }
        
        public static implicit operator IdxContextModel(IdxContext context)
        {
            return new IdxContextModel(context);
        }

        public IdxContextModel() { }

        public IdxContextModel(IdxContext context)
        {
            this.CodeVerifier = context.CodeVerifier;
            this.CodeChallenge = context.CodeChallenge;
            this.CodeChallengeMethod = context.CodeChallengeMethod;
            this.InteractionHandle = context.InteractionHandle;
            this.State = context.State;
        }

        public string? CodeVerifier { get; set; }

        public string? CodeChallenge { get; set; }

        public string? CodeChallengeMethod { get; set; }

        public string? InteractionHandle { get; set; }

        public string? State { get; set; }
    }
}
