import React, { useEffect, useState } from "react";
import Loader from "../components/Loader";
import Error from "../components/Error";

function Question() {
    const API_BASE_URL = process.env.REACT_APP_EVENTS_API_BASE_URL;
    const [userQuestions, setUserQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch(`${API_BASE_URL}/Questions`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then((data) => {
                setUserQuestions(data); // Ensure 'data' is an array or object with 'Questions'
                setLoading(false);
                console.log(data);
            })
            .catch((error) => {
                setError(error);
                console.log(error);
                setLoading(false);
            });
    }, []);

    if (loading) {
        return <Loader />;
    }

    if (error) {
        return <Error />;
    }

    return (
        <div className="container mt-5">
            <h2>User Questions Table</h2>
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>User Name</th>
                        <th>Question Text</th>
                        <th>Question Answer</th>
                    </tr>
                </thead>
                <tbody>
                    {userQuestions.length > 0 ? (
                        userQuestions.map((userQuestion, index) => (
                            <tr key={index}>
                                <td>{userQuestion.userName}</td>
                                <td>{userQuestion.questionText}</td>
                                <td>{userQuestion.questionAnswer}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="5">No data available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}

export default Question;
