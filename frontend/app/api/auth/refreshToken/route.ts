import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";
import {
  REFRESH_TOKEN_COOKIE,
  expiredRefreshTokenCookieOptions,
  refreshTokenCookieOptions,
} from "@/app/lib/authCookie";
import { isSameOriginRequest } from "@/app/lib/sameOrigin";

export async function POST(request: NextRequest) {
  if (!isSameOriginRequest(request)) {
    return NextResponse.json({ error: "Forbidden" }, { status: 403 });
  }

  const cookieStore = await cookies();
  const refreshToken = cookieStore.get(REFRESH_TOKEN_COOKIE)?.value;

  if (!refreshToken) {
    return NextResponse.json({ error: "No refresh token" }, { status: 401 });
  }

  let response: Response;
  try {
    response = await fetch(`${process.env.API_URL}/auth/refreshToken`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(refreshToken),
    });
  } catch {
    return NextResponse.json(
      { error: "Authentication service unavailable" },
      { status: 503 }
    );
  }

  // Only a rejected token ends the session; outages keep the cookie so the user can retry.
  if (response.status === 401) {
    cookieStore.set(REFRESH_TOKEN_COOKIE, "", expiredRefreshTokenCookieOptions);
    return NextResponse.json({ error: "Invalid refresh token" }, { status: 401 });
  }

  if (!response.ok) {
    return NextResponse.json(
      { error: "Authentication service unavailable" },
      { status: 503 }
    );
  }

  const { accessToken, refreshToken: rotatedRefreshToken } =
    await response.json();

  cookieStore.set(
    REFRESH_TOKEN_COOKIE,
    rotatedRefreshToken,
    refreshTokenCookieOptions
  );

  return NextResponse.json({ accessToken });
}
