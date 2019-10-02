using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
using NewAlbums.Configuration;
using NewAlbums.Emails;
using NewAlbums.Emails.Dto;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Utils;

namespace NewAlbums.Subscribers
{
    public class SubscriberAppService : BaseAppService, ISubscriberAppService
    {
        private readonly ICrudServicesAsync _crudServices;
        private readonly EmailManager _emailManager;
        private readonly IConfiguration _configuration;

        public SubscriberAppService(
            ICrudServicesAsync crudServices,
            EmailManager emailManager,
            IConfiguration configuration
            )
        {
            _crudServices = crudServices;
            _emailManager = emailManager;
            _configuration = configuration;
        }

        public async Task<GetOrCreateSubscriberOutput> GetOrCreate(GetOrCreateSubscriberInput input)
        {
            try
            {
                SubscriberDto subscriberDto = null;
                bool newSubscriber = false;
                string normalisedEmail = StringUtils.NormaliseEmailAddress(input.EmailAddress);

                subscriberDto = await _crudServices.ReadSingleAsync<SubscriberDto>(s => s.EmailAddress == normalisedEmail);
                if (subscriberDto == null)
                {
                    subscriberDto = await _crudServices.CreateAndSaveAsync(new SubscriberDto { EmailAddress = normalisedEmail });
                    if (!_crudServices.IsValid)
                    {
                        return new GetOrCreateSubscriberOutput
                        {
                            ErrorMessage = _crudServices.GetAllErrors()
                        };
                    }

                    newSubscriber = true;
                }

                return new GetOrCreateSubscriberOutput
                {
                    Subscriber = subscriberDto,
                    CreatedNewSubscriber = newSubscriber,
                    ErrorMessage = subscriberDto == null ? $"Error creating subscriber with email '{normalisedEmail}' in database" : null
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EmailAddress: " + input.EmailAddress);
                return new GetOrCreateSubscriberOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<NotifySubscribersOutput> NotifySubscribers(NotifySubscribersInput input)
        {
            if (input.Album == null)
                throw new ArgumentException("Album must not be null", "Album");

            if (input.Artist == null)
                throw new ArgumentException("Artist must not be null", "Artist");

            try
            {
                foreach (var subscription in input.Subscriptions)
                {
                    var subscriberDto = await _crudServices.ReadSingleAsync<SubscriberDto>(s => s.Id == subscription.SubscriberId);

                    Logger.LogInformation("Sending notification to {0} for SpotifyAlbumId: {1}", subscriberDto.EmailAddress, input.Album.SpotifyId);

                    string unsubscribeFromArtistUrl = GetUnsubscribeUrl(subscriberDto.UnsubscribeToken, subscription.ArtistId);
                    string unsubscribeFromAllUrl = GetUnsubscribeUrl(subscriberDto.UnsubscribeToken, null);

                    string emailText = GetNotificationAlbumText(input, unsubscribeFromArtistUrl, unsubscribeFromAllUrl);
                    string emailHtml = GetNotificationAlbumHtml(input, unsubscribeFromArtistUrl, unsubscribeFromAllUrl);

                    var email = new EmailMessage
                    {
                        BodyHtml = emailHtml,
                        BodyText = emailText,
                        Subject = $"New album from {input.Artist.Name}",
                        ToAddresses = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Address = subscriberDto.EmailAddress
                            }
                        }
                    };

                    await _emailManager.SendEmail(email);
                }
                
                return new NotifySubscribersOutput();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "SpotifyAlbumId: " + input.Album.SpotifyId);
                return new NotifySubscribersOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        private string GetNotificationAlbumText(NotifySubscribersInput input, string unsubscribeArtistUrl, string unsubscribeAllUrl)
        {
            string text = $"Good news, {input.Artist.Name} just released a new album called \"{input.Album.Name}\".\r\n\r\n"
                + $"Click the following link to listen: {Album.GetSpotifyUrl(input.Album.SpotifyId)}\r\n\r\n\r\n\r\n"
                + $"To unsubscribe from new albums for this artist, click the following link: {unsubscribeArtistUrl}\r\n"
                + $"To unsubscribe from all new albums from us, click the following link: {unsubscribeAllUrl}\r\n";

            return text;
        }

        //TODO: unsubscribe link
        //TODO: HTML email template
        private string GetNotificationAlbumHtml(NotifySubscribersInput input, string unsubscribeArtistUrl, string unsubscribeAllUrl)
        {
            return "";
        }

        private string GetUnsubscribeUrl(string unsubscribeToken, long? artistId)
        {
            string baseUrl = _configuration[AppSettingKeys.App.FrontEndRootUrl].TrimEnd('/');
            string unsubscribeUrl = $"{baseUrl}/unsubscribe?unsubscribeToken={unsubscribeToken}";

            if (artistId.HasValue)
            {
                unsubscribeUrl += $"&artistId={artistId.Value}";
            }

            return unsubscribeUrl;
        }
    }
}
