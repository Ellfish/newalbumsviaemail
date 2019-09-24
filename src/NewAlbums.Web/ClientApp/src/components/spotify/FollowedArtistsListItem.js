import React from 'react';
import './FollowedArtistListItem.scss';

export default function FollowedArtistsListItem(props) {

    return (
        <li className='artist-container'>
            <img className='artist-image' src={props.artist.image.url} loading='lazy' alt={props.artist.name} width='180' height='180' />
            <div className='artist-name'>{props.artist.name}</div>
        </li>
    );    
}
