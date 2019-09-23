import { useEffect, useState } from 'react';

/*  Example
    initialUrl: "/api/jobs"
    initialData: [] //usually empty array or object

    Adapted from: https://www.henriksommerfeld.se/error-handling-with-fetch/
*/
export const useOurApi = (initialUrl, initialData) => {
    const [url, setUrl] = useState(initialUrl);
    const [isLoading, setIsLoading] = useState(true);
    const [hasError, setHasError] = useState(false);
    const [errorMessage, setErrorMessage] = useState(null);
    const [fetchedData, setFetchedData] = useState(initialData);

    //TODO: add errorMessage, setErrorMessage

    useEffect(() => {
        let unmounted = false;

        const handleFetchResponse = response => {
            if (unmounted) return initialData;

            setHasError(!response.ok);
            setIsLoading(false);

            //We expect our API to always return consistent json responses. See NewAlbums.Web.Responses.Common classes
            if (!response.ok || !response.json) {
                if (response.json) {
                    response.json().then(data => {
                        setErrorMessage(data.message);
                    });
                } else {
                    setErrorMessage('Unexpected response format');
                }

                return initialData;
            }

            return response.json();
        };

        const fetchData = () => {
            setIsLoading(true);
            return fetch(url, { credentials: 'include' })
                .then(handleFetchResponse)
                .catch(handleFetchResponse);
        };

        if (initialUrl && !unmounted)
            fetchData().then(responseJson => !unmounted && setFetchedData(responseJson.result));

        return () => {
            unmounted = true;
        };
    }, [url]);

    return { isLoading, hasError, errorMessage, setUrl, data: fetchedData };
};
