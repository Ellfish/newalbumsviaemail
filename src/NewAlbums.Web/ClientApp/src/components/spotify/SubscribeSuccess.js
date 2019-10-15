import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class SubscribeSuccess extends Component {

    constructor(props) {
        super(props);

        this.state = {
            statusMessage: this.props.location.state ? this.props.location.state.statusMessage : null
        };
    }

    render() {
        if (!this.state.statusMessage) {
            return (
                <div>
                    <h1>Error</h1>
                    <p>An unexpected error occurred. Please <Link to='/'>go back and try again.</Link></p>
                </div>
            );
        }

        return (
            <div>
                <h1>Success!</h1>

                <p>{this.state.statusMessage}</p>

                <p>Keep an eye on your email inbox for new album notifications.</p>
            </div>
        );
    }
}
