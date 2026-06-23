# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Windows desktop tool that parses Guild Wars 2 arcDPS combat logs (`.evtc` / `.zevtc` / `.evtc.zip`) and produces per-fight HTML reports plus a custom multi-log summary (`index.html`). It is built on top of a vendored copy of the **GW2 Elite Insights Parser** (currently 3.24.0.0) and adds a WinForms front end, drag-and-drop queueing, and a rolled-up cross-fight player summary that upstream EI does not produce.

## Build / run

- **Solution:** `Gw2LogParser.sln` — one project (`Gw2LogParser.csproj`).
- **Target:** `net8-windows`, `WinExe`, `UseWindowsForms=true`, `Nullable=enable`, `AllowUnsafeBlocks=true`, `LangVersion=12.0`.
- **Build:** `dotnet build Gw2LogParser.sln -c Debug` (or open in Visual Studio 2022+).
- **Run:** launch the built `Gw2LogParser.exe` from `bin\Debug\net8-windows\` (or F5 from VS). The app is GUI-only — entry is `Program.Main` in `Program.cs`, which constructs `ProgramHelper` and `MainForm`.
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
- The fork's combination report (`ExportModels/LogBuilder.cs`) reads EI stats by **positional index** into `PhaseDto` arrays (`DefStats`/`OffensiveStats`/`SupportStats`/`DpsStats`/`GameplayStats`, the damage-distribution items, and the healing/barrier phase stats). After every sync, diff those DTO files (`git show HEAD:<file>` vs working) to confirm the documented column order in `Html/PhaseDto.cs` and the extension DTOs is unchanged — a reorder compiles fine but silently corrupts the leaderboard. (3.22 → 3.24: all unchanged, only additive fields.)
- 3.24 split `PolygonDecoration` into `Custom`/`RegularPolygonDecoration` and moved `GadgetInteractEvent` under `CastEvents/Gadget/`; clean-replacing the `EvtcParser` `.cs` tree (delete-all then copy) handles such renames/removals automatically.

## Settings

User settings are typed in `Properties/Settings.settings` with defaults in `App.config` (`Anonymous`, `SkipFailedTries`, `ParsePhases`, `ParseCombatReplay`, `ComputeDamageModifiers`, `CustomTooShort`, `DetailledWvW`, `LightTheme`, HTML script externalization, `AutoParse`, etc.). These flow into `EvtcParserSettings` and `HTMLSettings` inside `ProgramHelper.DoWork` / `GenerateFiles` — that is the canonical place to add a new setting end-to-end.
