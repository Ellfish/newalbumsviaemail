import React from 'react';
import { Link } from 'react-router-dom';
import './Footer.scss';

export default function Footer(props) {

    return (
        <footer>
            <ul className='footer-links-list list-unstyled'>
                <li>
                    New Albums via Email
                </li>
                <li>
                    An app by <a href='http://www.icebergweb.com.au'>Iceberg Web Development</a>
                </li>
                <li>
                    Source code on <a href='https://github.com/Ellfish/newalbumsviaemail'>Github</a>
                </li>
                <li>
                    <Link to='/'>Home</Link>
                </li>
                <li>
                    <Link to='/privacy-policy'>Privacy policy</Link>
                </li>
            </ul>

            <div className='spotify-logo-container'>
                Artist and album content including cover art supplied by:

                <a href='https://www.spotify.com'>
                    <img className='spotify-logo' src='/images/spotify-logo-white.png' alt='Spotify' />
                </a>
            </div>
        </footer>
    );
};