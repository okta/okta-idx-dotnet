using A18NAClient.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace A18NAClient
{
    public interface IA18nClient
    {
        Task<A18nProfile> CreateProfileAsync(CancellationToken cancellationToken = default);
        Task DeleteAllProfileEmailsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task DeleteAllProfileSmsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task DeleteEmailAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        Task DeleteLatestEmailAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task DeleteLatestSmsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task DeleteSmsAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        Task<ProfileList> GetActiveProfilesAsync(CancellationToken cancellationToken = default);
        Task<EmailMessageList> GetAllProfileEmailsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<SmsMessageList> GetAllProfileSmsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<EmailMessage> GetEmailMessageAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        Task<string> GetLastMessagePlainContentAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<string> GetLastSmsPlainContentAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<EmailMessage> GetLatestEmailMessageAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<SmsMessage> GetLatestSmsAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<string> GetPlainMessageContentAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        Task<string> GetPlainSmsContentAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        Task<A18nProfile> GetProfileAsync(string profileId = default, CancellationToken cancellationToken = default);
        Task<SmsMessage> GetSmsMessageAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default);
        void SetDefaultProfileId(string profileId);
    }
}
