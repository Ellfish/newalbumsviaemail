import React from 'react';
import classNames from 'classnames';
import './FollowedArtistsListItem.scss';

export default function FollowedArtistsListItem(props) {
    let liClass = classNames({
        'artist-container': true,
        'selected': props.artist.selected
    });

    return (
        <li className={liClass} onClick={() => props.handleClick(props.artist)}>
            <img className='artist-image' src={props.artist.image.url} loading='lazy' alt={props.artist.name} width='180' height='180' />
            <div className='artist-name'>{props.artist.name}</div>
        </li>
    );    
}
