import React, { useEffect, useState } from "react";
import styles from "../css/Competitions.module.css";
import { NavLink } from 'react-router-dom';

//http://localhost:3000/competitions

function Competitions() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {

        // Make GET request
        fetch(`http://localhost:5260/competitions/all`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                // console.log(response.json())
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
    }, []); // Empty dependency array to ensure it runs once after the component mounts

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    return (
        <div className={styles.MainContainer}>
            <div className={styles.grid_container}>
                {data && data.map((competition, index) => (
                    <NavLink to={`/competitions/${competition.id}`} key={competition.id}>
                        <img 
                            key={index} 
                            src={competition.emblem} 
                            alt={competition.name} 
                            className={`${styles.grid_item} ${styles.competitionImage}`} 
                        />
                    </NavLink>
                ))}
            </div>
        </div>
    );
};

export default Competitions;