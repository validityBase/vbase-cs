# Agent Memory

## GitHub Actions
- Third-party GitHub Actions are pinned to full commit SHAs.
- vBase-owned shared actions use reviewed `validityBase/vbase-github-actions` version tags.
- Documentation publishing uses `validityBase/vbase-github-actions/.github/actions/publish-docs@v1`.
- The docs workflow intentionally remains local because it needs Windows, MSBuild, NuGet restore, and the Node-based `patch-ms.js` post-processing script.
- `build-solution-action` is local because it uses Visual Studio/devenv.com installer-project behavior that is specific to this repo.
- Build tests use `API_KEY` and `PRIVATE_KEY` from GitHub Secrets.
