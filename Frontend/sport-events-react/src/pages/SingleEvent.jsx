import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';
import { Button, Row, Col, Container } from 'react-bootstrap';
import style from "../css/SingleEvent.module.css"
import { useNavigate } from 'react-router-dom';

function SingleEvent() {
    const { id } = useParams();
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isRegistered, setIsRegistered] = useState(false);
    const navigate = useNavigate();  // Use for redirection


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

    // Load the event data when the component mounts
    useEffect(() => {
        fetchData();
        checkAuthentication();
    }, []);


    const checkAuthentication = () => {
        // Check with backend if the user is authenticated
        fetch('https://localhost:7023/api/user/validate', {
            method: 'GET',
            credentials: 'include',  // This will send the HTTP-only cookie with the request
        })
            .then(response => {
                if (response.ok) {
                    setIsAuthenticated(true);
                } else {
                    setIsAuthenticated(false);
                }
            })
            .catch(() => setIsAuthenticated(false));
    };


    // Handle the "I Am Going" button click
    const handleGoingClick = () => {
        if (isAuthenticated) {
            // If the user is authenticated, send a request to the backend
            // urlto ne e implementirano!
            fetch(`https://localhost:7023/api/Events/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',  // Send the token (http-only cookie) along with the request
                body: JSON.stringify({ EventId: id }),
            })
                .then(response => {
                    if (response.ok) {
                        alert('Successfully registered for the event!');
                    } else if (response.status === 403) {
                        // If forbidden, redirect to login
                        navigate('/user/login');
                    } else {
                        alert('There was an error registering for the event. Try again later.');
                    }
                })
                .catch(() => {
                    alert('There was an error registering for the event.');
                });
        } else {
            // If the user is not authenticated, redirect them to the login page
            navigate('/user/login');
        }
    };




    // Helper function to format the date
    const formatDate = (dateString) => {
        const options = { year: 'numeric', month: 'long', day: 'numeric', hours: 'numeric', minutes: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    const resTo = (dateString) => {
        const options = { year: 'numeric', month: 'long', day: 'numeric' };

        // Create a new Date object and subtract 1 day
        let date = new Date(dateString);
        date.setDate(date.getDate() - 1); // Set to the previous day

        // Return the formatted date
        return date.toLocaleDateString(undefined, options);
    }

    const timeLeftToRes = (dateString, closeTime) => {
        const now = new Date(); // Get the current date and time

        // Check if closeTime is provided and append it to the dateString
        let targetDateTime;
        if (closeTime) {
            targetDateTime = new Date(`${dateString.split('T')[0]}T${closeTime}`); // Use only the date part of dateString and append time
        } else {
            targetDateTime = new Date(dateString); // If dateString already has time, use it directly
        }

        // Calculate the difference in milliseconds
        const timeDifference = targetDateTime - now;

        // Check if the target date is in the past
        if (timeDifference <= 0) {
            return "The target date has already passed.";
        }

        // Calculate time left in terms of days, hours, and minutes
        const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
        const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));

        return `${days} days, ${hours} hours, and ${minutes} minutes`;
    };


    // Loading and error handling
    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    // Render the event details
    return (
        <div >
            <Container style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
                <h1 style={{ textAlign: 'center' }}>{data.title}</h1>
                <Row className="mt-5">
                    <Col>

                        <h2>{data.location}</h2>
                        <h3>
                            {data.country}
                        </h3>
                        <br></br>
                        <p>
                            <strong>Address:</strong> {data.address}
                        </p>
                        <p>
                            <strong>Rating:</strong> ⭐ {data.rating}
                        </p>
                        <p>
                            <strong>Date:</strong> {formatDate(data.date)}
                        </p>
                        <p>
                            <strong>Event starts at:</strong> {data.startTime}h
                        </p>
                        <p>
                            <strong>Event closes at:</strong> {data.endTime}h
                        </p>
                        <p>
                            <strong>Gates open at:</strong> {data.gateOpenTime}h
                        </p>
                        <p>
                            <strong>Number of signed-up people:</strong> {data.signedUpPeople}
                        </p>
                        <p>
                            <strong>Capacity:</strong> {data.capacity} - Reservations left for: {data.capacity - data.signedUpPeople} people
                        </p>
                        <p>
                            {data.parking ? (
                                <span style={{ color: 'green' }}>Parking is available</span>
                            ) : (
                                <span style={{ color: 'red' }}>No parking available</span>
                            )}
                        </p>
                        <p>
                            <strong>${data.price}</strong>  / Person
                        </p>
                    </Col>
                    <Col>
                        <div className={style.descriptionDiv}>
                            <h4>Description:</h4>
                            <hr></hr>
                            <p>{data.description}</p>
                        </div>

                        <div style={{ marginTop: '20px' }}>
                            <p>
                                <strong>Reservation closes at:</strong> {data.reservationCloseTime}-  {resTo(data.date)}
                            </p>
                            <p>
                                <strong>Time left to reserve:</strong> {timeLeftToRes(data.date, data.reservationCloseTime)}
                            </p>
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
                        disabled={isRegistered} // Optionally disable the button if already registered
                    >
                        {isRegistered ? "Registered" : "I Am Going"}
                    </Button>
                </div>
            </Container>

        </div>
    );
}

export default SingleEvent;