import axios from "axios";
import { useAuthStore } from "../store/authStore";

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

let isRefreshing = false;
let refreshSubscribers = [];

const onRefreshed = (newToken) => {
  refreshSubscribers.forEach((callback) => callback(newToken));
  refreshSubscribers = [];
};

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

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve) => {
          refreshSubscribers.push((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(api(originalRequest));
          });
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const data = await fetch(
          `${process.env.NEXT_PUBLIC_BASE_URL}/api/auth/refreshToken`,
          {
            credentials: "include",
          }
        );

        if (!data.ok) throw Error("Refreshing token failed");

        const { accessToken: newToken } = await data.json();
        useAuthStore.getState().setAccessToken(newToken);
        onRefreshed(newToken);

        return api(originalRequest);
      } catch (refreshError) {
        useAuthStore.getState().clearAccessToken();
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default api;
