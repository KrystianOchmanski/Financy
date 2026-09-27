import { useAuthStore } from "../store/authStore";

export const useAuth = () => {
  const { accessToken, setAccessToken, clearAccessToken } = useAuthStore();

  return {
    accessToken,
    setAccessToken,
    clearAccessToken,
  };
};
