import React from 'react';
import { motion } from 'framer-motion';

const TYPE_OPTIONS = ['Preset', 'Custom'];

const NailDesignForm = ({
    createData,
    setCreateData,
    handleFileChange,
    handleCreate,
    preview,
    saving,
    onCancel
}) => {
    return (
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
                    onClick={onCancel}
                >
                    Huỷ
                </button>
            </div>
        </motion.form>
    );
};

export default NailDesignForm;
