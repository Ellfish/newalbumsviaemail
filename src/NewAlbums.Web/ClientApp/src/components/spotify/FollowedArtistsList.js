import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import ErrorMessage from '../common/ErrorMessage';
import LoadingSpinner from '../common/LoadingSpinner';
import FollowedArtistsListItem from './FollowedArtistsListItem';
import SubscribeForm from './SubscribeForm';
import './FollowedArtistsList.scss';

export default function FollowedArtistsList(props) {
    const url = `/api/Spotify/FollowedArtists?AccessToken=${props.accessToken}`;
    const { responseData, isLoading, hasError, errorMessage } = useOurApi(url, []);
    const [artists, setArtists] = useState([]);

    if (isLoading || (!hasError && responseData.length === 0)) {
        return <LoadingSpinner />;
    }

    if (hasError) {
        return <ErrorMessage message={errorMessage} />;
    }

    if (responseData.length > 0 && artists.length === 0) {
        setArtists(responseData);
    }    

    return (
        <div>
            <Button bsStyle='primary' className='m-r-10' onClick={() => setSelectedAllArtists(true)}>Select All</Button>
            <Button bsStyle='primary' onClick={() => setSelectedAllArtists(false)}>Select None</Button>

            <ul className='followed-artists-list list-unstyled'>
                {renderArtistListItems()}
            </ul>

            <Button bsStyle='primary' className='m-r-10' onClick={() => setSelectedAllArtists(true)}>Select All</Button>
            <Button bsStyle='primary' onClick={() => setSelectedAllArtists(false)}>Select None</Button>

            <SubscribeForm artists={artists} />
        </div>
    );

    function renderArtistListItems() {
        let artistListItems = [];
        for (let i = 0; i < artists.length; i++) {
            artistListItems.push(<FollowedArtistsListItem artist={artists[i]} key={artists[i].id} handleClick={(artist) => handleArtistClick(artist)} />);
        }

        return artistListItems;
    }

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
