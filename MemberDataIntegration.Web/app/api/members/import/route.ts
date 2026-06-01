import { NextResponse } from "next/server";

const apiBaseUrl = process.env.MEMBER_API_BASE_URL ?? "http://127.0.0.1:5199";

export async function POST() {
  const response = await fetch(`${apiBaseUrl}/members/import`, {
    method: "POST",
    headers: { Accept: "application/json" },
    cache: "no-store"
  });

  const body = await response.text();

  return new NextResponse(body, {
    status: response.status,
    headers: {
      "content-type": response.headers.get("content-type") ?? "application/json"
    }
  });
}
