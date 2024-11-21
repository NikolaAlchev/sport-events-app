import { useNavigate } from 'react-router-dom';
import styles from "../css/Matches.module.css";

function MatchRow({ homeTeam, awayTeam, score, homeCrest, awayCrest, matchId }) {
    const navigate = useNavigate();
    const goTo = () => {
        navigate(`/matches/${matchId}`);
    };
    return (
        <div className={styles.MatchRow} onClick={goTo}>
            {/* <div className={styles.MatchRow}> */}
            <img src={homeCrest} alt={`${homeTeam} crest`} className={styles.TeamPhoto} />
            <div className={styles.TeamName}>{homeTeam}</div>
            <div>{score}</div>
            <div className={styles.TeamName}>{awayTeam}</div>
            <img src={awayCrest} alt={`${awayTeam} crest`} className={styles.TeamPhoto} />
        </div>
    );
}

export default MatchRow;