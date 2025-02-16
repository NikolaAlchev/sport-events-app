import styles from "../css/Loader.module.css"

function Loader({height = "100vh"}) {
    return (
        <div className={styles.outerContainer} data-testid="loader" style={{height}}>
            <div className={styles.loader}></div>
        </div>
    );
};

export default Loader;