import CustomCard from "../components/CustomCard";
import React, { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';
import styles from "../css/FilterDiv.module.css"
import "../css/Events.css";
import { Row, Col, Container, Button, Form } from 'react-bootstrap';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";

function Events() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
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
    const imageUrl = "https://s3-alpha-sig.figma.com/img/8e87/f396/5bb6cdd58fdccad48a4414e14405c76c?Expires=1731283200&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=eRVakKFgZIPVcp30mNhANzbJy0-078oqPBX1ybO1wlZ94CmEy9BJYYEpUB7Gb7R54gZseZPr9somaxxkOzCzJGPv5csokoktnGwa5mNPLhQ-WOXuzxsJzoIHKFrDYHx9h~e5apzM7doGc3nGBwdSFwlduEC1GMg4J8WyIlsSGhS6i~8cHyvdwmewB8Csa1AgCcsRBOB9q5wkVBR1FtgSYotbxjnKX9m-8q7KS29OFpzeChEKej-ILHVc4XUFfBvV8KLWftM7stcTv4TgF~NRGlaokdPvM4eKBztitygxw7lxzSyUYwD6~QoHZL2SscckJlr~FJhdkSupdiaTm-Ezsw__";

    const navigate = useNavigate();

    function getTodayDate() {
        const today = new Date();
        return today.toISOString().split('T')[0];
    }

    const fetchData = (offset, price, parking, rating, country, date, freeTicket) => {
        setLoading(true);
        fetch(`https://localhost:7023/api/Events?offset=${offset}&limit=${limit}&date=${date}&country=${country}&price=${price}&parking=${parking}&rating=${rating}&freeTicket=${freeTicket}`)

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
        fetchData(offset, priceFilter, parking, starRating, country, date, freeTicket);
        // eslint-disable-next-line
    }, [offset, priceFilter, parking, starRating, date, freeTicket]);

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


    if (loading) {
        return <Loader/>;
    }

    if (error) {
        return <Error/>;
    }

    return (
        <div id="events-content">
            <ImageBanner image={imageUrl}></ImageBanner>
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
                                        <span className={styles.inputContainer}>
                                            From Lowest <input type="radio" name="price" value="1" onClick={handlePriceFilter} defaultChecked={priceFilter === 1} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            From Highest <input type="radio" name="price" value="2" onClick={handlePriceFilter} defaultChecked={priceFilter === 2} />
                                        </span>
                                    </div>
                                </div>
                                <hr className={styles.horizontalLine}/>
                                <div className={styles.filterOption}>
                                    <label>Popular Filters</label>
                                    <div>
                                        <span className={styles.inputContainer}>
                                            Free Ticket <input type="radio" name="freeTicket" onClick={handleFreeTicketFilter} defaultChecked={freeTicket === 1} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            Parking <input type="radio" name="parking" onClick={handleParkingFilter} defaultChecked={parking === 1} />
                                        </span>
                                    </div>
                                </div>
                                <hr className={styles.horizontalLine}/>
                                <div className={styles.filterOption}>
                                    <label>Star Ratings</label>
                                    <div>
                                        <span className={styles.inputContainer}>
                                            5 Stars <input type="radio" name="rating" value="5" onClick={handleRatingFilter} defaultChecked={starRating === 5} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            4 Stars <input type="radio" name="rating" value="4" onClick={handleRatingFilter} defaultChecked={starRating === 4} />
                                        </span>
                                        <span className={styles.inputContainer}>
                                            3 Stars<input type="radio" name="rating" value="3" onClick={handleRatingFilter} defaultChecked={starRating === 3} />
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </Col>

                        <Col md={9} className={styles.cardGrid}>
                            <Row className="g-4">
                                {data && data.length > 0 ? (
                                    data.map((event, index) => (
                                        <Col md={{ span: 5, offset: 1 }} key={index} className="mb-3">
                                            <CustomCard json={event} admin={isAdmin}/>
                                        </Col>
                                    ))
                                ) : (
                                    <div>No events found</div>
                                )}
                            </Row>


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