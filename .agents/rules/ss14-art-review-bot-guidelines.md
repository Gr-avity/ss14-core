# SS14-ART-CORE Review Bot Guidelines

Use this rule for CodeRabbit, Copilot, and other automated review guidance.

## Review Priorities

1. RobustToolbox edits.
2. Missing `_Art` placement or missing `Art-Edit` markers.
3. Prediction/networking bugs.
4. Server validation and hidden data leaks.
5. Localization omissions.
6. Duplicate mechanics or hardcoded prototype IDs.
7. Missing focused validation.

## Noise Control

- Do not request unrelated style churn.
- Do not flag old code unless the PR touched it or copied the pattern.
- Prefer one actionable comment with a suggested fix over broad advice.
- Mention the relevant `.agents` rule or skill when possible.
