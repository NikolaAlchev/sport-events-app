import React, { useEffect, useState } from "react";
import styles from "../css/Competitions.module.css";
import { NavLink } from 'react-router-dom';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";

function TopScorerLeague() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
 
    useEffect(() => {
        fetch(`http://localhost:5260/competitions/all`)
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
            <ImageBanner title={"Select a League"}></ImageBanner>
            <div className={styles.MainContainer}>
                <div className={styles.grid_container}>
                    {data && data.map((competition, index) => (
                        <NavLink to={`/player/${competition.id}`} key={competition.id} className={styles.leagueLink}>
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
        </div>

    );
};

export default TopScorerLeague;