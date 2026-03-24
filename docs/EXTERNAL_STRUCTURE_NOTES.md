# External structure notes (community research)

Summaries of **third-party** guidance on project structure, vertical slices, combat iteration, and content pipelines. **Do not mirror full copyrighted thread text** — link + short bullets + your own takeaways only.

**Compliance:** Respect each site’s **terms of service** and **robots.txt**. Use **low frequency**, **hand-picked URLs**, and **personal research** scope.

---

## FlareSolverr (Cloudflare bypass for HTML fetch)

When a site (e.g. [gamedev.net](https://gamedev.net/)) serves a Cloudflare challenge to raw HTTP clients, run **[FlareSolverr](https://github.com/FlareSolverr/FlareSolverr)** locally (commonly Docker on `127.0.0.1:8191`).

**Start (Docker, from upstream README):**

```bash
docker run -d --name=flaresolverr -p 8191:8191 -e LOG_LEVEL=info \
  ghcr.io/flaresolverr/flaresolverr:latest
```

Stop when finished: `docker stop flaresolverr` (remove with `docker rm flaresolverr`). Each one-off `request.get` can spin up a browser; for several URLs in one session, use FlareSolverr’s `sessions.create` + `session` on `request.get` to reuse cookies and reduce churn (see repo **Usage**).

1. Start FlareSolverr (command above or [releases / source install](https://github.com/FlareSolverr/FlareSolverr)).
2. `POST http://localhost:8191/v1` with JSON, e.g.:

```json
{
  "cmd": "request.get",
  "url": "https://example.com/your-thread",
  "maxTimeout": 60000
}
```

3. Read `solution.response` (HTML) or reuse `solution.cookies` for follow-up requests.
4. Distill into the **Entries** section below.

**Reusing clearance in other tools:** If you pass Cloudflare cookies to `curl` or similar, also send the same **`User-Agent`** FlareSolverr returns in `solution.userAgent`, or the site may challenge again (upstream warning).

**When automation cannot run Docker:** Use a normal browser and paste links + notes manually into **Entries**.

---

## Entries (add as you research)

### Template

- **Source:** URL  
- **Fetched:** YYYY-MM-DD (FlareSolverr / browser)  
- **Topics:** structure | slice | combat | pipeline  
- **Takeaways:**  
  - …  
- **Kynde Blade hook:** which doc or subsystem this informs (e.g. [GAME_STRUCTURE.md](GAME_STRUCTURE.md), [PARITY_GAPS.md](../KyndeBlade_Godot/PARITY_GAPS.md)).

---

### GameDev.net — site homepage (FlareSolverr smoke)

- **Source:** [https://gamedev.net/](https://gamedev.net/)  
- **Fetched:** 2026-03-24 via FlareSolverr `request.get` (`maxTimeout` 120000 ms); API `status` **ok**, HTTP **200** (“Challenge not detected” in this run).  
- **Topics:** structure | pipeline | slice (community / learning)  
- **Takeaways (paraphrase, not page copy):**  
  - Positions the site as a **long-lived dev community**: learning, discussion, blogs, project showcases, and news — i.e. **many channels**, not one “correct” engine architecture.  
  - For **Kynde Blade**, treat threads as **opinion + patterns** to compare against your own docs ([GAME_STRUCTURE.md](GAME_STRUCTURE.md), [GAME_SKELETON.md](../KyndeBlade_Godot/docs/GAME_SKELETON.md)), not as a spec to copy wholesale.  
  - **Vertical slice** and **combat iteration** advice from forums is most useful when tied to a **single shipped path** and measurable checks (wireframe checklist, headless combat scenarios) — same spirit as Phase 0 in GAME_STRUCTURE.  
- **Kynde Blade hook:** [GAME_STRUCTURE.md](GAME_STRUCTURE.md) Phase 0 pillars; [PARITY_GAPS.md](../KyndeBlade_Godot/PARITY_GAPS.md) for anything you decide to adopt vs keep Godot-first.

---

### Synthesized themes (no fetch required)

Aligned with in-repo [ARCHITECTURE.md](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/ARCHITECTURE.md) and common indie practice:

- **Separation of concerns** — one clear job per script; Godot: autoloads for cross-scene services, scenes for UI + local flow.
- **Modularity** — data (`data/world/*.json`, `*.tres`) loaded by thin loaders; gameplay reads resources instead of hardcoding.
- **Vertical slice** — ship one playable path before scaling content; matches [GAME_STRUCTURE.md](GAME_STRUCTURE.md) Phase 0 pillars.
- **Regression** — headless combat scenarios + PARITY_GAPS honesty when Godot diverges from Unity.

_Add GameDev.net or other URLs above using the template when you have specific threads._
