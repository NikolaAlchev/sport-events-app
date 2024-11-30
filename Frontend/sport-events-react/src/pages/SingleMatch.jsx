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
import { Scrollbar } from 'react-scrollbars-custom';
import TeamLeague from "../components/TeamLeague";
import Flag from "react-world-flags";
import countryToISO from "../countryToISO";

function SingleMatch() {
    const [data, setData] = useState(null);
    const [standings, setStandings] = useState(null);
    const [head2head, setHead2Head] = useState(null);
    const [leagues, setLeagues] = useState(null);
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
                console.log(data)
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
        fetch(`http://localhost:5260/matches/standings?homeTeam=${data.homeTeamId}&awayTeam=${data.awayTeamId}`)
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

    const getHead2HeadData = async () => {
        fetch(`http://localhost:5260/matches/head2head?id=${id}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setHead2Head(data);
                const temp = data.reduce((acc, match) => {
                    const { competitionName } = match;
                    if (!acc[competitionName]) {
                        acc[competitionName] = [];
                    }
                    acc[competitionName].push(match);
                    return acc;
                }, {});
                setLeagues(temp);
                console.log(temp);
                setContentLoading(false);
            })
            .catch((error) => {
                setError(error);
            });
    }



    const buttonPosition = {
        details: 0,
        head2head: 33.33,
        standings: 67
    }[activeButton];

    const timeOnly = new Date(data.utcDate).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });

    return (
        <div>
            <ImageBanner title={data.homeTeamName + " vs " + data.awayTeamName}></ImageBanner>
            <BackButton />
            <Container>
                <div className={styles.upperContent}>
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
                </div>
                <div className={styles.ButtonContainer}>
                    <div className={styles.ButtonBackground}>
                        <div className={`${styles.Button} `} style={{ left: `${buttonPosition}%` }}></div>
                        <div className={`${styles.Text} ${activeButton === "details" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("details")}>Details</div>
                        <div className={`${styles.Text} ${activeButton === "head2head" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => {
                                handleButtonClick("head2head");
                                getHead2HeadData();
                            }}>Head2Head</div>
                        <div className={`${styles.Text} ${activeButton === "standings" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => {
                                handleButtonClick("standings");
                                getStandingsData();
                            }}>Standings</div>
                    </div>
                </div>
                <div className={styles.outerContainer}>
                    <div className={styles.Content}>
                        {activeButton === "details" &&

                            <div style={{ width: "100%" }}>
                                <div style={{ display: "flex", alignItems: "start" }}>
                                    <div style={{ flex: "1", marginRight: "100px" }}>
                                        <div style={{ display: "flex", justifyContent: "start", alignItems: "end" }}>
                                            <img className={styles.icon} src="https://cdn-icons-png.flaticon.com/512/6409/6409873.png" alt="" />
                                            <div style={{ fontSize: "1.4rem" }}>Referees</div>
                                        </div>
                                        <hr />
                                        <div>
                                            {data.referees && data.referees.length > 0 ? (
                                                data.referees.map((referee, index) => (
                                                    <div style={{ display: "flex", justifyContent: "start", alignItems: "center" }}>
                                                        <Flag code={countryToISO[referee.nationality]} className={styles.Nationality} />
                                                        <div key={index}>{referee.name}</div>
                                                    </div>
                                                ))
                                            ) : (
                                                <p>Referees not listed</p>
                                            )}
                                        </div>

                                    </div>

                                    <div style={{ flex: "1" }}>
                                        <div style={{ display: "flex", justifyContent: "end", alignItems: "end" }}>
                                            <div style={{ fontSize: "1.4rem" }}>Venue</div>
                                            <img className={styles.icon} src="https://cdn-icons-png.flaticon.com/512/4905/4905563.png" alt="" />
                                        </div>
                                        <hr />
                                        <p style={{ textAlign: "end" }}>{data.venue ? data.venue : "No information"}</p>
                                    </div>
                                </div>
                            </div>

                        }
                        {activeButton === "head2head" ? (
                            contentLoading ? (
                                <Loader height="100px" />
                            ) : (
                                <Scrollbar style={{ width: "100%", height: 600 }}>
                                    {Object.entries(leagues).map(([leagueName, matches]) => (
                                        <TeamLeague key={leagueName} name={leagueName} matches={matches} emblem={matches[0].leagueEmblem} />
                                    ))}
                                </Scrollbar>
                            )
                        ) : null}
                        {activeButton === "standings" ? (
                            contentLoading ? (
                                <Loader height="100px" />
                            ) : (
                                <Scrollbar style={{ width: "100%", height: 400 }}>
                                    <Standings data={standings[0]} />
                                </Scrollbar>
                            )
                        ) : null}
                    </div>
                </div>
            </Container >
        </div >

    );
};

export default SingleMatch;