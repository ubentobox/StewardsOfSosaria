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

## 4) Verify runtime accessor style

Run:

```bash
rg -n "StewardsRuntime\.(TownService|TaskService|AuditService|PossessionPolicy)" Custom
```

Expected: no property-style matches.

Runtime usage should be method-style:
- `StewardsRuntime.GetTownService()`
- `StewardsRuntime.GetTaskService()`
- `StewardsRuntime.GetAuditService()`
- `StewardsRuntime.GetPossessionPolicy()`

## 5) Recompile scripts

After checks pass, re-run shard script compilation.
