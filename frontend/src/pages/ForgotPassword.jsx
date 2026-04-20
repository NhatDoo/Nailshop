import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import axiosClient from '../utils/axiosClient';

const formVariants = {
    hidden: { opacity: 0, y: 40 },
    visible: { opacity: 1, y: 0, transition: { duration: 0.6, type: 'spring', stiffness: 90 } }
};

const ForgotPassword = () => {
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        setLoading(true);
        try {
            await axiosClient.post('/auth/forgot-password', { email });
            setSuccess('📧 Chúng tôi đã gửi hướng dẫn đặt lại mật khẩu về email của bạn. Vui lòng kiểm tra hộp thư!');
        } catch (err) {
            setError(err.response?.data?.message || 'Đã xảy ra lỗi, vui lòng thử lại sau!');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="contact" style={{ paddingTop: '80px', paddingBottom: '80px' }}>
            <div className="container">
                <motion.div className="row" initial="hidden" animate="visible" variants={formVariants}>
                    <div className="col-md-12">
                        <div className="titlepage">
                            <h2>Forgot Password</h2>
                            <p style={{ color: '#888', marginTop: '8px' }}>
                                Nhập email đăng ký, chúng tôi sẽ gửi link đặt lại mật khẩu cho bạn.
                            </p>
                        </div>
                    </div>
                </motion.div>

                <div className="row">
                    <div className="col-md-6 offset-md-3">
                        <motion.form
                            className="main_form"
                            onSubmit={handleSubmit}
                            initial="hidden"
                            animate="visible"
                            variants={formVariants}
                        >
                            {error && <div className="alert alert-danger">{error}</div>}
                            {success && (
                                <motion.div
                                    className="alert alert-success"
                                    initial={{ opacity: 0, scale: 0.95 }}
                                    animate={{ opacity: 1, scale: 1 }}
                                >
                                    {success}
                                </motion.div>
                            )}

                            <div className="row">
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Email của bạn"
                                        type="email"
                                        required
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        disabled={!!success}
                                    />
                                </div>
                                <div className="col-md-12" style={{ marginTop: '8px', textAlign: 'center' }}>
                                    <motion.button
                                        type="submit"
                                        className="send_btn"
                                        disabled={loading || !!success}
                                        whileHover={{ scale: 1.05 }}
                                        whileTap={{ scale: 0.96 }}
                                    >
                                        {loading ? 'Đang gửi...' : 'Send Reset Link'}
                                    </motion.button>
                                </div>
                                <div className="col-md-12" style={{ textAlign: 'center', marginTop: '16px' }}>
                                    <Link to="/login" style={{ color: '#e91e8c', fontWeight: 600 }}>
                                        ← Quay lại Sign In
                                    </Link>
                                </div>
                            </div>
                        </motion.form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ForgotPassword;
