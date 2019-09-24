import React, { Component } from 'react';
import parseHash from '../utils/parseHash';
import parseSearch from '../utils/parseSearch';
import FollowedArtistsList from './FollowedArtistsList';

export class SpotifyCallback extends Component {

    constructor(props) {
        super(props);

        const hashValues = parseHash();
        const searchValues = parseSearch();

        this.state = {
            error: searchValues.error,
            accessToken: hashValues.access_token,
            followedArtists: [],
            loading: true
        };
    }

    render() {
        if (this.state.error) {
            return (
                <div>
                    <h1>Error</h1>
                    <p>You must authorise access to your Spotify account so we can read your followed artists. <a href='/'>Go back and try again.</a></p>
                </div>
            );
        }

        if (this.state.accessToken) {
            return (
                <div>
                    <h1>Select artists to subscribe to</h1>
                    <FollowedArtistsList accessToken={this.state.accessToken} />
                </div>
            );
        }
    }
}
