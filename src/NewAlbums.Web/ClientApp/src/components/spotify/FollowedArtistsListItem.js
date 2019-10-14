import React from 'react';
import classNames from 'classnames';
import './FollowedArtistsListItem.scss';

export default function FollowedArtistsListItem(props) {
    let liClass = classNames({
        'artist-container': true,
        'selected': props.artist.selected
    });

    return (
        <div className={liClass} onClick={() => props.handleClick(props.artist)}>
            <div className='square'>
                <img className='artist-image' src={props.artist.image.url} loading='lazy' alt={props.artist.name} width='120' height='120' />
            </div>
            <div className='artist-name'>{props.artist.name}</div>
        </div>
    );    
}
