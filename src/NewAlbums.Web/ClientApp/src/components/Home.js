import React, { Component } from 'react';

export class Home extends Component {
    displayName = Home.name

    render() {
        const redirectUrl = encodeURIComponent(`${process.env.REACT_APP_API_URL}/spotifycallback/`);
        const scopes = encodeURIComponent('user-follow-read');
        const spotifyAuthoriseUrl = `https://accounts.spotify.com/authorize?client_id=${process.env.REACT_APP_SPOTIFY_CLIENT_ID}&response_type=token&redirect_uri=${redirectUrl}&scope=${scopes}&show_dialog=true`;

        return (
            <div>
                <h1>New Albums via Email</h1>
                <p>Receive an email alert when an artist you follow releases a new album.</p>

                <h2>Spotify already sends me weekly new release emails, why do I need this?</h2>
                <ul>
                    <li>Receive emails within one day of a new album release</li>
                    <li>We only email you about albums, which are more important than single releases</li>
                    <li>Spotify's emails are biased towards featured artists, and reportedly ignore some of your followed artists</li>
                </ul>

                <p>
                    Click the button below to let us access your Spotify account. The only data we access is your followed artists.
                    We will never sell or do anything unethical with your data.
                    In fact, you can check our source code to verify this on <a href='https://github.com/Ellfish/newalbumsviaemail'>Github</a>.
                </p>

                <a href={spotifyAuthoriseUrl} className='btn btn-primary btn-lg'>Get Started</a>
            </div>
        );
    }
}
