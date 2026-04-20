import axios from 'axios';
import { store } from '../store';
import { logout } from '../store/slices/authSlice';

// Cấu hình URL mặc định tới Backend .NET (cổng 5017 HTTP hoặc tuỳ bạn chỉnh)
const axiosClient = axios.create({
    baseURL: 'http://localhost:5017/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor cho REQUEST: Luôn tự động đính kèm Token nếu user đã login
axiosClient.interceptors.request.use(
    (config) => {
        // Lấy state trực tiếp từ Redux store chứ không cần dùng localStorage.getItem()
        const state = store.getState();
        const token = state.auth.accessToken;

        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`; // Cú pháp chuẩn của JWT Bearer
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Interceptor cho RESPONSE: Bắt lỗi toàn cục, ví dụ Token hết hạn
axiosClient.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        // Nếu API trả về 401 Unauthorized (Lỗi token, hết hạn hoặc không hợp lệ)
        if (error.response && error.response.status === 401) {
            console.warn("Token JWT đã hết hạn hoặc không hợp lệ, đang tiến hành Đăng xuất...");
            // Kích hoạt action logout của Redux để clear Token và đẩy giao diện về khách
            store.dispatch(logout());

            // (Optional) tự động redirect về trang chủ hoặc refresh page nếu ở ngoài component
            // window.location.href = '/login'; 
        }
        return Promise.reject(error);
    }
);

export default axiosClient;
