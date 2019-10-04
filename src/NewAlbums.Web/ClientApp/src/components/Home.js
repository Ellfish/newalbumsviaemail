import React, { Component } from 'react';
import { Button } from 'react-bootstrap';

export class Home extends Component {
    displayName = Home.name

    render() {
        const redirectUrl = encodeURIComponent(`${process.env.REACT_APP_API_URL}/spotify-callback/`);
        const scopes = encodeURIComponent('user-follow-read');
        //Set this to true to always show the Spotify auth dialog, even if already authenticated.
        const showDialog = true;
        const spotifyAuthoriseUrl = `https://accounts.spotify.com/authorize?client_id=${process.env.REACT_APP_SPOTIFY_CLIENT_ID}&response_type=token&redirect_uri=${redirectUrl}&scope=${scopes}&show_dialog=${showDialog}`;

        return (
            <div>
                <h1>Subscribe to new album releases from your followed Spotify artists</h1>

                <h2>How does it work?</h2>

                <ol>
                    <li>Allow this app access to your followed artists on Spotify</li>
                    <li>Choose which followed artists to subscribe to</li>
                    <li>Enter your email address</li>
                </ol>

                <p>
                    Our app checks for new album releases daily. If it finds one from an artist that you've subscribed to, we'll send you an email notification. It's easy to unsubscribe at any time.
                </p>

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

                <Button bsStyle='primary' bsSize='lg' className='m-t-50' href={spotifyAuthoriseUrl}>Get Started</Button>
            </div>
        );
    }
}
