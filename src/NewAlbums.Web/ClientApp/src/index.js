import './styles/bootstrap-override.scss';
import './index.scss';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import ScrollToTop from './components/common/ScrollToTop';
import App from './App';
import { unregister } from './registerServiceWorker';
import smoothscroll from 'smoothscroll-polyfill';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
    <BrowserRouter basename={baseUrl}>
        <ScrollToTop />
        <App />
    </BrowserRouter>,
    rootElement);

//Don't want the service worker, causing too many caching issues
//registerServiceWorker();
unregister();

smoothscroll.polyfill();
