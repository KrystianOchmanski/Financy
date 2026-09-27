import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

export async function GET(request: NextRequest) {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get("refreshToken")?.value;

  if (!refreshToken) {
    //return NextResponse.json({ error: "No refresh token" }, { status: 401 });
    return NextResponse.redirect(new URL("/login", request.url));
  }

  try {
    const response = await fetch(`${process.env.API_URL}/auth/refreshToken`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(refreshToken),
    });

    if (!response.ok) {
      cookieStore.delete("refreshToken");
      return NextResponse.json(
        { error: "Invalid refresh token" },
        { status: 401 }
      );
    }

    const { accessToken } = await response.json();

    return NextResponse.json({ accessToken });
  } catch (err) {
    return NextResponse.json(
      { error: "Something went wrong" },
      { status: 500 }
    );
  }
}
