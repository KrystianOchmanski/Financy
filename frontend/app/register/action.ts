"use server";

import { registerSchema } from "../lib/definitions";

export async function register(prevState: any, formData: FormData) {
  const result = registerSchema.safeParse(Object.fromEntries(formData));

  if (!result.success) {
    return {
      errors: {
        firstName: result.error.flatten().fieldErrors.firstName || [],
        email: result.error.flatten().fieldErrors.email || [],
        password: result.error.flatten().fieldErrors.password || [],
      },
    };
  }

  const { firstName, email, password } = result.data;

  const response = await fetch(`${process.env.API_URL}/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password, firstName }),
  });

  if (!response.ok) {
    const errorMessages = await response.json();

    return {
      errors: {
        general: errorMessages,
      },
    };
  }

  return {
    success: "Registration successful. Redirecting...",
    redirect: "/login",
  };
}
