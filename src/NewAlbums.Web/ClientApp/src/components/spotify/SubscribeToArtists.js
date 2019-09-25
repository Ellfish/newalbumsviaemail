import React, { Component } from 'react';
import FollowedArtistsList from './FollowedArtistsList';

export class SubscribeToArtists extends Component {

    constructor(props) {
        super(props);

        this.state = {
            accessToken: this.props.location.state.accessToken
        };
    }

    render() {
        return (
            <div>
                <h2>Select artists to subscribe to</h2>

                <FollowedArtistsList accessToken={this.state.accessToken} />
            </div>
        );
    }
}
