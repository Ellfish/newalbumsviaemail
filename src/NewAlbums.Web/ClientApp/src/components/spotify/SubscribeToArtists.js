import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import FollowedArtistsList from './FollowedArtistsList';

export class SubscribeToArtists extends Component {

    constructor(props) {
        super(props);

        this.state = {
            accessToken: this.props.location.state ? this.props.location.state.accessToken : null
        };
    }

    render() {
        if (!this.state.accessToken) {
            return (
                <div>
                    <h1>Error</h1>
                    <p>No Spotify access token set. Please <Link to='/'>go back and try again.</Link></p>
                </div>
            );
        }

        return (
            <div>
                <h1>Select artists to subscribe to</h1>

                <FollowedArtistsList accessToken={this.state.accessToken} />
            </div>
        );
    }
}
