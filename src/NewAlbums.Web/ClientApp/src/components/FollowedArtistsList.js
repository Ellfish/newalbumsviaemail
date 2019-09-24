import React from 'react';
import Loader from 'react-loader-spinner';
import { useOurApi } from '../hooks/useOurApi';
import ErrorMessage from './common/ErrorMessage';

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
        artists.push(<li key={data[i].id}>{data[i].name}</li>);
    }

    return (
        <ul className="followed-artists-list">
            {artists}
        </ul>
    );    
}
