import React, { Component } from 'react';
import queryString from 'query-string';

export class SpotifyCallback extends Component {

    constructor(props) {
        super(props);

        const queryValues = queryString.parse(this.props.location.search);
        this.state = { error: queryValues.error, accessToken: queryValues.access_token };
    }

    componentDidMount() {
        if (this.state.accessToken) {
            //TODO: load followed artists via API
        }
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

        return (
            <div>
                <h1>Select artists to subscribe to</h1>
            </div>
        );
    }
}
