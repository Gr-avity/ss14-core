# Verification Rules

## Before Calling Work Done

- Check `git diff` for accidental upstream churn.
- Check non-`_Art` edits for `Art-Edit` markers.
- Check every player-facing string has FTL.
- Check networked fields are dirtied.
- Check client-origin actions are server-validated.
- Run the smallest meaningful validation command.

## Report Format

Say:

- what changed;
- what validation ran;
- what was not validated;
- any residual prediction, UI, DB, or asset risk.
