import React, { useEffect, useState } from "react";
import { useParams, NavLink } from 'react-router-dom';
import styles from "../css/TeamsInCompetition.module.css";
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";
import BackButton from "../components/BackButton";

function TeamsInCompetition() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    
    useEffect(() => {
        fetch(`http://localhost:5260/competitions/${id}/teams`)
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
    }, []);

    if (loading) {
        return <Loader/>;
    }

    if (error) {
        return <Error/>;
    }

    return (

        <div>
            <ImageBanner title={"Select a Team"}></ImageBanner>
            <BackButton />
            <div className={styles.MainContainer}>
                <div className={styles.grid_container}>
                    {data && data.map((competition) => (
                        <NavLink to={`/team/${competition.id}`} key={competition.id} className={styles.teamLink}>
                            <img
                                src={competition.crest}
                                alt={competition.name}
                                className={`${styles.grid_item} ${styles.teamImage}`}
                            />
                        </NavLink>
                    ))}
                </div>
            </div>
        </div>

    );
};

export default TeamsInCompetition;