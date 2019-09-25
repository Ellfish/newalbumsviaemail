//From: https://medium.com/@malith.priyashan.dev/track-users-in-your-react-app-with-google-analytics-6364ebfcbae8

import ReactGA from "react-ga";

export const InitGA = (trackingID) => {
    ReactGA.initialize(trackingID);
};

export const PageView = () => {
    ReactGA.pageview(window.location.pathname +
        window.location.search);
};

export const Event = (category, action, label) => {
    ReactGA.event({
        category: category,
        action: action,
        label: label
    });
};
