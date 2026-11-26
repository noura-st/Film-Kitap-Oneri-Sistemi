# Film & Kitap Öneri Sistemi

A Windows Forms app in C# that recommends films and books from what you have
already rated. Built as a university project; later revisited end to end.

`C#` · `.NET Framework 4.7.2` · `Windows Forms` · `ADO.NET` · `SQL Server` · `PBKDF2-SHA256` · **0 build warnings** · **13 unit tests** · **transactional writes**

> **Status:** builds clean — 0 errors, 0 warnings — and the password hashing is
> covered by 13 tests.

## What I built

- **A working two-tier app** — Windows Forms UI over SQL Server, with accounts,
  a catalogue, 1–5 ratings and genre-based suggestions.
- **An object model worth showing** — `ItemBase` abstract, `Item` implementing it,
  `Book` and `Film` extending it, and `User` validating through property setters
  rather than exposing public fields.
- **A security pass a year later** — I reopened this project, found that passwords
  were stored in plain text and that the build was broken, and fixed both. The
  [security review](#security-review) below documents what was wrong and why.

`C#` · `.NET Framework 4.7.2` · `Windows Forms` · `ADO.NET` · `SQL Server` · `PBKDF2`

## What it does

1. **Register / log in** — accounts are stored in SQL Server.
2. **Browse** the catalogue of films and books loaded from the database.
3. **Rate** each item from 1 to 5.
4. **Get recommendations** — the app finds the genre with the highest total score
   for that user, then suggests items in that genre the user has not rated yet.

The recommendation itself is intentionally simple: it is a one-line `SUM(Score)
GROUP BY Genre`. It is enough to demonstrate the data flow end to end — UI, domain
objects, database, and back.

## Architecture

```
RecommendationApp/
├── RecommendationApp/
│   ├── ItemBase.cs        abstract base: ItemID, Title, Type, Genre, GetInfo()
│   ├── Item.cs            concrete catalogue item from the database
│   ├── Book.cs / Film.cs  subclasses adding Author / Director
│   ├── User.cs            user with validated setters
│   ├── PasswordHasher.cs  PBKDF2-HMAC-SHA256 hashing, constant-time verify
│   ├── Database.cs        the single connection-string source
│   ├── LoginForm.cs       register / log in, hash verification
│   ├── MainForm.cs        catalogue list + recommendation query
│   └── RateForm.cs        rating entry, transactional save
├── SQLQuery1.sql          schema, constraints, seed data
└── tests/                 PasswordHasher test runner (13 cases)
```

**Stack:** C# · .NET Framework 4.7.2 · Windows Forms · ADO.NET (SqlClient) · SQL Server Express

OOP is the point of the coursework: `ItemBase` is abstract, `Item` implements it,
`Book` and `Film` extend it with their own fields, and `User` uses property setters
with validation instead of public fields.

## Security review

When I reopened this project a year later, it did not run, and what did run was
unsafe. Both are fixed here.

### What was wrong

**1. Passwords were stored in plain text.** The original `LOGIN` query compared the
typed password directly against a `Password` column:

```csharp
// before
string query = "SELECT UserID FROM Users WHERE Username = @username AND Password = @password";
```

Anyone with read access to the database — or a backup, or a screenshot — had every
user's password. And because people reuse passwords, that leaks beyond this app.

**2. No salt, no hashing, no length limit.** `NVARCHAR(50)` for a password means the
schema itself encourages short passwords.

**3. The schema did not match the code.** `SQLQuery1.sql` created `Users` without a
`Password` column and added it later with `ALTER TABLE`; it created a `Ratings` table
with `UserID`/`ItemID`, then dropped it and made `UserRatings` with `Username`/`ItemId`.
The C# queries used `Ratings`, `ItemID` and `Score` — none of which the final schema
had. The app could not have worked against this file.

**4. The forms did not compile.** `LoginForm.Designer.cs` declared `txtUsername`,
`txtPassword` and `btnLogin` but `InitializeComponent` instantiated `textBox1`,
`button` and `label3`. `MainForm.Designer.cs` wired a click handler named
`btnShowRecommendations_Click` while the method is `BtnShowRecommendations_Click`.
Case matters in C#.

**5. A duplicate rating was silently possible.** Nothing stopped a second insert for
the same user and item, so double-clicking "Kaydet" skewed the totals the
recommendation is based on.

**6. A connection string was copy-pasted into four methods.** Changing the server
meant editing all four, and there was no config file at all.

### What changed

| Before | After |
|---|---|
| Plain-text password column | `PasswordHash` — PBKDF2-HMAC-**SHA256**, 100 000 iterations, 16-byte random salt per user |
| `WHERE Password = @password` | Fetch the hash by username, verify in code with a constant-time compare |
| `NVARCHAR(50)` password | `NVARCHAR(200)` hash, minimum 8 characters enforced in the form |
| Password visible while typing | `UseSystemPasswordChar = true` |
| Duplicate ratings allowed | `UNIQUE (UserID, ItemID)` + `MERGE` upsert inside a transaction |
| Four hard-coded connection strings | One `Database.ConnectionString`, read from `App.config` |
| Schema that did not match the code | One idempotent script with keys, check constraints and an index |
| Forms that did not compile | Designer files rebuilt to match the code |

The stored format is self-describing, so the iteration count can be raised later
without invalidating existing rows:

```
pbkdf2-sha256$100000$<base64 salt>$<base64 hash>
```

`PasswordHasher.Verify` parses that string, so old hashes keep working after a
parameter change — only new hashes use the new cost.

### Tests

`tests/` holds a small runner with 13 cases for `PasswordHasher`, covering the
cases that matter: a wrong password is rejected, two hashes of the same password
differ (so the salt works), a plain-text string is not accepted as a hash, and
malformed or truncated hash strings fail closed instead of throwing.

```bash
cd tests
dotnet run
```

```
  PASS  correct password verifies
  PASS  wrong password rejected
  PASS  same password -> different hash (salt)
  PASS  sha256 is named, not the sha1 default
  ...
  13 passed, 0 failed
```

The runner targets .NET 8 so it needs only the SDK — no Windows Forms, no SQL
Server. `PasswordHasher` has no UI or database dependency, which is why it can be
tested on its own.

### What is still missing

Being explicit about this is more useful than pretending it is done:

- **The connection uses `Trusted_Connection=True`** (Windows auth), so there are no
  database credentials in the source at all. That is the right default for a local
  project; a deployed version would need a dedicated least-privilege SQL login.
- **No account lockout or rate limiting** on the login form. Offline brute force is
  expensive thanks to PBKDF2, but online guessing is not slowed down.
- **The app trusts the client.** All validation is in the form; there is no server
  tier, so a determined user can talk to SQL Server directly.
- **`AddWithValue`** is used for parameters. It is safe against injection but can
  hurt query-plan reuse because it infers the type and size — `Add("@p",
  SqlDbType.NVarChar, 50).Value = ...` is the more precise form.

## How to run

Requires Visual Studio 2019+ (or MSBuild) with .NET Framework 4.7.2 and a local
SQL Server Express instance.

**1. Create the database**

Open `SQLQuery1.sql` in SSMS or Azure Data Studio against your `localhost\SQLEXPRESS`
instance and run it. It creates `RecommendationDB`'s tables, constraints and seed
items (5 films and books). It is idempotent — running it twice drops and recreates.

```sql
CREATE DATABASE RecommendationDB;   -- once, if it does not exist
```

Then run `SQLQuery1.sql` **inside** that database.

**2. Point the app at your server**

Edit `App.config` if your instance is not `localhost\SQLEXPRESS`:

```xml
<add name="RecommendationDB"
     connectionString="Server=YOUR_SERVER;Database=RecommendationDB;Trusted_Connection=True;"
     providerName="System.Data.SqlClient" />
```

**3. Build and run**

```bash
git clone https://github.com/noura-st/Film-Kitap-Oneri-Sistemi.git
```

Open `RecommendationApp/RecommendationApp.sln` in Visual Studio and press F5.

**4. Try it**

Register an account (8+ characters), rate a few items, then press
"Önerileri Göster". With no ratings yet you get a friendly message instead of an
empty list.

## Licence

MIT — see [LICENSE](LICENSE).
