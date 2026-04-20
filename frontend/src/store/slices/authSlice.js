import { createSlice } from '@reduxjs/toolkit';

const initialState = {
    user: null,             // Chứa thông tin người dùng (id, name, email...)
    accessToken: null,      // JWT Access Token dùng để gọi API
    isAuthenticated: false, // Cờ kiểm tra trạng thái login
    status: 'idle',         // 'idle' | 'loading' | 'succeeded' | 'failed'
    error: null,
};

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        // Action khi Login thành công (từ Form SignIn / JWT Token trả về)
        loginSuccess: (state, action) => {
            state.user = action.payload.user;
            state.accessToken = action.payload.accessToken;
            state.isAuthenticated = true;
            state.error = null;
        },
        // Action khi Logout hoặc Token hết hạn
        logout: (state) => {
            state.user = null;
            state.accessToken = null;
            state.isAuthenticated = false;
            state.error = null;
        },
        // Update thông tin User sau này (đổi tên, avatar)
        updateUserProfile: (state, action) => {
            if (state.user) {
                state.user = { ...state.user, ...action.payload };
            }
        },
        // Set lỗi nếu call api xác thực thất bại
        setAuthError: (state, action) => {
            state.error = action.payload;
        }
    },
});

export const { loginSuccess, logout, updateUserProfile, setAuthError } = authSlice.actions;

export default authSlice.reducer;
