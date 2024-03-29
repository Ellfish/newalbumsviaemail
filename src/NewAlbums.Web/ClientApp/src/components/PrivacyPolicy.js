import React, { Component } from 'react';

export class PrivacyPolicy extends Component {
    displayName = PrivacyPolicy.name;

    render() {
        
        return (
            <div>
                <h1>Privacy Policy</h1>

                <p>
                    We respect your privacy. This app is non-commercial and open source, and we plan to keep it that way. We will never sell or disclose your data to third parties.
                    Your data is securely stored in our database following security best-practices.
                </p>

                <h2>What data do we collect?</h2>

                <p>When you complete the subscribe to artists form, we only save the data needed for the app to function:</p>

                <ul>
                    <li>Your email address so we can send you emails</li>
                    <li>A list of the Spotify artists that you selected to subscribe to</li>
                    <li>The date that the subscription was created</li>
                </ul>

                <p>
                    In order to access the artists you follow on Spotify, you need to login to your Spotify account and allow our app access.
                    We only access your followed artists, top artists, and email address once &ndash; we do not access your Spotify data again, unless you complete the subscribe process again.
                    You may note that Spotify is also giving us access to your public playlists &ndash; this is by default, and we do not actually read this data.
                </p>

                <h2>How is this data used?</h2>

                <p>
                    We only use this data to check for new album releases from artists that you chose to subscribe to.
                    When we find a match, we email you about the new release. You can use the links at the bottom of the email to unsubscribe at any time.
                </p>

                <h2>Google Analytics</h2>

                <p>
                    We use Google Analytics tracking because we want to know how many people are using the app, and how they're using it.
                    Google Analytics only captures non-personally-identifiable information. If you don't want to be tracked, we recommend using the latest version of Firefox browser,
                    installing an ad-blocker such as uBlock Origin or Adblock Plus, or installing the <a href="https://tools.google.com/dlpage/gaoptout">Google Analytics Opt-out Browser Add-on.</a>
                </p>

                <h2>Cookies</h2>

                <p>
                    We use cookies from Google Analytics, see above. These should be the only cookies you get from this app.
                </p>

                <h2>Legal</h2>

                <p>By using this app you consent to this Privacy Policy. This policy was last updated on 26th October 2019.</p>
            </div>
        );
    }
}
