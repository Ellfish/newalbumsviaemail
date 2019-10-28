using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GenericServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
using NewAlbums.Configuration;
using NewAlbums.Emails;
using NewAlbums.Emails.Dto;
using NewAlbums.Emails.Templates;
using NewAlbums.Emails.Templates.Dto;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Utils;

namespace NewAlbums.Subscribers
{
    public class SubscriberAppService : BaseAppService, ISubscriberAppService
    {
        private readonly ICrudServicesAsync _crudServices;
        private readonly EmailManager _emailManager;
        private readonly TemplateManager _templateManager;
        private readonly IConfiguration _configuration;

        public SubscriberAppService(
            ICrudServicesAsync crudServices,
            EmailManager emailManager,
            TemplateManager templateManager,
            IConfiguration configuration
            )
        {
            _crudServices = crudServices;
            _emailManager = emailManager;
            _templateManager = templateManager;
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
                    string emailVerifyCode = !input.EmailAddressVerified
                        ? Guid.NewGuid().ToString("N").Truncate(Subscriber.MAX_LENGTH_EMAIL_VERIFY_CODE)
                        : null;

                    subscriberDto = await _crudServices.CreateAndSaveAsync(new SubscriberDto 
                    { 
                        EmailAddress = normalisedEmail,
                        EmailAddressVerified = input.EmailAddressVerified,
                        EmailVerifyCode = emailVerifyCode
                    });

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

        public async Task<UpdateSubscriberOutput> Update(UpdateSubscriberInput input)
        {
            try
            {
                var subscriber = await _crudServices.ReadSingleAsync<Subscriber>(input.Id);
                if (subscriber == null)
                {
                    return new UpdateSubscriberOutput
                    {
                        ErrorMessage = $"Subscriber with Id {input.Id} not found."
                    };
                }

                subscriber.EmailAddressVerified = input.EmailAddressVerified;

                await _crudServices.UpdateAndSaveAsync(subscriber);

                return new UpdateSubscriberOutput
                {
                    ErrorMessage = !_crudServices.IsValid ? _crudServices.GetAllErrors() : null
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Id {input.Id}");
                return new UpdateSubscriberOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<CheckEmailVerificationOutput> CheckEmailVerification(CheckEmailVerificationInput input)
        {
            try
            {
                var subscriber = await _crudServices.ReadSingleAsync<Subscriber>(s =>
                    s.EmailAddress == StringUtils.NormaliseEmailAddress(input.EmailAddress)
                    && s.EmailVerifyCode == input.VerifyCode);

                if (subscriber.EmailAddressVerified)
                {
                    return new CheckEmailVerificationOutput
                    {
                        ErrorMessage = "You've already verified your email address."
                    };
                }

                if (subscriber == null)
                {
                    return new CheckEmailVerificationOutput
                    {
                        ErrorMessage = "Invalid email verification code."
                    };
                }

                return new CheckEmailVerificationOutput();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Code {input.VerifyCode}");
                return new CheckEmailVerificationOutput
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
                    if (subscriberDto == null)
                    {
                        Logger.LogError("Subscriber with Id {0} not found in database", subscription.SubscriberId);
                        continue;
                    }

                    if (!subscriberDto.EmailAddressVerified)
                    {
                        Logger.LogInformation("Skipping notification email for Subscriber with Id {0}, email address not verified", subscription.SubscriberId);
                        continue;
                    }

                    Logger.LogInformation("Sending notification to {0} for SpotifyAlbumId: {1}", subscriberDto.EmailAddress, input.Album.SpotifyId);

                    string unsubscribeFromArtistUrl = GetUnsubscribeUrl(subscriberDto.UnsubscribeToken, subscription.ArtistId);
                    string unsubscribeFromAllUrl = GetUnsubscribeUrl(subscriberDto.UnsubscribeToken, null);

                    string emailText = GetNotificationAlbumText(input, unsubscribeFromArtistUrl, unsubscribeFromAllUrl);
                    string emailHtml = await GetNotificationAlbumHtml(input, unsubscribeFromArtistUrl, unsubscribeFromAllUrl);

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
            string text = $"Good news, {input.Artist.Name} just released a new album: {input.Album.Name}.\r\n\r\n"
                + $"Click the following link to listen: {Album.GetSpotifyUrl(input.Album.SpotifyId)}\r\n\r\n\r\n\r\n"
                + $"To unsubscribe from new albums for this artist, click the following link: {unsubscribeArtistUrl}\r\n"
                + $"To unsubscribe from all new albums from us, click the following link: {unsubscribeAllUrl}\r\n"
                + _templateManager.GetEmailTextFooter();

            return text;
        }

        private async Task<string> GetNotificationAlbumHtml(NotifySubscribersInput input, string unsubscribeArtistUrl, string unsubscribeAllUrl)
        {
            string albumImageWidth = "280";

            var getTemplateInput = new GetTemplateInput
            {
                TemplateType = TemplateTypes.Alert,
                SimpleVariableValues = new Dictionary<string, string>
                {
                    { TemplateVariables.Heading,  $"New album from {input.Artist.Name}" }
                },
                BodyParagraphs = new List<BodyParagraph>
                {
                    new BodyParagraph { HtmlText = $"Good news, {input.Artist.Name} just released a new album: {HttpUtility.HtmlEncode(input.Album.Name)}." },
                    new BodyParagraph { HtmlText = $"<img alt=\"{HttpUtility.HtmlEncode(input.Album.Name)}\" src=\"{input.Album.Image.Url}\" width=\"{albumImageWidth}\" height=\"{albumImageWidth}\" />" },
                    new BodyParagraph { HtmlText = "Click Here to Listen", ButtonUrl = $"{Album.GetSpotifyUrl(input.Album.SpotifyId)}" }
                },
                FooterLines = new List<FooterLine>
                { 
                    new FooterLine { HtmlText = _templateManager.GetEmailLink(unsubscribeArtistUrl, "Unsubscribe", null, "12px") + " from new albums for this artist." },
                    new FooterLine { HtmlText = _templateManager.GetEmailLink(unsubscribeAllUrl, "Unsubscribe", null, "12px") + " from all new albums from us." }
                }
            };

            var template = await _templateManager.GetHtmlEmailTemplate(getTemplateInput);

            return template.ToString();
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
