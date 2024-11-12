import React, { useEffect, useState } from "react";

//http://localhost:3000/matches

function Matches() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        let today = new Date();
        let formattedToday = today.toISOString().split('T')[0];
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

export default Matches;