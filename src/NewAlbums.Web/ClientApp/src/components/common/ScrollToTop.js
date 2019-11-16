//From: https://reacttraining.com/react-router/web/guides/scroll-restoration

import { useEffect } from "react";
import { useLocation } from "react-router-dom";

export default function ScrollToTop() {
    const { pathname } = useLocation();

    useEffect(() => {
        let top = 0;

        //Don't cut off the header on the home page
        if (pathname !== '/') {
            let header = document.querySelector('header');
            if (header) {
                top = header.offsetHeight;
            }
        }

        //We have smoothscroll-polyfill to cover browsers which don't support "behavior"
        window.scrollTo({
            top: top,
            left: 0,
            behavior: 'smooth'
        });
    }, [pathname]);

    return null;
}