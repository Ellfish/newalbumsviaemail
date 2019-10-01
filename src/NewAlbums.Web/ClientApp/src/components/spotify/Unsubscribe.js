import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import parseSearch from '../../utils/parseSearch';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';

export default function Unsubscribe(props) {
    const searchValues = parseSearch();
    const [artistId, setArtistId] = useState(searchValues.artistId);
    const [unsubscribeToken, setUnsubscribeToken] = useState(searchValues.unsubscribeToken);
    const [unsubscribeUrl, setUnsubscribeUrl] = useState(null);

    let postData = {
        artistId: artistId,
        unsubscribeToken: unsubscribeToken
    };
    const { isLoading, hasError, errorMessage, responseData } = useOurApi(unsubscribeUrl, {}, 'POST', postData);

    if (!unsubscribeToken) {
        return (
            <div>
                <h1>Invalid Unsubscribe Link</h1>
                <p>Please ensure you click one of the unsubscribe links from an email.</p>
            </div>
        );
    }

    return (
        <div>
            <h1>Are you sure you want to unsubscribe?</h1>

            {renderForm()}
        </div>
    );

    function renderForm() {
        if (isLoading) {
            return <LoadingSpinner />;
        } else if (hasError) {
            return <ErrorMessage message={errorMessage} />;
        } else if (responseData === true) {
            return (
                <p>Successfully unsubscribed.</p>
            );
        } else {
            return (
                <form onSubmit={(e) => handleSubmit(e)}>
                    <Button type='submit' bsStyle='primary' bsSize='lg'>Unsubscribe</Button>
                </form>
            );
        }
    }

    function handleSubmit(e) {
        e.preventDefault();

        //Triggers useOurApi to post
        setUnsubscribeUrl('/api/Subscription/Unsubscribe');
    }
}
