import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Member Data Integration",
  description: "Local member database console"
};

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
