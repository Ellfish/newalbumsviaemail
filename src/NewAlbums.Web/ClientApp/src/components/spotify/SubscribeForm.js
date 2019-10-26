import React, { useState } from 'react';
import { Redirect } from 'react-router-dom';
import { Button, FormGroup, FormControl, ControlLabel, Row, Col } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './SubscribeForm.scss';

export default function SubscribeForm(props) {
    const [emailAddress, setEmailAddress] = useState('');
    const [emailAddressMatchesSpotify, setEmailAddressMatchesSpotify] = useState(true);
    const [postData, setPostData] = useState(null);
    const [subscribeUrl, setSubscribeUrl] = useState(null);

    //Triggered upon page load
    const { hasError: emailHasError, responseData: responseEmailAddress } = useOurApi(`/api/Spotify/UserEmail?AccessToken=${props.accessToken}`, '');

    //Not triggered under setSubscribeUrl is called in handleSubmit()
    const {
        isLoading: subscribeLoading,
        hasError: subscribeHasError,
        errorMessage: subscribeErrorMessage,
        responseData: responseSubscribe
    } = useOurApi(subscribeUrl, {}, 'POST', postData);   

    //Attempt to auto-fill their Spotify account email address
    if (responseEmailAddress && !emailAddress) {
        setEmailAddress(responseEmailAddress);
        setEmailAddressMatchesSpotify(true);
    } else if (emailHasError) {
        setEmailAddressMatchesSpotify(false);
    }

    return (
        <div className='subscribe-form-container'>
            <Row className='no-gutter'>
                <Col xs={12} sm={6}>
                    <h3>{getHeading()}</h3>

                    {renderForm()}
                </Col>
            </Row>
        </div>
    );

    function renderForm() {
        if (subscribeLoading) {
            return <LoadingSpinner />;
        } else if (subscribeHasError) {
            return <ErrorMessage message={subscribeErrorMessage} />;
        } else if (responseSubscribe && responseSubscribe.length) {
            return (
                <Redirect to={{
                    pathname: '/subscribe-success',
                    state: { statusMessage: responseSubscribe }
                }}
                />
            );
        } else {
            return (
                <form onSubmit={(e) => handleSubmit(e)}>
                    <FormGroup
                        bsSize='lg'
                        controlId='email'
                    >
                        <ControlLabel>Your email</ControlLabel>
                        <FormControl
                            type='email'
                            placeholder='name@example.com'
                            spellCheck={false}
                            value={emailAddress}
                            onChange={handleEmailChange}
                            required
                        />
                        { !emailAddressMatchesSpotify
                            ? <div className='text-danger m-t-5 email-warning'>Using a different email to your Spotify account requires an extra verification step.</div>
                            : null }
                    </FormGroup>
                    <Button type='submit' bsStyle='primary' bsSize='lg' disabled={getNumSelected() === 0}>Subscribe</Button>
                </form>
            );
        }
    }

    function handleEmailChange(e) {
        setEmailAddress(e.target.value);
        setEmailAddressMatchesSpotify(e.target.value === responseEmailAddress);
    }

    function handleSubmit(e) {
        e.preventDefault();

        const spotifyArtists = getSelectedArtists();
        
        setPostData({
            emailAddress: emailAddress,
            spotifyArtists: spotifyArtists
        });

        //Triggers useOurApi to post
        setSubscribeUrl('/api/Subscription/SubscribeToArtists');
    }

    function getSelectedArtists() {
        return props.artists.filter((artist) => {
            return artist.selected;
        });
    }

    function getNumSelected() {
        let count = 0;

        props.artists.forEach((a) => {
            if (a.selected) {
                count++;
            }
        });

        return count;
    }

    function getHeading() {
        const numSelected = getNumSelected();

        return `Subscribing to ${numSelected} artist${numSelected === 1 ? '' : 's'}`;
    }
}
