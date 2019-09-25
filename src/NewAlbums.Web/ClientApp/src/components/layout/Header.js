import React from 'react';
import './Header.scss';

export default function Header(props) {

    return (
        <header>
            <h1 className='app-name'>New Albums via Email</h1>
            <div className='app-subheading'>Receive email notifications when artists you follow on Spotify release new albums</div>
        </header>
    );
};