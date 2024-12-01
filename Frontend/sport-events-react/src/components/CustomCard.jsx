import React from 'react';
import { Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import '../css/CustomCard.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faLocationDot, faPenToSquare } from '@fortawesome/free-solid-svg-icons'
import { faStar } from '@fortawesome/free-regular-svg-icons'

const CustomCard = ({ json, admin = false }) => {
    return (
        <Link to={`/events/${json.id}`} style={{ textDecoration: 'none' }}>
            <Card className="event-card mx-auto mb-4">

                <Card.Img variant="top" src={json.imageUrl} alt="Event Image" className="event-image" />

                <Card.Body>
                    <Card.Title className="text-truncate event-title">{json.title}</Card.Title>

                    <Card.Text className="event-address text-muted">
                        <FontAwesomeIcon icon={faLocationDot} /> {json.address}, <br /> {json.country}
                    </Card.Text>

                    <Card.Text className="event-rating">
                        <FontAwesomeIcon icon={faStar} /> {json.rating}
                    </Card.Text>

                    <Card.Text className="event-price">
                        {
                            json.price === 0 ?
                                <div>
                                    <strong>Free</strong> <span style={{ color: "#505050", fontSize: "1.2rem" }}> entrance</span>
                                </div>

                                :
                                <div>
                                    <strong>${json.price}</strong> <span style={{ color: "#505050", fontSize: "1.2rem" }}>/ Person</span>
                                </div>
                        }
                    </Card.Text>
                </Card.Body>
                {admin &&

                    <Link to={`/admin/event/add`} state={{ initialData: json }} className='event-edit-icon'>
                        <FontAwesomeIcon icon={faPenToSquare} />
                    </Link>
                }

            </Card>
        </Link>
    );
};

export default CustomCard;
