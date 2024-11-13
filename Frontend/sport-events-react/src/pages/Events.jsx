import CustomCard from "../components/CustomCard";
import React, { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';
import styles from "../css/FilterDiv.module.css"
import "../css/Events.css";
import { Row, Col, Container, Button, Form } from 'react-bootstrap';

function Events() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [offset, setOffset] = useState(0);
    const [country, setCountry] = useState("");
    const [date, setDate] = useState(getTodayDate());
    const [priceFilter, setPriceFilter] = useState(0);
    const [parking, setParking] = useState(0);
    const [starRating, setStarRating] = useState(0);
    const [isAdmin, setIsAdmin] = useState(false);
    const limit = 6;

    const navigate = useNavigate();

    function getTodayDate() {
        const today = new Date();
        return today.toISOString().split('T')[0]; // This returns the date in 'YYYY-MM-DD' format
    }

    const fetchData = (offset, price, parking, rating, country, date) => {
        setLoading(true);
        fetch(`https://localhost:7023/api/Events?offset=${offset}&limit=${limit}&date=${date}&country=${country}&price=${price}&parking=${parking}&rating=${rating}`)

            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setData(data);
                setLoading(false);
            })
            .catch((error) => {
                setError(error);
                setLoading(false);
            });
    };

    useEffect(() => {
        fetchData(offset, priceFilter, parking, starRating, country, date);
    }, [offset, priceFilter, parking, starRating, date]);

    useEffect(() => {
        fetch(`https://localhost:7023/api/User/is-admin`, {
            credentials: 'include'
        })
            .then((response) => {
                if (response.ok) {
                    setIsAdmin(response.body);
                }
            })
            .catch((error) => {
                console.log(error);
            });
    }, []);


    const handleNextPage = () => {
        setOffset(offset + limit);
    };

    const handlePreviousPage = () => {
        if (offset >= limit) {
            setOffset(offset - limit);
        }
    };

    const handlePriceFilter = (event) => {
        const value = parseInt(event.target.value);
        setPriceFilter(value === priceFilter ? 0 : value);
    }

    const handleParkingFilter = (event) => {
        const value = parseInt(event.target.value);
        setParking(value === parking ? 0 : value);
    }

    const handleRatingFilter = (event) => {
        const value = parseInt(event.target.value);
        setStarRating(value === starRating ? 0 : value);
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        fetchData(offset, priceFilter, parking, starRating, country, date)
    }


    // const handleCountryFilter = (event) => {
    //     const value = event.target.value;
    //     setCountry(value);
    // }

    const handleAddEventClick = () => {
        navigate('/admin/event/add');
    };


    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    return (
        <div id="events-content">
            <div id="events-top-container">

            </div>
            <div id="events-bottom-container">
                {isAdmin ?
                    <div id="add-event-container">
                        <div className="line"></div>
                        <button id="add-event-button" onClick={handleAddEventClick}>ADD EVENT</button>
                        <div className="line"></div>
                    </div>
                    : ""}
                <Container className={styles.container}>
                    <Row>
                        {/* Left Filter Section */}
                        <Col xs={12} sm={6} md={3} lg={3} xl={2} className={styles.filterDiv}>
                            <div>
                                <h4>Filters</h4>

                                <div className={styles.filterOption}>
                                    <label>Date</label>
                                    <input type="date" className="form-control" value={date} onChange={(e) => setDate(e.target.value)} />
                                </div>
                                <Form onSubmit={handleSubmit}>
                                    <div className={styles.filterOption}>
                                        <label>Country</label>
                                        <input type="text" className="form-control" placeholder="Type..." value={country} onChange={(e) => setCountry(e.target.value)} />
                                    </div>
                                </Form>
                                <div className={styles.filterOption}>
                                    <label>Price</label>
                                    <div>
                                        <input type="radio" name="price" value="1" onClick={handlePriceFilter} checked={priceFilter === 1} /> From Lowest <br />
                                        <input type="radio" name="price" value="2" onClick={handlePriceFilter} checked={priceFilter === 2} /> From Highest
                                    </div>
                                </div>
                                <div className={styles.filterOption}>
                                    <label>Popular Filters</label>
                                    <div>
                                        <input type="radio" name="filter" value="1" onClick={handleParkingFilter} checked={parking === 1} /> Free Ticket <br />
                                        <input type="radio" name="filter" value="2" onClick={handleParkingFilter} checked={parking === 2} /> Parking
                                    </div>
                                </div>
                                <div className={styles.filterOption}>
                                    <label>Star Ratings</label>
                                    <div>
                                        <input type="radio" name="rating" value="5" onClick={handleRatingFilter} checked={starRating === 5} /> 5 Stars <br />
                                        <input type="radio" name="rating" value="4" onClick={handleRatingFilter} checked={starRating === 4} /> 4 Stars <br />
                                        <input type="radio" name="rating" value="3" onClick={handleRatingFilter} checked={starRating === 3} /> 3 Stars
                                    </div>
                                </div>
                            </div>
                        </Col>

                        {/* Right Card Section */}
                        <Col md={9} className={styles.cardGrid}>
                            <Row className="g-4">
                                {data && data.length > 0 ? (
                                    data.map((event, index) => (
                                        <Col md={{ span: 5, offset: 1 }} key={index} className="mb-3">
                                            <CustomCard json={event} />
                                        </Col>
                                    ))
                                ) : (
                                    <div>No events found</div>
                                )}
                            </Row>

                            {/* Pagination Controls */}


                        </Col>
                        <Row className="mt-4 g-4">
                            <Col className="text-center">
                                <Button
                                    variant="primary"
                                    onClick={handlePreviousPage}
                                    disabled={offset === 0}
                                    className="me-2"
                                >
                                    Previous
                                </Button>
                                <Button
                                    variant="primary"
                                    onClick={handleNextPage}
                                    disabled={data && data.length < limit}
                                >
                                    Next
                                </Button>
                            </Col>
                        </Row>
                    </Row>
                </Container>
            </div>

        </div>

    )
}

export default Events;