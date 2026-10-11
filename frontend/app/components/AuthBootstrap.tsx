"use client";

import { useEffect, useState } from "react";
import { usePathname } from "next/navigation";
import { useAuthStore } from "../store/authStore";
import { isPublicRoute } from "../lib/routes";
import {
  redirectToLogin,
  refreshAccessToken,
  SessionExpiredError,
} from "../lib/session";

export default function AuthBootstrap({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const hasAccessToken = useAuthStore((state) => state.accessToken !== null);
  const [refreshFailed, setRefreshFailed] = useState(false);
  const [attempt, setAttempt] = useState(0);

  const needsAccessToken = !isPublicRoute(pathname) && !hasAccessToken;

  useEffect(() => {
    if (!needsAccessToken) {
      return;
    }

    let cancelled = false;
    setRefreshFailed(false);

    refreshAccessToken().catch((error) => {
      if (error instanceof SessionExpiredError) {
        redirectToLogin();
      } else if (!cancelled) {
        setRefreshFailed(true);
      }
    });

    return () => {
      cancelled = true;
    };
  }, [needsAccessToken, attempt]);

  useEffect(() => {
    // A page restored from the back-forward cache would still hold the token after logout.
    const reloadRestoredPage = (event: PageTransitionEvent) => {
      if (event.persisted) {
        window.location.reload();
      }
    };

    window.addEventListener("pageshow", reloadRestoredPage);
    return () => window.removeEventListener("pageshow", reloadRestoredPage);
  }, []);

  if (needsAccessToken && refreshFailed) {
    return (
      <div className="p-8 text-gray-500">
        <p>Could not connect to the server.</p>
        <button
          type="button"
          onClick={() => setAttempt((value) => value + 1)}
          className="underline text-blue-600 pt-2"
        >
          Try again
        </button>
      </div>
    );
  }

  if (needsAccessToken) {
    return <p className="text-gray-500 p-8">Loading...</p>;
  }

  return <>{children}</>;
}
