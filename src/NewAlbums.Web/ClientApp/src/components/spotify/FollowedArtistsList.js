import React from 'react';
import Loader from 'react-loader-spinner';
import { Button } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import ErrorMessage from '../common/ErrorMessage';
import FollowedArtistsListItem from './FollowedArtistsListItem';

export default function FollowedArtistsList(props) {
    const url = `/api/Spotify/FollowedArtists?AccessToken=${props.accessToken}`;
    const { data, isLoading, hasError, errorMessage } = useOurApi(url, []);

    if (isLoading) {
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

    let artists = [];
    for (let i = 0; i < data.length; i++) {
        artists.push(<FollowedArtistsListItem artist={data[i]} key={data[i].id} />);
    }

    return (
        <div>
            <Button>Select All</Button>
            <Button>Select None</Button>

            <ul className="followed-artists-list">
                {artists}
            </ul>
        </div>
    );    
}
