import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import parseHash from '../../utils/parseHash';
import parseSearch from '../../utils/parseSearch';

export class SpotifyCallback extends Component {

    constructor(props) {
        super(props);

        const hashValues = parseHash();
        const searchValues = parseSearch();

        this.state = {
            error: searchValues.error,
            accessToken: hashValues.access_token
        };
    }

    render() {
        if (this.state.error) {
            return (
                <div>
                    <h2>Error</h2>
                    <p>You must authorise access to your Spotify account so we can read your followed artists. <a href='/'>Go back and try again.</a></p>
                </div>
            );
        }

        if (this.state.accessToken) {
            return (
                <Redirect to={{
                    pathname: '/subscribe-to-artists',
                    state: { accessToken: this.state.accessToken }
                }}
                />
            );
        }
    }
}
