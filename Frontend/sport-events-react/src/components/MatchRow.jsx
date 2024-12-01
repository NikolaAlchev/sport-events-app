import { useNavigate } from 'react-router-dom';
import styles from "../css/Matches.module.css";

function MatchRow({ homeTeam, awayTeam, score, homeCrest, awayCrest, matchId, utcDate, status }) {
    const navigate = useNavigate();
    const goTo = () => {
        navigate(`/matches/${matchId}`);
    };

    const timeOnly = new Date(utcDate).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });
    return (
        <div className={styles.MatchRow} onClick={goTo}>
            <div className={styles.TeamPhotoWrapper}>
                <img src={homeCrest} alt={`${homeTeam} crest`} className={styles.TeamPhoto} />
            </div>
            <div className={styles.TeamName}>{homeTeam}</div>
            <div className={styles.score}>{status === "TIMED" || status === "SCHEDULED" ? timeOnly : score}</div>
            <div className={styles.TeamName}>{awayTeam}</div>
            <div className={styles.TeamPhotoWrapper}>
                <img src={awayCrest} alt={`${awayTeam} crest`} className={styles.TeamPhoto} />
            </div>
        </div>
    );
}

export default MatchRow;