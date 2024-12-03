import React, { useEffect, useState } from "react";
import styles from "../css/Matches.module.css";
import League from "../components/League";
import DateSelector from "../components/DateSelector";
import ImageBanner from "../components/ImageBanner";
import Loader from "../components/Loader";
import Error from "../components/Error";

function Matches() {
    const API_BASE_URL = process.env.REACT_APP_MATCHES_API_BASE_URL;

    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [contentLoading, setContentLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedDate, setSelectedDate] = useState(getTodayDate());

    function getTodayDate() {
        const today = new Date();
        return today.toISOString().split('T')[0];
    }

    useEffect(() => {
        setContentLoading(true);
        const formattedToday = selectedDate.split('T')[0].replace(/-/g, '-');
        fetch(`${API_BASE_URL}/matches/all?fromDate=${formattedToday}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setData(data);
                setContentLoading(false);
                setLoading(false);
            })
            .catch((error) => {
                setError(error);
                setContentLoading(false);
                setLoading(false);
            });
    }, [selectedDate]);

    if (loading) {
        return <Loader />;
    }

    if (error) {
        return <Error />;
    }

    const leagues = data
        ? data.reduce((acc, match) => {
            const { competitionName } = match;
            if (!acc[competitionName]) {
                acc[competitionName] = [];
            }
            acc[competitionName].push(match);
            return acc;
        }, {})
        : {};

    return (
        <div>
            <ImageBanner title={"Select a match"}></ImageBanner>
            <div className={styles.MatchMainContainer}>
                <div className={styles.Container}>
                    <DateSelector date={selectedDate} dateFunc={setSelectedDate} />
                    {contentLoading ? (
                        <Loader height={"300px"} />
                    ) : (
                        <div className="league-list">
                            {Object.entries(leagues).map(([leagueName, matches]) => (
                                <League
                                    key={leagueName}
                                    name={leagueName}
                                    matches={matches}
                                    emblem={matches[0]?.leagueEmblem}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default Matches;
