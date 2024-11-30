import React, { useEffect, useState } from "react";
import styles from "../css/SingleMatch.module.css";
import { useParams } from 'react-router-dom';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";
import { Container } from 'react-bootstrap';
import BackButton from "../components/BackButton";
import { NavLink } from 'react-router-dom';
import Standings from "../components/Standings";

function SingleMatch() {
    const [data, setData] = useState(null);
    const [standings, setStandings] = useState(null);
    const [loading, setLoading] = useState(true);
    const [contentLoading, setContentLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    const [activeButton, setActiveButton] = useState("details");

    useEffect(() => {

        fetch(`http://localhost:5260/matches/${id}`)
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
        return <Loader />;
    }

    if (error) {
        return <Error />;
    }

    const handleButtonClick = (buttonNumber) => {
        setActiveButton(buttonNumber);
        setContentLoading(true);
    };

    const getStandingsData = async () => {
        fetch(`http://localhost:5260/matches/team/standings?homeTeam=${data.homeTeamId}&awayTeam=${data.awayTeamId}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setStandings(data);
                setContentLoading(false);
            })
            .catch((error) => {
                setError(error);
            });
    }

    const buttonPosition = {
        details: 0,
        lineups: 33.33,
        standings: 67
    }[activeButton];

    const timeOnly = new Date(data.utcDate).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });

    return (
        <div>
            <ImageBanner title={data.homeTeamName + " vs " + data.awayTeamName}></ImageBanner>
            <BackButton />
            <Container className={styles.MainContainer}>
                <div className={styles.MatchRow}>
                    <NavLink to={`/team/${data.homeTeamId}`} className={styles.TeamCrestContainer}>
                        <img src={data.homeTeamCrest} alt={`${data.homeTeamCrest} crest`} className={styles.TeamPhoto} />
                        <div className={styles.TeamName}>{data.homeTeamName}</div>
                    </NavLink>

                    <div className={styles.score}>{data.status === "TIMED" || data.status === "SCHEDULED" ? timeOnly : `${data.homeTeamScore} : ${data.awayTeamScore}`}</div>

                    <NavLink to={`/team/${data.awayTeamId}`} className={styles.TeamCrestContainer}>
                        <div className={styles.TeamName}>{data.awayTeamName}</div>
                        <img src={data.awayTeamCrest} alt={`${data.awayTeamCrest} crest`} className={styles.TeamPhoto} />
                    </NavLink>
                </div>
                <div className={styles.ButtonContainer}>
                    <div className={styles.ButtonBackground}>
                        <div className={`${styles.Button} `} style={{ left: `${buttonPosition}%` }}></div>
                        <div className={`${styles.Text} ${activeButton === "details" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("details")}>Details</div>
                        <div className={`${styles.Text} ${activeButton === "lineups" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("lineups")}>Lineups</div>
                        <div className={`${styles.Text} ${activeButton === "standings" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => {
                                handleButtonClick("standings");
                                getStandingsData();
                            }}>Standings</div>
                    </div>
                </div>
                <div className={styles.Content}>
                    {activeButton === "details" && <div>Details is pressed</div>}
                    {activeButton === "lineups" && <div>Line Ups is pressed</div>}
                    {activeButton === "standings" ? (
                        contentLoading ? (
                            <Loader height="100px" />
                        ) : (
                            <Standings data={standings[0]}/>
                        )
                    ) : null}
                </div>
            </Container>
        </div>

    );
};

export default SingleMatch;