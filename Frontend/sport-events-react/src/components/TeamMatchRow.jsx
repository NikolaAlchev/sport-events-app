import { useNavigate } from 'react-router-dom';
import styles from "../css/TeamMatchRow.module.css";

function TeamMatchRow({ homeTeam, awayTeam, score, homeCrest, awayCrest, matchId, utcDate, status, year }) {
    const navigate = useNavigate();
    const goTo = () => {
        navigate(`/matches/${matchId}`);
    };

    const timeOnly = new Date(utcDate).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });


    const dateOnly = year ? new Date(utcDate).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: "2-digit" }) :
    new Date(utcDate).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit' })

    return (
        <div className={styles.MatchRow} onClick={goTo} data-testid="match-row">
            <div className={styles.leftSide}>
                <div>
                    <p style={{margin: '0px'}}>{dateOnly}</p>
                </div>
                <div>
                    <div className={styles.TeamCrestContainer}>
                        <img src={homeCrest} alt={`${homeTeam} crest`} className={styles.TeamPhoto} />
                        <p className={styles.TeamName}>{homeTeam}</p>
                    </div>
                    <div className={styles.TeamCrestContainer}>
                        <img src={awayCrest} alt={`${awayTeam} crest`} className={styles.TeamPhoto} />
                        <p className={styles.TeamName}>{awayTeam}</p>
                    </div>
                </div>
            </div>


            <div className={styles.score}>{status === "TIMED" || status === "SCHEDULED" ? timeOnly : score}</div>

        </div>
    );
}

export default TeamMatchRow;