import React, { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import "../css/AddEventAdmin.css";

const AddEventAdmin = () => {
    const navigate = useNavigate();

    useEffect(() => {
        checkAuthentication();
    }, []);


    const checkAuthentication = () => {
        fetch('https://localhost:7023/api/user/is-admin', {
            method: 'GET',
            credentials: 'include'
        })
            .then(response => {
                if (response.ok) {
                    console.log(response.body)
                    if (!response.body) {
                        navigate('/user/login');
                    }
                } else if (response.status === 401) {

                    navigate('/user/login');
                }
            })
            .catch(() => navigate('/user/login'));
    };

    const [eventData, setEventData] = useState({
        Title: '',
        Country: '',
        Address: '',
        Rating: '',
        Capacity: '',
        Parking: false,
        ImageUrl: '',
        Date: '',
        StartTime: '',
        EndTime: '',
        GateOpenTime: '',
        ReservationCloseTime: '',
        Price: '',
        Label: '',
        Description: '',
    });

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;

        if (type === 'radio') {
            setEventData((prevData) => ({
                ...prevData,
                [name]: value === "true",
            }));
        } else if (type === 'checkbox') {
            setEventData((prevData) => ({
                ...prevData,
                [name]: checked,
            }));
        } else {
            setEventData((prevData) => ({
                ...prevData,
                [name]: value,
            }));
        }
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        const updatedEventData = {
            ...eventData,
            StartTime: eventData.StartTime + ":00",
            EndTime: eventData.EndTime + ":00",
            GateOpenTime: eventData.GateOpenTime + ":00",
            ReservationCloseTime: eventData.ReservationCloseTime + ":00"
        };

        console.log(JSON.stringify(updatedEventData));

        fetch('https://localhost:7023/api/Events/AddEvent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(updatedEventData),
        })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                } else if (response.status === 401) {
                    navigate('/user/login')
                } else {
                    console.log(response.body)
                    throw new Error("Couldn't add new event")
                }
            })
            .then((data) => {
                console.log('Event created successfully:', data);
                navigate(`/events/${data.id}`);
            })
            .catch((error) => {
                console.error('Error:', error);
            });
    };

    const excelImportButtonClick = () => {

    }

    return (
        <div id="add-event-content">
            <div id="add-event-top-container">
                <div>
                    Add Event
                </div>
            </div>
            <div id="add-event-bottom-container">
                <Container>
                    <form onSubmit={handleSubmit} className="form-container">
                        <div className="left-side">
                            <div className="form-group">
                                <label htmlFor="Title">Event Title</label>
                                <input type="text" id="Title" name="Title" value={eventData.Title} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Country">Country</label>
                                <input type="text" id="Country" name="Country" value={eventData.Country} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Address">Address</label>
                                <input type="text" id="Address" name="Address" value={eventData.Address} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Rating">Rating</label>
                                <input type="number" id="Rating" name="Rating" value={eventData.Rating} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Capacity">Capacity</label>
                                <input type="number" id="Capacity" name="Capacity" value={eventData.Capacity} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Price">Price</label>
                                <input type="number" id="Price" name="Price" value={eventData.Price} onChange={handleChange} />
                            </div>
                            <div className="form-group parking-group">
                                <label>Parking</label>
                                <label>
                                    <input type="radio" name="Parking" value="true" checked={eventData.Parking === true} onChange={handleChange} />
                                    Yes
                                </label>
                                <label>
                                    <input type="radio" name="Parking" value="false" checked={eventData.Parking === false} onChange={handleChange} />
                                    No
                                </label>
                            </div>
                            <div className="form-group">
                                <label>Select Image</label>
                                <input type="text" id="ImageUrl" name="ImageUrl" value={eventData.ImageUrl} onChange={handleChange} />
                            </div>
                        </div>

                        <div className="right-side">
                            <div className="form-group">
                                <label htmlFor="Date">Event Date</label>
                                <input type="date" id="Date" name="Date" value={eventData.Date} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="StartTime">Starts at</label>
                                <input type="time" id="StartTime" name="StartTime" value={eventData.StartTime} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="EndTime">Closes at</label>
                                <input type="time" id="EndTime" name="EndTime" value={eventData.EndTime} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="GateOpenTime">Gate open at</label>
                                <input type="time" id="GateOpenTime" name="GateOpenTime" value={eventData.GateOpenTime} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="ReservationCloseTime">Reservation closes at</label>
                                <input type="time" id="ReservationCloseTime" name="ReservationCloseTime" value={eventData.ReservationCloseTime} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Label">Label</label>
                                <input type="text" id="Label" name="Label" value={eventData.Label} onChange={handleChange} />
                            </div>
                            <div className="form-group">
                                <label htmlFor="Description">Description</label>
                                <textarea id="Description" name="Description" value={eventData.Description} onChange={handleChange} />
                            </div>
                            <div className="button-group">
                                <button type="button" onClick={excelImportButtonClick} className="excel-button">Import from Excel</button>
                                <button type="submit" className="add-button">ADD</button>
                            </div>
                        </div>
                    </form>
                </Container>
            </div>


        </div >
    );
};

export default AddEventAdmin;