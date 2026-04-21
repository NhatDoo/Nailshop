import React from 'react';
import { motion } from 'framer-motion';

const PromotionForm = ({
    promoData,
    setPromoData,
    savePromo,
    onCancel
}) => {
    return (
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
                        min="1"
                        max="100"
                        required
                        value={promoData.discountPercent}
                        onChange={e => setPromoData({ ...promoData, discountPercent: parseInt(e.target.value) })}
                    />
                </div>
                <div className="col-md-4 mb-3">
                    <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}>Ngày bắt đầu</label>
                    <input
                        className="contactus"
                        type="date"
                        required
                        value={promoData.startDate}
                        onChange={e => setPromoData({ ...promoData, startDate: e.target.value })}
                    />
                </div>
                <div className="col-md-4 mb-3">
                    <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, color: '#01579b', fontSize: 13 }}>Ngày kết thúc</label>
                    <input
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
                    ✅ Áp dụng KM
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

export default PromotionForm;
