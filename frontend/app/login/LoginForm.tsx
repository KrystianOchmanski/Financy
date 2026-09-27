"use client";

import { useActionState, useEffect, useState } from "react";
import { login } from "./actions";
import Link from "next/link";
import { useAuth } from "../hooks/useAuth";
import { useRouter } from "next/navigation";
import { SubmitButton } from "../components/SubmitButton";

export default function LoginForm() {
  const [state, loginAction, pending] = useActionState(login, undefined);
  const [redirecting, setRedirecting] = useState<boolean>(false);
  const { setAccessToken } = useAuth();
  const router = useRouter();

  useEffect(() => {
    router.prefetch("/dashboard");
  }, []);

  useEffect(() => {
    if (state?.accessToken) {
      // Saving token in memory and redirect
      setAccessToken(state.accessToken);

      setRedirecting(true);
      router.push("/dashboard");
    }
  }, [state?.accessToken]);

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="bg-white p-6 border border-gray-100 shadow-xs rounded-xl w-96">
        <form action={loginAction}>
          <div className="mb-4">
            <label className="block mb-1 font-medium">Email</label>
            <input name="email" className="w-full p-2 border rounded" />

            {state?.errors?.email && (
              <p className="text-red-500">{state.errors.email}</p>
            )}
          </div>

          <div className="mb-4">
            <label className="block mb-1 font-medium">Password</label>
            <input
              type="password"
              name="password"
              className="w-full p-2 border rounded"
            />

            {state?.errors?.password && (
              <p className="text-red-500">{state.errors.password}</p>
            )}
          </div>

          <SubmitButton
            pending={pending || redirecting}
            className="w-full bg-blue-500 text-white p-2 rounded flex justify-center items-center"
          >
            Login
          </SubmitButton>
        </form>
        <Link
          href={"/register"}
          className="underline text-blue-600 block text-center pt-2"
        >
          Register
        </Link>
      </div>
    </div>
  );
}
