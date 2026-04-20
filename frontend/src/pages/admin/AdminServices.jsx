import React, { useState, useEffect } from 'react';
import axiosClient from '../../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';

const AdminServices = () => {
    const { user } = useSelector(s => s.auth);
    const navigate = useNavigate();
    const [services, setServices] = useState([]);

    // Form states
    const [showForm, setShowForm] = useState(false);
    const [editingId, setEditingId] = useState(null);
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        price: 0,
        duration: 30,
        category: 0 // 0=MANICURE, 1=PEDICURE, 2=NAIL_ART
    });

    const [promoForm, setPromoForm] = useState(null); // id of service being added a promo
    const [promoData, setPromoData] = useState({
        title: '', description: '', discountPercent: 0, startDate: '', endDate: ''
    });

    useEffect(() => {
        if (user?.role !== 'Admin') {
            navigate('/');
            return;
        }
        loadServices();
    }, [user]);

    const loadServices = async () => {
        try {
            const { data } = await axiosClient.get('/nailservice');
            setServices(data);
        } catch (e) {
            console.error(e);
        }
    };

    const handleSave = async (e) => {
        e.preventDefault();
        try {
            if (editingId) {
                await axiosClient.put(`/nailservice/${editingId}`, formData);
            } else {
                await axiosClient.post('/nailservice', formData);
            }
            setShowForm(false);
            setEditingId(null);
            setFormData({ name: '', description: '', price: 0, duration: 30, category: 0 });
            loadServices();
        } catch (err) {
            alert("Lỗi: " + (err.response?.data || err.message));
        }
    };

    const handleEdit = (svc) => {
        setFormData({
            name: svc.name,
            description: svc.description || '',
            price: svc.price,
            duration: svc.duration,
            category: svc.category === 'MANICURE' ? 0 : svc.category === 'PEDICURE' ? 1 : 2
        });
        setEditingId(svc.id);
        setShowForm(true);
    };

    const handleToggle = async (id) => {
        await axiosClient.patch(`/nailservice/${id}/toggle`);
        loadServices();
    };

    const savePromo = async (e) => {
        e.preventDefault();
        try {
            await axiosClient.post(`/nailservice/${promoForm}/promotions`, promoData);
            setPromoForm(null);
            setPromoData({ title: '', description: '', discountPercent: 0, startDate: '', endDate: '' });
            loadServices();
        } catch (err) {
            alert("Lỗi thêm mã: " + (err.response?.data || err.message));
        }
    };

    return (
        <motion.div
            className="container"
            style={{ padding: '40px 0', minHeight: '80vh' }}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.5 }}
        >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <h2 style={{ color: '#c2185b' }}>Quản lý Dịch vụ & Khuyến mãi (Admin)</h2>
                <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    className="send_btn"
                    onClick={() => { setShowForm(!showForm); setEditingId(null); }}
                    style={{ padding: '8px 16px' }}
                >
                    {showForm ? 'Đóng form' : '+ Thêm Dịch vụ'}
                </motion.button>
            </div>

            <AnimatePresence>
                {showForm && (
                    <motion.form
                        initial={{ opacity: 0, height: 0, overflow: 'hidden' }}
                        animate={{ opacity: 1, height: 'auto', overflow: 'visible' }}
                        exit={{ opacity: 0, height: 0, overflow: 'hidden' }}
                        transition={{ duration: 0.3 }}
                        onSubmit={handleSave}
                        style={{ background: '#fce4ec', padding: 24, borderRadius: 12, marginBottom: 24, boxShadow: '0 4px 16px rgba(194,24,91,0.08)' }}
                    >
                        <h5 style={{ color: '#c2185b', marginBottom: 20, fontWeight: 700 }}>
                            {editingId ? '✏️ Sửa Dịch vụ' : '➕ Thêm Dịch vụ mới'}
                        </h5>
                        <div className="row">
                            <div className="col-md-6 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Tên dịch vụ <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="svc-name"
                                    className="contactus"
                                    placeholder="VD: Sơn gel cao cấp"
                                    required
                                    value={formData.name}
                                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                                />
                            </div>
                            <div className="col-md-6 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Danh mục <span style={{ color: 'red' }}>*</span>
                                </label>
                                <select
                                    id="svc-category"
                                    className="contactus"
                                    value={formData.category}
                                    onChange={e => setFormData({ ...formData, category: parseInt(e.target.value) })}
                                >
                                    <option value={0}>💅 Manicure (Làm móng tay)</option>
                                    <option value={1}>🦶 Pedicure (Làm móng chân)</option>
                                    <option value={2}>🎨 Nail Art (Vẽ nghệ thuật)</option>
                                </select>
                            </div>
                            <div className="col-md-4 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Giá (VND) <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="svc-price"
                                    className="contactus"
                                    type="number"
                                    placeholder="VD: 150000"
                                    min="0"
                                    required
                                    value={formData.price}
                                    onChange={e => setFormData({ ...formData, price: Number(e.target.value) })}
                                />
                            </div>
                            <div className="col-md-4 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Thời gian (phút) <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="svc-duration"
                                    className="contactus"
                                    type="number"
                                    placeholder="VD: 60"
                                    min="1"
                                    max="65535"
                                    required
                                    value={formData.duration}
                                    onChange={e => setFormData({ ...formData, duration: Number(e.target.value) })}
                                />
                            </div>
                            <div className="col-md-12 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Mô tả dịch vụ
                                </label>
                                <textarea
                                    id="svc-description"
                                    className="contactus"
                                    placeholder="Nhập mô tả chi tiết về dịch vụ..."
                                    value={formData.description}
                                    onChange={e => setFormData({ ...formData, description: e.target.value })}
                                    rows={3}
                                />
                            </div>
                        </div>
                        <div style={{ display: 'flex', gap: 10 }}>
                            <button type="submit" className="send_btn" style={{ padding: '8px 24px' }}>
                                💾 Lưu Dịch vụ
                            </button>
                            <button
                                type="button"
                                className="send_btn"
                                style={{ padding: '8px 20px', background: '#9e9e9e' }}
                                onClick={() => { setShowForm(false); setEditingId(null); setFormData({ name: '', description: '', price: 0, duration: 30, category: 0 }); }}
                            >
                                Huỷ
                            </button>
                        </div>
                    </motion.form>
                )}
            </AnimatePresence>

            <AnimatePresence>
                {promoForm && (
                    <motion.form
                        initial={{ opacity: 0, scale: 0.95 }}
                        animate={{ opacity: 1, scale: 1 }}
                        exit={{ opacity: 0, scale: 0.95 }}
                        transition={{ duration: 0.3 }}
                        onSubmit={savePromo}
                        style={{ background: '#e1f5fe', padding: 24, borderRadius: 12, marginBottom: 24, boxShadow: '0 4px 16px rgba(2,119,189,0.10)' }}
                    >
                        <h5 style={{ color: '#0277bd', marginBottom: 20, fontWeight: 700 }}>🏷️ Thêm Khuyến mãi cho Dịch vụ</h5>
                        <div className="row">
                            <div className="col-md-6 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}>
                                    Tên khuyến mãi <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="promo-title"
                                    className="contactus"
                                    placeholder="VD: Ưu đãi ngày lễ 8/3"
                                    required
                                    value={promoData.title}
                                    onChange={e => setPromoData({ ...promoData, title: e.target.value })}
                                />
                            </div>
                            <div className="col-md-3 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}>
                                    % Giảm giá <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="promo-discount"
                                    className="contactus"
                                    type="number"
                                    placeholder="VD: 20"
                                    min="0"
                                    max="100"
                                    required
                                    value={promoData.discountPercent}
                                    onChange={e => setPromoData({ ...promoData, discountPercent: Number(e.target.value) })}
                                />
                            </div>
                            <div className="col-md-6 mb-3">
                                <label
                                    htmlFor="promo-start"
                                    style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}
                                >
                                    Ngày bắt đầu <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="promo-start"
                                    className="contactus"
                                    type="date"
                                    required
                                    value={promoData.startDate}
                                    onChange={e => setPromoData({ ...promoData, startDate: e.target.value })}
                                />
                            </div>
                            <div className="col-md-6 mb-3">
                                <label
                                    htmlFor="promo-end"
                                    style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}
                                >
                                    Ngày kết thúc <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="promo-end"
                                    className="contactus"
                                    type="date"
                                    required
                                    value={promoData.endDate}
                                    onChange={e => setPromoData({ ...promoData, endDate: e.target.value })}
                                />
                            </div>
                        </div>
                        <div style={{ display: 'flex', gap: 10 }}>
                            <button type="submit" className="send_btn" style={{ padding: '8px 24px', background: '#0288d1' }}>
                                💾 Lưu Khuyến mãi
                            </button>
                            <button type="button" onClick={() => setPromoForm(null)} className="send_btn" style={{ background: '#9e9e9e', padding: '8px 24px' }}>
                                Huỷ
                            </button>
                        </div>
                    </motion.form>
                )}
            </AnimatePresence>

            <table className="table table-bordered bg-white">
                <thead>
                    <tr style={{ background: '#f8bbd0' }}>
                        <th>Tên</th>
                        <th>Danh mục</th>
                        <th>Giá</th>
                        <th>Thời lượng</th>
                        <th>Trạng thái</th>
                        <th>Khuyến mãi</th>
                        <th>Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    <AnimatePresence>
                        {services.map(s => (
                            <motion.tr
                                key={s.id}
                                initial={{ opacity: 0, x: -20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: 20 }}
                                transition={{ duration: 0.3 }}
                            >
                                <td>
                                    <b>{s.name}</b><br />
                                    <small className="text-muted" style={{ fontSize: 11 }}>{s.description}</small>
                                </td>
                                <td>{s.category}</td>
                                <td style={{ color: '#e91e8c', fontWeight: 'bold' }}>{(s.price * 1000).toLocaleString()} đ</td>
                                <td>{s.duration} phút</td>
                                <td>
                                    <span style={{ color: s.isActive ? 'green' : 'red', fontWeight: 'bold' }}>
                                        {s.isActive ? 'Active' : 'Hidden'}
                                    </span>
                                </td>
                                <td>
                                    {s.promotions?.length === 0 && <span className="text-muted">Không có</span>}
                                    {s.promotions?.map(p => (
                                        <div key={p.id} style={{ fontSize: 13, background: '#e1f5fe', padding: '2px 6px', borderRadius: 4, marginBottom: 4 }}>
                                            <b>{p.title}</b> (-{p.discountPercent}%) <br />
                                            {new Date(p.startDate).toLocaleDateString()} - {new Date(p.endDate).toLocaleDateString()}
                                        </div>
                                    ))}
                                </td>
                                <td style={{ minWidth: 200 }}>
                                    <button onClick={() => handleEdit(s)} className="btn btn-sm btn-outline-primary mr-1">Sửa</button>
                                    <button onClick={() => setPromoForm(s.id)} className="btn btn-sm btn-outline-info mr-1">+KM</button>
                                    <button onClick={() => handleToggle(s.id)} className="btn btn-sm btn-outline-secondary">
                                        {s.isActive ? 'Ẩn' : 'Hiện'}
                                    </button>
                                </td>
                            </motion.tr>
                        ))}
                    </AnimatePresence>
                    {services.length === 0 && <tr><td colSpan={7} className="text-center">Chưa có dữ liệu</td></tr>}
                </tbody>
            </table>
        </motion.div>
    );
};

export default AdminServices;
