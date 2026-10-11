export const publicRoutes = ["/login", "/register"];

export const isPublicRoute = (pathname: string) =>
  publicRoutes.includes(pathname);
