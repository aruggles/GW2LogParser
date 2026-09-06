# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Windows desktop tool that parses Guild Wars 2 arcDPS combat logs (`.evtc` / `.zevtc` / `.evtc.zip`) and produces per-fight HTML reports plus a custom multi-log summary (`index.html`). It is built on top of a vendored copy of the **GW2 Elite Insights Parser** (currently 3.28.0.1) and adds a WinForms front end, drag-and-drop queueing, and a rolled-up cross-fight player summary that upstream EI does not produce.

## Build / run

- **Solution:** `Gw2LogParser.sln` — one project (`Gw2LogParser.csproj`).
- **Target:** `net8-windows`, `WinExe`, `UseWindowsForms=true`, `Nullable=enable`, `AllowUnsafeBlocks=true`, `LangVersion=12.0`.
- **Build:** `dotnet build Gw2LogParser.sln -c Release` for actual use; `-c Debug` for debugging (or open in Visual Studio 2022+).
- **Run:** launch the built `Gw2LogParser.exe` from `bin\Release\net8-windows\`. The app is GUI-only — entry is `Program.Main` in `Program.cs`, which constructs `ProgramHelper` and `MainForm`.
- **IMPORTANT — run Release, not Debug, to actually parse logs.** The vendored EI parser is peppered with `#if DEBUG` developer assertions that `throw` on data EI's static models don't perfectly match (e.g. `Buff.VerifyBuffInfoEvent` throws `InvalidDataException` "Incoherent stack type for &lt;buff&gt;" when arcDPS reports a different `BuffStackType` than EI hardcodes, for logs from GW2 builds newer than EI's balance data). EI ships **Release**, which compiles all 19 of these assertions out; a **Debug** build surfaces them to the end user as `ProgramException: Operation aborted` and kills the whole parse. This is not a one-off — each new game balance patch can introduce a fresh buff-info mismatch, so any log newer than the vendored EI version will abort under Debug. All `#if DEBUG` blocks are in vendored EI code; no fork code uses the DEBUG symbol, so Release is strictly safer for users. (If a Debug run is ever needed on such logs, the alternative is stripping `DEBUG` from `<DefineConstants>` in the Debug `PropertyGroup`.)
- **After editing `Resources/template.html` (or any resx-referenced file), build with `--no-incremental`.** The template is embedded via a `ResXFileRef` in `Properties/Resources.resx`, and the incremental `GenerateResource` step does not notice that the referenced file changed — a plain `dotnet build` reports success but ships the stale `obj/.../Gw2LogParser.Properties.Resources.resources`, so `index.html` silently comes out without your change. `dotnet build Gw2LogParser.sln -c Release --no-incremental` (or deleting that `.resources` file) fixes it. Check with `grep -c <new-marker> bin/Release/net8-windows/Gw2LogParser.dll`.
- **No test project exists.** Don't invent test commands.
- **Dependencies:** only `Newtonsoft.Json` (NuGet). All parser/builder code is vendored in-tree, not referenced as packages.

## Architecture — important to understand before editing

Although there is only **one** `.csproj`, the code is split across several top-level folders that each correspond to a distinct logical assembly with its own namespace. Treat the folder boundaries as module boundaries — files in `EvtcParser/`, `GW2EIBuilders/`, and `GW2Api/` are tracked against the upstream Elite Insights Parser project and should be edited conservatively.

| Folder | Namespace | Role |
|---|---|---|
| `EvtcParser/` | `GW2EIEvtcParser.*` | Vendored EI parser core. Binary `.evtc` decoder, `EvtcParser.ParseLog` → `ParsedEvtcLog`. Subfolders: `LogLogic/` (per-encounter logic: `Raids`, `Fractals`, `Convergences`, `Golem`, `OpenWorld`, `Story`, `WvW`, `Unknown`), `EIData/` (actors, buffs, damage modifiers, mechanics, phases, `ProfHelpers/<Profession>/`), `ParsedData/` (`Agents`, `CombatEvents`, `Skills`), `Extensions/` (arcDPS extensions — healing/barrier stats). |
| `GW2EIBuilders/` | `GW2EIBuilders` | Vendored EI output layer. `HTMLBuilder` renders single-fight reports using the templates under `Resources/eiparser/`. `Json/` builds the upstream JSON DTOs (used by both upstream output and this fork's summary). |
| `GW2Api/` | `GW2EIGW2API` | Calls the official GW2 API to populate skill / spec / trait caches written to `Content/SkillList.json`, `SpecList.json`, `TraitList.json` next to the exe. `MainForm`'s "Refresh API" button drives `GW2APIController.WriteAPI*ToFile`. |
| `EvtcParserExtensions/` | `Gw2LogParser.EvtcParserExtensions`, `GW2EIParserCommons*` | This fork's glue between the WinForms UI and EI: `OperationController` / `FormOperationController` (per-file state machine: Ready → Pending → Queued → Run → Complete/Cancel), `LogContainer` (parsed-log + source `FileInfo` pair). |
| `ExportModels/` | `Gw2LogParser.ExportModels[.Report]` | **The custom feature.** `LogBuilder` adapts a `ParsedEvtcLog` to an upstream `LogDataDto`, then `Report/HTMLReportBuilder` aggregates many `LogReport`s into one `Report` and emits `index.html` (the multi-fight summary that sums player stats across all parsed logs). |
| `Resources/eiparser/` | (embedded `Content`) | Upstream EI HTML/CSS/JS templates copied to output. The `Content Include="..."` block in the csproj enumerates each file — when upgrading EI, this list must match the new template set or `HTMLBuilder` will fail to substitute placeholders. |
| Top level | `Gw2LogParser` | `Program` (entry), `MainForm` (drag-drop grid + parse queue), `ProgramHelper` (orchestrates parse → HTML write per file, then `GenerateSummary` after all complete), `ProcessManager` (legacy `BackgroundWorker` helpers, partly unused). |

### Concurrency model (in `MainForm` + `ProgramHelper`)

- `MainForm._RunOperation` launches each parse on a `Task`, with a `ContinueWith` on the UI sync context to update grid state. Up to `ProgramHelper.GetMaxParallelRunning()` (currently `3`) parses run concurrently; the rest sit in `_logQueue`.
- After every queue drain, `_RunNextOperation` checks `_anyRunning` and calls `ProgramHelper.GenerateSummary` — this is what produces the cross-fight `index.html` and is the main place this fork diverges behaviorally from upstream EI.
- `ProgramHelper.CompletedLogs` is a `ConcurrentBag<LogContainer>` shared across all parse tasks; clear it (via `BtnParse_Click` / `BtnCancel_Click`) before starting a new batch or the summary will include stale logs.
- `ProgramHelper.ExecuteMemoryCheckTask` will hard-kill the process (`Environment.Exit(2)`) if private memory exceeds 100 MB — but only when the static `MemoryCheck` flag is true (off by default). Don't enable this casually; the threshold is unrealistically low for real logs.

### Output layout

For each batch, files go under `<input-folder>/combat_report_<ProgramHelper.timestamp>/`:
- `fight_<index>.html` — one per parsed log, produced by `HTMLBuilder`.
- `index.html` — the rolled-up summary, produced by `HTMLReportBuilder` only if at least one log completed.
- `<name>.log` — trace file, only when `ProgramHelper.EnableTracing` is true.

`ProgramHelper.timestamp` is reset on each "Parse" button click so a new batch lands in a new directory.

## Upstream sync notes

When pulling a new EI version (the recent commits — `1df96fe`, `28d5daa` etc. — are exactly this), the changes typically span `EvtcParser/`, `GW2EIBuilders/`, `GW2Api/`, and the `Resources/eiparser/` templates. The fork-specific code (`MainForm*`, `Program*`, `ProcessManager`, `EvtcParserExtensions/`, `ExportModels/`) usually only needs touch-ups when EI breaks an API the glue calls (e.g. `LogDataDto.BuildLogData`, `ParsedEvtcLog.LogData/LogMetadata`, `HTMLBuilder` ctor signature, `EvtcParserSettings` fields). After a sync, verify the `<Content Include="Resources/eiparser/...">` list in the csproj still matches the file set on disk.

**Folder mapping** (upstream EI repo → fork folder):
- `GW2EIEvtcParser/` → `EvtcParser/`
- `GW2EIBuilders/HtmlModels/` → `GW2EIBuilders/Html/` (same `GW2EIBuilders.HtmlModels` namespace)
- `GW2EIBuilders/JsonModels/` → `GW2EIBuilders/Json/Builders/` (same `GW2EIBuilders.JsonModels` namespace)
- `GW2EIJSON/` → `GW2EIBuilders/Json/Models/` (same `GW2EIJSON` namespace)
- `GW2EIBuilders/*.cs` (top-level) → `GW2EIBuilders/*.cs` (top-level)
- `GW2EIGW2API/` → `GW2Api/`
- `GW2EIBuilders/Resources/` → `Resources/eiparser/`
- `GW2EIBuilders/Properties/Resources.resx` is **not** copied — embedding is done from the fork's top-level `Properties/Resources.resx`, which references files via `..\Resources\eiparser\…` paths (instead of upstream's `..\Resources\…`).

**Fork-modified files inside vendored folders** (must reapply patches after a wholesale copy):
- `GW2EIBuilders/HTMLAssets.cs` — every `Properties.Resources.X` → `Gw2LogParser.Properties.Resources.X`.
- `GW2EIBuilders/HTMLBuilder.cs` — same `Properties.Resources` redirect. (An older `?version=<rev>` cache-buster on external script URLs was removed during the 3.22 sync since upstream now puts the version in the filename itself.)
- `GW2Api/GW2{Skill,Spec,Map}APIController.cs` + `GW2Api/GW2APIController.cs` — fork-added `WriteCachedAPI*ToFile` / `WriteCachedAPIToFile` (marked `// FORK:`). They serialize the *in-memory* cache without re-calling the API; `ProgramHelper.SaveAPICacheIfMissing()` uses them so the startup download is persisted to `Content/` (upstream never writes the cache unless the user clicks "Refresh API Cache", which re-downloads). Without this patch every launch re-fetches ~10 MB of skills (~10-20 s); with it, launches after the first take well under a second. `MainForm.BtnRefreshAPI_Click` also writes `MapList.json` now, since maps are cached as well.
- Per-assembly `[assembly: CLSCompliant(false)]` attributes from `GW2APIController.cs`, `HTMLBuilder.cs`, and `Json/Models/JsonLog.cs` must be removed because the fork is a single assembly — keep only the one in `EvtcParser/EvtcParser.cs`.

**`Color` ambiguity**: WinForms implicitly imports `System.Drawing`, which conflicts with `GW2EIEvtcParser.EIData.Color` introduced in 3.22. The csproj has `<Using Include="GW2EIEvtcParser.EIData.Color" Alias="Color" />` to make `Color` always resolve to the EI type project-wide.

**ClickOnce / `dotnet build`**: the legacy ClickOnce properties (`<GenerateManifests>`, `<TargetZone>`, etc.) trigger a `GenerateTrustInfo` task that .NET Core MSBuild can't execute. The csproj sets `<GenerateManifests>false</GenerateManifests>` and `<GenerateTrustInfo>false</GenerateTrustInfo>` so `dotnet build` works from the CLI; this also disables the legacy ClickOnce publish workflow. If you ever want ClickOnce publish back, you'll need full Visual Studio + .NET Framework MSBuild, not `dotnet`.

**Regenerating `Properties/Resources.Designer.cs`**: it's auto-generated from `Properties/Resources.resx`, but `dotnet build` doesn't refresh it — only Visual Studio does on .resx save. When the resx changes (e.g. during an upstream sync that adds/removes templates), regenerate the Designer.cs manually. The simplest path is to enumerate every `<data name="…" type="System.Resources.ResXFileRef…">` entry in the resx and emit a corresponding `public static string Name => ResourceManager.GetString("Name", resourceCulture);` accessor.

**Glue surface against EI APIs**: keep an eye on these signatures, which moved during the 3.10 → 3.22 sync:
- `GW2APIController(skill, spec, trait, map)` — fourth `mapLocation` arg added; the fork now passes `Content/MapList.json`.
- `EvtcParserSettings(... long tooShortLimit, long tooBigLimit, bool detailedWvW)` — `tooBigLimit` (MB) inserted before `detailedWvW`. Fork passes `ProgramHelper.DefaultTooBigLimitMB` (1500).
- `UploadResults` — old `(string, string)` ctor gone; use parameterless `new UploadResults()` for the no-upload case.
- `LogDataDto.BuildLogData(log, cr, light, parserVersion, UploadResults)` — takes `UploadResults` directly now, not `string[]`. Removed `uploadResults.ToArray()` in `ExportModels/LogBuilder.cs`.
- `LogData.Success` is `private`. Read fight success via `log.LogData.GetMainPhase(log).Success` (which returns the `EncounterPhaseData.Success` on the main encounter phase).

Newer breakage during the **3.22 → 3.24 sync** (both now use an init-property pattern — only the size limits are ctor args, everything else is an object initializer):
- `EvtcParserSettings(long tooShortLimit, long tooBigLimit)` — all the booleans (`AnonymousPlayers`, `SkipFailedTries`, `ComputePhases`, `ComputeCombatReplay`, `ComputeDamageModifiers`, `DetailedWvWParse`) moved to `init` properties. Fixed in `ProgramHelper.cs` and `ProcessManager.cs`.
- `HTMLSettings(string externalHTMLScriptsPath, string externalHTMLScriptsCdn)` — `HTMLLightTheme`, `ExternalHTMLScripts`, `CompressJson` moved to `init` properties. Fixed in `ProgramHelper.cs` and `ProcessManager.cs`.
- The fork's combination report (`ExportModels/LogBuilder.cs`) reads EI stats by **positional index** into `PhaseDto` arrays (`DefStats`/`OffensiveStats`/`SupportStats`/`DpsStats`/`GameplayStats`, the damage-distribution items, and the healing/barrier phase stats). After every sync, diff those DTO files (`git show HEAD:<file>` vs working) to confirm the documented column order in `Html/PhaseDto.cs` and the extension DTOs is unchanged — a reorder compiles fine but silently corrupts the leaderboard. (3.22 → 3.24, 3.24 → 3.26, 3.26 → 3.27.1 and 3.27.1 → 3.28: all unchanged, only additive fields.)
- 3.24 split `PolygonDecoration` into `Custom`/`RegularPolygonDecoration` and moved `GadgetInteractEvent` under `CastEvents/Gadget/`; clean-replacing the `EvtcParser` `.cs` tree (delete-all then copy) handles such renames/removals automatically.

The **3.24 → 3.26 sync** broke *no* glue APIs — after the mechanical copy plus the standard patches (`Properties.Resources` redirect, duplicate `[assembly: CLSCompliant]` removal) the build was clean with zero fork-code changes. What it *did* change was the resource template set, which must be mirrored in three places or the build/runtime breaks:
- Removed `tmplCombatReplay{Player,Target}{Stats,Status}` (the `ActorStatus/Player` and `ActorStatus/Target` subfolders are gone), replaced by unified `ActorStatus/tmplCombatReplayActor{Stats,Status,Breakbars}`.
- `htmlTemplates/tmplSimpleRotation.html` moved into a new `htmlTemplates/SimpleRotation/` folder and gained `tmplSimpleRotationSelector.html`.
- EI's `Spec` enum is unchanged in 3.26, so the fork's profession→core-class map and icon fallback in `Resources/template.html` needed no edit.

The **3.26 → 3.27.1 sync** was the smallest yet: 36 changed files under `EvtcParser/`, 2 under `GW2EIBuilders/`, and **zero fork-code changes**. Nothing in `GW2Api/`, `Json/Builders/` or `Json/Models/` changed upstream at all — the only diffs there were the fork's own patches, so those files came back byte-identical after re-patching. Specifics worth remembering:
- **The resource file *set* is identical to 3.26** (108 files, same names/paths) even though 27 of them changed content. That means the resx / `Resources.Designer.cs` / csproj `<Content>` triple needed **no** regeneration — the recipe below can be skipped entirely when `diff -rq` on the two resource trees reports only content differences and no `Only in ...` lines. Always check that first; it is most of the work of a sync.
- One file moved: `LogLogic/LogIDs.cs` → `ParserHelpers/IDs/LogIDs.cs`, with its namespace changing from `GW2EIEvtcParser.LogLogic` to `GW2EIEvtcParser`. The csproj globs `.cs` (only two explicit `<Compile Update>` entries, both for Designer files), so a delete-all-then-copy picks this up automatically. `EvtcParserExtensions/OperationController.cs` still has `using GW2EIEvtcParser.LogLogic;` and still compiles — that namespace continues to exist for `LogLogic.cs` itself.
- `Spec` is unchanged again in 3.27.1 (`ParserHelper.cs` differs only by the `InchDistanceThreshold` constant, now `sqrt(2)` plus a new `…Squared`), so `Resources/template.html` needed no edit.
- `Html/PhaseDto.cs` and both healing/barrier extension phase DTOs are untouched, so the positional-index leaderboard risk did not materialise. The two changed output files are `Html/HtmlActors/ActorDetailsDto.cs` (target-side combat-replay rotation/boon-graph gating — `DmgDistributions` still gets `EmptyInstance` on that branch, and the fork only reads *player* `Details.DmgDistributions`) and `Html/HtmlCharts/PhaseChartDataDto.cs` (chart target inclusion). Neither affects the summary.
- **Expect small boon-uptime deltas after this upgrade, and do not treat them as corruption.** 3.27.1 contains a real buff-simulation fix: `CombatData.cs` now passes `forceActive: buffDesc?.StackingType == BuffStackType.StackingConditionalLoss` into `BuffExtensionEvent.OffsetNewDuration`, which accumulates an `addedExtension` across stack-inactive gaps, and `BuffsContainer.cs` now folds `BuffStackActiveEvent` gaps into the tracked duration. Re-running the same five WvW logs on 3.26 vs 3.27.1 gave 766,195 identical leaf values and 357 differences — *all* of them in `boonStats`/`boonGen*Stats` for **Regeneration (718)** and **Stability (1122)** only, magnitude ~0.001–0.07. Every damage, healing, defense, support, barrier and fight-outcome figure was bit-identical.

The **3.27.1 → 3.28.0.1 sync** was much larger in file count (166 changed files under `EvtcParser/`, 19 under `GW2EIBuilders/`) but needed exactly **one** fork-code change. Specifics:
- **The one glue break: `ParserController.Reset()` is no longer `virtual`.** 3.28 splits it into `virtual ResetContent()` + `virtual ResetState()`, with a non-virtual `Reset()` that calls both. `EvtcParserExtensions/OperationController.cs` was overriding `Reset()` → `error CS0506`. Fixed by splitting the override the same way upstream did: content fields (`BasicMetaData`, `DPSReportLink`, `OutLocation`, `_GeneratedFiles`, `_OpenableFiles`) into `ResetContent()`, `Elapsed` into `ResetState()`. Because the base `Reset()` still calls both, `MainForm.cs:206` and `ProgramHelper.DoWork` keep calling `Reset()` with **identical** behaviour to before — no call-site change needed. (Upstream's own `MainForm` switched its post-failure call to `ResetContent()` so the failure status text survives; the fork doesn't need this because `FinalizeStatus` already copies the message into the separate `Status` property that `Reset()` never clears.)
- Resource set: exactly one file **removed** — `combatReplayTemplates/DamageTable/tmplCombatReplayDamageData.html`, along with its `HTMLAssets.cs` reference. So the resx / `Resources.Designer.cs` / csproj `<Content>` triple needed one deletion each (107 eiparser files + the fork-only `template_html` = 108 resx entries / 108 Designer accessors / 107 csproj `<Content>` lines).
- `HTMLSettings` ctor only gained `?` on its two string params — source-compatible, no `ProgramHelper`/`ProcessManager` change. `ParserHelpers/ParserSettings.cs` (`EvtcParserSettings`) is untouched.
- `Spec` is unchanged again, so `Resources/template.html` needed no edit. `Html/PhaseDto.cs` and the healing/barrier extension DTOs are untouched.
- Upstream dropped MistWarrior upload support and refactored `GW2EIParserCommons/ProgramSettings`; neither is vendored by the fork, so nothing to mirror. The fork's `Properties/Settings.settings` needed no new entries.
- The three changed output-side files (`Html/MechanicDto.cs`, `Json/Builders/JsonMechanicsBuilder.cs`, `Json/Models/JsonMechanics.cs`, plus `JsonDamageModifierDataBuilder.cs`) are all mechanics/damage-modifier related — **the fork's summary never touches mechanics** (`grep Mechanic ExportModels/` is empty), so they cannot affect `index.html`.
- **Expect tiny `avgDistanceToSquad` / `avgDistanceToTag` deltas, and do not treat them as corruption.** 3.28 improves position interpolation: a new `TeleportEvent` (+ `JumpEvent`, + `StateChange.Jump` excluded from `ParserHelper.IsKnownStateChange`) feeds `CombatReplay.AddTeleport`, which injects a zero-velocity sample so interpolation stops at a teleport instead of gliding through it; `CombatReplay` also relaxed the position-gap condition to `velocity.Length() < 1e-3` alone. A/B over the same five WvW logs gave **39,773 identical leaf values, 0 structural diffs, and 19 value diffs — all of them `gameplay.avgDistanceTo{Squad,Tag}`**, magnitude ~0.1–8 units on values in the thousands. Every damage, healing, boon, defense, support, barrier and fight-outcome figure was bit-identical.

**Resource-set sync recipe** (the fiddly part of any upgrade — all three must agree, and only the resx is checked at build time):
1. `Properties/Resources.resx` — regenerate wholesale from upstream `GW2EIBuilders/Properties/Resources.resx`, replacing `..\Resources\` with `..\Resources\eiparser\`, then re-append the fork-only `template_html` entry (which points at `..\Resources\template.html`, the fork's *own* summary shell used by `ExportModels/Report/HTMLReportBuilder.cs` — not EI's `eiparser\template.html`, which upstream names `tmplMain`). Note the resx header comment contains 4 example `<data name=...>` lines, so a raw grep count reads 4 higher than the real entry count.
2. `Properties/Resources.Designer.cs` — regenerate one `public static string X => ResourceManager.GetString("X", resourceCulture);` accessor per resx entry (VS only refreshes this on .resx save; `dotnet build` will not).
3. `Gw2LogParser.csproj` `<Content Include="...">` block — regenerate from the files actually on disk under `Resources\eiparser\`, plus `Resources\template.html`.
A missing/renamed template surfaces as a build error from the resx file-ref, so a clean build proves all three lists resolve.

**Verifying a sync end-to-end**: the app is GUI-only (`Program.Main` takes no args), but `ProgramHelper.DoWork(OperationController)` and `ProgramHelper.GenerateSummary(Version)` are both public and `OperationController` is an abstract class with no abstract members — so a throwaway console harness referencing `bin\Release\net8-windows\Gw2LogParser.dll` (with `Content\*.json` copied alongside, since the API cache path is resolved from the assembly location) can parse real `.zevtc` files and emit both the per-fight HTML and `index.html` headlessly. Drive it exactly like `MainForm` does: clear `ProgramHelper.CompletedLogs`, reset `ProgramHelper.timestamp`, `DoWork` per file, then one `GenerateSummary` at the end. Note `GetSaveDirectory` writes `combat_report_<timestamp>/` **into the input folder**, so copy sample logs to a scratch folder first rather than parsing the arcdps log directory in place. Real logs live in `Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs\`, and a usable API cache is in `Documents\GW2LogParser\Content\` (`SkillList`/`SpecList`/`TraitList`; `MapList.json` is absent and EI just fetches maps from the API).

To inspect the summary payload, gunzip+base64-decode the long quoted string that `${logDataJson}` is replaced with. Good invariants to assert on the decoded JSON: `power + condi == allDamage`, `targetPower + targetCondi == targetDamage`, `healingPower + conversion == all` for both incoming and outgoing, and `boonStats[].id` matching real GW2 buff IDs (740 Might, 725 Fury, 1187 Quickness, 717 Protection, 1122 Stability, …). These catch exactly the positional-index corruption that compiles cleanly.

**The strongest check is an A/B diff against the previous version.** Commit the sync-in-progress on top of a clean baseline commit, then `git worktree add <path> HEAD~1`, build Release there, point the same harness at the old DLL, run both over the *same* logs, and deep-diff the two decoded JSON payloads leaf-by-leaf. That turns "it builds and looks right" into an exact inventory of what the upgrade changed — which is how the 3.27.1 boon deltas above were isolated to two buff IDs out of 766k values. Two gotchas: put the worktree at a **short** path (e.g. `C:\wt326`) because EI's deep `EIData/Mechanics/IDBasedMechanics/...` paths blow the Windows path limit and `git worktree add` fails with "Filename too long"; and remember the harness copies the referenced DLL at *its* build time, so stage a second output folder with the old `Gw2LogParser.dll` rather than rebuilding the harness twice.

**False positive to ignore when checking generated HTML**: scanning `fight_*.html` for leftover `${...}` placeholders reports ~22 hits (`${green}`, `${minutes}`, `${translateValue}`, …). These are JavaScript template literals inside backtick strings in EI's own Vue templates (`tmplEncounter.html`, `tmplBuffTable.html`, `functions.js`, …), evaluated in the browser by design — not unsubstituted C# placeholders. The fork's own `index.html` does come out clean of `${...}`, so that one is a meaningful check.

## Settings

User settings are typed in `Properties/Settings.settings` with defaults in `App.config` (`Anonymous`, `SkipFailedTries`, `ParsePhases`, `ParseCombatReplay`, `ComputeDamageModifiers`, `CustomTooShort`, `DetailledWvW`, `LightTheme`, HTML script externalization, `AutoParse`, etc.). These flow into `EvtcParserSettings` and `HTMLSettings` inside `ProgramHelper.DoWork` / `GenerateFiles` — that is the canonical place to add a new setting end-to-end.
