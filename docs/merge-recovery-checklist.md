# Stewards of Sosaria - Merge Recovery Checklist

Use this when GitHub conflict resolution accidentally keeps both versions of script blocks.

## 1) Verify a single runtime definition

Run:

```bash
rg -n "class StewardsRuntime|_townService|_taskService|_possessionPolicy" Custom/StewardsOfSosaria
```

Expected:
- one `class StewardsRuntime` declaration,
- **no** legacy duplicate backing field names (`_townService`, `_taskService`, `_possessionPolicy`).

## 2) Verify no unresolved conflict markers

Run:

```bash
rg -n "<<<<<<<|=======|>>>>>>>" Custom docs
```

Expected: no matches.

## 3) Verify one registration per command

Run:

```bash
rg -n "CommandSystem.Register\(" Custom/StewardsOfSosaria/Commands/StewardsCommands.cs
```

Expected: each command appears once.

## 4) Verify runtime accessor surface matches command usage

Run:

```bash
rg -n "GetTownService\(|GetTaskService\(|GetAuditService\(|GetPossessionPolicy\(|StewardsRuntime\.(TownService|TaskService|AuditService|PossessionPolicy)" Custom/StewardsOfSosaria
```

Expected:
- `Runtime/StewardsRuntime.cs` contains the property surface (`StewardsRuntime.TownService`, etc.).
- Command and item scripts should use that same property surface consistently.

## 5) Clean stale local build artifacts (for local csproj builds)

If you compile via MSBuild/dotnet and see duplicate assembly attribute errors (for example `TargetFrameworkAttribute`), clean generated artifacts and rebuild:

```bash
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
```

This does not affect shard script files under `Custom/`.

## 6) Recompile scripts

After checks pass, re-run shard script compilation.
