import styles from "../css/TeamPlayerRow.module.css";
import Flag from "react-world-flags";
import countryToISO from "../countryToISO";

function TeamPlayerRow({ name, position, nationality }) {

    return (
        <div className={styles.PlayerRow}>
            <div className={styles.leftSide}>
                <div>
                    <div className={styles.PlayerContainer}>
                        <Flag code={countryToISO[nationality]} className={styles.Nationality} />
                        <p className={styles.Name}>{name}</p>
                    </div>
                </div>
            </div>


            <div className={styles.position}>{position}</div>

        </div>
    );
}

export default TeamPlayerRow;