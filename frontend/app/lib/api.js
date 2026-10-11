import axios from "axios";
import { useAuthStore } from "../store/authStore";
import {
  refreshAccessToken,
  redirectToLogin,
  SessionExpiredError,
} from "./session";

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

api.interceptors.request.use(
  async (config) => {
    const token = useAuthStore.getState().accessToken;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      // Concurrent 401s wait for the same refresh, and all of them fail if it fails.
      const newToken = await refreshAccessToken();
      originalRequest.headers.Authorization = `Bearer ${newToken}`;

      return api(originalRequest);
    } catch (refreshError) {
      if (refreshError instanceof SessionExpiredError) {
        redirectToLogin();
      }
      return Promise.reject(refreshError);
    }
  }
);

export default api;
