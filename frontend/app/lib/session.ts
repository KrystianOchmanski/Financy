import { useAuthStore } from "../store/authStore";

export class SessionExpiredError extends Error {
  constructor() {
    super("Session expired");
    this.name = "SessionExpiredError";
  }
}

let refreshPromise: Promise<string> | null = null;

// The backend rotates refresh tokens, so every caller must share one in-flight refresh.
export function refreshAccessToken() {
  refreshPromise ??= requestAccessToken().finally(() => {
    refreshPromise = null;
  });

  return refreshPromise;
}

async function requestAccessToken(): Promise<string> {
  const response = await fetch("/api/auth/refreshToken", { method: "POST" });

  if (response.status === 401) {
    throw new SessionExpiredError();
  }

  if (!response.ok) {
    throw new Error(`Refreshing access token failed (${response.status})`);
  }

  const { accessToken } = await response.json();
  useAuthStore.getState().setAccessToken(accessToken);

  return accessToken;
}

// A full page load drops all in-memory state: the access token and the SWR cache.
export function redirectToLogin() {
  window.location.replace("/login");
}

export async function logout() {
  const response = await fetch("/api/auth/logout", { method: "POST" });

  if (!response.ok) {
    throw new Error(`Logout failed (${response.status})`);
  }

  redirectToLogin();
}
