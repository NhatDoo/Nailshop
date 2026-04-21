import React from 'react';
import { motion } from 'framer-motion';

const fadeUp = {
    hidden: { opacity: 0, y: 32 },
    visible: (i = 0) => ({
        opacity: 1, y: 0,
        transition: { duration: 0.5, delay: i * 0.07, type: 'spring', stiffness: 90 }
    })
};

const StepBar = ({ currentStep, steps }) => {
    return (
        <motion.div initial="hidden" animate="visible" variants={fadeUp}
            style={{ display: 'flex', justifyContent: 'center', gap: 0, marginBottom: 40 }}
        >
            {steps.map((label, i) => (
                <div key={i} style={{ display: 'flex', alignItems: 'center' }}>
                    <div style={{
                        width: 36, height: 36, borderRadius: '50%',
                        background: currentStep > i + 1 ? '#e91e8c' : currentStep === i + 1 ? '#c2185b' : '#f8bbd0',
                        color: currentStep >= i + 1 ? '#fff' : '#c2185b',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        fontWeight: 700, fontSize: 14,
                        boxShadow: currentStep === i + 1 ? '0 4px 16px rgba(194,24,136,0.35)' : 'none',
                        transition: 'all 0.3s'
                    }}>{i + 1}</div>
                    <div style={{
                        fontSize: 13,
                        color: currentStep === i + 1 ? '#c2185b' : '#aaa',
                        marginLeft: 6,
                        fontWeight: currentStep === i + 1 ? 700 : 400
                    }}>{label}</div>
                    {i < steps.length - 1 && <div style={{ width: 40, height: 2, background: '#f8bbd0', margin: '0 12px' }} />}
                </div>
            ))}
        </motion.div>
    );
};

export default StepBar;
