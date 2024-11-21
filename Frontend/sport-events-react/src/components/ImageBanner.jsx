import styles from "../css/ImageBanner.module.css";

function ImageBanner ({image, title}) {
    return (
        <div className={styles.ImageBannerContainer} style={{backgroundImage: `url(${image})`}}>
            <h1 className={styles.ImageBannerTitle}>{title}</h1>
        </div>
    );
}

export default ImageBanner;