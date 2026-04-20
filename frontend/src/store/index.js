import { configureStore, combineReducers } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import { persistStore, persistReducer, FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER } from 'redux-persist';
// Fix tương thích cho Vite build CJS/ESM
const customStorage = {
    getItem: (key) => {
        return Promise.resolve(window.localStorage.getItem(key));
    },
    setItem: (key, value) => {
        window.localStorage.setItem(key, value);
        return Promise.resolve();
    },
    removeItem: (key) => {
        window.localStorage.removeItem(key);
        return Promise.resolve();
    },
};

// Cấu hình Persist (Lưu trữ Redux State vào LocalStorage)
const persistConfig = {
    key: 'root',
    version: 1,
    storage: customStorage,
    whitelist: ['auth'], // Chỉ lưu state của 'auth' (không lưu các state rác/tạm thời khác)
};

const rootReducer = combineReducers({
    auth: authReducer,
    // Sau này có thể thêm các Slice khác như: bookingReducer, todoReducer, nailDesignReducer...
});

const persistedReducer = persistReducer(persistConfig, rootReducer);

export const store = configureStore({
    reducer: persistedReducer,
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
            serializableCheck: {
                ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
            },
        }),
});

export const persistor = persistStore(store);
