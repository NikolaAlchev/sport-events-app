import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from "../css/CreateUser.module.css"
import { NavLink } from 'react-router-dom';

const CreateUser = () => {
    const API_BASE_URL = process.env.REACT_APP_EVENTS_API_BASE_URL;

    const [formData, setFormData] = useState({
        UserName: '',
        Email: '',
        Password: ''
    });

    const navigate = useNavigate();
    const [isAdmin, setIsAdmin] = useState(null);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/User/is-admin`, {
            credentials: 'include',
        })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Failed to fetch admin status');
            })
            .then((data) => {
                console.log(data)
                setIsAdmin(data);
            })
            .catch((error) => {
                console.error('Error fetching admin status:', error);
                setIsAdmin(null);
            });
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const response = await fetch( isAdmin ? `${API_BASE_URL}/api/User/CreateAdmin` : `${API_BASE_URL}/api/User/CreateUser`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const data = await response.json();
                setMessage(`Account created successfully`);
                setTimeout(() => {
                    navigate("/user/login");
                }, 500);
            } else {
                const errorData = await response.json();
                setError('Error: ' + errorData.errors.map((err) => err.description).join(', '));
            }
        } catch (err) {
            setError('Error: Unable to create user');
        }
    };

    return (
        <div className={styles.outerContainer}>
            <div className={styles.registerContainer}>
                <h2 style={{ textAlign: "center" }}>Register</h2>
                <hr style={{ margin: "25px 20%" }} />

                {message && <p style={{ color: 'green' }}>{message}</p>}
                {error && <p style={{ color: 'red' }}>{error}</p>}

                <form onSubmit={handleSubmit}>
                    <div style={{ marginBottom: '10px' }}>
                        <input
                            type="text"
                            name="UserName"
                            placeholder='Username'
                            value={formData.UserName}
                            onChange={handleChange}
                            required
                            className={styles.inputField}
                        />
                    </div>

                    <div style={{ marginBottom: '10px' }}>
                        <input
                            type="email"
                            name="Email"
                            placeholder='Email'
                            value={formData.Email}
                            onChange={handleChange}
                            required
                            className={styles.inputField}
                        />
                    </div>

                    <div style={{ marginBottom: '10px' }}>
                        <input
                            type="password"
                            name="Password"
                            placeholder='Password'
                            value={formData.Password}
                            onChange={handleChange}
                            required
                            className={styles.inputField}
                        />
                    </div>

                    <button type="submit" className={styles.registerButton}>REGISTER</button>
                </form>
                <p style={{textAlign: "center"}}>Have an account? <NavLink to={"/user/login"}>Log In</NavLink></p>
            </div>
        </div>
    );
};


export default CreateUser