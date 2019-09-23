import React from 'react';
import { useOurApi } from '../hooks/useOurApi';

export default function FollowedArtistsList(props) {
    const url = `/api/Spotify/FollowedArtists?AccessToken=${props.accessToken}`;
    const { data, isLoading, hasError, errorMessage } = useOurApi(url, []);

    //if (isLoading) return <Spinner />;
    if (isLoading) return <p>Loading...</p>;

    if (hasError) {
        //return <ErrorMessage message={`Failed to fetch open jobs`} />;
        return <p>Error: {errorMessage}</p>;
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
