import styles from "../css/ImageBanner.module.css";
import bannerImage from "../assets/stadium_2.jpg";

function ImageBanner ({title, image = bannerImage}) {
    return (
        <div className={styles.ImageBannerContainer} style={{backgroundImage: `url(${image})`}}>
            <h1 className={styles.ImageBannerTitle}>{title}</h1>
        </div>
    );
}

export default ImageBanner;