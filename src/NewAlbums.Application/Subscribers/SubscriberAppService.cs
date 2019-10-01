using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
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

        public SubscriberAppService(
            ICrudServicesAsync crudServices,
            EmailManager emailManager
            )
        {
            _crudServices = crudServices;
            _emailManager = emailManager;
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
                string emailText = GetNotificationAlbumText(input);
                string emailHtml = GetNotificationAlbumHtml(input);

                foreach (var subscription in input.Subscriptions)
                {
                    var subscriberDto = await _crudServices.ReadSingleAsync<SubscriberDto>(s => s.Id == subscription.SubscriberId);

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
                Logger.LogError(ex, "Album.SpotifyId: " + input.Album.SpotifyId);
                return new NotifySubscribersOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        //TODO: unsubscribe link
        private string GetNotificationAlbumText(NotifySubscribersInput input)
        {
            return $"Good news, {input.Artist.Name} just released a new album called \"{input.Album.Name}\".\r\n\r\n"
                + $"Click the following link to listen: {Album.GetSpotifyUrl(input.Album.SpotifyId)}";
        }

        //TODO: unsubscribe link
        private string GetNotificationAlbumHtml(NotifySubscribersInput input)
        {
            return "";
        }
    }
}
