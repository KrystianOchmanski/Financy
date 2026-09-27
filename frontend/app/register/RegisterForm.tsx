"use client";

import { useActionState, useEffect } from "react";
import { register } from "./action";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { SubmitButton } from "../components/SubmitButton";

export default function RegisterForm() {
  const router = useRouter();
  const [state, registerAction, pending] = useActionState(register, undefined);

  useEffect(() => {
    if (state?.redirect) {
      setTimeout(() => {
        router.push(state.redirect);
      }, 1000);
    }
  }, [state?.redirect, router]);

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="bg-white p-6 border border-gray-100 shadow-xs rounded-xl w-96">
        <form action={registerAction}>
          <div className="mb-4">
            <label className="block mb-1 font-medium">Name</label>
            <input name="firstName" className="w-full p-2 border rounded" />

            {state?.errors?.firstName && (
              <p className="text-red-500">{state.errors.firstName}</p>
            )}
          </div>

          <div className="mb-4">
            <label className="block mb-1 font-medium">Email</label>
            <input name="email" className="w-full p-2 border rounded" />

            {state?.errors?.email && (
              <p className="text-red-500">{state.errors.email[0]}</p>
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
              <>
                <p className="text-red-500">Password must:</p>
                {state.errors.password.map((error: string, index: number) => (
                  <p key={index} className="text-red-500">
                    - {error}
                  </p>
                ))}
              </>
            )}
          </div>

          {state?.errors?.general && (
            <div className="text-red-500">
              {state.errors.general.map((error: string, index: number) => (
                <p key={index}>{error}</p>
              ))}
            </div>
          )}

          {state?.success && (
            <p className="text-center text-green-600">{state.success}</p>
          )}

          <SubmitButton
            pending={pending}
            className="w-full bg-blue-500 text-white p-2 rounded flex justify-center items-center"
          >
            Register
          </SubmitButton>
        </form>
        <Link
          href={"/login"}
          className="underline text-blue-600 block text-center pt-2"
        >
          Login
        </Link>
      </div>
    </div>
  );
}
