import React, { useEffect, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import '../css/Navbar.css';


function Navbar() {
    const navigate = useNavigate();

    const handleLoginClick = () => {
        navigate('/user/login');
      };

    return (
        <nav>
            <div id="navbar-left-side">
                <div class="button button-left" id="navbar-container">
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
                <div>
                    Hello
                </div>
            </div>


            <div id="login-button-container">
                <button class="navbar-button" onClick={handleLoginClick}>Login</button>
            </div>
        </nav>
    );
};

export default Navbar;