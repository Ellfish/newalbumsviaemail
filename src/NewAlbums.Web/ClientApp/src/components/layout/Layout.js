import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import Header from './Header';
import Footer from './Footer';
import './Layout.scss';

export class Layout extends Component {
    displayName = Layout.name

    render() {
        return (
            <div>
                <div>
                    <Row className='no-gutters'>
                        <Col xs={12}>
                            <Header />
                        </Col>
                    </Row>
                </div>
                <Grid className='page-container'>
                    <Row>
                        <Col xs={12}>
                            {this.props.children}
                        </Col>
                    </Row>
                </Grid>
                <div>
                    <Row className='no-gutters'>
                        <Col xs={12}>
                            <Footer />
                        </Col>
                    </Row>
                </div>
            </div>
        );
    }
}
