import React, { useState } from 'react';
import classNames from 'classnames';
import './FollowedArtistListItem.scss';

export default function FollowedArtistsListItem(props) {
    const [selected, setSelected] = useState(false);

    let liClass = classNames({
        'artist-container': true,
        'selected': selected
    });

    return (
        <li className={liClass} onClick={() => setSelected(!selected)}>
            <img className='artist-image' src={props.artist.image.url} loading='lazy' alt={props.artist.name} width='180' height='180' />
            <div className='artist-name'>{props.artist.name}</div>
        </li>
    );    
}
