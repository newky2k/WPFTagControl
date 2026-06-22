# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`AltWPFTagControl` — a WPF tag/token control (add/remove/edit tags), forked variant of Kai Timmermann's control. Published as a NuGet package (`PackageId` = `AltWPFTagControl`, assembly = `WPFTagControl`). Key differences from the original: data-bound to arbitrary objects via `Tags` + `DisplayMemberPath`, scrollable when tags overflow, themeable, and tags are driven by the data model rather than control-internal state.

## Build / run

Solution file is `WPFTagControl.slnx` (new XML solution format; the old `.sln` is deleted). Use the `.slnx` explicitly — older `dotnet`/`msbuild` may not understand it; build the project files directly if so.

```
dotnet restore WPFTagControl.slnx
dotnet build WPFTagControl.slnx -c Release
dotnet build src/WPFTagControl/WPFTagControl.csproj        # library only
dotnet run --project WPFSample/WPFSample.csproj            # run the sample app (net9.0-windows)
```

- Library multi-targets `net461;net8-windows7.0;net9-windows7.0`. The sample targets `net9.0-windows`.
- `GeneratePackageOnBuild=true` (set in `Directory.Build.props`) — every build of the library produces a `.nupkg`. Release builds add SourceLink + embedded PDB.
- Assembly is strong-name signed via `DSoft.snk` (referenced in `Directory.Build.props`); the key file is not in the repo, so signing may fail locally on a fresh clone.
- No test project exists. `WPFSample/` is the manual verification harness, not automated tests.

CI: `azure-pipelines-release.yml` (triggers on `master`) builds Release and publishes `DSoft.*.nupkg`. `azure-pipelines-mergetest.yml` runs on PRs.

## Architecture

Lookless WPF controls — all visuals live in XAML themes, code-behind only wires behavior.

- **`TagControl`** (`src/WPFTagControl/TagControl.cs`) — subclasses `ListBox`. Public `Tags` (`IEnumerable`) and `DisplayMemberPath` are the binding surface. On `Tags` change (`OnTagsChanged`), it projects each bound object into a `TagItem` via `GetMemberValue<string>(DisplayMemberPath)` and sets `ItemsSource` to that `List<TagItem>`. Subscribes to `INotifyCollectionChanged` to re-project on Add. Raises `TagAdded`/`TagRemoved`/`TagEdited`/`TagClick`, and a consolidated `TagsChanged`.
- **`TagItem`** (`src/WPFTagControl/TagItem.cs`) — subclasses `Control`. Holds `Text` (display) + `Value` (original bound object). Manages the per-tag edit lifecycle: a `TextBox` (`PART_InputBox`, swapped in by the `IsEditing` template trigger) for editing, Enter/Tab to commit (with duplicate check via `isDuplicate`), Escape to cancel, Backspace-on-empty to step back to the previous tag. Finds its owning `TagControl` by walking the visual tree (`FindUpVisualTree`), since items aren't directly parented in code.
- **`ObjectExtensions.GetMemberValue<T>`** — reflection helper that reads `DisplayMemberPath` off a bound object, falling back to `ToString()`. This is how arbitrary objects become tag text.
- **`MultiWidthMultiConverter`** — layout converter used by the templates for wrap/scroll sizing.

### Template parts (named `PART_*`)

Behavior depends on these existing in the XAML themes; renaming a part in code requires the matching change in `Themes/`:
- `TagControl`: `PART_CreateTagButton` (and `PART_TagIcon`, currently inert).
- `TagItem`: `PART_InputBox` (TextBox), `PART_TagButton`, `PART_DeleteTagButton`.

### Themes (`src/WPFTagControl/Themes/`)

- `generic.xaml` — default lookless styles (loaded automatically via `DefaultStyleKeyProperty` override in both controls).
- `TagControl.xaml`, `ColorsAndIcons.xaml` — control template and color/icon resources.

Consumers must merge `pack://application:,,,/WPFTagControl;component/Themes/ColorsAndIcons.xaml` in their `App.xaml`. Styling is overridden by redefining brush resources: `TagHighlightBrush`, `TagForegroundBrush`, `TagBackgroundBrush`.

### Dependencies

None. The library is pure WPF framework — no external NuGet runtime dependencies. (It previously used `DotNetProjects.WpfToolkit` for an `AutoCompleteBox` editor; that was replaced with a plain `TextBox`.)

## Conventions

- Public API carries full XML doc comments (`GenerateDocumentationFile=true`); match that when adding public members.
- Some commented-out `SelectedTags` plumbing remains in `TagControl` (`UpdateSelectedTagsOn*`); these are intentionally dormant — don't assume they're live.
- Tag editing logic in `TagItem.inputBox_LostFocus`/`inputBox_Loaded` is a dense state machine (duplicate handling, escape, edit-vs-add). Trace `valueBeforeEditing` and `isEscapeClicked` before changing it.
