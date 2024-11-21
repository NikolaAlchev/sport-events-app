import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from 'react-router-dom';
import Loader from "../components/Loader";

// http://localhost:3000/team/2013?name=Fluminense%20FC

function Team() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();
    const [searchParams] = useSearchParams();

    useEffect(() => {
        const teamName = searchParams.get("name")
        const encodedTeamName = encodeURIComponent(teamName)
        console.log(teamName)
        console.log(encodedTeamName)
        console.log(`http://localhost:5260/team/${id}?name=${encodedTeamName}`)

        fetch(`http://localhost:5260/team/${id}?name=${encodedTeamName}`)
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
            <h1>Data from API:</h1>
            <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
    );
};

export default Team;