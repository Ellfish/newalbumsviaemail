import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import './Home.scss';

export class Home extends Component {
    displayName = Home.name

    render() {
        const redirectUrl = encodeURIComponent(`${process.env.REACT_APP_API_URL}/spotify-callback/`);
        const scopes = encodeURIComponent('user-follow-read user-top-read user-read-email');
        //Set this to true to always show the Spotify auth dialog, even if already authenticated.
        const showDialog = true;
        const spotifyAuthoriseUrl = `https://accounts.spotify.com/authorize?client_id=${process.env.REACT_APP_SPOTIFY_CLIENT_ID}&response_type=token&redirect_uri=${redirectUrl}&scope=${scopes}&show_dialog=${showDialog}`;

        return (
            <div>
                <div className='row'>
                    <div className='col-xs-12 col-md-10'>
                        <h1>Subscribe to new album releases on&nbsp;Spotify</h1>

                        <h2>Who is this for?</h2>

                        <p>You need a Spotify account, premium or free, which follows at least one artist. You may know that Spotify already sends weekly new release emails, however:</p>

                        <ul>
                            <li>We only email you about albums, which (we think) are more important to know about than single releases, and therefore worthy of a spot in your crowded inbox.</li>
                            <li>Spotify's emails are biased towards featured/promoted artists, and miss some artists.</li>
                            <li>You may not care about new releases from all your followed artists, especially if you share an account with someone. This app lets you choose which artists to be notified about.</li>
                        </ul>

                        <h2>How does it work?</h2>

                        <p>It only takes a minute and you don't even need to register an account.</p>
                    </div>
                </div>

                <div className='how-does-it-work-container'>
                    <div className='how-does-it-work-step'>
                        <div className='image-container'>
                            <img src='/images/how-does-it-work-step-1.jpg' alt='Allow Spotify access' width='223' height='223' />
                            <div className='center-hole' />
                        </div>

                        <h4>1.<br />Allow this app limited access to your Spotify data</h4>
                    </div>
                    <div className='how-does-it-work-step'>
                        <div className='image-container'>
                            <img src='/images/how-does-it-work-step-2.jpg' alt='Choose artists' width='223' height='223' />
                            <div className='center-hole' />
                        </div>

                        <h4>2.<br />Select which artists to subscribe to</h4>
                    </div>
                    <div className='how-does-it-work-step'>
                        <div className='image-container'>
                            <img src='/images/how-does-it-work-step-3.jpg' alt='Enter email address' width='223' height='223' />
                            <div className='center-hole' />
                        </div>

                        <h4>3.<br />Enter your email address and click Subscribe</h4>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-xs-12 col-md-10'>

                        <p>
                            Our app checks for new album releases daily. If it finds one from an artist that you've subscribed to, we'll send you an email notification. It's easy to unsubscribe at any time by clicking the link in the email.
                        </p>

                        <h2>Ready?</h2>

                        <p>
                            Click the button below to let us access (some of) your Spotify data. The only data we access is your followed artists (to let you choose which to subscribe to),
                            your top artists (to pre-select your top 50 artists), and your email (for anti-spam verification).
                            We will never sell or do anything unethical with your data.
                            You can even see our source code on <a href='https://github.com/Ellfish/newalbumsviaemail'>Github</a>.
                        </p>
                    </div>
                </div>

                <Button bsStyle='primary' bsSize='lg' className='m-t-50' href={spotifyAuthoriseUrl}>Get Started</Button>
            </div>
        );
    }
}
