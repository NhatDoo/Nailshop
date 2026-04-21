import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import axiosClient from '../utils/axiosClient';

// Components
import StepBar from '../components/booking/StepBar';
import ServiceCard from '../components/booking/ServiceCard';
import BookingSummary from '../components/booking/BookingSummary';

// ── Animation Variants ──────────────────────────────────────────────────────
const fadeUp = {
    hidden: { opacity: 0, y: 32 },
    visible: (i = 0) => ({
        opacity: 1, y: 0,
        transition: { duration: 0.5, delay: i * 0.07, type: 'spring', stiffness: 90 }
    })
};

// ── Helpers ──────────────────────────────────────────────────────────────────
const today = () => new Date().toISOString().split('T')[0];

const Booking = () => {
    const { isAuthenticated } = useSelector((s) => s.auth);
    const navigate = useNavigate();

    // ----- State -----
    const [services, setServices] = useState([]);
    const [slots, setSlots] = useState([]);
    const [selectedDate, setSelectedDate] = useState(today());
    const [selectedSlot, setSelectedSlot] = useState(null);
    const [selectedService, setSelectedService] = useState(null);
    const [note, setNote] = useState('');
    const [step, setStep] = useState(1);       // 1 = chọn dịch vụ, 2 = chọn giờ, 3 = xác nhận
    const [submitting, setSubmitting] = useState(false);
    const [successMsg, setSuccessMsg] = useState('');
    const [errorMsg, setErrorMsg] = useState('');

    const steps = ['Chọn dịch vụ', 'Chọn ngày & giờ', 'Xác nhận'];

    // ----- Load Dịch vụ Nail (public) -----
    useEffect(() => {
        axiosClient.get('/nailservice')
            .then(r => setServices(r.data.filter(s => s.isActive)))
            .catch(() => setServices([]));
    }, []);

    // ----- Load Lịch trống theo ngày -----
    useEffect(() => {
        setSelectedSlot(null);
        axiosClient.get(`/booking/available-slots?date=${selectedDate}`)
            .then(r => setSlots(r.data))
            .catch(() => setSlots([]));
    }, [selectedDate]);

    const handleBooking = async () => {
        if (!isAuthenticated) { navigate('/login'); return; }
        if (!selectedService || !selectedSlot) return;
        setSubmitting(true);
        setErrorMsg('');
        try {
            await axiosClient.post('/booking', {
                serviceName: selectedService.name,
                price: selectedService.price ?? 0,
                bookingTime: selectedSlot.isoString,
                note,
            });
            setSuccessMsg('🎉 Đặt lịch thành công! Chúng tôi sẽ liên hệ xác nhận sớm nhất.');
            setStep(4);
        } catch (err) {
            setErrorMsg(err.response?.data?.message || err.response?.data || 'Đặt lịch thất bại, vui lòng thử lại!');
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div style={{ minHeight: '80vh', paddingBottom: 80 }}>
            {/* ── Hero Banner ── */}
            <motion.div
                initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.8 }}
                style={{
                    background: 'linear-gradient(135deg, #f9c5d1 0%, #fce4ec 50%, #fff 100%)',
                    padding: '60px 0 40px',
                    textAlign: 'center'
                }}
            >
                <motion.h1
                    initial={{ y: -30, opacity: 0 }} animate={{ y: 0, opacity: 1 }} transition={{ delay: 0.2, duration: 0.6 }}
                    style={{ color: '#c2185b', fontFamily: 'Georgia, serif', fontSize: 38, marginBottom: 8 }}
                >
                    🌸 Đặt Lịch Làm Nail
                </motion.h1>
                <motion.p
                    initial={{ y: 10, opacity: 0 }} animate={{ y: 0, opacity: 1 }} transition={{ delay: 0.4 }}
                    style={{ color: '#888', fontSize: 16 }}
                >
                    Chọn dịch vụ yêu thích và thời gian phù hợp với bạn
                </motion.p>
            </motion.div>

            <div className="container" style={{ marginTop: 40 }}>

                {/* ── Step Bar ── */}
                {step <= 3 && <StepBar currentStep={step} steps={steps} />}

                {/* ══ STEP 1: Chọn Dịch Vụ ══ */}
                <AnimatePresence mode="wait">
                    {step === 1 && (
                        <motion.div key="step1" initial={{ opacity: 0, x: 40 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: -40 }}>
                            <h3 style={{ color: '#c2185b', marginBottom: 24, textAlign: 'center' }}>Chọn Dịch Vụ</h3>
                            {services.length === 0 ? (
                                <div style={{ textAlign: 'center', color: '#aaa', padding: 40 }}>
                                    Chưa có dịch vụ nào. Admin vui lòng thêm mẫu nail trước.
                                </div>
                            ) : (
                                <div className="row">
                                    {services.map((s, i) => (
                                        <ServiceCard
                                            key={s.id}
                                            index={i}
                                            service={s}
                                            isSelected={selectedService?.id === s.id}
                                            onClick={(svc) => { setSelectedService(svc); setStep(2); }}
                                        />
                                    ))}
                                </div>
                            )}
                        </motion.div>
                    )}

                    {/* ══ STEP 2: Chọn Ngày & Giờ ══ */}
                    {step === 2 && (
                        <motion.div key="step2" initial={{ opacity: 0, x: 40 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: -40 }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
                                <h3 style={{ color: '#c2185b', margin: 0 }}>Chọn Ngày & Giờ</h3>
                                <button onClick={() => setStep(1)} style={{ background: 'none', border: '1px solid #e91e8c', color: '#e91e8c', borderRadius: 8, padding: '6px 16px', cursor: 'pointer' }}>
                                    ← Quay lại
                                </button>
                            </div>

                            <div style={{ marginBottom: 28, textAlign: 'center' }}>
                                <label style={{ color: '#555', fontSize: 14, marginBottom: 8, display: 'block', fontWeight: 600 }}>Chọn ngày làm:</label>
                                <input
                                    type="date"
                                    value={selectedDate}
                                    min={today()}
                                    onChange={e => setSelectedDate(e.target.value)}
                                    style={{
                                        border: '2px solid #f8bbd0', borderRadius: 10, padding: '10px 18px',
                                        fontSize: 15, outline: 'none', color: '#333', cursor: 'pointer'
                                    }}
                                />
                            </div>

                            <h5 style={{ color: '#888', textAlign: 'center', marginBottom: 20 }}>Chọn khung giờ:</h5>
                            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 10, justifyContent: 'center', marginBottom: 32 }}>
                                {slots.map((slot, i) => (
                                    <motion.button
                                        key={i}
                                        custom={i}
                                        variants={fadeUp}
                                        initial="hidden"
                                        animate="visible"
                                        whileHover={{ scale: 1.08 }}
                                        whileTap={{ scale: 0.94 }}
                                        onClick={() => setSelectedSlot(slot)}
                                        style={{
                                            padding: '10px 20px', borderRadius: 10, border: 'none', cursor: 'pointer', fontSize: 14, fontWeight: 600,
                                            background: selectedSlot?.display === slot.display ? '#e91e8c' : '#fce4ec',
                                            color: selectedSlot?.display === slot.display ? '#fff' : '#c2185b',
                                            boxShadow: selectedSlot?.display === slot.display ? '0 4px 14px rgba(194,24,136,0.35)' : 'none',
                                            transition: 'all 0.2s'
                                        }}
                                    >
                                        {slot.display}
                                    </motion.button>
                                ))}
                            </div>

                            <div style={{ textAlign: 'center' }}>
                                <motion.button
                                    onClick={() => setStep(3)}
                                    disabled={!selectedSlot}
                                    whileHover={{ scale: selectedSlot ? 1.05 : 1 }}
                                    whileTap={{ scale: 0.96 }}
                                    className="send_btn"
                                    style={{ opacity: selectedSlot ? 1 : 0.5, cursor: selectedSlot ? 'pointer' : 'not-allowed' }}
                                >
                                    Tiếp theo →
                                </motion.button>
                            </div>
                        </motion.div>
                    )}

                    {/* ══ STEP 3: Xác Nhận ══ */}
                    {step === 3 && (
                        <motion.div key="step3" initial={{ opacity: 0, x: 40 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: -40 }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
                                <h3 style={{ color: '#c2185b', margin: 0 }}>Xác Nhận Đặt Lịch</h3>
                                <button onClick={() => setStep(2)} style={{ background: 'none', border: '1px solid #e91e8c', color: '#e91e8c', borderRadius: 8, padding: '6px 16px', cursor: 'pointer' }}>
                                    ← Quay lại
                                </button>
                            </div>

                            <div className="col-md-8 offset-md-2">
                                <BookingSummary
                                    service={selectedService}
                                    date={selectedDate}
                                    slot={selectedSlot}
                                />

                                <textarea
                                    className="contactus"
                                    placeholder="Ghi chú thêm (tuỳ chọn) — VD: muốn màu hồng pastel, gel 2 lớp..."
                                    value={note}
                                    onChange={e => setNote(e.target.value)}
                                    rows={3}
                                    style={{ width: '100%', resize: 'vertical', marginBottom: 20, borderRadius: 10, border: '2px solid #f8bbd0', padding: '12px 16px' }}
                                />

                                {errorMsg && <div className="alert alert-danger">{errorMsg}</div>}

                                {!isAuthenticated && (
                                    <div className="alert alert-warning" style={{ borderRadius: 10 }}>
                                        Bạn cần <Link to="/login" style={{ color: '#e91e8c' }}>đăng nhập</Link> để xác nhận đặt lịch.
                                    </div>
                                )}

                                <motion.button
                                    className="send_btn"
                                    onClick={handleBooking}
                                    disabled={submitting}
                                    whileHover={{ scale: 1.04 }}
                                    whileTap={{ scale: 0.96 }}
                                    style={{ width: '100%', fontSize: 16 }}
                                >
                                    {submitting ? '⏳ Đang xử lý...' : '✅ Xác Nhận Đặt Lịch'}
                                </motion.button>
                            </div>
                        </motion.div>
                    )}

                    {/* ══ STEP 4: Thành Công ══ */}
                    {step === 4 && (
                        <motion.div key="step4" initial={{ opacity: 0, scale: 0.8 }} animate={{ opacity: 1, scale: 1 }} transition={{ type: 'spring', stiffness: 100 }}
                            style={{ textAlign: 'center', padding: '60px 20px' }}
                        >
                            <motion.div animate={{ rotate: [0, 10, -10, 0] }} transition={{ repeat: 2, duration: 0.4 }} style={{ fontSize: 72, marginBottom: 20 }}>🎉</motion.div>
                            <h2 style={{ color: '#c2185b', fontSize: 28 }}>Đặt Lịch Thành Công!</h2>
                            <p style={{ color: '#888', fontSize: 16, marginBottom: 32 }}>{successMsg}</p>
                            <div style={{ display: 'flex', gap: 16, justifyContent: 'center', flexWrap: 'wrap' }}>
                                <motion.button className="get_btn" onClick={() => { setStep(1); setSelectedService(null); setSelectedSlot(null); setNote(''); setSuccessMsg(''); }} whileHover={{ scale: 1.05 }}>
                                    Đặt Lịch Thêm
                                </motion.button>
                                <motion.button className="get_btn" onClick={() => navigate('/')} whileHover={{ scale: 1.05 }} style={{ background: '#fff', color: '#c2185b', border: '2px solid #e91e8c' }}>
                                    Về Trang Chủ
                                </motion.button>
                            </div>
                        </motion.div>
                    )}
                </AnimatePresence>
            </div>
        </div>
    );
};

export default Booking;
