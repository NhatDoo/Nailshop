import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import axiosClient from '../utils/axiosClient';
import { loginSuccess, setAuthError } from '../store/slices/authSlice';

const formVariants = {
    hidden: { opacity: 0, y: 40 },
    visible: { opacity: 1, y: 0, transition: { duration: 0.6, type: 'spring', stiffness: 90 } }
};

const Register = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const [form, setForm] = useState({ ten: '', sdt: '', email: '', password: '', confirmPassword: '' });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const handleRegister = async (e) => {
        e.preventDefault();
        setError('');
        if (form.password !== form.confirmPassword) {
            setError('Mật khẩu xác nhận không khớp!');
            return;
        }
        setLoading(true);
        try {
            await axiosClient.post('/auth/register', {
                ten: form.ten,
                sdt: form.sdt,
                email: form.email,
                password: form.password,
                role: 'Customer',
            });
            setSuccess('Đăng ký thành công! Đang chuyển sang trang đăng nhập...');
            setTimeout(() => navigate('/login'), 2000);
        } catch (err) {
            setError(err.response?.data?.message || err.response?.data || 'Đăng ký thất bại, vui lòng thử lại!');
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
                            <h2>Create Account</h2>
                        </div>
                    </div>
                </motion.div>

                <div className="row">
                    <div className="col-md-6 offset-md-3">
                        <motion.form className="main_form" onSubmit={handleRegister} initial="hidden" animate="visible" variants={formVariants}>
                            {error && <div className="alert alert-danger">{error}</div>}
                            {success && <div className="alert alert-success">{success}</div>}

                            <div className="row">
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Họ và Tên"
                                        type="text"
                                        name="ten"
                                        required
                                        value={form.ten}
                                        onChange={handleChange}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Số Điện Thoại"
                                        type="tel"
                                        name="sdt"
                                        required
                                        value={form.sdt}
                                        onChange={handleChange}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Email"
                                        type="email"
                                        name="email"
                                        required
                                        value={form.email}
                                        onChange={handleChange}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Mật Khẩu"
                                        type="password"
                                        name="password"
                                        required
                                        minLength={6}
                                        value={form.password}
                                        onChange={handleChange}
                                    />
                                </div>
                                <div className="col-md-12">
                                    <input
                                        className="contactus"
                                        placeholder="Xác Nhận Mật Khẩu"
                                        type="password"
                                        name="confirmPassword"
                                        required
                                        value={form.confirmPassword}
                                        onChange={handleChange}
                                    />
                                </div>
                                <div className="col-md-12" style={{ marginTop: '8px', textAlign: 'center' }}>
                                    <motion.button
                                        type="submit"
                                        className="send_btn"
                                        disabled={loading}
                                        whileHover={{ scale: 1.05 }}
                                        whileTap={{ scale: 0.96 }}
                                    >
                                        {loading ? 'Đang đăng ký...' : 'Register'}
                                    </motion.button>
                                </div>
                                <div className="col-md-12" style={{ textAlign: 'center', marginTop: '16px' }}>
                                    <span style={{ color: '#666' }}>Đã có tài khoản? </span>
                                    <Link to="/login" style={{ color: '#e91e8c', fontWeight: 600 }}>Sign In</Link>
                                </div>
                            </div>
                        </motion.form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Register;
