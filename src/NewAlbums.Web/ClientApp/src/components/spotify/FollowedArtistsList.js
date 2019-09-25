import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import Loader from 'react-loader-spinner';
import { useOurApi } from '../../hooks/useOurApi';
import ErrorMessage from '../common/ErrorMessage';
import FollowedArtistsListItem from './FollowedArtistsListItem';

export default function FollowedArtistsList(props) {
    const url = `/api/Spotify/FollowedArtists?AccessToken=${props.accessToken}`;
    const { data, isLoading, hasError, errorMessage } = useOurApi(url, []);
    const [artists, setArtists] = useState([]);

    if (isLoading || (!hasError && data.length === 0)) {
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

    if (hasError) {
        return <ErrorMessage message={errorMessage} />;
    }

    if (data.length > 0 && artists.length === 0) {
        setArtists(data);
    }    

    let artistListItems = [];
    for (let i = 0; i < artists.length; i++) {
        artistListItems.push(<FollowedArtistsListItem artist={artists[i]} key={artists[i].id} handleClick={(artist) => handleArtistClick(artist)} />);
    }

    return (
        <div>
            <Button bsStyle='primary' onClick={() => setSelectedAllArtists(true)}>Select All</Button>
            <Button bsStyle='primary' onClick={() => setSelectedAllArtists(false)}>Select None</Button>

            <ul className="followed-artists-list">
                {artistListItems}
            </ul>
        </div>
    );

    function handleArtistClick(artist) {
        setArtists(artists.map((a) => {
            if (a.id === artist.id) {
                a.selected = !a.selected;
            }

            return a;
        }));
    }

    function setSelectedAllArtists(selected) {
        setArtists(artists.map((artist) => {
            artist.selected = selected;
            return artist;
        }));
    }
}
