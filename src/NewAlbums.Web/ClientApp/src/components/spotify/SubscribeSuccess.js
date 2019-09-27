import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class SubscribeSuccess extends Component {

    constructor(props) {
        super(props);

        this.state = {
            artists: this.props.location.state ? this.props.location.state.artists : null
        };
    }

    render() {
        if (!this.state.artists || this.state.artists.length === 0) {
            return (
                <div>
                    <h1>Error</h1>
                    <p>No artists provided. Please <Link to='/'>go back and try again.</Link></p>
                </div>
            );
        }

        return (
            <div>
                <h1>Success!</h1>

                <p>You're now subscribed to {this.state.artists.length} artists.</p>

                <p>Keep an eye on your email inbox for new album notifications.</p>
            </div>
        );
    }
}
