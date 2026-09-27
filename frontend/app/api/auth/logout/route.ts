import { cookies } from "next/headers";
import { NextResponse } from "next/server";

export async function GET() {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get("refreshToken")?.value;

  fetch(`${process.env.API_URL}/auth/logout`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(refreshToken),
  });

  cookieStore.delete("refreshToken");

  return NextResponse.json({ status: 200, message: "Logged out" });
}
