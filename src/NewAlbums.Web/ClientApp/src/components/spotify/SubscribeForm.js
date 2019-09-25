import React, { useState } from 'react';
import { Button, FormGroup, FormControl, ControlLabel, Row, Col } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import LoadingSpinner from '../common/LoadingSpinner';
import './SubscribeForm.scss';

export default function SubscribeForm(props) {
    const [emailAddress, setEmailAddress] = useState('');
    
    //if (isLoading) {
    //    return <LoadingSpinner />;
    //}

    return (
        <div className='subscribe-form-container'>
            <Row className='no-gutter'>
                <Col xs={12} sm={6}>
                    <h3>{getHeading()}</h3>

                    <form onSubmit={() => handleSubmit()}>
                        <FormGroup
                            bsSize='lg'
                            controlId='email'
                        >
                            <ControlLabel>Your email</ControlLabel>
                            <FormControl 
                                type='email'
                                placeholder='name@example.com'
                                value={emailAddress}
                                onChange={handleEmailChange}
                                required
                            />
                        </FormGroup>
                        <Button type='submit' bsStyle='primary' bsSize='lg' disabled={getNumSelected() === 0}>Subscribe</Button>
                    </form>
                </Col>
            </Row>
        </div>
    );

    function handleEmailChange(e) {
        setEmailAddress(e.target.value);
    }

    function handleSubmit() {
        const url = '/api/Spotify/SubscribeToArtists';
        //const { data, isLoading, hasError, errorMessage } = useOurApi(url, {});
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
