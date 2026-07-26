# Project Skills

Project-scoped skills for Meowgoku. Claude Code discovers these automatically —
they appear alongside the built-in and plugin skills.

## Layout

One directory per skill, each containing a `SKILL.md`:

```
.claude/skills/
  my-skill/
    SKILL.md          # required: frontmatter + instructions
    references/       # optional: supporting docs the skill reads
    scripts/          # optional: helper scripts
```

## SKILL.md frontmatter

```markdown
---
name: my-skill
description: When to use this skill. Be specific about triggers — this is
  the only text Claude sees when deciding whether to load it.
---

Instructions go here.
```

The `description` is the whole matching signal, so write it as *when to reach
for this*, not *what it is*. Name concrete triggers (file types, commands,
phrases the user says).

## Notes

- Run `/reload-skills` after adding or editing a skill; no restart needed.
- These are separate from the installed plugins (superpowers, ponytail), which
  live in `~/.claude/plugins/` and are shared across all projects.
- Good candidates here: Unity build/player steps, project-specific test or
  profiling workflows, asset import conventions.
