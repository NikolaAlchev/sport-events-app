import TeamPlayerRow from "../components/TeamPlayerRow"
import styles from "../css/TeamPosition.module.css";

function TeamPosition({ position, players}) {
    return (
        <div className={styles.PositionContainer}>
            <div className={styles.PositionInnerContainer}>
                <h3>{position}</h3>
            </div>
            <div className={styles.PlayersContainer}>
                {players.map((player) => (
                    <TeamPlayerRow 
                        key={player.id} 
                        name={player.name}
                        position={player.position}
                        nationality={player.nationality}
                    />
                ))}
            </div>
        </div>
    );
}

export default TeamPosition;