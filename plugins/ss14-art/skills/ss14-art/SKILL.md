---
name: ss14-art
description: Repo-local bridge for the SS14 Art plugin. Use when Codex is working in this repository, changing Content.Shared, Content.Server, Content.Client, Resources, tests, review configuration, or any SS14/RobustToolbox-adjacent code and should load the canonical .agents rules and skills.
---

# SS14-Art Plugin Bridge

This plugin is an adapter. The canonical source of truth is in the repository:

- `.agents/rules/`
- `.agents/skills/`
- root and subtree `AGENTS.md`

Before coding, load `.agents/skills/ss14-art-core/SKILL.md`, then follow `.agents/rules/ss14-art-hard-guardrails.md` and `.agents/rules/ss14-skill-preflight-and-refresh.md`.

Never treat this bridge as the canon. If the bridge conflicts with `.agents`, `.agents` wins.
