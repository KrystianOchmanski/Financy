import { NextRequest, NextResponse } from "next/server";
import { isPublicRoute } from "./app/lib/routes";
import { REFRESH_TOKEN_COOKIE } from "./app/lib/authCookie";
import { buildContentSecurityPolicy } from "./app/lib/contentSecurityPolicy";

export default async function middleware(req: NextRequest) {
  const path = req.nextUrl.pathname;
  const isPublic = isPublicRoute(path);

  // Optimistic check only: the backend validates the session on every API call.
  const refreshToken = req.cookies.get(REFRESH_TOKEN_COOKIE)?.value;

  // Protect every route by default except explicit public pages.
  if (!isPublic && !refreshToken) {
    return NextResponse.redirect(new URL("/login", req.nextUrl));
  }

  if (isPublic && refreshToken) {
    return NextResponse.redirect(new URL("/dashboard", req.nextUrl));
  }

  // Next.js reads the nonce from the request CSP header and adds it to its own scripts.
  const contentSecurityPolicy = buildContentSecurityPolicy(
    btoa(crypto.randomUUID())
  );
  const requestHeaders = new Headers(req.headers);
  requestHeaders.set("Content-Security-Policy", contentSecurityPolicy);

  const response = NextResponse.next({ request: { headers: requestHeaders } });
  response.headers.set("Content-Security-Policy", contentSecurityPolicy);

  return response;
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico|.*\\..*).*)"],
};
