import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import style from "../css/Login.module.css"


function LoginUser() {
    const API_BASE_URL = process.env.REACT_APP_EVENTS_API_BASE_URL;

    const navigate = useNavigate()
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        Email: '',
        Password: ''
    });

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
            const response = await fetch(`${API_BASE_URL}/api/User/Login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
                credentials: 'include',
            });

            if (response.ok) {
                const data = await response.json();
                setMessage(data.message);
                navigate("/matches")
            } else {
                const errorData = await response.json();
                setError('Error: ' + errorData.errors.map((err) => err.description).join(', '));
            }
        } catch (err) {
            setError('Unable to Log in');
        }
    };

    return (
        <div className={style.outerContainer}>
            <div className={style.loginContainer}>
                <h2 style={{ textAlign: "center" }}>Login</h2>
                <hr style={{ margin: "25px 20%" }} />

                {message && <p style={{ color: 'green', textAlign: 'center' }}>{message}</p>}
                {error && <p style={{ color: 'red', textAlign: 'center'  }}>{error}</p>}

                <form onSubmit={handleSubmit}>
                    <div style={{ marginBottom: '10px' }}>
                        <input
                            type="email"
                            name="Email"
                            placeholder='Email'
                            value={formData.Email}
                            onChange={handleChange}
                            required
                            className={style.inputField}
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
                            className={style.inputField}
                        />
                    </div>

                    <button type="submit" className={style.loginButton}>LOGIN</button>
                </form>
            </div>
        </div>
    );
}

export default LoginUser