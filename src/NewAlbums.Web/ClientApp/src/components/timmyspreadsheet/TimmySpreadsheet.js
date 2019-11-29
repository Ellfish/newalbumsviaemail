import React, { useState } from 'react';
import { Redirect } from 'react-router-dom';
import { Button, FormGroup, FormControl, ControlLabel, Row, Col } from 'react-bootstrap';
import { useOurApi } from '../../hooks/useOurApi';
import objectEmpty from '../../utils/objectEmpty';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './TimmySpreadsheet.scss';

export default function TimmySpreadsheet(props) {
    const [uploadUrl, setUploadUrl] = useState(null);

    //Not triggered under setSubscribeUrl is called in handleSubmit()
    const {
        isLoading: uploadLoading,
        hasError: uploadHasError,
        errorMessage: uploadErrorMessage,
        responseData: responseUpload
    } = useOurApi(uploadUrl, {}, 'POST', {});   

    return (
        <div className='spreadsheet-form-container'>
            <Row>
                <Col xs={12}>
                    <h1>Timmy's special spreadsheet upload</h1>

                    {renderForm()}
                </Col>
            </Row>
        </div>
    );

    function renderForm() {
        if (uploadLoading) {
            return <LoadingSpinner />;
        } else if (uploadHasError) {
            return <ErrorMessage message={uploadErrorMessage} />;
        } else {
            return (
                <form onSubmit={(e) => handleSubmit(e)}>
                    <FormGroup
                        bsSize='lg'
                        controlId='email'
                    >
                        <ControlLabel>Select spreadsheet</ControlLabel>
                        <FormControl
                            type='file'
                            required
                        />
                    </FormGroup>
                    <Button type='submit' bsStyle='primary' bsSize='lg'>Upload</Button>
                </form>
            );
        }
    }

    function handleSubmit(e) {
        e.preventDefault();

        //Triggers useOurApi to post
        setUploadUrl('/api/Upload/Upload');
    }
}
