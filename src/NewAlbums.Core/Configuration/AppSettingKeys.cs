﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Configuration
{
    public static class AppSettingKeys
    {
        public static class App
        {
            public const string Name = "App:Name";

            public const string FrontEndRootUrl = "App:FrontEndRootUrl";

            /// <summary>
            /// Stored as an app secret in development
            /// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows
            /// </summary>
            public const string AdminEmailAddress = "App:AdminEmailAddress";

            /// <summary>
            /// Stored as an app secret in development
            /// </summary>
            public const string AdminFullName = "App:AdminFullName";

            public const string SystemEmailAddress = "App:SystemEmailAddress";
        }

        public static class Mailgun
        {
            public const string ApiKey = "Mailgun:ApiKey";
            public const string ApiUrl = "Mailgun:ApiUrl";
            public const string Domain = "Mailgun:Domain";
        }
    }
}
