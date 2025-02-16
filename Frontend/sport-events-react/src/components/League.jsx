import MatchRow from "../components/MatchRow";
import styles from "../css/Matches.module.css";

function League({ name, matches, emblem }) {
    return (
        <div className={styles.LeagueContainer} data-test="league-container">
            <div className={styles.LeagueInnerContainer}>
                <img className={styles.LeaguePhoto} src={emblem} alt="" />
                <h3>{name}</h3>
            </div>
            <div className={styles.MatchesContainer}>
                {matches.map((match) => (
                    <MatchRow 
                        key={match.id} 
                        matchId={match.id}
                        homeTeam={match.homeTeamName} 
                        awayTeam={match.awayTeamName} 
                        score={`${match.homeTeamScore} : ${match.awayTeamScore}`}
                        homeCrest={match.homeTeamCrest}
                        utcDate={match.utcDate}
                        status={match.status}
                    awayCrest={match.awayTeamCrest}
                    />
                ))}
            </div>
        </div>
    );
}

export default League;