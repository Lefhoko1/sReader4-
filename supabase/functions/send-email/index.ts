// sReader · send-email Edge Function
// ───────────────────────────────────
// Holds the Resend API key SERVER-SIDE so it never ships in the app.
// The Unity client calls {SupabaseUrl}/functions/v1/send-email with the
// anon key; this function forwards to the Resend API.
//
// Deploy (one time, from the project root):
//   supabase login
//   supabase link --project-ref wmfaumjseuzhlwwhaudy
//   supabase secrets set RESEND_API_KEY=<your resend key>
//   supabase secrets set SEND_FROM="sReader <onboarding@resend.dev>"   # use your domain once verified
//   supabase functions deploy send-email

Deno.serve(async (req: Request) => {
  if (req.method !== "POST") {
    return json({ error: "Method not allowed" }, 405);
  }

  const apiKey = Deno.env.get("RESEND_API_KEY");
  if (!apiKey) {
    return json({ error: "RESEND_API_KEY secret is not set" }, 500);
  }

  let payload: { to?: string; subject?: string; html?: string; text?: string };
  try {
    payload = await req.json();
  } catch {
    return json({ error: "Invalid JSON body" }, 400);
  }

  const { to, subject, html, text } = payload;
  if (!to || !subject || (!html && !text)) {
    return json({ error: "Required: to, subject, and html or text" }, 400);
  }

  const from = Deno.env.get("SEND_FROM") ?? "sReader <onboarding@resend.dev>";

  const resendResponse = await fetch("https://api.resend.com/emails", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${apiKey}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ from, to: [to], subject, html, text }),
  });

  const body = await resendResponse.text();
  return new Response(body, {
    status: resendResponse.status,
    headers: { "Content-Type": "application/json" },
  });
});

function json(value: unknown, status: number): Response {
  return new Response(JSON.stringify(value), {
    status,
    headers: { "Content-Type": "application/json" },
  });
}
