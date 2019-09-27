import React, { useState } from 'react';
import { Redirect } from 'react-router-dom';
import { Button, FormGroup, FormControl, ControlLabel, Row, Col } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './SubscribeForm.scss';

export default function SubscribeForm(props) {
    const [emailAddress, setEmailAddress] = useState('');
    const [postData, setPostData] = useState(null);
    const [subscribeUrl, setSubscribeUrl] = useState(null);
    const { isLoading, hasError, errorMessage, responseData } = useOurApi(subscribeUrl, {}, 'POST', postData);

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
        if (isLoading) {
            return <LoadingSpinner />;
        } else if (hasError) {
            return <ErrorMessage message={errorMessage} />;
        } else if (responseData === true) {
            return (
                <Redirect to={{
                    pathname: '/subscribe-success',
                    state: { artists: getSelectedArtists() }
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
                    </FormGroup>
                    <Button type='submit' bsStyle='primary' bsSize='lg' disabled={getNumSelected() === 0}>Subscribe</Button>
                </form>
            );
        }
    }

    function handleEmailChange(e) {
        setEmailAddress(e.target.value);
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
