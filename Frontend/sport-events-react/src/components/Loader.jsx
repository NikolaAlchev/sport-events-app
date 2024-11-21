import styles from "../css/Loader.module.css"

function Loader() {
    return (
        <div className={styles.outerContainer}>
            <div className={styles.loader}></div>
        </div>
    );
};

export default Loader;