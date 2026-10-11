import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";
import {
  REFRESH_TOKEN_COOKIE,
  expiredRefreshTokenCookieOptions,
} from "@/app/lib/authCookie";
import { isSameOriginRequest } from "@/app/lib/sameOrigin";

export async function POST(request: NextRequest) {
  if (!isSameOriginRequest(request)) {
    return NextResponse.json({ error: "Forbidden" }, { status: 403 });
  }

  const cookieStore = await cookies();
  const refreshToken = cookieStore.get(REFRESH_TOKEN_COOKIE)?.value;

  if (refreshToken) {
    try {
      const response = await fetch(`${process.env.API_URL}/auth/logout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(refreshToken),
      });

      if (!response.ok) {
        console.error(`Revoking refresh token failed (${response.status})`);
      }
    } catch (error) {
      console.error("Revoking refresh token failed", error);
    }
  }

  // The browser session ends even if the backend could not revoke the token.
  cookieStore.set(REFRESH_TOKEN_COOKIE, "", expiredRefreshTokenCookieOptions);

  return NextResponse.json({ message: "Logged out" });
}
