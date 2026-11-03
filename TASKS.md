# Tasks

Work through the parts in order. Times are a guide for a 90-minute session.

## The business rules (the specification)

The code should follow these rules. Where the code disagrees with them, the code is wrong.

**Certification levels.** An audit score is a whole number from **0 to 100 inclusive**. Any other score is invalid.

| Score | Level |
|---|---|
| 85 to 100 | Gold |
| 70 to 84 | Silver |
| 55 to 69 | Bronze |
| 0 to 54 | Not certified |

**A farm's current level** is the level of its **most recent** audit. A farm with no audits is Not certified.

**Farms** must have a name (max 200 characters) and a country (max 100 characters).

**Country searches** should not care about case: `kenya` and `Kenya` find the same farms.

---

## Part 0: Get it running (15 min)

Press F5 on the AppHost. The solution doesn't start cleanly.

**Goal:** the `webfrontend` resource is healthy and the **Farms** page lists the seeded farms.

Use the Aspire dashboard (resource states, console logs, structured logs and traces) to find each cause. Fix one problem at a time and write down what each one was. There is more than one.

## Part 1: Code review (10 min)

Create `REVIEW.md` at the root. List the problems you can see across the whole solution, **most important first**. For each one, give a short line on why it matters.

Cover the AppHost, the API, the data model, the Blazor page and **both test projects**. Don't fix anything in this part, just write it down.

## Part 2: Fix the API (15 min)

Make the farm endpoints behave like a well-designed REST API:

1. Put the farm endpoints in a `/farms` **route group**.
2. **Never return EF entities** from the API. Return response DTOs.
3. `GET /farms/{id}` returns **404** when the farm doesn't exist. Constrain the route so `/farms/abc` doesn't match.
4. `POST /farms` takes a **request DTO** (not the entity), returns **400** with validation details for an invalid farm, and **201 Created** with a `Location` header for a valid one.
5. `GET /farms?country=` filters **in the database**, not in memory, and is case-insensitive.
6. Every farm in a response includes its **current certification level**, worked out from its most recent audit.
7. Pass `CancellationToken` through to EF Core.

## Part 3: Audits feature (20 min)

1. Make `CertificationRules` match the specification exactly. Decide how to handle a score outside 0 to 100.
2. Add `POST /farms/{id}/audits` with `{ "score": 91, "auditor": "J. Kinyua" }`:
   - 404 if the farm doesn't exist
   - 400 if the score is outside 0 to 100 or the auditor is missing
   - 201 Created, with the level worked out by `CertificationRules`
3. Add `GET /farms/{id}/audits` returning the farm's audits, **newest first**.
4. Fix `/stats` so it counts each farm's **current** level, not every audit, and does the work in the database where possible.

## Part 4: Blazor (15 min)

On the **Farms** page:

1. Show the farms in a table with name, country and current level.
2. Make **Search** work: filter by country through the API, and show a message when nothing matches. An empty search shows all farms.
3. Make **Add a farm** validate (empty fields show a message), show an error if the API rejects it, and refresh the list after a successful add.
4. Add a **farm details** page at `/farms/{id}` that lists the farm's audits and has a form to add a new audit.

## Part 5: Tests (15 min)

1. Look closely at the existing tests in **both** test projects. Fix every test that can pass when the code is wrong.
2. **Unit tests:** cover `CertificationRules` at **every boundary** in the specification, plus invalid scores, in a single `[Theory]`.
3. **Integration tests:**
   - `GET /farms/{id}` for an unknown id returns 404
   - `POST /farms` with an empty name returns 400
   - `POST /farms` followed by `GET` on the `Location` header returns the new farm
   - adding a Gold-scoring audit makes the farm's current level Gold
4. Check your tests can fail: break the code on purpose and watch them go red.

---

## Stretch goals

Pick any, in any order:

- The integration tests start the whole app for **every test**. Start it **once per test class** instead (a class fixture with `IAsyncLifetime`).
- Use `DateTimeOffset` for audit dates, and inject `TimeProvider` so tests control "now".
- Replace `EnsureCreated` with EF Core migrations.
- Store `CertificationLevel` as a string in the database.
- Add paging to `GET /farms` (`?page=1&pageSize=20`).
- Add Redis output caching to `GET /farms/{id}` through Aspire, and invalidate it when an audit is added.
- An audit older than three years means the farm's current level is Not certified, whatever the score.

## When you're done

Commit, then compare your work with the `answer-key` branch:

```
git fetch origin
git checkout answer-key -- ANSWER_KEY.md
```
