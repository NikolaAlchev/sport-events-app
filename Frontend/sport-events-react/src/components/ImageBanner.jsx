import styles from "../css/ImageBanner.module.css";

function ImageBanner ({title, image = "https://s3-alpha-sig.figma.com/img/538e/043c/26152d8ff671e48e2db70ae0ecbf5b6c?Expires=1733097600&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=RzS8YbPX14vehZFb~AOWUsXtbbfPa28OgdTjW9HkrxDx4GHtmPztwVYZnfao0dMp1H2Z1R~O67Uwp4x~FpZbWcaKKOJdZaDOXmc7phvSH7UIyKUgc0CEFBO~9KEIkQuMiiboUt9adIzo2B5LKMBpCCbMHDzytTyUXqkcQlQCqc-EN~iJLKe1ZvRaUmzWfeoNtAen94PwZK-ZtJa8wuWevNCXN~wv-eExN~-kZ9vrK-MFcshXWyohpTKI8RNaP8grBbWRNADMl9DWvakiDCGw4ATFDhoJqRGoiCiAW8RhFTEzpMB12X66g7YMrFQpx7GAHuDXft2Yd2Lbh5PV314Vvw__"}) {
    return (
        <div className={styles.ImageBannerContainer} style={{backgroundImage: `url(${image})`}}>
            <h1 className={styles.ImageBannerTitle}>{title}</h1>
        </div>
    );
}

export default ImageBanner;