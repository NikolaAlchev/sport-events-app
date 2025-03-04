import CustomCard from "../components/CustomCard";
import React, { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';
import styles from "../css/FilterDiv.module.css"
import "../css/Events.css";
import { Row, Col, Container, Button, Form } from 'react-bootstrap';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";
import bannerImage from "../assets/stadium.jpg";

function Events() {
    const API_BASE_URL = process.env.REACT_APP_EVENTS_API_BASE_URL;
    
    const [data, setData] = useState(null);
    const [contentLoading, setContentLoading] = useState(true);
    const [error, setError] = useState(null);
    const [offset, setOffset] = useState(0);
    const [country, setCountry] = useState("");
    const [date, setDate] = useState(getTodayDate());
    const [priceFilter, setPriceFilter] = useState(0);
    const [parking, setParking] = useState(0);
    const [freeTicket, setFreeTicket] = useState(0);
    const [starRating, setStarRating] = useState(0);
    const [isAdmin, setIsAdmin] = useState(false);
    const limit = 6;
    
    const navigate = useNavigate();

    function getTodayDate() {
        const today = new Date();
        return today.toISOString().split('T')[0];
    }

    const fetchData = (offset, price, parking, rating, country, date, freeTicket) => {
        setContentLoading(true)
        fetch(`${API_BASE_URL}/api/Events?offset=${offset}&limit=${limit}&date=${date}&country=${country}&price=${price}&parking=${parking}&rating=${rating}&freeTicket=${freeTicket}`)

            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setData(data);
                setContentLoading(false);
            })
            .catch((error) => {
                setError(error);
                setContentLoading(false);
            });
    };

    useEffect(() => {
        fetchData(offset, priceFilter, parking, starRating, country, date, freeTicket);
        // eslint-disable-next-line
    }, [offset, priceFilter, parking, starRating, date, freeTicket]);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/User/is-admin`, {
            credentials: 'include',
        })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Failed to fetch admin status');
            })
            .then((data) => {
                console.log(data);
                setIsAdmin(data);
            })
            .catch((error) => {
                console.error('Error fetching admin status:', error);
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

    const handleParkingFilter = () => {
        setParking(parking ? 0 : 1);
    }

    const handleFreeTicketFilter = () => {
        setFreeTicket(freeTicket ? 0 : 1);
    }

    const handleRatingFilter = (event) => {
        const value = parseInt(event.target.value);
        setStarRating(value === starRating ? 0 : value);
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        fetchData(offset, priceFilter, parking, starRating, country, date, freeTicket)
    }

    const handleAddEventClick = () => {
        navigate('/admin/event/add');
    };

    const saveAsPDF = () => {
        fetch(`${API_BASE_URL}/api/Events/SaveAsPDF?offset=${offset}&limit=${limit}&date=${date}&country=${country}&price=${priceFilter}&parking=${parking}&rating=${starRating}&freeTicket=${freeTicket}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        })
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.blob();
        })
        .then((blob) => {
            const url = window.URL.createObjectURL(blob);
            
            const link = document.createElement('a');
            link.href = url;
            link.download = 'Events.pdf';
            
            link.click();
            
            window.URL.revokeObjectURL(url);
        })
        .catch((error) => {
            console.error("Error during file download:", error);
        });
    };

    if (error) {
        return <Error />;
    }

    return (
        <div id="events-content">
            <ImageBanner image={bannerImage} title={"Find, Choose, Attend!"}></ImageBanner>
            <div id="events-bottom-container">
                {isAdmin ?
                    <div id="add-event-container">
                        <div className="line"></div>
                        <button id="add-event-button" onClick={handleAddEventClick}>ADD EVENT</button>
                        <div className="line"></div>
                    </div>
                    : ""}
                <Container className={styles.container}>
                    <Row className={styles.Row}>
                        <Col xs={12} sm={6} md={3} lg={3} xl={2} className={styles.filterDiv} style={{ width: "250px", maxWidth: "250px" }}>
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
                                        <span className={styles.inputContainer}>
                                            From Lowest <input type="radio" name="price" value="1" onClick={handlePriceFilter} checked={priceFilter === 1} defaultChecked={priceFilter === 1} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            From Highest <input type="radio" name="price" value="2" onClick={handlePriceFilter} checked={priceFilter === 2}  defaultChecked={priceFilter === 2} />
                                        </span>
                                    </div>
                                </div>
                                <hr className={styles.horizontalLine} />
                                <div className={styles.filterOption}>
                                    <label>Popular Filters</label>
                                    <div>
                                        <span className={styles.inputContainer}>
                                            Free Ticket <input type="radio" name="freeTicket" onClick={handleFreeTicketFilter} checked={freeTicket === 1} defaultChecked={freeTicket === 1} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            Parking <input type="radio" name="parking" onClick={handleParkingFilter} checked={parking === 1} defaultChecked={parking === 1} />
                                        </span>
                                    </div>
                                </div>
                                <hr className={styles.horizontalLine} />
                                <div className={styles.filterOption}>
                                    <label>Star Ratings</label>
                                    <div>
                                        <span className={styles.inputContainer}>
                                            5 Stars <input type="radio" name="rating" value="5" onClick={handleRatingFilter} checked={starRating === 5} defaultChecked={starRating === 5} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            4 Stars <input type="radio" name="rating" value="4" onClick={handleRatingFilter} checked={starRating === 4} defaultChecked={starRating === 4} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            3 Stars<input type="radio" name="rating" value="3" onClick={handleRatingFilter} checked={starRating === 3} defaultChecked={starRating === 3} />
                                        </span>
                                    </div>
                                </div>
                            </div>
                            
                        </Col>

                        <Col md={9} className={styles.cardGrid}>
                            {contentLoading ?
                                <Loader height="100%" />
                                :
                                <Row className="g-4" data-testid="event-list">
                                    {data && data.length > 0 ? (
                                        data.map((event, index) => (
                                            <Col md={{ span: 5, offset: 1 }} key={index} className="mb-3" style={{ marginLeft: "60px", width: "380px" }}>
                                                <CustomCard json={event} admin={isAdmin} />
                                            </Col>
                                        ))
                                    ) : (
                                        <div style={{ fontSize: "1.6rem", fontWeight: "600", margin: "50px" }}>No events found</div>
                                    )}
                                </Row>
                            }
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
                        <button className="pdf-button" onClick={saveAsPDF}>Save as PDF</button>
                    </Row>
                </Container>
            </div>

        </div>

    )
}

export default Events;