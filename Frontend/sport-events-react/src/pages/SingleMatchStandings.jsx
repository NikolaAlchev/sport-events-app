import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from 'react-router-dom';

//http://localhost:3000/matches/502427/standings?homeTeam=4&awayTeam=20

function SingleMatchStandings() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchParams] = useSearchParams();

    useEffect(() => {

        const homeTeamId = searchParams.get("homeTeam")
        const awayTeamId = searchParams.get("awayTeam")


        // Make GET request
        fetch(`http://localhost:5260/matches/team/standings?homeTeam=${homeTeamId}&awayTeam=${awayTeamId}`)
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
    }, []); // Empty dependency array to ensure it runs once after the component mounts

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    return (
        <div>
            <h1>Data from API:</h1>
            <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
    );
};

export default SingleMatchStandings;