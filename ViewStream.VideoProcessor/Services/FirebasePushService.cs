using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Shared.Options;

namespace ViewStream.VideoProcessor.Services
{
    public class FirebasePushService : IPushNotificationService
    {
        private readonly FirebaseApp _app;

        public FirebasePushService(IOptions<FirebaseOptions> options)
        {
            var credentialPath = options.Value.CredentialPath;
            if (!File.Exists(credentialPath))
                throw new FileNotFoundException($"Firebase credentials not found at {credentialPath}");

            _app = FirebaseApp.Create(new FirebaseAdmin.AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
        }

        public async Task SendAsync(string deviceToken, string title, string body, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                Token = deviceToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };
            await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
        }
    }

}
