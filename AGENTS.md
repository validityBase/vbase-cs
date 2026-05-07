# AGENTS.md

## Setup
- GitHub Actions notes: [specs/github-actions.md](specs/github-actions.md)
- Persistent agent memory: [agents/memory/](agents/memory/)

## Workflow
When making .NET, docs, build, or CI changes in this repo:
1. Confirm the intended behavior before editing.
2. Keep changes scoped to the relevant workflow, module, docs, or config.
3. Update specs or operational docs when CI behavior changes.
4. Run relevant validation, or list the exact commands that could not be run.
5. Prepare a PR-ready summary with validation and follow-ups.

## GitHub Actions
- Pin third-party actions to full commit SHAs.
- Use shared `validityBase/vbase-github-actions` actions/workflows by reviewed version tags.
- Keep the Windows/MSBuild docs and installer build flows local unless deliberately changing the build contract.
- Keep workflow permissions explicit and minimal where the called action/workflow contract allows it.
- Do not commit secrets, private tokens, webhook URLs, or generated `.env` payloads.
