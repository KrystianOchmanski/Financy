"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { MenuItemProps } from "../lib/definitions";

export default function MenuItem({ href, label }: MenuItemProps) {
  const pathname = usePathname();
  const isActive = pathname === href;

  return (
    <Link
      href={href}
      className={`px-3 py-2.5 mx-0.5 font-semibold text-xs lg:text-[16px] rounded-md ${
        isActive ? "text-brand bg-[#F5F8FF]" : "text-gray-600 hover:bg-gray-100"
      }`}
    >
      {label}
    </Link>
  );
}
