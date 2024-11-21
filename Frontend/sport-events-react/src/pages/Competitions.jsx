import React, { useEffect, useState } from "react";
import styles from "../css/Competitions.module.css";
import { NavLink } from 'react-router-dom';
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";

function Competitions() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const imageUrl = "https://s3-alpha-sig.figma.com/img/538e/043c/26152d8ff671e48e2db70ae0ecbf5b6c?Expires=1733097600&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=RzS8YbPX14vehZFb~AOWUsXtbbfPa28OgdTjW9HkrxDx4GHtmPztwVYZnfao0dMp1H2Z1R~O67Uwp4x~FpZbWcaKKOJdZaDOXmc7phvSH7UIyKUgc0CEFBO~9KEIkQuMiiboUt9adIzo2B5LKMBpCCbMHDzytTyUXqkcQlQCqc-EN~iJLKe1ZvRaUmzWfeoNtAen94PwZK-ZtJa8wuWevNCXN~wv-eExN~-kZ9vrK-MFcshXWyohpTKI8RNaP8grBbWRNADMl9DWvakiDCGw4ATFDhoJqRGoiCiAW8RhFTEzpMB12X66g7YMrFQpx7GAHuDXft2Yd2Lbh5PV314Vvw__";

    useEffect(() => {
        fetch(`http://localhost:5260/competitions/all`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setData(data);
                setLoading(false);
            })
            .catch((error) => {
                setError(error);
                setLoading(false);
            });
    }, []);

    if (loading) {
        return <Loader/>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    return (
        <div>
            <ImageBanner image={imageUrl} title={"Select a league"}></ImageBanner>
            <div className={styles.MainContainer}>
                <div className={styles.grid_container}>
                    {data && data.map((competition, index) => (
                        <NavLink to={`/competitions/${competition.id}`} key={competition.id}>
                            <img
                                key={index}
                                src={competition.emblem}
                                alt={competition.name}
                                className={`${styles.grid_item} ${styles.competitionImage}`}
                            />
                        </NavLink>
                    ))}
                </div>
            </div>
        </div>

    );
};

export default Competitions;