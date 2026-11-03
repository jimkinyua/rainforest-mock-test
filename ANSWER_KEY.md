# Answer key

Every problem planted in the mock test, by layer, with the fix and the line to say out loud. Compare this with your `REVIEW.md` and your code. Count how many you found unaided.

## Part 0: why it didn't start (4 problems, in the order you hit them)

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 1 | `apiservice` exits at startup: *"ConnectionString is missing… 'ConnectionStrings:certificationdb'"* | AppHost calls the database `certifydb`; the API asks for `certificationdb` | Make the two names identical |
| 2 | `apiservice` exits at startup: *"Cannot consume scoped service 'CertifyDbContext' from singleton 'FarmStatsService'"* | **Captive dependency**: a singleton holds a scoped `DbContext` | Register `FarmStatsService` as **scoped** (or inject `IServiceScopeFactory`, but scoped is right here) |
| 3 | Farms page shows an error; the API is never called | Web's `HttpClient` base address is `https+http://certify-api`, but the resource is called `apiservice` | Use `https+http://apiservice` |
| 4 | Farms page shows a 500; API logs show *"A possible object cycle was detected"* | `/farms` returns EF entities with `Include(Audits)`, and each `Audit.Farm` points back at the farm | Return DTOs (Part 2). `ReferenceHandler.IgnoreCycles` hides the symptom but is the wrong fix |

> Say: *"I read the dashboard console logs for the failing resource first, fixed one cause at a time, and re-ran. The first two were wiring, the third was service discovery naming, and the fourth was returning entities from the API."*

## API (`Certify.ApiService`)

| Problem | Why it matters | Fix |
|---|---|---|
| Returns EF entities | Object cycles, leaks the data model, couples clients to the schema, over-fetches | Response DTOs, projected with `Select` |
| `GET /farms` loads **every farm and audit**, then filters in memory | Doesn't scale; filtering belongs in SQL | `Where` on the `IQueryable` before `ToListAsync` |
| In-memory filter is **case-sensitive** (`kenya` finds nothing) | Contradicts the spec | Filter in the query: SQL Server's default collation is case-insensitive |
| `GET /farms/{id}` returns **200 with `null`** for a missing farm | Wrong status code; clients can't tell "not found" from "empty" | `TypedResults.NotFound()` |
| `{id}` has no route constraint | `/farms/abc` hits the handler and fails binding | `{id:int}` |
| `POST /farms` binds the **entity** | **Over-posting**: a client can set `Id` or `Audits` | Request DTO |
| `POST /farms` has **no validation** | Empty names and over-long strings reach the database | Validate (`AddValidation()` + data annotations, or manual) → 400 `ValidationProblem` |
| `POST /farms` returns **200 OK** | A create should be 201 with a `Location` | `TypedResults.Created($"/farms/{id}", dto)` |
| No route group | Repeated prefixes, nowhere to apply shared metadata | `app.MapGroup("/farms")` |
| No `CancellationToken` | Abandoned requests keep querying | Add the parameter and pass it to EF |
| `/stats` counts **every audit**, not each farm's current level | Wrong answer (Kericho is counted twice) | Latest audit per farm, grouped in the query |
| `/stats` loads all audits with `ToList()` and groups in memory, synchronously | Doesn't scale, blocks a thread | `GroupBy` in the query, `ToListAsync` |
| Startup uses **sync** `EnsureCreated()` and seeding | Blocks during startup | `EnsureCreatedAsync`, async seeding (or migrations) |
| `Farm`'s current level isn't exposed anywhere | The spec's main concept is missing | Work it out from the latest audit, in the DTO projection |

## Domain and data

| Problem | Why it matters | Fix |
|---|---|---|
| `CertificationRules`: `score > 85` | **85 should be Gold**, but is Silver | `>= 85` |
| `CertificationRules`: `score > 55` | **55 should be Bronze**, but is Not certified | `>= 55` |
| No range check | 101 becomes Gold, −5 becomes Not certified | Reject outside 0 to 100 (`ArgumentOutOfRangeException`, and a 400 at the API) |
| Rules are an `if` chain | Hard to read against the spec; easy to get a boundary wrong | Table-driven: `(MinScore, Level)` rows, highest first. Same idea as the RPS rules |
| Seed data stored levels using the buggy rules | Kericho's 85 audit is stored as **Silver** | After fixing the rules, fix the data (reseed, or recompute) and **mention it**: fixing code doesn't fix data already written |
| `Audit.AuditedOn` is `DateTime` set with `DateTime.Now` | Server-time-zone dependent, ambiguous after a round trip, untestable | `DateTimeOffset` + `TimeProvider` |
| `CertificationLevel` stored as an int | Unreadable in the database; reordering the enum corrupts data | `HasConversion<string>()` with a max length |
| No index on `Farm.Country` | Country search scans the table | `HasIndex(f => f.Country)` |

## Blazor (`Certify.Web`)

| Problem | Why it matters | Fix |
|---|---|---|
| Wrong service-discovery name (Part 0 #3) | API unreachable | `https+http://apiservice` |
| **No `@rendermode`** on the Farms page | Page is static: **Search does nothing**, and the form can't submit interactively | `@rendermode InteractiveServer` |
| A new `<ul>` per farm | Invalid list structure | One `<ul>`/table outside the loop |
| **Search filters on the client** after fetching everything | Defeats the API filter; doesn't scale | Pass `country` to the API |
| Search is case-sensitive and **an empty search shows nothing** | Contradicts the spec | Use the API's filter; empty term → all farms |
| No "no results" message | Blank page looks broken | `@if (!_farms.Any())` |
| `EditForm` has **no `DataAnnotationsValidator`** and uses **`OnSubmit`** | `[Required]` is ignored; empty farms are posted | Add the validator + `ValidationSummary`/`ValidationMessage`, use `OnValidSubmit` |
| `AddFarmAsync` in the client **ignores the response status** | A 400 from the API looks like success | `EnsureSuccessStatusCode()` or read the ProblemDetails and show it |
| The list **isn't refreshed** after adding | User can't see the new farm | Reload after a successful add |
| `SearchAsync` and `AddFarmAsync` have **no try/catch** | An exception in an event handler can kill the Blazor Server circuit | Catch, set `_error` |
| `OnInitializedAsync` runs twice with prerendering | Two API calls per page load | Know it; `prerender: false` or `PersistentComponentState` if it matters |

## Tests

| Problem | Why it matters | Fix |
|---|---|---|
| `HighScoreGetsALevel` asserts **`NotNull` on an enum** | An enum can never be null: **the test can't fail** (xUnit warns: xUnit2002) | Assert the actual level |
| The `[Theory]` tests 95, 75, 60, 30 only | **Misses every boundary**, so the 85 and 55 bugs pass | Rows for 100, 85, 84, 70, 69, 55, 54, 0, plus invalid −1 and 101 |
| `GetFarms_ReturnsSeededFarms` asserts **inside `if (IsSuccessStatusCode)`** | A 500 skips the assert and **the test passes** | Assert the status code first, then the content |
| `GetFarm_ReturnsFarm` asserts **`NotNull(response)`** | A response is never null: **can't fail**. It passes on 200-null, 404 and 500 | Assert `HttpStatusCode.OK` and the body |
| Both integration tests **start the whole app each time** | Slow (SQL container per test) | Class fixture with `IAsyncLifetime` |
| No tests for 404, 400, Created or the audits flow | The behaviour that matters is unprotected | The Part 5 tests |
| The integration tests read `/farms/1`, assuming seed order | Brittle | Create the data in the test, then read it back |

## Things to say if asked "what next?"

- Migrations instead of `EnsureCreated`, run as a deployment step, not at startup.
- `TimeProvider` everywhere time matters.
- A class fixture for integration tests; unit tests for rules, integration tests for wiring.
- Paging for list endpoints.
- Authentication: there is none.
- Rate limiting and output caching on read endpoints.

## Scoring yourself

- **Part 0:** all 4 fixed without hints → strong debugging.
- **Review:** 20+ of the issues above → senior-level review. Prioritising the boundary bugs, the never-failing tests and over-posting above style issues is what matters most.
- **Tests:** you broke the code on purpose and saw them go red.
