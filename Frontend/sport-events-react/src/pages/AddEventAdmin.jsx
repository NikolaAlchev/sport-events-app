import React, { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import "../css/AddEventAdmin.css";
import ImageBanner from "../components/ImageBanner";

const AddEventAdmin = () => {
    const navigate = useNavigate();
    const imageUrl = "https://c4.wallpaperflare.com/wallpaper/971/967/737/sports-images-for-desktop-background-wallpaper-preview.jpg";

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

    function appendMillisecondsIfNeeded(time) {
        return time && time.split(":").length === 2 ? time + ":00" : time;
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        const updatedEventData = {
            ...eventData,
            StartTime: appendMillisecondsIfNeeded(eventData.StartTime),
            EndTime: appendMillisecondsIfNeeded(eventData.EndTime),
            GateOpenTime: appendMillisecondsIfNeeded(eventData.GateOpenTime),
            ReservationCloseTime: appendMillisecondsIfNeeded(eventData.ReservationCloseTime)
        };

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

    const handleFileChange = async (e) => {
        const file = e.target.files[0];
        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch('https://localhost:7023/api/Events/UploadExcel', {
                method: 'POST',
                body: formData
            });
            
            if (response.ok) {
                const data = await response.json();
                setEventData({
                    Title: data.title || '',
                    Country: data.country || '',
                    Address: data.address || '',
                    Rating: data.rating || '',
                    Capacity: data.capacity || '',
                    Parking: data.parking || false,
                    ImageUrl: data.imageUrl || '',
                    Date: data.date || '',
                    StartTime: data.startTime || '',
                    EndTime: data.endTime || '',
                    GateOpenTime: data.gateOpenTime || '',
                    ReservationCloseTime: data.reservationCloseTime || '',
                    Price: data.price || '',
                    Label: data.label || '',
                    Description: data.description || '',
                });
            } else {
                console.error("Failed to upload file.");
            }
        } catch (error) {
            console.error("Error uploading file:", error);
        }
    };


    return (
        <div id="add-event-content">
            <ImageBanner image={imageUrl} title={"Add Event"}></ImageBanner>
            <div id="add-event-bottom-container">
                <Container>
                    <form onSubmit={handleSubmit} className="form-container">
                        <div className="left-side">
                            <div className="form-group">
                                <label htmlFor="Title">Event Title</label>
                                <input type="text" id="Title" name="Title" value={eventData.Title} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Country">Country</label>
                                <input type="text" id="Country" name="Country" value={eventData.Country} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Address">Address</label>
                                <input type="text" id="Address" name="Address" value={eventData.Address} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Rating">Rating</label>
                                <input type="number" id="Rating" name="Rating" value={eventData.Rating} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Capacity">Capacity</label>
                                <input type="number" id="Capacity" name="Capacity" value={eventData.Capacity} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Price">Price</label>
                                <input type="number" id="Price" name="Price" value={eventData.Price} onChange={handleChange} required/>
                            </div>
                            <div className="form-group parking-group">
                                <label>Parking</label>
                                <label>
                                    <input type="radio" name="Parking" value="true" checked={eventData.Parking === true} onChange={handleChange} required/>
                                    Yes
                                </label>
                                <label>
                                    <input type="radio" name="Parking" value="false" checked={eventData.Parking === false} onChange={handleChange} required/>
                                    No
                                </label>
                            </div>
                            <div className="form-group">
                                <label>Select Image</label>
                                <input type="text" id="ImageUrl" name="ImageUrl" value={eventData.ImageUrl} onChange={handleChange} required/>
                            </div>
                        </div>

                        <div className="right-side">
                            <div className="form-group">
                                <label htmlFor="Date">Event Date</label>
                                <input type="date" id="Date" name="Date" value={eventData.Date} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="StartTime">Starts at</label>
                                <input type="time" id="StartTime" name="StartTime" value={eventData.StartTime} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="EndTime">Closes at</label>
                                <input type="time" id="EndTime" name="EndTime" value={eventData.EndTime} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="GateOpenTime">Gate open at</label>
                                <input type="time" id="GateOpenTime" name="GateOpenTime" value={eventData.GateOpenTime} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="ReservationCloseTime">Reservation closes at</label>
                                <input type="time" id="ReservationCloseTime" name="ReservationCloseTime" value={eventData.ReservationCloseTime} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Label">Label</label>
                                <input type="text" id="Label" name="Label" value={eventData.Label} onChange={handleChange} required/>
                            </div>
                            <div className="form-group">
                                <label htmlFor="Description">Description</label>
                                <textarea id="Description" name="Description" value={eventData.Description} onChange={handleChange} required/>
                            </div>
                            <div className="button-group">
                                <button type="button" className="excel-button" onClick={() => document.getElementById('file-upload').click()}>Import from Excel</button>
                                <button type="submit" className="add-button">ADD</button>
                                <input type="file" id="file-upload" style={{ display: 'none' }} accept=".xlsx, .xls" onChange={handleFileChange}/>
                            </div>
                        </div>
                    </form>
                </Container>
            </div>


        </div >
    );
};

export default AddEventAdmin;