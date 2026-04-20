import React, { useState, useEffect } from 'react';
import axiosClient from '../../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';

const AdminTodo = () => {
    const { user } = useSelector(s => s.auth);
    const navigate = useNavigate();
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);

    useEffect(() => {
        if (user?.role !== 'Admin') {
            navigate('/');
            return;
        }
        loadBookings();
    }, [user, selectedDate]);

    const loadBookings = async () => {
        setLoading(true);
        try {
            const { data } = await axiosClient.get(`/booking/date/${selectedDate}`);
            setBookings(data);
        } catch (e) {
            console.error(e);
        } finally {
            setLoading(false);
        }
    };

    const getStatusColor = (status) => {
        switch (status) {
            case 'Pending': return '#ffc107';
            case 'Confirmed': return '#2196f3';
            case 'Completed': return '#4caf50';
            case 'Cancelled': return '#f44336';
            default: return '#9e9e9e';
        }
    };

    return (
        <motion.div
            className="container"
            style={{ padding: '40px 0', minHeight: '80vh' }}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
        >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 30, flexWrap: 'wrap', gap: 20 }}>
                <div>
                    <h2 style={{ color: '#c2185b', margin: 0 }}>Lịch làm việc hôm nay</h2>
                    <p style={{ color: '#888', margin: '5px 0 0' }}>Quản lý danh sách khách đặt lịch theo ngày</p>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                    <label style={{ margin: 0, fontWeight: 600 }}>Chọn ngày:</label>
                    <input
                        className="contactus"
                        type="date"
                        value={selectedDate}
                        onChange={e => setSelectedDate(e.target.value)}
                        style={{ width: 'auto', marginBottom: 0, padding: '8px 15px' }}
                    />
                </div>
            </div>

            {loading ? (
                <div style={{ textAlign: 'center', padding: '50px' }}>Đang tải danh sách...</div>
            ) : (
                <div className="row">
                    <AnimatePresence>
                        {bookings.length > 0 ? (
                            bookings.map((b, i) => (
                                <motion.div
                                    key={b.id}
                                    className="col-md-12 mb-3"
                                    initial={{ opacity: 0, x: -20 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    exit={{ opacity: 0, x: 20 }}
                                    transition={{ duration: 0.3, delay: i * 0.05 }}
                                >
                                    <div style={{
                                        background: '#fff',
                                        borderRadius: 12,
                                        padding: '15px 20px',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'space-between',
                                        borderLeft: `5px solid ${getStatusColor(b.status)}`,
                                        boxShadow: '0 2px 8px rgba(0,0,0,0.05)'
                                    }}>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: 20 }}>
                                            <div style={{ textAlign: 'center', minWidth: 70 }}>
                                                <div style={{ fontSize: 18, fontWeight: 'bold', color: '#c2185b' }}>
                                                    {new Date(b.bookingTime).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
                                                </div>
                                                <div style={{ fontSize: 12, color: '#aaa', textTransform: 'uppercase' }}>Giờ hẹn</div>
                                            </div>

                                            <div style={{ height: 40, width: 1, background: '#eee' }}></div>

                                            <div>
                                                <div style={{ fontSize: 16, fontWeight: 600 }}>{b.serviceName}</div>
                                                <div style={{ fontSize: 13, color: '#666' }}>
                                                    {b.note ? `Ghi chú: ${b.note}` : 'Không có ghi chú'}
                                                </div>
                                            </div>
                                        </div>

                                        <div style={{ display: 'flex', alignItems: 'center', gap: 15 }}>
                                            <div style={{ textAlign: 'right' }}>
                                                <div style={{ fontWeight: 'bold', color: '#e91e8c' }}>{(b.price * 100).toLocaleString()}đ</div>
                                                <div style={{
                                                    fontSize: 11,
                                                    color: '#fff',
                                                    background: getStatusColor(b.status),
                                                    padding: '2px 8px',
                                                    borderRadius: 10,
                                                    display: 'inline-block'
                                                }}>
                                                    {b.status}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </motion.div>
                            ))
                        ) : (
                            <motion.div
                                className="col-12"
                                initial={{ opacity: 0 }}
                                animate={{ opacity: 1 }}
                                style={{ textAlign: 'center', padding: '100px 20px', background: '#f9f9f9', borderRadius: 20 }}
                            >
                                <div style={{ fontSize: 40, marginBottom: 10 }}>📅</div>
                                <h4 style={{ color: '#aaa' }}>Không có lịch hẹn nào cho ngày này</h4>
                            </motion.div>
                        )}
                    </AnimatePresence>
                </div>
            )}
        </motion.div>
    );
};

export default AdminTodo;
