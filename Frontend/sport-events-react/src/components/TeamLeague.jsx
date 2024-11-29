import TeamMatchRow from "../components/TeamMatchRow"
import styles from "../css/TeamLeague.module.css";

function TeamLeague({ name, matches, emblem }) {
    return (
        <div className={styles.LeagueContainer}>
            <div className={styles.LeagueInnerContainer}>
                <img className={styles.LeaguePhoto} src={emblem} alt="" />
                <h3>{name}</h3>
            </div>
            <div className={styles.MatchesContainer}>
                {matches.map((match) => (
                    <TeamMatchRow 
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

export default TeamLeague;