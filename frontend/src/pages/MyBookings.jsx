import React, { useState, useEffect } from 'react';
import axiosClient from '../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import StatusBadge from '../components/common/StatusBadge';

const MyBookings = () => {
    const { isAuthenticated } = useSelector(state => state.auth);
    const navigate = useNavigate();
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
            return;
        }

        const fetchBookings = async () => {
            try {
                const response = await axiosClient.get('/booking/my');
                setBookings(response.data);
            } catch (err) {
                console.error("Lỗi khi lấy danh sách booking:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchBookings();
    }, [isAuthenticated, navigate]);



    if (loading) return <div style={{ minHeight: '60vh', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>Đang tải lịch hẹn...</div>;

    return (
        <motion.div
            className="container"
            style={{ padding: '60px 0', minHeight: '80vh' }}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
        >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 30 }}>
                <h2 style={{ color: '#c2185b', margin: 0 }}>Lịch hẹn của tôi</h2>
                <Link to="/booking" className="send_btn" style={{ padding: '8px 20px', textDecoration: 'none' }}>
                    + Đặt lịch mới
                </Link>
            </div>

            {bookings.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '60px 20px', background: '#fce4ec', borderRadius: 16 }}>
                    <div style={{ fontSize: 48, marginBottom: 16 }}>🥺</div>
                    <h4 style={{ color: '#c2185b' }}>Bạn chưa có lịch hẹn nào!</h4>
                    <p style={{ color: '#888' }}>Hãy chọn một dịch vụ thật đẹp và đặt lịch ngay nhé.</p>
                </div>
            ) : (
                <div className="row">
                    <AnimatePresence>
                        {bookings.map((b, i) => (
                            <motion.div
                                key={b.id}
                                className="col-md-6 col-lg-4 mb-4"
                                initial={{ opacity: 0, scale: 0.9 }}
                                animate={{ opacity: 1, scale: 1 }}
                                transition={{ duration: 0.3, delay: i * 0.1 }}
                            >
                                <div style={{
                                    background: '#fff',
                                    border: '2px solid #f8bbd0',
                                    borderRadius: 16,
                                    padding: 24,
                                    height: '100%',
                                    display: 'flex',
                                    flexDirection: 'column',
                                    boxShadow: '0 4px 15px rgba(194,24,136,0.05)'
                                }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 16 }}>
                                        <h4 style={{ color: '#c2185b', margin: 0, fontSize: 18, fontWeight: 'bold' }}>{b.serviceName}</h4>
                                        <StatusBadge status={b.status} />
                                    </div>

                                    <div style={{ marginBottom: 12 }}>
                                        <span style={{ color: '#888', fontSize: 13, display: 'block' }}>Thời gian hẹn</span>
                                        <strong style={{ color: '#333', fontSize: 15 }}>
                                            {new Date(b.bookingTime).toLocaleString('vi-VN', {
                                                weekday: 'long',
                                                year: 'numeric',
                                                month: '2-digit',
                                                day: '2-digit',
                                                hour: '2-digit',
                                                minute: '2-digit'
                                            })}
                                        </strong>
                                    </div>

                                    <div style={{ marginBottom: 12 }}>
                                        <span style={{ color: '#888', fontSize: 13, display: 'block' }}>Giá tiền</span>
                                        <strong style={{ color: '#e91e8c', fontSize: 16 }}>
                                            {(b.price * 1000).toLocaleString()} VNĐ
                                        </strong>
                                    </div>

                                    {b.note && (
                                        <div style={{ marginTop: 'auto', background: '#f9f9f9', padding: '10px 14px', borderRadius: 8 }}>
                                            <span style={{ color: '#888', fontSize: 12, display: 'block' }}>Ghi chú:</span>
                                            <span style={{ color: '#555', fontSize: 14 }}>{b.note}</span>
                                        </div>
                                    )}
                                </div>
                            </motion.div>
                        ))}
                    </AnimatePresence>
                </div>
            )}
        </motion.div>
    );
};

export default MyBookings;
