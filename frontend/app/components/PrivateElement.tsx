"use client";

import { usePathname } from "next/navigation";
import React from "react";
import { isPublicRoute } from "../lib/routes";

// Hides UI on public pages. This is presentation only; it does not protect anything.
export default function PrivateElement({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();

  if (isPublicRoute(pathname)) {
    return null;
  }

  return <>{children}</>;
}
