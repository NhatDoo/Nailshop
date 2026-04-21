import React, { useState, useEffect } from 'react';
import axiosClient from '../../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';

// Components
import ServiceForm from '../../components/admin/services/ServiceForm';
import PromotionForm from '../../components/admin/services/PromotionForm';

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
                    <ServiceForm
                        editingId={editingId}
                        formData={formData}
                        setFormData={setFormData}
                        handleSave={handleSave}
                        onCancel={() => { setShowForm(false); setEditingId(null); setFormData({ name: '', description: '', price: 0, duration: 30, category: 0 }); }}
                    />
                )}
            </AnimatePresence>

            <AnimatePresence>
                {promoForm && (
                    <PromotionForm
                        promoData={promoData}
                        setPromoData={setPromoData}
                        savePromo={savePromo}
                        onCancel={() => setPromoForm(null)}
                    />
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
