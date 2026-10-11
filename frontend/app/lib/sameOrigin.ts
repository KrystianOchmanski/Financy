import { NextRequest } from "next/server";

// Rejects cross-site requests to cookie-authenticated route handlers (CSRF).
export function isSameOriginRequest(request: NextRequest) {
  const origin = request.headers.get("origin");
  const host =
    request.headers.get("x-forwarded-host") ?? request.headers.get("host");

  if (!origin || !host) {
    return false;
  }

  try {
    return new URL(origin).host === host;
  } catch {
    return false;
  }
}
