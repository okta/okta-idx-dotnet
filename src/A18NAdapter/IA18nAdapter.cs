using A18NAdapter.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace A18NAdapter
{
    public interface IA18nAdapter
    {
        Task<A18nProfile> CreateProfileAsync(CancellationToken cancellationToken = default);
        Task DeleteAllProfileEmailsAsync(string profileId, CancellationToken cancellationToken = default);
        Task DeleteAllProfileSmsAsync(string profileId, CancellationToken cancellationToken = default);
        Task DeleteEmailAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        Task DeleteLatestEmailAsync(string profileId, CancellationToken cancellationToken = default);
        Task DeleteLatestSmsAsync(string profileId, CancellationToken cancellationToken = default);
        Task DeleteSmsAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        Task<ProfileList> GetActiveProfilesAsync(CancellationToken cancellationToken = default);
        Task<EmailMessageList> GetAllProfileEmailsAsync(string profileId, CancellationToken cancellationToken = default);
        Task<SmsMessageList> GetAllProfileSmsAsync(string profileId, CancellationToken cancellationToken = default);
        Task<EmailMessage> GetEmailMessageAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        Task<string> GetLastMessagePlainContentAsync(string profileId, CancellationToken cancellationToken = default);
        Task<string> GetLastSmsPlainContentAsync(string profileId, CancellationToken cancellationToken = default);
        Task<EmailMessage> GetLatestEmailMessageAsync(string profileId, CancellationToken cancellationToken = default);
        Task<SmsMessage> GetLatestSmsAsync(string profileId, CancellationToken cancellationToken = default);
        Task<string> GetPlainMessageContentAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        Task<string> GetPlainSmsContentAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        Task<A18nProfile> GetProfileAsync(string profileId, CancellationToken cancellationToken = default);
        Task<SmsMessage> GetSmsMessageAsync(string profileId, string messageId, CancellationToken cancellationToken = default);
        void SetDefaultProfileId(string profileId);
    }
}
