import React, { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const AddEventAdmin = () => {
    const navigate = useNavigate();

    useEffect(() => {
        checkAuthentication();
    }, []);


    const checkAuthentication = () => {
        // Check with backend if the user is authenticated
        fetch('https://localhost:7023/api/user/AddEvent', {
            method: 'GET',
            credentials: 'include',  // This will send the HTTP-only cookie with the request
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
        console.log(name)
        console.log(value)
        console.log(type)
        console.log(checked)
        // Update state based on the type of input
        if (type === 'radio') {
            console.log("here")
            setEventData((prevData) => ({
                ...prevData,
                [name]: value === "true", // Convert string to boolean
            }));
        } else if (type === 'checkbox') {
            setEventData((prevData) => ({
                ...prevData,
                [name]: checked,  // For checkboxes, checked is used to get boolean
            }));
        } else {
            setEventData((prevData) => ({
                ...prevData,
                [name]: value,  // For text, number, date, etc.
            }));
        }
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log(e)
        console.log(eventData)
        fetch('https://localhost:7023/api/Events/AddEvent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(eventData),
        })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                } else if (response.status === 401) {

                    navigate('/user/login')
                } else {
                    console.log(response.body)
                    throw new Error("could'nt add new event")
                }
            })
            .then((data) => {
                console.log('Event created successfully:', data);
                navigate(`/events/${data.id}`); // Navigate to the event's detail page or show a success message
            })
            .catch((error) => {
                console.error('Error:', error);
            });
    };

    const excelImportButtonClick = () => {

    }

    return (
        <Container>
            <form onSubmit={handleSubmit}>
                <div className='row'>
                    <div className='col-md-6'>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Title">Title:</label>
                            <br />
                            <input className='mx-3'
                                type="text"
                                id="Title"
                                name="Title"
                                value={eventData.Title}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Country">Country:</label>
                            <br />
                            <input className='mx-3'
                                type="text"
                                id="Country"
                                name="Country"
                                value={eventData.Country}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Address">Address:</label>
                            <br />
                            <input className='mx-3'
                                type="text"
                                id="Address"
                                name="Address"
                                value={eventData.Address}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Rating">Rating:</label>

                            <input className='mx-3'
                                type="number"
                                id="Rating"
                                name="Rating"
                                value={eventData.Rating}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Capacity">Capacity:</label>
                            <input className='mx-3'
                                type="number"
                                id="Capacity"
                                name="Capacity"
                                value={eventData.Capacity}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <label className='mx-3' htmlFor="Parking">Parking:</label>
                            <div className='mt-3'>
                                <label className='mx-3'>
                                    <input className='mx-3'
                                        type="radio"
                                        id="parkingYes"
                                        name="Parking"
                                        value="true"
                                        checked={eventData.Parking === true}
                                        onChange={handleChange}
                                    />
                                    Yes
                                </label>
                                <label className='mx-3' >
                                    <input className='mx-3'
                                        type="radio"
                                        id="parkingNo"
                                        name="Parking"
                                        value="false"
                                        checked={eventData.Parking === false}
                                        onChange={handleChange}
                                    />
                                    No
                                </label>
                            </div>
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="ImageUrl">Image URL:</label>
                            <input className='mx-3'
                                type="text"
                                id="ImageUrl"
                                name="ImageUrl"
                                value={eventData.ImageUrl}
                                onChange={handleChange}
                            />
                        </div>
                    </div>
                    <div className='col-md-6'>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Date">Date:</label>
                            <input className='mx-3'
                                type="date"
                                id="Date"
                                name="Date"
                                value={eventData.Date}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="StartTime">Start Time:</label>
                            <input className='mx-3'
                                type="time"
                                id="StartTime"
                                name="StartTime"
                                value={eventData.StartTime}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="EndTime">End Time:</label>
                            <input className='mx-3'
                                type="time"
                                id="EndTime"
                                name="EndTime"
                                value={eventData.EndTime}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="GateOpenTime">Gate Open Time:</label>
                            <input className='mx-3'
                                type="time"
                                id="GateOpenTime"
                                name="GateOpenTime"
                                value={eventData.GateOpenTime}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="ReservationCloseTime">Reservation Close Time:</label>
                            <input className='mx-3'
                                type="time"
                                id="ReservationCloseTime"
                                name="ReservationCloseTime"
                                value={eventData.ReservationCloseTime}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Price">Price:</label>
                            <input className='mx-3'
                                type="number"
                                id="Price"
                                name="Price"
                                value={eventData.Price}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Label">Label:</label>
                            <input className='mx-3'
                                type="text"
                                id="Label"
                                name="Label"
                                value={eventData.Label}
                                onChange={handleChange}
                            />
                        </div>
                        <div className='mt-3'>
                            <label className='mx-3' htmlFor="Description">Description:</label>
                            <textarea className='mx-3'
                                id="Description"
                                name="Description"
                                value={eventData.Description}
                                onChange={handleChange}
                            />
                        </div>
                    </div>
                    <button className='mt-3' type="submit"> ADD</button>
                    <button style={{ backgroundColor: '#A8D9AD' }} className='mt-3' onClick={excelImportButtonClick}> Import from excel </button>
                </div>
            </form>


        </Container>
    );
};

export default AddEventAdmin;