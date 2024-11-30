import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';
import { Button, Row, Col, Container } from 'react-bootstrap';
import style from "../css/SingleEvent.module.css";
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLocationDot, faCircleChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { faStar } from '@fortawesome/free-regular-svg-icons';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";
import BackButton from "../components/BackButton";

function SingleEvent() {
    const { id } = useParams();
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [isRegistered, setIsRegistered] = useState(false);
    const navigate = useNavigate();


    const fetchData = () => {
        setLoading(true);

        fetch(`https://localhost:7023/api/Events/GetEvent/${id}`)

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
        fetchData();
        checkRegistered();
    }, []);

    const checkRegistered = () => {
        fetch(`https://localhost:7023/api/Events/register/check?eventId=${id}`, {
            method: 'GET',
            credentials: 'include',
        })
            .then(response => {
                if (response.ok) {

                    return response.json()
                } else {
                    throw new Error("Failed to check registration status");
                }
            }).then(data => {

                console.log("Registration status:", data.isRegistered);
                setIsRegistered(data.isRegistered === "true");
            })
            .catch(error => {
                console.error("Error fetching registration status:", error);
                setIsRegistered(false);
            });
    };

    const handleGoingClick = () => {

        fetch(`https://localhost:7023/api/Events/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({ EventId: id }),
        })
            .then(response => {
                if (response.ok) {
                    alert('Successfully registered for the event!');
                    setIsRegistered(true);
                } else if (response.status === 401) {
                    navigate('/user/login');
                } else if (response.status === 405) {
                    alert("Cannot reserve a seat. The event has already ended.")
                } else if (response.status === 406) {
                    alert("Can't Reserve more then 1 seat");
                } else if (response.status === 407) {
                    alert("Can't reserve seat. The event is booked");
                }
            })
            .catch(() => {
                alert('There was an error registering for the event.');
            });

    };

    const formatDate = (dateString) => {
        const options = { year: 'numeric', month: 'long', day: 'numeric', hours: 'numeric', minutes: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    const resTo = (dateString) => {
        const options = { year: 'numeric', month: 'long', day: 'numeric' };

        let date = new Date(dateString);
        date.setDate(date.getDate() - 1);

        return date.toLocaleDateString(undefined, options);
    }

    const timeLeftToRes = (dateString, closeTime) => {
        const now = new Date();
        let targetDateTime;
        if (closeTime) {
            targetDateTime = new Date(`${dateString.split('T')[0]}T${closeTime}`);
        } else {
            targetDateTime = new Date(dateString);
        }

        const timeDifference = targetDateTime - now;

        if (timeDifference <= 0) {
            return "The target date has already passed.";
        }

        const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
        const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));

        return `${days} days, ${hours} hours, and ${minutes} minutes`;
    };


    if (loading) {
        return <Loader/>;
    }

    if (error) {
        return <Error/>;
    }

    return (
        <div className={style.singleEventContent}>
            <ImageBanner image={data.imageUrl} title={data.title}></ImageBanner>
            <BackButton />
            <div className={style.singleEventBottomContainer}>
                <Container style={{ padding: '20px', maxWidth: '900px', margin: '0 auto' }}>
                    <h1 style={{ textAlign: 'center' }}>{data.title}</h1>
                    <Row className="mt-5" style={{ columnGap: "60px" }}>
                        <Col>

                            <h2>{data.location}</h2>
                            <p style={{ fontSize: "1.8rem" }}>
                                {data.country}
                            </p>
                            <p style={{ fontSize: "1.4rem" }}>
                                <FontAwesomeIcon icon={faLocationDot} /> {data.address}
                            </p>
                            <p style={{ fontSize: "1.4rem" }}>
                                <FontAwesomeIcon icon={faStar} /> {data.rating}
                            </p>
                            <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                <p style={{ fontSize: "1.1rem" }}>Date:</p>
                                <p style={{ fontSize: "1.2rem" }}>{formatDate(data.date)}</p>
                            </div>
                            <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                <p style={{ fontSize: "1.1rem" }}>Event starts at:</p>
                                <p style={{ fontSize: "1.2rem" }}>{data.startTime}h</p>
                            </div>
                            <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                <p style={{ fontSize: "1.1rem" }}>Event closes at:</p>
                                <p style={{ fontSize: "1.2rem" }}>{data.endTime}h</p>
                            </div>
                            <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                <p style={{ fontSize: "1.1rem" }}>Gates open at:</p>
                                <p style={{ fontSize: "1.2rem" }}>{data.gateOpenTime}h</p>
                            </div>
                            <p style={{ fontSize: "1.1rem" }}>Number of signed-up people:</p>
                            <p style={{ fontSize: "1.2rem" }}>{data.signedUpPeople}</p>
                            <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                <div style={{ display: "flex", flexDirection: "column", justifyContent: "space-evenly", alignContent: "start" }}>
                                    <p style={{ fontSize: "1.1rem" }}>Capacity:</p>
                                    <p style={{ fontSize: "1.2rem" }}>{data.capacity}</p>
                                </div>

                                <div style={{ display: "flex", flexDirection: "column", justifyContent: "space-evenly", alignContent: "start" }}>
                                    <p style={{ fontSize: "1.1rem" }}>Reservations left for:</p>
                                    <p style={{ fontSize: "1.2rem" }}>{data.capacity - data.signedUpPeople} people</p>
                                </div>
                            </div>

                            <p style={{ fontSize: "1.1rem" }}>
                                {data.parking ? (
                                    <span style={{ color: 'green' }}>Parking is available</span>
                                ) : (
                                    <span style={{ color: 'red' }}>No parking available</span>
                                )}
                            </p>
                            <p>
                                <strong style={{ fontSize: "1.2rem" }}>${data.price}</strong><span style={{ fontSize: "1.1rem", color: "#505050" }}> / Person</span>
                            </p>
                        </Col>
                        <Col>
                            <div className={style.descriptionDiv}>
                                <p style={{ fontSize: "1.2rem" }}>Description:</p>
                                <hr style={{ width: "60%" }}></hr>
                                <p>{data.description}</p>
                            </div>

                            <div style={{ marginTop: '20px' }}>
                                <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                    <p>Reservation closes at</p> <p>{data.reservationCloseTime} - {resTo(data.date)}</p>
                                </div>
                                <div style={{ display: "flex", justifyContent: "space-between", alignContent: "center" }}>
                                    <p>Time left to reserve:</p> <p>{timeLeftToRes(data.date, data.reservationCloseTime)}</p>

                                </div>

                            </div>

                            <div style={{ marginTop: '20px', fontSize: '12px', color: 'gray' }}>
                                <p>Event Published: {formatDate(data.publishedDate)} at {data.publishedTime}</p>
                            </div>
                        </Col>
                    </Row>
                    <div style={{ marginTop: '20px', textAlign: 'center' }} >
                        <Button
                            variant={isRegistered ? "yes" : "no"}
                            onClick={handleGoingClick}
                            disabled={isRegistered}
                            style={{ backgroundColor: "#3D3D3D", color: "white", padding: "5px 25px" }}
                        >
                            {isRegistered ? "Registered" : "I Am Going"}
                        </Button>
                    </div>
                </Container>
            </div>

        </div>
    );
}

export default SingleEvent;