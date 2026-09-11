# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.ClearBit` from a single-version build to
the multi-version targeting pattern. Modeled on the prior migrations of
`CluedIn.Connector.Dataverse.V2`, `CluedIn.Enricher.Gleif`, `CluedIn.Enricher.OpenCorporates`,
`CluedIn.Enricher.Permid`, and `CluedIn.Enricher.Brreg` (all done the same day, same pattern).

---

## Overview

Target matrix: `4.7.0` (net6.0), `4.8.0` (net6.0), `5.0.0-beta.*` (net10.0) — verified `5.0.0-*`
resolves to `5.0.0-beta.576` on this repo's own `NuGet.config` feeds via a throwaway restore, not
assumed. `4.6.0` excluded — no `IStreamRepository`/stream-API usage in `src/`, matching the other
enrichers' precedent.

Branch: `feature/multi-version-targeting` (off `develop`).

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Done**

Replaced the single-version `crawler.build.yml` steps-template (with explicit `UseDotNet@2 8.0.x`
and `NuGetAuthenticate@0` steps) with `crawler.build.jobs.yml`, `multiVersionCluedInTargets` set to
the three targets above, `useGitVersionDotNetTool: true` and the other publish parameters set from
the start (a sibling repo in this batch — Permid — lost a full CI run by omitting
`useGitVersionDotNetTool: true`, which silently fell back to a retired legacy GitVersion task).
Pool switched from `windows-latest` to `ubuntu-22.04` to match the shared template's expected
environment.

---

## Step 2 — `Directory.Build.props`

Status: **Done**

Honours `CluedInMultiVersionTargetFramework` (net10.0 local fallback), derives
`CLUEDIN_V47`/`V48`/`V50` `DefineConstants`, and pins `LangVersion` to `13.0` up front (several
prior repos in this batch hit `CS8936` on net6.0 without this).

---

## Step 3 — `Packages.props`

Status: **Done**

`_CluedIn` guarded. Test package versions (`Microsoft.NET.Test.Sdk`, `xunit`/`xunit.v3`,
`AutoFixture.Xunit2`/`Xunit3`) split into `CLUEDIN_V50`-conditional `ItemGroup`s — the original file
unconditionally pinned `xunit.v3`/`AutoFixture.Xunit3`/`Microsoft.NET.Test.Sdk 18.3.0`, none of
which support net6.0. `CluedIn.Testing.Base` switched to the version-suffixed package ID per leg
(`CluedIn.Testing.Base.470`/`.480`/`.500`, computed via `_CluedInPackageSuffix`) — confirmed all
three actually exist on the feed and restore cleanly, not assumed.

`test/unit/Directory.Build.props` deleted — dead scaffold pinning ancient `Moq 4.5.30`/`Should
1.1.20`, no `test/unit` csproj exists to consume it (same dead-scaffold pattern GoogleMaps' repo
had).

---

## Step 4 — `NuGet.config`

Status: **Done**

`git mv`'d `Nuget.config` → `NuGet.config` (two-step rename). Feeds already sufficient (`develop`,
`release`, `AzurePipelines`, `nuget.org`) — no `public` feed needed, confirmed by a clean restore at
4.7.0/4.8.0.

---

## Step 5 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-beta.*

Status: **Done**

Built for real against all three legs, `src/` (both `ExternalSearch.Providers.ClearBit` and
`ExternalSearch.Providers.ClearBit.Provider`) and the integration test project.

**RestSharp 106-vs-114 break** (same family every enricher in this batch hit) — 3 call sites in
`ClearBitExternalSearchProvider.cs`:
- `ConstructVerifyConnectionResponse(RestResponse response)` parameter type → `#if CLUEDIN_V50
  RestResponse #else IRestResponse #endif`.
- Two `Method.Get` (114.x, PascalCase) → `Method.GET` (106.x, uppercase) call sites, both guarded.

No other API breaks — `src/` and the integration test project (using the suffixed
`CluedIn.Testing.Base` packages from Step 3) both build 0 errors on all three legs. No
`GlobalUsings.cs` was needed: the only test file (`ClearBitTests.cs`) uses bare `using Xunit;` for
`[Fact]`/`[Theory]` (identical namespace in both xunit v2/v3) and no `AutoFixture` attributes or
`ITestOutputHelper` — nothing namespace-sensitive to guard.

---

## Step 6 — Reset the semantic version (`GitVersion.yml`)

Status: **Done**

```yaml
next-version: 1.0
ignore:
  sha: []
  commits-before: 2026-06-20T00:00:00
```

Highest pre-existing tag is `4.6.2` at `2026-06-17T17:22:01+10:00`. Padded to **2 full days** past
it (not 1) — `GitVersion.Tool 5.9.0` appears to parse `commits-before` using local machine time,
not UTC, and a tighter margin silently failed to exclude the old tag on a sibling repo in this
batch (Gleif). Verified directly with the pipeline's actual pinned tool (installed to a scratch
path): `MajorMinorPatch: "1.0.0"`, confirmed correct.

---

## Step 7 — Push and confirm CI

Status: **Done**

Fully green on the first push (build 151974, PR #41): all three `Multi-version build+test` legs
(4.7.0, 4.8.0, 5.0.0-beta.*) and `Multi-version: publish` passed.

---

## Checklist

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-beta.*); pool switched to ubuntu-22.04; `useGitVersionDotNetTool: true` set from the start
- [x] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework` with net10.0 local fallback; `DefineConstants` derived; `LangVersion` pinned to 13.0
- [x] `Packages.props` — `_CluedIn` guarded; test packages split by `CLUEDIN_V50`; `CluedIn.Testing.Base` suffixed per leg
- [x] `test/unit/Directory.Build.props` — deleted (dead scaffold, no project consumed it)
- [x] `NuGet.config` — renamed from `Nuget.config`; feeds confirmed sufficient as-is
- [x] Source — `#if CLUEDIN_V50` guards for the RestSharp 106↔114 break (3 call sites in `ClearBitExternalSearchProvider.cs`); both src projects + integration tests build 0 errors on all three legs
- [x] `GitVersion.yml` — `next-version: 1.0`; `ignore.commits-before: 2026-06-20T00:00:00` (2-day padding); verified `MajorMinorPatch: "1.0.0"` with the pinned GitVersion.Tool 5.9.0
- [x] Pushed branch and confirmed the Azure DevOps pipeline is green end-to-end — PR #41, build 151974: all three legs + `Multi-version: publish` passed on the first run
