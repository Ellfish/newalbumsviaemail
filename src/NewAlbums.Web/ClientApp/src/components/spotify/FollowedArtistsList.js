import React, { useState } from 'react';
import { Button, FormGroup, FormControl, ControlLabel, Row, Col } from 'react-bootstrap';
import { StickyContainer, Sticky } from 'react-sticky';
import { useOurApi } from '../../hooks/useOurApi';
import ErrorMessage from '../common/ErrorMessage';
import LoadingSpinner from '../common/LoadingSpinner';
import FollowedArtistsListItem from './FollowedArtistsListItem';
import SubscribeForm from './SubscribeForm';
import isInViewport from '../../utils/isInViewport';
import './FollowedArtistsList.scss';

export default function FollowedArtistsList(props) {
    const url = `/api/Spotify/FollowedArtists?AccessToken=${props.accessToken}`;
    const { responseData, isLoading, hasError, errorMessage } = useOurApi(url, []);
    const [artists, setArtists] = useState([]);
    const [artistNameFilter, setArtistNameFilter] = useState('');
    const [preselectedArtistsCount, setPreselectedArtistsCount] = useState(0);
    let artistFilterAnchor = React.createRef();

    if (isLoading || (!hasError && responseData.length === 0)) {
        return <LoadingSpinner />;
    }

    if (hasError) {
        return <ErrorMessage message={errorMessage} />;
    }

    if (responseData.length > 0 && artists.length === 0) {
        setArtists(responseData);
        setPreselectedArtistsCount(getNumSelectedArtists(responseData));
    }

    return (
        <div>
            <p className='m-b-40'>
                You're following {artists.length} artists on Spotify. {renderPreselectionMessage()}
                Tap or click artists to select them.
            </p>

            <Button bsStyle='primary' className='m-b-20 m-r-10' onClick={() => setSelectedAllArtists(true)}>Select All</Button>
            <Button bsStyle='primary' className='m-b-20' onClick={() => setSelectedAllArtists(false)}>Select None</Button>

            <div id='artist-filter-anchor' ref={artistFilterAnchor} />
            <StickyContainer>
                <Row className='no-gutter m-t-10'>
                    <Col xs={12} sm={6}>
                        <Sticky bottomOffset={20}>{({ style, isSticky }) =>
                            <div className={'sticky-artist-filter-container is-sticky-' + isSticky} style={style}>
                                <FormGroup
                                    bsSize='lg'
                                    controlId='artist-name-filter'
                                    className='m-b-10'
                                >
                                    <ControlLabel>Filter by name</ControlLabel>
                                    <FormControl
                                        type='text'
                                        className='artist-filter-input'
                                        placeholder='eg: Crowded House'
                                        spellCheck={false}
                                        autoComplete='off'
                                        value={artistNameFilter}
                                        onChange={handleArtistNameFilterChange}
                                    />
                                </FormGroup>
                            </div>
                        }
                        </Sticky>
                    </Col>
                </Row>

                <div className='followed-artists-list-container'>
                    <div className='followed-artists-list'>
                        {renderArtistListItems()}
                    </div>
                </div>

            </StickyContainer>

            <Button bsStyle='primary' className='m-r-10' onClick={() => setSelectedAllArtists(true)}>Select All</Button>
            <Button bsStyle='primary' onClick={() => setSelectedAllArtists(false)}>Select None</Button>

            <SubscribeForm artists={artists} accessToken={props.accessToken} />
        </div>
    );

    function renderPreselectionMessage() {
        if (preselectedArtistsCount > 0) {
            return 'We\'ve preselected ' + preselectedArtistsCount + ' of your top artists (according to Spotify) for you. ';
        }
    }

    function renderArtistListItems() {
        let artistListItems = [];
        for (let i = 0; i < artists.length; i++) {
            if (!artistNameFilter || artists[i].name.toLowerCase().indexOf(artistNameFilter.toLowerCase()) >= 0) {
                artistListItems.push(<FollowedArtistsListItem artist={artists[i]} key={artists[i].id} handleClick={(artist) => handleArtistClick(artist)} />);
            }
        }

        return artistListItems;
    }

    function handleArtistClick(artist) {
        setArtists(artists.map((a) => {
            if (a.id === artist.id) {
                a.selected = !a.selected;
            }

            return a;
        }));
    }

    function handleArtistNameFilterChange(e) {
        setArtistNameFilter(e.target.value);

        //Filtering artists can cause the artists list to shrink to above the current viewport. Reset the window scroll to the top of the artists list.
        if (!isInViewport(artistFilterAnchor.current)) {
            window.scrollTo(0, artistFilterAnchor.current.getBoundingClientRect().top + window.pageYOffset + 40);
        }
    }

    function setSelectedAllArtists(selected) {
        if (selected) {
            setArtistNameFilter('');
        }

        setArtists(artists.map((artist) => {
            artist.selected = selected;
            return artist;
        }));
    }

    function getNumSelectedArtists(artistsResponseData) {
        let count = 0;

        artistsResponseData.forEach((a) => {
            if (a.selected) {
                count++;
            }
        });

        return count;
    }
}
