import React from 'react';
import { Link } from 'react-router-dom';
import './Header.scss';

export default function Header(props) {

    return (
        <header>
            <Link to='/' className='home-link'>
                <div className='app-name'>New Albums via Email</div>
            </Link>
            <div className='app-subheading'>Receive email notifications when artists you follow on Spotify release new albums</div>
        </header>
    );
};