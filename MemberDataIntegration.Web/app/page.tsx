"use client";

import { useEffect, useMemo, useState } from "react";

type Member = {
  id: number;
  email: string;
  memberPressName: string | null;
  mailchimpName: string | null;
  awardForceName: string | null;
  phone: string | null;
  isMemberPress: boolean;
  isMailchimp: boolean;
  isAwardForce: boolean;
};

type StatusTone = "default" | "warn" | "error";

type MemberPressTestResult = {
  statusCode: number;
  isSuccess: boolean;
  bodyPreview: string;
};

function Badge({ value }: { value: boolean }) {
  return <span className={`badge ${value ? "on" : "off"}`}>{value ? "Y" : "-"}</span>;
}

function EmptyCell({ value }: { value: string | number | null | undefined }) {
  return value ? <>{value}</> : <span className="muted">-</span>;
}

export default function Home() {
  const [members, setMembers] = useState<Member[]>([]);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("Ready");
  const [tone, setTone] = useState<StatusTone>("default");
  const [loading, setLoading] = useState(false);

  const filteredMembers = useMemo(() => {
    const query = search.trim().toLowerCase();

    if (!query) {
      return members;
    }

    return members.filter((member) =>
      [
        member.email,
        member.memberPressName,
        member.mailchimpName,
        member.awardForceName,
        member.phone
      ]
        .join(" ")
        .toLowerCase()
        .includes(query)
    );
  }, [members, search]);

  async function requestJson<T>(url: string, options?: RequestInit): Promise<T> {
    const response = await fetch(url, {
      headers: { Accept: "application/json" },
      ...options
    });

    if (!response.ok) {
      const body = await response.text();
      throw new Error(body || `${response.status} ${response.statusText}`);
    }

    return response.json() as Promise<T>;
  }

  async function loadMembers() {
    setLoading(true);
    setTone("default");
    setStatus("Loading database...");

    try {
      const data = await requestJson<Member[]>("/api/members/db");
      setMembers(data);
      setStatus(`Loaded ${data.length} member${data.length === 1 ? "" : "s"}.`);
    } catch (error) {
      setTone("error");
      setStatus(error instanceof Error ? error.message : "Failed to load members.");
    } finally {
      setLoading(false);
    }
  }

  async function importMemberPress() {
    setLoading(true);
    setTone("warn");
    setStatus("Importing from MemberPress...");

    try {
      const importedCount = await requestJson<number>("/api/members/import", { method: "POST" });
      const data = await requestJson<Member[]>("/api/members/db");
      setMembers(data);
      setTone("default");
      setStatus(`MemberPress import completed. Source rows processed: ${importedCount}.`);
    } catch (error) {
      setTone("error");
      setStatus(error instanceof Error ? error.message : "Import failed.");
    } finally {
      setLoading(false);
    }
  }

  async function testMemberPress(url: string, label: string) {
    setLoading(true);
    setTone("warn");
    setStatus(`Testing MemberPress ${label}...`);

    try {
      const result = await requestJson<MemberPressTestResult>(url);
      setTone(result.isSuccess ? "default" : "error");
      setStatus(
        `${label}: ${result.statusCode} ${result.isSuccess ? "OK" : "Failed"}${result.bodyPreview ? ` - ${result.bodyPreview}` : ""}`
      );
    } catch (error) {
      setTone("error");
      setStatus(error instanceof Error ? error.message : `${label} test failed.`);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadMembers();
  }, []);

  return (
    <main className="shell">
      <header className="page-header">
        <div>
          <h1>Member Data Integration</h1>
          <p className="subtitle">Local database console</p>
        </div>
        <div className="actions">
          <button
            className="button"
            type="button"
            onClick={() => testMemberPress("/api/memberpress/me", "Me")}
            disabled={loading}
          >
            Test Me
          </button>
          <button
            className="button"
            type="button"
            onClick={() => testMemberPress("/api/memberpress/members", "Get Members")}
            disabled={loading}
          >
            Get Members
          </button>
          <button className="button" type="button" onClick={loadMembers} disabled={loading}>
            Refresh
          </button>
          <button className="button primary" type="button" onClick={importMemberPress} disabled={loading}>
            Import MemberPress
          </button>
        </div>
      </header>

      <section className="summary" aria-label="Member summary">
        <div className="metric">
          <span>Total</span>
          <strong>{members.length}</strong>
        </div>
        <div className="metric">
          <span>MemberPress</span>
          <strong>{members.filter((member) => member.isMemberPress).length}</strong>
        </div>
        <div className="metric">
          <span>Mailchimp</span>
          <strong>{members.filter((member) => member.isMailchimp).length}</strong>
        </div>
        <div className="metric">
          <span>AwardForce</span>
          <strong>{members.filter((member) => member.isAwardForce).length}</strong>
        </div>
      </section>

      <section className="panel" aria-label="Members">
        <div className="toolbar">
          <input
            className="search"
            type="search"
            placeholder="Search email, name, or phone"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
          <div className={`status ${tone === "default" ? "" : tone}`.trim()}>{status}</div>
        </div>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Email</th>
                <th>MemberPress name</th>
                <th>Mailchimp name</th>
                <th>AwardForce name</th>
                <th>Phone</th>
                <th>MemberPress</th>
                <th>Mailchimp</th>
                <th>AwardForce</th>
              </tr>
            </thead>
            <tbody>
              {filteredMembers.map((member) => (
                <tr key={member.id}>
                  <td>{member.id}</td>
                  <td>{member.email}</td>
                  <td><EmptyCell value={member.memberPressName} /></td>
                  <td><EmptyCell value={member.mailchimpName} /></td>
                  <td><EmptyCell value={member.awardForceName} /></td>
                  <td><EmptyCell value={member.phone} /></td>
                  <td><Badge value={member.isMemberPress} /></td>
                  <td><Badge value={member.isMailchimp} /></td>
                  <td><Badge value={member.isAwardForce} /></td>
                </tr>
              ))}
            </tbody>
          </table>
          {filteredMembers.length === 0 ? <div className="empty">No members found.</div> : null}
        </div>
      </section>
    </main>
  );
}
