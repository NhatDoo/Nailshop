import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import axiosClient from '../utils/axiosClient';
import { loginSuccess, setAuthError } from '../store/slices/authSlice';

const formVariants = {
    hidden: { opacity: 0, y: 40 },
    visible: { opacity: 1, y: 0, transition: { duration: 0.6, type: 'spring', stiffness: 90 } }
};

const Login = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { error, isAuthenticated } = useSelector((state) => state.auth);

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);

    // Nếu người dùng đã đăng nhập thì tự động đá ra Home
    React.useEffect(() => {
        if (isAuthenticated) {
            navigate('/');
        }
    }, [isAuthenticated, navigate]);

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            // POST API Auth/Login
            const response = await axiosClient.post('/auth/login', {
                email,
                password,
            });

            dispatch(loginSuccess({
                accessToken: response.data.accessToken,
                user: {
                    email: response.data.email,
                    role: response.data.role
                }
            }));

            navigate('/'); // Điều hướng về Home
        } catch (err) {
            if (err.response && err.response.data) {
                dispatch(setAuthError(err.response.data.message || 'Sai tài khoản hoặc mật khẩu!'));
            } else {
                dispatch(setAuthError('Máy chủ không phản hồi!'));
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="contact" style={{ paddingTop: '80px', marginBottom: '80px' }}>
            <div className="container">
                <motion.div
                    className="row"
                    initial="hidden"
                    animate="visible"
                    variants={formVariants}
                >
                    <div className="col-md-12">
                        <div className="titlepage">
                            <h2>Sign In</h2>
                        </div>
                    </div>
                </motion.div>
                <div className="row">
                    <div className="col-md-6 offset-md-3">
                        <motion.form
                            className="main_form"
                            onSubmit={handleLogin}
                            initial="hidden"
                            animate="visible"
                            variants={formVariants}
                        >
                            {error && (
                                <div className="alert alert-danger" role="alert">
                                    {error}
                                </div>
                            )}
                            <div className="row">
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Email"
                                        type="email"
                                        required
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Password"
                                        type="password"
                                        required
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <motion.button
                                        type="submit"
                                        className="send_btn"
                                        disabled={loading}
                                        whileHover={{ scale: 1.05 }}
                                        whileTap={{ scale: 0.96 }}
                                    >
                                        {loading ? 'Processing...' : 'Login'}
                                    </motion.button>
                                </div>
                                <div className="col-md-12" style={{ textAlign: 'center', marginTop: '16px' }}>
                                    <Link to="/forgot-password" style={{ color: '#e91e8c', fontSize: '14px' }}>Quên mật khẩu?</Link>
                                </div>
                                <div className="col-md-12" style={{ textAlign: 'center', marginTop: '8px' }}>
                                    <span style={{ color: '#666' }}>Chưa có tài khoản? </span>
                                    <Link to="/register" style={{ color: '#e91e8c', fontWeight: 600 }}>Register</Link>
                                </div>
                            </div>
                        </motion.form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login;
