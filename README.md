# PersonalSite

[![Deploy](https://github.com/tzer0m/PersonalSite/actions/workflows/deploy.yml/badge.svg)](https://github.com/tzer0m/PersonalSite/actions/workflows/deploy.yml) [![Dependabot](https://github.com/tzer0m/PersonalSite/actions/workflows/dependabot/dependabot-updates/badge.svg)](https://github.com/tzer0m/PersonalSite/actions/workflows/dependabot/dependabot-updates)

A personal landing page styled like a terminal / systemd status readout, with a big ASCII-art name banner and a `• active (running)` status line, linking out to contact details and social/portfolio profiles.

## What it does

- Renders an ASCII-art name banner (a compact variant swaps in for mobile widths) inside a bordered terminal-style card.
- Lists three link groups: **contact** (email, phone), **social** (profile links), and **portfolio** (code hosting, package registry, other projects).
- No backend logic beyond serving the page — it's a single static Razor Page with no database, external calls, or configuration.

## Tech Stack

- ASP.NET Core Razor Pages on .NET 10

## Deployment

Deployed via GitHub Actions on push to `master`, using a self-hosted runner. The workflow stops the `PersonalSite.service` systemd unit, publishes a fresh build, and restarts the service.
