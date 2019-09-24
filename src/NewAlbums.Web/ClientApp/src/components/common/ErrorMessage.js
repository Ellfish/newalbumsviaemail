import React from 'react';
import './ErrorMessage.scss';

export default function ErrorMessage(props) {
    return (
        <div className='error-message-container'>
            <div className='error-message'>
                {props.message}
            </div>
        </div>
    );    
}
