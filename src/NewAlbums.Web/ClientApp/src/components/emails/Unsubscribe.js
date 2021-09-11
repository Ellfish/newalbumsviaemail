import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import { useOurApi } from '../../hooks/UseOurApi';
import parseSearch from '../../utils/parseSearch';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './Unsubscribe.scss';

export default function Unsubscribe(props) {
    const searchValues = parseSearch();
    const [unsubscribeUrl, setUnsubscribeUrl] = useState(null);

    let postData = {
        artistId: searchValues.artistId,
        unsubscribeToken: searchValues.unsubscribeToken
    };

    const { isLoading, hasError, errorMessage, responseData } = useOurApi(unsubscribeUrl, {}, 'POST', postData);

    if (!postData.unsubscribeToken) {
        return (
            <div>
                <h1>Invalid Unsubscribe Link</h1>
                <p>Please ensure you click one of the unsubscribe links from an email.</p>
            </div>
        );
    }

    return (
        <div>
            <h1>{getHeading()}</h1>

            <div className='unsubscribe-container'>
                {renderForm()}
            </div>
        </div>
    );

    function getHeading() {
        if (postData.artistId) {
            return 'Are you sure you want to unsubscribe from this artist?';
        } else {
            return 'Are you sure you want to unsubscribe from all artists?';
        }
    }

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
