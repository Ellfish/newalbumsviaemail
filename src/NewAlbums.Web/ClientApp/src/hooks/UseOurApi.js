import { useEffect, useState } from 'react';

/*  Example
    url: "/api/jobs"
    initialData: [] //usually empty array or object

    Adapted from: https://www.henriksommerfeld.se/error-handling-with-fetch/
*/
export const useOurApi = (url, initialData, method, postData) => {
    const [isLoading, setIsLoading] = useState(false);
    const [hasError, setHasError] = useState(false);
    const [errorMessage, setErrorMessage] = useState(null);
    const [fetchedData, setFetchedData] = useState(initialData);

    useEffect(() => {
        let unmounted = false;

        const handleFetchResponse = response => {
            if (unmounted) return initialData;

            setHasError(!response.ok);
            
            const contentType = response.headers.get("content-type");
            const isJson = contentType && contentType.indexOf("application/json") !== -1;

            //We expect our API to always return consistent json responses. See NewAlbums.Web.Responses.Common classes
            if (!response.ok || !isJson) {
                if (isJson) {
                    response.json().then(data => {
                        setErrorMessage(data.message);
                    });
                } else {
                    response.text().then(text => {
                        setErrorMessage(text);
                    });
                }

                return initialData;
            }

            return response.json();
        };

        const fetchData = () => {
            setIsLoading(true);

            let fetchOptions = {
                credentials: 'same-origin',
                method: method ? method : 'GET'
            };

            if (method === 'POST') {
                fetchOptions.headers = {
                    'Content-Type': 'application/json'
                };

                fetchOptions.body = JSON.stringify(postData);
            }

            return fetch(url, fetchOptions)
                .then(handleFetchResponse)
                .catch(handleFetchResponse);
        };

        if (url && !unmounted) {
            fetchData().then(responseJson => {
                if (!unmounted) {
                    setFetchedData(responseJson.result);
                    setIsLoading(false);
                }
            });

            //Only return the cleanup function when we've actually called fetchData()
            return () => {
                unmounted = true;
            };
        }
    }, [url]);

    return { isLoading, hasError, errorMessage, responseData: fetchedData };
};
