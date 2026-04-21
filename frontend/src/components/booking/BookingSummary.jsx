import React from 'react';
import { motion } from 'framer-motion';

const fadeUp = {
    hidden: { opacity: 0, y: 32 },
    visible: (i = 0) => ({
        opacity: 1, y: 0,
        transition: { duration: 0.5, delay: i * 0.07, type: 'spring', stiffness: 90 }
    })
};

const BookingSummary = ({ service, date, slot }) => {
    return (
        <motion.div
            initial="hidden" animate="visible" variants={fadeUp}
            style={{ background: '#fff', borderRadius: 16, border: '2px solid #f8bbd0', padding: '28px 32px', marginBottom: 24 }}
        >
            <div style={{ display: 'flex', justifyContent: 'space-between', paddingBottom: 14, borderBottom: '1px solid #fce4ec', marginBottom: 14 }}>
                <span style={{ color: '#aaa' }}>Dịch vụ</span>
                <b style={{ color: '#c2185b' }}>{service?.name}</b>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', paddingBottom: 14, borderBottom: '1px solid #fce4ec', marginBottom: 14 }}>
                <span style={{ color: '#aaa' }}>Ngày</span>
                <b>{new Date(date).toLocaleDateString('vi-VN', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</b>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', paddingBottom: 14, borderBottom: '1px solid #fce4ec', marginBottom: 14 }}>
                <span style={{ color: '#aaa' }}>Giờ</span>
                <b style={{ color: '#c2185b' }}>{slot?.display}</b>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <span style={{ color: '#aaa' }}>Tổng tiền</span>
                <b style={{ color: '#e91e8c', fontSize: 20 }}>{(service?.price * 1000).toLocaleString()} đ</b>
            </div>
        </motion.div>
    );
};

export default BookingSummary;
