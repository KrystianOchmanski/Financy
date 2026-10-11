const isProduction = process.env.NODE_ENV === "production";

// "__Host-" makes the browser reject the cookie unless it is Secure, host-only and scoped to "/".
export const REFRESH_TOKEN_COOKIE = isProduction
  ? "__Host-refreshToken"
  : "refreshToken";

// Keep maxAge in sync with JwtSettings.RefreshTokenExpiryDays on the backend.
export const refreshTokenCookieOptions = {
  httpOnly: true,
  secure: isProduction,
  sameSite: "lax" as const,
  path: "/",
  maxAge: 60 * 60 * 24 * 7,
};

// Deleting a "__Host-" cookie also requires the Secure attribute, so reuse the same options.
export const expiredRefreshTokenCookieOptions = {
  ...refreshTokenCookieOptions,
  maxAge: 0,
};
