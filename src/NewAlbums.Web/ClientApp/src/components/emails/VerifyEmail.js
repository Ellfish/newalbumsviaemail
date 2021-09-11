import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import { useOurApi } from '../../hooks/UseOurApi';
import parseSearch from '../../utils/parseSearch';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './VerifyEmail.scss';

export default function VerifyEmail(props) {
    const searchValues = parseSearch();
    const [verifyUrl, setVerifyUrl] = useState(null);

    let postData = {
        emailAddress: searchValues.emailAddress,
        verifyCode: searchValues.verifyCode
    };

    const { isLoading, hasError, errorMessage, responseData: verifyResponse } = useOurApi(verifyUrl, {}, 'POST', postData);

    if (!postData.emailAddress || !postData.verifyCode) {
        return (
            <div>
                <h1>Invalid Verify Link</h1>
                <p>Please ensure you click the verify link or button in the email.</p>
            </div>
        );
    }

    return (
        <div>
            <h1>Verification</h1>

            <p>Please click the button below to verify {postData.emailAddress}</p>

            <div className='verify-container m-t-50'>
                {renderForm()}
            </div>
        </div>
    );

    function renderForm() {
        if (isLoading) {
            return <LoadingSpinner />;
        } else if (hasError) {
            return <ErrorMessage message={errorMessage} />;
        } else if (verifyResponse === true) {
            return (
                <p>Your email is now verified, and you can receive email notifications. Thank you.</p>
            );
        } else {
            return (
                <form onSubmit={(e) => handleSubmit(e)}>
                    <Button type='submit' bsStyle='primary' bsSize='lg'>Verify my email</Button>
                </form>
            );
        }
    }

    function handleSubmit(e) {
        e.preventDefault();

        //Triggers useOurApi to post
        setVerifyUrl('/api/Subscriber/VerifyEmail');
    }
}
