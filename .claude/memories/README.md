# Project Memories

Durable notes about Meowgoku that aren't derivable from the code or git history:
decisions and their rationale, constraints, gotchas, things that were tried and
abandoned.

## Important: not auto-loaded

Claude Code does **not** read this directory automatically. Two things that do
load on their own:

- `CLAUDE.md` at the repo root — read into context every session.
- `~/.claude/projects/d--Meow-Meowgoku/memory/` — the per-user memory store,
  outside the repo and not shared with anyone who clones it.

This directory is a **committed, shared** alternative: notes that belong to the
project and should reach anyone who clones it. To use one, point Claude at it
("read .claude/memories/…"), or reference it from `CLAUDE.md` so it loads every
session.

## Format

One topic per file, kebab-case name:

```markdown
# Title

**Context:** what prompted this
**Decision:** what we settled on
**Why:** the reasoning, especially alternatives rejected
**Date:** 2026-07-27
```

Use absolute dates, never "last week". Delete files that stop being true —
a stale memory is worse than none.

## What not to put here

Anything the repo already records: file structure, past bug fixes, commit
history, or anything already in `CLAUDE.md`.
