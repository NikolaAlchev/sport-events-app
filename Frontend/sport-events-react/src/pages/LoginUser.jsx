
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';


function LoginUser() {
    const navigate = useNavigate()
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        Email: '',
        Password: ''
    });



    // Handle form input changes
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
            const response = await fetch('https://localhost:7023/api/User/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',

                },
                body: JSON.stringify(formData),
                credentials: 'include',
                // Ensure cookies are included in the request
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
        <div style={{ maxWidth: '400px', margin: '0 auto', padding: '20px', border: '1px solid #ccc' }}>
            <h2>Log in</h2>
            {message && <p style={{ color: 'green' }}>{message}</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <form onSubmit={handleSubmit}>
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

                <button type="submit" style={{ padding: '10px 15px', cursor: 'pointer' }}>Log in</button>
            </form>
        </div>
    );
}

export default LoginUser