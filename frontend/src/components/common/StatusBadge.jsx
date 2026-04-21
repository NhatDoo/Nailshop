import React from 'react';

const StatusBadge = ({ status }) => {
    switch (status) {
        case 'Pending':
            return <span className="badge badge-warning" style={{ fontSize: 13, padding: '6px 10px' }}>Chờ xác nhận</span>;
        case 'Confirmed':
            return <span className="badge badge-primary" style={{ fontSize: 13, padding: '6px 10px' }}>Đã xác nhận</span>;
        case 'Completed':
            return <span className="badge badge-success" style={{ fontSize: 13, padding: '6px 10px' }}>Đã hoàn thành</span>;
        case 'Cancelled':
            return <span className="badge badge-danger" style={{ fontSize: 13, padding: '6px 10px' }}>Đã huỷ</span>;
        default:
            return <span className="badge badge-secondary">{status}</span>;
    }
};

export default StatusBadge;
