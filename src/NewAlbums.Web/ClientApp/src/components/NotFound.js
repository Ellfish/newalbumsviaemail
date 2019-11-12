import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class NotFound extends Component {
    displayName = NotFound.name;

    render() {
        return (
            <div>
                <h1>Not Found</h1>

                <p>
                    That page was not found. Please <Link to='/'>click here</Link> to return home.
                </p>
            </div>
        );
    }
}
