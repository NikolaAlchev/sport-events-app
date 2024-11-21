import React, { useEffect, useState } from "react";
import { useParams, NavLink } from 'react-router-dom';
import styles from "../css/TeamsInCompetition.module.css";

//http://localhost:3000/competitions/2013

function TeamsInCompetition() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();

    useEffect(() => {

        // Make GET request
        fetch(`http://localhost:5260/competitions/${id}/teams`)
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
             {data && data.map((competition) => (
                 <NavLink to={`/competitions/${competition.id}/teams`} key={competition.id}>
                     <img 
                         src={competition.crest} 
                         alt={competition.name} 
                         className={`${styles.grid_item} ${styles.competitionImage}`} 
                     />
                 </NavLink>
             ))}
         </div>
     </div>
    );
};

export default TeamsInCompetition;