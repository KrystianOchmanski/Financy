"use client";

import { SubmitButtonProps } from "../lib/definitions";
import Spinner from "./Spinner";

export function SubmitButton({
  pending,
  children,
  className,
}: SubmitButtonProps) {
  return (
    <button type="submit" disabled={pending} className={className}>
      {pending ? <Spinner className="mr-2" /> : children}
    </button>
  );
}
