import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from 'react-router-dom';
import Loader from "../components/Loader";
import Error from "../components/Error";
import styles from "../css/Team.module.css";
import ImageBanner from "../components/ImageBanner";
import { Row, Col, Container, Button, Form } from 'react-bootstrap';
import { Scrollbar } from 'react-scrollbars-custom';
import TeamLeague from "../components/TeamLeague";
import TeamPosition from "../components/TeamPosition";

// http://localhost:3000/team/2013?name=Fluminense%20FC

function Team() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    const [searchParams] = useSearchParams();
    const [activeButton, setActiveButton] = useState("profile");
    const imageUrl = "https://s3-alpha-sig.figma.com/img/538e/043c/26152d8ff671e48e2db70ae0ecbf5b6c?Expires=1733097600&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=RzS8YbPX14vehZFb~AOWUsXtbbfPa28OgdTjW9HkrxDx4GHtmPztwVYZnfao0dMp1H2Z1R~O67Uwp4x~FpZbWcaKKOJdZaDOXmc7phvSH7UIyKUgc0CEFBO~9KEIkQuMiiboUt9adIzo2B5LKMBpCCbMHDzytTyUXqkcQlQCqc-EN~iJLKe1ZvRaUmzWfeoNtAen94PwZK-ZtJa8wuWevNCXN~wv-eExN~-kZ9vrK-MFcshXWyohpTKI8RNaP8grBbWRNADMl9DWvakiDCGw4ATFDhoJqRGoiCiAW8RhFTEzpMB12X66g7YMrFQpx7GAHuDXft2Yd2Lbh5PV314Vvw__";

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
            <ImageBanner image={imageUrl} title={data.name}></ImageBanner>
            <Container>
                <div className={styles.upperContent}>
                    <div className={styles.teamCrestContainer}>
                        <img src={data.crest} alt="Team Crest" />
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
                                    <TeamLeague key={leagueName} name={leagueName} matches={matches} emblem={matches[0].leagueEmblem} />
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