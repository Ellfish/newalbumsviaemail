import React from 'react';
import Loader from 'react-loader-spinner';

export default function LoadingSpinner(props) {
    return (
        <Loader
            type='Oval'
            visible='true'
            height={50}
            width={50}
            timeout={15}
        />
    ); 
}
