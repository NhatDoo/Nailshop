import React from 'react';
import { motion } from 'framer-motion';

const ServiceForm = ({
    editingId,
    formData,
    setFormData,
    handleSave,
    onCancel
}) => {
    return (
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
                    onClick={onCancel}
                >
                    Huỷ
                </button>
            </div>
        </motion.form>
    );
};

export default ServiceForm;
