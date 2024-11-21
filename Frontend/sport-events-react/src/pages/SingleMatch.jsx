import React, { useEffect, useState } from "react";
import styles from "../css/SingleMatch.module.css";
import { useParams, NavLink } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleChevronLeft } from '@fortawesome/free-solid-svg-icons';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";

function SingleMatch() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    const [activeButton, setActiveButton] = useState("details");
    const imageUrl = "https://s3-alpha-sig.figma.com/img/538e/043c/26152d8ff671e48e2db70ae0ecbf5b6c?Expires=1733097600&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=RzS8YbPX14vehZFb~AOWUsXtbbfPa28OgdTjW9HkrxDx4GHtmPztwVYZnfao0dMp1H2Z1R~O67Uwp4x~FpZbWcaKKOJdZaDOXmc7phvSH7UIyKUgc0CEFBO~9KEIkQuMiiboUt9adIzo2B5LKMBpCCbMHDzytTyUXqkcQlQCqc-EN~iJLKe1ZvRaUmzWfeoNtAen94PwZK-ZtJa8wuWevNCXN~wv-eExN~-kZ9vrK-MFcshXWyohpTKI8RNaP8grBbWRNADMl9DWvakiDCGw4ATFDhoJqRGoiCiAW8RhFTEzpMB12X66g7YMrFQpx7GAHuDXft2Yd2Lbh5PV314Vvw__";

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
        return <Loader/>;
    }

    if (error) {
        return <Error/>;
    }

    const handleButtonClick = (buttonNumber) => {
        setActiveButton(buttonNumber);
    };

    const buttonPosition = {
        details: 0,
        lineups: 33.33,
        standings: 67
    }[activeButton];

    const timeOnly = new Date(data.utcDate).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });
    console.log(timeOnly)

    return (
        <div>
            <ImageBanner image={imageUrl} title={data.homeTeamName + " vs " + data.awayTeamName}></ImageBanner>
            <div className={styles.MainContainer}>
                <NavLink to="/matches" className={styles.IconContainer}>
                    <FontAwesomeIcon icon={faCircleChevronLeft} />
                </NavLink>
                <div className={styles.MatchRow}>
                    <img src={data.homeTeamCrest} alt={`${data.homeTeamCrest} crest`} className={styles.TeamPhoto} />
                    <div className={styles.TeamName}>{data.homeTeamName}</div>
                    <div className={styles.score}>{data.status === "TIMED" || data.status === "SCHEDULED" ? timeOnly : `${data.homeTeamScore} : ${data.awayTeamScore}`}</div>
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
        </div>

    );
};

export default SingleMatch;