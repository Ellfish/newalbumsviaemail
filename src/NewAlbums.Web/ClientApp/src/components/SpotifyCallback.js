import React, { Component } from 'react';
import queryString from 'query-string';
import FollowedArtistsList from './FollowedArtistsList';

export class SpotifyCallback extends Component {

    constructor(props) {
        super(props);

        const hashValues = queryString.parse(this.props.location.hash);
        const searchValues = queryString.parse(this.props.location.search);

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
