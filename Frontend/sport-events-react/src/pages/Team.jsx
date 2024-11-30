import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from 'react-router-dom';
import Loader from "../components/Loader";
import Error from "../components/Error";
import styles from "../css/Team.module.css";
import ImageBanner from "../components/ImageBanner";
import { Container } from 'react-bootstrap';
import { Scrollbar } from 'react-scrollbars-custom';
import TeamLeague from "../components/TeamLeague";
import TeamPosition from "../components/TeamPosition";
import BackButton from "../components/BackButton";

function Team() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    const [activeButton, setActiveButton] = useState("profile");
    
    useEffect(() => {
        fetch(`http://localhost:5260/team/${id}`)
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

    const handleButtonClick = (buttonNumber) => {
        setActiveButton(buttonNumber);
    };

    const buttonPosition = {
        profile: 0,
        schedule: 33.33,
        players: 67
    }[activeButton];

    if (loading) {
        return <Loader />;
    }

    if (error) {
        return <Error />;
    }

    const leaguesPast = data.pastMatches.reduce((acc, match) => {
        const { competitionName } = match;
        if (!acc[competitionName]) {
            acc[competitionName] = [];
        }
        acc[competitionName].push(match);
        return acc;
    }, {});

    const leaguesFuture = data.futureMatches.reduce((acc, match) => {
        const { competitionName } = match;
        if (!acc[competitionName]) {
            acc[competitionName] = [];
        }
        acc[competitionName].push(match);
        return acc;
    }, {});

    const positionCategories = {
        Goalkeeper: 'Goalkeepers',
        'Centre-Back': 'Defenders',
        'Left-Back': 'Defenders',
        'Right-Back': 'Defenders',
        'Defensive Midfield': 'Midfielders',
        'Central Midfield': 'Midfielders',
        'Attacking Midfield': 'Midfielders',
        'Left Winger': 'Forwards',
        'Right Winger': 'Forwards',
        'Centre-Forward': 'Forwards',
        Coach: 'Coach'
    };

    const categoryOrder = ["Goalkeepers", "Defenders", "Midfielders", "Forwards", "Coach"];

    const categorizedSquad = data.squad.reduce((acc, player) => {
        const broadCategory = positionCategories[player.position] || 'Others';
        if (!acc[broadCategory]) {
            acc[broadCategory] = [];
        }
        acc[broadCategory].push(player);
        return acc;
    }, {});

    return (
        <div>
            <ImageBanner title={data.name}></ImageBanner>
            <BackButton />
            <Container>
                <div className={styles.upperContent}>
                    <div className={styles.teamCrestContainer}>
                        <img className={styles.teamCrest} src={data.crest} alt="Team Crest" />
                        <h1 style={{ marginLeft: "20px" }}>{data.name}</h1>
                    </div>
                    <div>
                        <p>{data.venue}</p>
                    </div>
                </div>

                <div className={styles.ButtonContainer}>
                    <div className={styles.ButtonBackground}>
                        <div className={`${styles.Button} `} style={{ left: `${buttonPosition}%` }}></div>
                        <div className={`${styles.Text} ${activeButton === "profile" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("profile")}>Profile</div>
                        <div className={`${styles.Text} ${activeButton === "schedule" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("schedule")}>Schedule</div>
                        <div className={`${styles.Text} ${activeButton === "players" ? styles.ActiveText : styles.NotActiveText}`}
                            onClick={() => handleButtonClick("players")}>Players</div>
                    </div>
                </div>
                <div className={styles.outerContainer}>
                    <div className={styles.Content}>
                        {activeButton === "profile" &&

                            <Scrollbar style={{ width: "100%", height: 600 }}>
                                {Object.entries(leaguesPast).map(([leagueName, matches]) => (
                                    <TeamLeague key={leagueName} name={leagueName} matches={[...matches].reverse()} emblem={matches[0].leagueEmblem} />
                                ))}
                            </Scrollbar>
                        }

                        {activeButton === "schedule" &&

                            <Scrollbar style={{ width: "100%", height: 600 }}>
                                {Object.entries(leaguesFuture).map(([leagueName, matches]) => (
                                    <TeamLeague key={leagueName} name={leagueName} matches={matches} emblem={matches[0].leagueEmblem} />
                                ))}
                            </Scrollbar>
                        }

                        {activeButton === "players" &&

                            <Scrollbar style={{ width: "100%", height: 600 }}>
                                {categoryOrder.filter((category) => categorizedSquad[category]).map((category) => (
                                        <TeamPosition key={category} position={category} players={categorizedSquad[category]}/>
                                    ))}
                            </Scrollbar>
                        }
                    </div>
                </div>
            </Container>
        </div>

    );
};

export default Team;