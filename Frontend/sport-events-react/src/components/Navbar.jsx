import React, { useEffect, useState } from "react";
import { NavLink, useNavigate, useLocation } from "react-router-dom";
import '../css/Navbar.css';


function Navbar() {
    const navigate = useNavigate();
    const location = useLocation();
    const [username, setUsername] = useState(null);
    const [loggedIn, setLoggedIn] = useState(false);

    useEffect(() => {
        fetch('https://localhost:7023/api/User/validate', {
            method: 'GET',
            credentials: 'include',
        })
        .then(response => {
            if (response.ok) {
                return response.text();
            }
            throw new Error("Not authenticated");
        })
        .then(data => {
            setUsername(data);
            setLoggedIn(true);
        })
        .catch(error => {
            console.error("Error fetching username:", error);
            setUsername(null);
            setLoggedIn(false);
        });
    }, [location]);


    const handleLoginClick = () => {
        navigate('/user/login');
      };

    const handleLogout = async () => {
        try {
            const response = await fetch('https://localhost:7023/api/User/Logout', {
                method: 'POST',
                credentials: 'include',
            });

            if (response.ok) {
                setUsername(null);
                setLoggedIn(false);
                navigate('/matches');
            } else {
                console.error("Logout failed");
            }
        } catch (error) {
            console.error("Error logging out:", error);
        }
    };

    return (
        <nav>
            <div id="navbar-left-side">
                <div className="button button-left" id="navbar-container">
                    <NavLink to="/events" className="navbar-links">
                        <p>Events</p>
                    </NavLink>

                    <NavLink to="/matches" className="navbar-links">
                        <p>Matches</p>
                    </NavLink>

                    <NavLink to="/team" className="navbar-links">
                        <p>Teams</p>
                    </NavLink>

                    <NavLink to="/player" className="navbar-links">
                        <p>Players</p>
                    </NavLink>
                </div>
                {username ? 
                    <div id="username-container">
                        {username}
                    </div> 
                : ""}
                
            </div>


            <div id="login-button-container">
                {loggedIn ? 
                    <button className="navbar-button" onClick={handleLogout}>Logout</button> 
                    : 
                    <button className="navbar-button" onClick={handleLoginClick}>Login</button> 
                }
                
            </div>
        </nav>
    );
};

export default Navbar;