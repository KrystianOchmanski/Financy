"use client";

import { usePathname } from "next/navigation";
import React from "react";

export default function PrivateElement({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();

  const publicPaths = ["/login", "/register"];
  const isPublicPath = publicPaths.includes(pathname);

  if (!isPublicPath) {
    return <>{children}</>;
  }
}
