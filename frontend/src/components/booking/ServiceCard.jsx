import React from 'react';
import { motion } from 'framer-motion';

const fadeUp = {
    hidden: { opacity: 0, y: 32 },
    visible: (i = 0) => ({
        opacity: 1, y: 0,
        transition: { duration: 0.5, delay: i * 0.07, type: 'spring', stiffness: 90 }
    })
};

const ServiceCard = ({ service, index, isSelected, onClick }) => {
    return (
        <motion.div
            className="col-md-3 col-sm-6"
            custom={index}
            variants={fadeUp}
            initial="hidden"
            animate="visible"
        >
            <motion.div
                whileHover={{ scale: 1.04, boxShadow: '0 8px 30px rgba(194,24,136,0.2)' }}
                whileTap={{ scale: 0.97 }}
                onClick={() => onClick(service)}
                style={{
                    cursor: 'pointer', borderRadius: 16, overflow: 'hidden',
                    border: isSelected ? '2.5px solid #e91e8c' : '2px solid #f8bbd0',
                    background: '#fff', marginBottom: 24, transition: 'border 0.2s'
                }}
            >
                <div style={{ height: 160, overflow: 'hidden', background: '#fce4ec' }}>
                    {service.imageUrl
                        ? <img src={service.imageUrl} alt={service.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                        : <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%', fontSize: 48 }}>💅</div>
                    }
                </div>
                <div style={{ padding: '14px 16px' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                        <h5 style={{ color: '#c2185b', margin: 0, fontSize: 15, fontWeight: 700 }}>{service.name}</h5>
                        <span style={{ color: '#e91e8c', fontWeight: 700, fontSize: 14 }}>
                            {(service.price * 1000).toLocaleString()}đ
                        </span>
                    </div>
                    <p style={{ color: '#888', fontSize: 13, margin: '4px 0 0' }}>{service.description || 'Dịch vụ chăm sóc móng chuyên nghiệp'}</p>
                </div>
            </motion.div>
        </motion.div>
    );
};

export default ServiceCard;
