import React from 'react'
import { Card, Button, Row, Col } from 'react-bootstrap';
import { Link } from 'react-router-dom';
const CustomCard = ({ json }) => {
    console.log(json)
    return (



        <Card style={{ width: '18rem' }} className="mx-auto">
            <Link to={`/events/${json.id}`}>
                <Card.Img variant="top" src={json.imageUrl} alt="Event Image" />
            </Link>
            <Card.Body>
                <Card.Title>{json.title}</Card.Title>
                <Card.Text>
                    <p>{json.address}</p>
                    <p>{json.rating}</p>
                    <p>{json.price}</p>
                </Card.Text>
            </Card.Body>
        </Card>



    );
};

export default CustomCard;