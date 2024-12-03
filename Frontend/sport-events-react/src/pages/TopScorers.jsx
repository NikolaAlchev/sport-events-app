import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from 'react-router-dom';
import Loader from "../components/Loader";
import Error from "../components/Error";
import ImageBanner from "../components/ImageBanner";
import styles from "../css/PlayerCard.module.css";
import BackButton from "../components/BackButton";
import Flag from "react-world-flags";
import countryToISO from "../countryToISO";

// http://localhost:3000/player/1077

function TopScorers() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();

    useEffect(() => {
        fetch(`http://localhost:5260/player/${id}/topScorers`)
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
            <ImageBanner title={"Top Scorers"}></ImageBanner>
            <BackButton />
            <div className={styles.players_list}>
                {data && data.map((player, index) => (
                    <div className={styles.player_card} key={player.id}>
                        <div className={styles.rank}>{index + 1}</div>
                        <img 
                            src={player.currentTeamCrest} 
                            alt={`${player.currentTeamName} crest`} 
                            className={styles.team_crest}/>
                        <h3>{player.firstName} {player.lastName}</h3>
                        <Flag code={countryToISO[player.nationality]} className={styles.Nationality} />
                        <p>Goals: {player.goals}</p>
                        <p>Assists: {player.assists}</p>
                        <p>Penalties: {player.penalties}</p>
                        <p>Team: {player.currentTeamName}</p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default TopScorers;