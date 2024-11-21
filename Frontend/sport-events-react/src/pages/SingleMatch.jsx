import React, { useEffect, useState } from "react";
import styles from "../css/SingleMatch.module.css";
import { useParams, NavLink } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleChevronLeft } from '@fortawesome/free-solid-svg-icons';

//http://localhost:3000/matches/502427

function SingleMatch() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams(); // /matches/:id
    const [activeButton, setActiveButton] = useState("details");

    useEffect(() => {

        // Make GET request
        fetch(`http://localhost:5260/matches/${id}`)
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

    const handleButtonClick = (buttonNumber) => {
        setActiveButton(buttonNumber);  // Update active button
    };

    const buttonPosition = {
        details: 0,
        lineups: 33.33,
        standings: 67
    }[activeButton];

    return (
        <div className={styles.MainContainer}>
            <NavLink to="/matches" className={styles.IconContainer}>
                <FontAwesomeIcon icon={faCircleChevronLeft} />
            </NavLink>
            <div className={styles.MatchRow}>
                <img src={data.homeTeamCrest} alt={`${data.homeTeamCrest} crest`} className={styles.TeamPhoto} />
                <div className={styles.TeamName}>{data.homeTeamName}</div>
                <div>{`${data.homeTeamScore} : ${data.awayTeamScore}`}</div>
                <div className={styles.TeamName}>{data.awayTeamName}</div>
                <img src={data.awayTeamCrest} alt={`${data.awayTeamCrest} crest`} className={styles.TeamPhoto} />
            </div>
            <div className={styles.ButtonContainer}>
                <div className={styles.ButtonBackground}>
                    <div className={`${styles.Button} `} style={{ left: `${buttonPosition}%` }}></div>
                    <div className={`${styles.Text} ${activeButton === "details" ? styles.ActiveText : styles.NotActiveText}`}
                        onClick={() => handleButtonClick("details")}>Details</div>
                    <div className={`${styles.Text} ${activeButton === "lineups" ? styles.ActiveText : styles.NotActiveText}`}
                        onClick={() => handleButtonClick("lineups")}>Lineups</div>
                    <div className={`${styles.Text} ${activeButton === "standings" ? styles.ActiveText : styles.NotActiveText}`}
                        onClick={() => handleButtonClick("standings")}>Standings</div>
                </div>
            </div>
            <div className={styles.Content}>
                {activeButton === "details" && <div>Details is pressed</div>}
                {activeButton === "lineups" && <div>Line Ups is pressed</div>}
                {activeButton === "standings" && <div>Standings is pressed</div>}
            </div>
        </div>
    );
};

export default SingleMatch;