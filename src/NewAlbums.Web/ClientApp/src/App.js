import React, { Component } from 'react';
import { Route } from 'react-router';
import { InitGA, PageView } from './components/tracking/Tracking';
import { Layout } from './components/layout/Layout';
import { Home } from './components/Home';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { SpotifyCallback } from './components/spotify/SpotifyCallback';
import { SubscribeToArtists } from './components/spotify/SubscribeToArtists';
import { SubscribeSuccess } from './components/spotify/SubscribeSuccess';

export default class App extends Component {
    displayName = App.name

    componentDidMount() {
        if (process.env.NODE_ENV === 'production') {
            InitGA('UA-43690368-29');
            PageView();
        }
    }

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/spotify-callback' component={SpotifyCallback} />
                <Route path='/subscribe-to-artists' component={SubscribeToArtists} />
                <Route path='/subscribe-success' component={SubscribeSuccess} />
                <Route path='/privacy-policy' component={PrivacyPolicy} />
            </Layout>
        );
    }
}
