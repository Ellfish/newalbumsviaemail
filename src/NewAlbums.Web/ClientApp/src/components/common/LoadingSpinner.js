import React from 'react';
import Loader from 'react-loader-spinner';

export default function LoadingSpinner(props) {
    return (
        <Loader
            type='Oval'
            visible='true'
            height={50}
            width={50}
            //Using the timeout option was causing this issue: https://github.com/mhnpd/react-loader-spinner/issues/52#issue-510096779
            //timeout={15000}
        />
    ); 
}
