import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';


const CreateUser = () => {
    const API_BASE_URL = process.env.REACT_APP_EVENTS_API_BASE_URL;

    const [formData, setFormData] = useState({
        UserName: '',
        Email: '',
        Password: ''
    });

    const navigate = useNavigate()
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

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
            const response = await fetch(`${API_BASE_URL}/api/User/CreateUser`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const data = await response.json();
                setMessage(`Account created successfully`);
                navigate("/user/login")
            } else {
                const errorData = await response.json();
                setError('Error: ' + errorData.errors.map((err) => err.description).join(', '));
            }
        } catch (err) {
            setError('Error: Unable to create user');
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto', padding: '20px', border: '1px solid #ccc' }}>
            <h2>Create User</h2>
            {message && <p style={{ color: 'green' }}>{message}</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Username:
                        <input
                            type="text"
                            name="UserName"
                            value={formData.UserName}
                            onChange={handleChange}
                            required
                            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
                        />
                    </label>
                </div>

                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Email:
                        <input
                            type="email"
                            name="Email"
                            value={formData.Email}
                            onChange={handleChange}
                            required
                            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
                        />
                    </label>
                </div>

                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Password:
                        <input
                            type="password"
                            name="Password"
                            value={formData.Password}
                            onChange={handleChange}
                            required
                            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
                        />
                    </label>
                </div>

                <button type="submit" style={{ padding: '10px 15px', cursor: 'pointer' }}>Create User</button>
            </form>
        </div>
    );
};


export default CreateUser