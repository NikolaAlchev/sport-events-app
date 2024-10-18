import React, { useEffect, useState } from "react";

function Matches() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        let today = new Date();
        let formattedToday = today.toISOString().split('T')[0]; // yyyy-mm-dd format from ISO string
        let [year, month, day] = formattedToday.split('-'); // split to get parts
        formattedToday = `${month}-${day}-${year}`;
        // Make GET request
        fetch(`http://localhost:5260/matches/all?fromDate=${formattedToday}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                // console.log(response.json())
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