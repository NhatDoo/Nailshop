import React, { useState, useEffect } from 'react';
import axiosClient from '../../utils/axiosClient';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';

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
                    <motion.form
                        initial={{ opacity: 0, height: 0, overflow: 'hidden' }}
                        animate={{ opacity: 1, height: 'auto', overflow: 'visible' }}
                        exit={{ opacity: 0, height: 0, overflow: 'hidden' }}
                        transition={{ duration: 0.3 }}
                        onSubmit={handleCreate}
                        style={{
                            background: '#fce4ec', padding: 24, borderRadius: 12,
                            marginBottom: 28, boxShadow: '0 4px 16px rgba(194,24,91,0.10)'
                        }}
                    >
                        <h5 style={{ color: '#c2185b', fontWeight: 700, marginBottom: 20 }}>➕ Thêm Mẫu Nail mới</h5>
                        <div className="row">
                            <div className="col-md-5 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Tên mẫu nail <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="nd-name"
                                    className="contactus"
                                    placeholder="VD: French White Tip"
                                    required
                                    value={createData.name}
                                    onChange={e => setCreateData({ ...createData, name: e.target.value })}
                                />
                            </div>
                            <div className="col-md-3 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Loại <span style={{ color: 'red' }}>*</span>
                                </label>
                                <select
                                    id="nd-type"
                                    className="contactus"
                                    value={createData.type}
                                    onChange={e => setCreateData({ ...createData, type: e.target.value })}
                                >
                                    {TYPE_OPTIONS.map(t => <option key={t} value={t}>{t}</option>)}
                                </select>
                            </div>
                            <div className="col-md-12 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Mô tả
                                </label>
                                <textarea
                                    id="nd-description"
                                    className="contactus"
                                    placeholder="Mô tả chi tiết mẫu nail..."
                                    rows={2}
                                    value={createData.description}
                                    onChange={e => setCreateData({ ...createData, description: e.target.value })}
                                />
                            </div>
                            <div className="col-md-6 mb-3">
                                <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>
                                    Ảnh mẫu nail <span style={{ color: 'red' }}>*</span>
                                </label>
                                <input
                                    id="nd-image"
                                    type="file"
                                    accept=".jpg,.jpeg,.png,.webp"
                                    className="contactus"
                                    style={{ paddingTop: 8 }}
                                    onChange={handleFileChange}
                                />
                                <small style={{ color: '#888', fontSize: 11 }}>Hỗ trợ: jpg, jpeg, png, webp. Tối đa 5MB.</small>
                            </div>
                            {preview && (
                                <div className="col-md-3 mb-3">
                                    <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#880e4f', fontSize: 13 }}>Xem trước</label>
                                    <img
                                        src={preview}
                                        alt="preview"
                                        style={{ width: '100%', maxHeight: 120, objectFit: 'cover', borderRadius: 8, border: '2px solid #f48fb1' }}
                                    />
                                </div>
                            )}
                        </div>
                        <div style={{ display: 'flex', gap: 10 }}>
                            <button type="submit" className="send_btn" style={{ padding: '8px 24px' }} disabled={saving}>
                                {saving ? '⏳ Đang lưu...' : '💾 Lưu Mẫu Nail'}
                            </button>
                            <button
                                type="button"
                                className="send_btn"
                                style={{ padding: '8px 20px', background: '#9e9e9e' }}
                                onClick={() => { setShowForm(false); setPreview(null); setImageFile(null); }}
                            >
                                Huỷ
                            </button>
                        </div>
                    </motion.form>
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
                                    onError={e => { e.target.src = 'https://via.placeholder.com/200x170?text=No+Image'; }}
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
