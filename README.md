# Certify: mock live technical test

A practice solution shaped like the Rainforest Alliance preflight repo: **.NET 10, Aspire, Minimal API, EF Core on SQL Server, Blazor and xUnit**.

Certify tracks sustainability audits for partner farms. Each audit has a score from 0 to 100, and the score decides the farm's certification level.

The code has been written by a "previous developer". **It doesn't work, and it has problems at every layer.** Your job is to get it running, review it, fix it and extend it, the way you would in a live pairing session.

## Rules

- **No AI tools.** Turn off Copilot and IntelliCode whole-line completions. Normal IntelliSense, snippets and `Ctrl+.` are fine.
- **Timebox it.** The core tasks are designed for about 90 minutes. Use a timer.
- **Talk out loud** as if someone is watching your screen share.
- **Commit after each part**, with a clear message.
- Don't open the `answer-key` branch until you've finished.

## Prerequisites

- .NET 10 SDK
- Visual Studio 2026
- Docker Desktop, running before you press F5

## Run it

1. Open `src/Certify.sln`.
2. Set **Certify.AppHost** as the startup project.
3. Press F5. The Aspire dashboard opens.

It won't work first time. That's Part 0.

## Solution layout

| Project | What it is |
|---|---|
| `Certify.AppHost` | Aspire orchestrator: SQL Server, the API and the web front end |
| `Certify.ServiceDefaults` | Shared telemetry, health checks and service discovery |
| `Certify.ApiService` | Minimal API with EF Core |
| `Certify.Web` | Blazor web front end |
| `Certify.Tests` | Aspire integration tests (start the whole app, including SQL) |
| `Certify.UnitTests` | Fast unit tests for domain logic |

The tasks are in [`TASKS.md`](TASKS.md).
