import React, { useEffect, useState } from "react";
import styles from "../css/Matches.module.css";
import League from "../components/League";
import DateSelector from "../components/DateSelector";
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";

function Matches() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedDate, setSelectedDate] = useState(getTodayDate());
    const imageUrl = "https://s3-alpha-sig.figma.com/img/538e/043c/26152d8ff671e48e2db70ae0ecbf5b6c?Expires=1733097600&Key-Pair-Id=APKAQ4GOSFWCVNEHN3O4&Signature=RzS8YbPX14vehZFb~AOWUsXtbbfPa28OgdTjW9HkrxDx4GHtmPztwVYZnfao0dMp1H2Z1R~O67Uwp4x~FpZbWcaKKOJdZaDOXmc7phvSH7UIyKUgc0CEFBO~9KEIkQuMiiboUt9adIzo2B5LKMBpCCbMHDzytTyUXqkcQlQCqc-EN~iJLKe1ZvRaUmzWfeoNtAen94PwZK-ZtJa8wuWevNCXN~wv-eExN~-kZ9vrK-MFcshXWyohpTKI8RNaP8grBbWRNADMl9DWvakiDCGw4ATFDhoJqRGoiCiAW8RhFTEzpMB12X66g7YMrFQpx7GAHuDXft2Yd2Lbh5PV314Vvw__";

    function getTodayDate() {
        const today = new Date();
        today.setDate(23);
        today.setHours(0, 0, 0, 0);
        return today.toISOString().split('T')[0];
    }

    useEffect(() => {
        let formattedToday = selectedDate.split('T')[0];
        let [year, month, day] = formattedToday.split('-');
        formattedToday = `${month}-${day}-${year}`;
        fetch(`http://localhost:5260/matches/all?fromDate=${formattedToday}`)
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
    }, [selectedDate]);

    if (loading) {
        return <Loader />;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    const leagues = data.reduce((acc, match) => {
        const { competitionName } = match;
        if (!acc[competitionName]) {
            acc[competitionName] = [];
        }
        acc[competitionName].push(match);
        return acc;
    }, {});

    return (
        <div>
            <ImageBanner image={imageUrl} title={"Select a match"}></ImageBanner>
            <div className={styles.MatchMainContainer}>

                <div className={styles.Container}>
                    <DateSelector date={selectedDate} dateFunc={setSelectedDate} />
                    <div className="league-list">
                        {Object.entries(leagues).map(([leagueName, matches]) => (
                            <League key={leagueName} name={leagueName} matches={matches} emblem={matches[0].leagueEmblem} />
                        ))}
                    </div>
                </div>
            </div>
        </div>

    );

};

export default Matches;