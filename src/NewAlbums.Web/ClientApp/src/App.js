import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/layout/Layout';
import { Home } from './components/Home';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { SpotifyCallback } from './components/spotify/SpotifyCallback';

export default class App extends Component {
    displayName = App.name

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/spotify-callback' component={SpotifyCallback} />
                <Route path='/privacy-policy' component={PrivacyPolicy} />
            </Layout>
        );
    }
}
