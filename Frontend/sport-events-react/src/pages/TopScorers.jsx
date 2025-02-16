import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';
import Loader from "../components/Loader";
import Error from "../components/Error";
import ImageBanner from "../components/ImageBanner";
import styles from "../css/PlayerCard.module.css";
import BackButton from "../components/BackButton";
import Flag from "react-world-flags";
import countryToISO from "../countryToISO";
import { Container } from 'react-bootstrap';

function TopScorers() {
    const API_BASE_URL = process.env.REACT_APP_MATCHES_API_BASE_URL;

    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();

    useEffect(() => {
        fetch(`${API_BASE_URL}/player/${id}/topScorers`)
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
            <Container className={styles.outerContainer}>
            <div className={styles.players_list}>
                {data && data.map((player, index) => (
                    <div className={styles.player_card} key={player.id} data-testid="player-card">
                        <div className={styles.rank}>{index + 1}</div>
                        <img 
                            src={player.currentTeamCrest} 
                            alt={`${player.currentTeamName} crest`} 
                            className={styles.team_crest}/>
                        <h3 className={styles.playerName} data-testid="player-name">{player.firstName} {player.lastName}</h3>
                        <Flag code={countryToISO[player.nationality]} className={styles.Nationality} />
                        <p className={styles.playerStats} data-testid="player-stats">Goals: {player.goals}</p>
                        <p className={styles.playerStats}>Assists: {player.assists}</p>
                        <p className={styles.playerStats}>Penalties: {player.penalties}</p>
                        <p className={styles.playerStats}>Team: {player.currentTeamName}</p>
                    </div>
                ))}
            </div>
            </Container>
        </div>
    );
};

export default TopScorers;