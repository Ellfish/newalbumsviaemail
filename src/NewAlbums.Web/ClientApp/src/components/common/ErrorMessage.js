import React from 'react';
import './ErrorMessage.scss';

export default function ErrorMessage(props) {
    let message = props.message;

    if (message && message.toLowerCase().indexOf('error') !== 0) {
        message = 'Error: ' + message;
    }

    return (
        <div className='error-message-container'>
            <div className='error-message'>
                {message}
            </div>
        </div>
    );    
}
