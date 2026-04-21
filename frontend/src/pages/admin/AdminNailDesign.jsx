import React, { useState, useEffect } from 'react';
import axiosClient from '../../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import NailDesignForm from '../../components/admin/NailDesignForm';

const TYPE_OPTIONS = ['Preset', 'Custom'];

const AdminNailDesign = () => {
    const { user } = useSelector(s => s.auth);
    const navigate = useNavigate();

    const [designs, setDesigns] = useState([]);
    const [loading, setLoading] = useState(false);

    // Create form
    const [showForm, setShowForm] = useState(false);
    const [createData, setCreateData] = useState({ name: '', type: 'Preset', description: '' });
    const [imageFile, setImageFile] = useState(null);
    const [preview, setPreview] = useState(null);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        if (user?.role !== 'Admin') { navigate('/'); return; }
        loadDesigns();
    }, [user]);

    const loadDesigns = async () => {
        setLoading(true);
        try {
            const { data } = await axiosClient.get('/naildesign');
            setDesigns(data);
        } catch (e) {
            console.error(e);
        } finally {
            setLoading(false);
        }
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (!file) return;
        setImageFile(file);
        setPreview(URL.createObjectURL(file));
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        if (!imageFile) { alert('Vui lòng chọn ảnh mẫu nail.'); return; }
        setSaving(true);
        try {
            const fd = new FormData();
            fd.append('Name', createData.name);
            fd.append('Type', createData.type);
            fd.append('Description', createData.description);
            fd.append('Image', imageFile);

            await axiosClient.post('/naildesign', fd, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });

            setShowForm(false);
            setCreateData({ name: '', type: 'Preset', description: '' });
            setImageFile(null);
            setPreview(null);
            loadDesigns();
        } catch (err) {
            alert('Lỗi: ' + (err.response?.data || err.message));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Xác nhận xóa mẫu nail này?')) return;
        try {
            await axiosClient.delete(`/naildesign/${id}`);
            loadDesigns();
        } catch (err) {
            alert('Lỗi xóa: ' + (err.response?.data || err.message));
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
            {/* Header */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
                <h2 style={{ color: '#c2185b', margin: 0 }}>🎨 Quản lý Nail Design (Admin)</h2>
                <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    className="send_btn"
                    onClick={() => { setShowForm(!showForm); }}
                    style={{ padding: '8px 18px' }}
                >
                    {showForm ? 'Đóng form' : '+ Thêm Mẫu Nail'}
                </motion.button>
            </div>

            {/* Create Form */}
            <AnimatePresence>
                {showForm && (
                    <NailDesignForm
                        createData={createData}
                        setCreateData={setCreateData}
                        handleFileChange={handleFileChange}
                        handleCreate={handleCreate}
                        preview={preview}
                        saving={saving}
                        onCancel={() => { setShowForm(false); setPreview(null); setImageFile(null); }}
                    />
                )}
            </AnimatePresence>

            {/* Grid */}
            {loading && <p style={{ textAlign: 'center', color: '#c2185b' }}>Đang tải...</p>}

            <div
                style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))',
                    gap: 20
                }}
            >
                <AnimatePresence>
                    {designs.map(d => (
                        <motion.div
                            key={d.id}
                            initial={{ opacity: 0, scale: 0.9 }}
                            animate={{ opacity: 1, scale: 1 }}
                            exit={{ opacity: 0, scale: 0.9 }}
                            transition={{ duration: 0.3 }}
                            style={{
                                background: '#fff',
                                borderRadius: 12,
                                overflow: 'hidden',
                                boxShadow: '0 4px 16px rgba(0,0,0,0.10)',
                                display: 'flex',
                                flexDirection: 'column'
                            }}
                        >
                            <div style={{ position: 'relative', height: 170, background: '#f5f5f5' }}>
                                <img
                                    src={d.imageUrl}
                                    alt={d.name}
                                    style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                                    onError={e => { e.target.src = '/images/logo.png'; }}
                                />
                                <span style={{
                                    position: 'absolute', top: 8, right: 8,
                                    background: d.type === 'Preset' ? '#c2185b' : '#7b1fa2',
                                    color: '#fff', fontSize: 11, padding: '2px 8px', borderRadius: 20, fontWeight: 600
                                }}>
                                    {d.type}
                                </span>
                            </div>
                            <div style={{ padding: '10px 12px', flex: 1 }}>
                                <b style={{ fontSize: 14, display: 'block', marginBottom: 4 }}>{d.name}</b>
                                {d.description && (
                                    <small style={{ color: '#888', fontSize: 12 }}>{d.description}</small>
                                )}
                            </div>
                            <div style={{ padding: '8px 12px', borderTop: '1px solid #f0f0f0' }}>
                                <button
                                    onClick={() => handleDelete(d.id)}
                                    className="btn btn-sm btn-outline-danger"
                                    style={{ fontSize: 12, width: '100%' }}
                                >
                                    🗑 Xóa
                                </button>
                            </div>
                        </motion.div>
                    ))}
                </AnimatePresence>
            </div>

            {!loading && designs.length === 0 && (
                <div style={{ textAlign: 'center', color: '#aaa', marginTop: 60 }}>
                    <p style={{ fontSize: 48 }}>🎨</p>
                    <p>Chưa có mẫu nail nào. Hãy thêm mẫu đầu tiên!</p>
                </div>
            )}
        </motion.div>
    );
};

export default AdminNailDesign;
