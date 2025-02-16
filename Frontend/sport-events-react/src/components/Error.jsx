import styles from "../css/Error.module.css"

function Error() {

    const imageUrl = "https://cdni.iconscout.com/illustration/premium/thumb/football-game-black-white-error-404-illustration-download-in-svg-png-gif-file-formats--kick-ball-gate-soccer-sport-oldschool-and-fantasy-pack-people-illustrations-9708255.png?f=webp";

    const handleRefresh = () => {
        window.location.reload();
      };
    
    return (
        <div className={styles.outerContainer}>
            <div className={styles.errorContainer}>
                <img src={imageUrl} alt="" style={{width: "300px"}}/>
                <h1 data-testid="error-message">Oops! Something went wrong...</h1>
                <button className={styles.refreshButton} onClick={handleRefresh}>Refresh Page</button>
            </div>
        </div>
    );
};

export default Error;