import React, { useEffect, useState } from "react";
import styles from "../css/Matches.module.css";
import { useNavigate } from 'react-router-dom';

//http://localhost:3000/matches

function Matches() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedDate, setSelectedDate] = useState(getTodayDate());

    function getTodayDate() {
        const today = new Date();
        today.setDate(23);
        today.setHours(0, 0, 0, 0);
        return today.toISOString().split('T')[0]; // This returns the date in 'YYYY-MM-DD' format
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
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    // Group matches by competition (league)
    const leagues = data.reduce((acc, match) => {
        const { competitionName } = match;
        if (!acc[competitionName]) {
            acc[competitionName] = [];
        }
        acc[competitionName].push(match);
        return acc;
    }, {});

    return (
        <div className={styles.MatchMainContainer}>
            <div className={styles.Container}>
                <DateSelector date={selectedDate}  dateFunc={setSelectedDate}/>
                <div className="league-list">
                    {Object.entries(leagues).map(([leagueName, matches]) => (
                        <League key={leagueName} name={leagueName} matches={matches} />
                    ))}
                </div>
            </div>
        </div>
    );

};

function DateSelector({ date }) {
    // const handleDateChange = (event) => {
    //     dateFunc(event.target.value);
    // };

    return (
        <div className={styles.CalendarContainer}>
            <div className={styles.CalendarInnerContainer}>
                <p className={styles.CalendarText}>select a date</p>
                {/* treba da se sredi kalendarot */}
                {/* <input type="date" value={date} onChange={handleDateChange}/> */}
                <input type="date" value={date}/>
                <button className="calendar-button">📅</button>
                <span className="date">{date}</span>
                <hr />
            </div>
        </div>
    );
}

function League({ name, matches }) {
    return (
        <div className={styles.LeagueContainer}>
            <div className={styles.LeagueInnerContainer}>
                <img className={styles.LeaguePhoto} src="" alt="" />
                <h3>{name}</h3>
            </div>
            <div className={styles.MatchesContainer}>
                {matches.map((match) => (
                    <MatchRow 
                        key={match.id} 
                        matchId={match.id}
                        homeTeam={match.homeTeamName} 
                        awayTeam={match.awayTeamName} 
                        score={`${match.homeTeamScore} : ${match.awayTeamScore}`}
                        homeCrest={match.homeTeamCrest}
                    awayCrest={match.awayTeamCrest}
                    />
                ))}
            </div>
        </div>
    );
}

function MatchRow({ homeTeam, awayTeam, score, homeCrest, awayCrest, matchId }) {
    const navigate = useNavigate();  // Use for redirection
    const goTo = () => {
        navigate(`/matches/${matchId}`);
    };
    return (
        <div className={styles.MatchRow} onClick={goTo}>
            {/* <div className={styles.MatchRow}> */}
            <img src={homeCrest} alt={`${homeTeam} crest`} className={styles.TeamPhoto} />
            <div className={styles.TeamName}>{homeTeam}</div>
            <div>{score}</div>
            <div className={styles.TeamName}>{awayTeam}</div>
            <img src={awayCrest} alt={`${awayTeam} crest`} className={styles.TeamPhoto} />
        </div>
    );
}

export default Matches;