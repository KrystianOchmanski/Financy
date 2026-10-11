"use server";

import { cookies } from "next/headers";
import { loginSchema } from "@/app/lib/definitions";
import {
  REFRESH_TOKEN_COOKIE,
  refreshTokenCookieOptions,
} from "@/app/lib/authCookie";

export async function login(prevState: any, formData: FormData) {
  const result = loginSchema.safeParse(Object.fromEntries(formData));

  if (!result.success) {
    return {
      errors: result.error.flatten().fieldErrors,
    };
  }

  const { email, password } = result.data;

  const response = await fetch(`${process.env.API_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) {
    return {
      errors: {
        email: ["Invalid email or password"],
      },
    };
  }

  const { accessToken, refreshToken } = await response.json();

  const cookieStore = await cookies();
  cookieStore.set(REFRESH_TOKEN_COOKIE, refreshToken, refreshTokenCookieOptions);

  return { accessToken };
}
