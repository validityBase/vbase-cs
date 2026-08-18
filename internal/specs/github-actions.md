# GitHub Actions

## Policy
- Third-party actions are pinned by full commit SHA for reproducibility.
- Shared vBase-owned actions use `validityBase/vbase-github-actions` with reviewed release tags such as `@v1`.
- Workflow permissions should be explicit and minimal when compatible with the called action/workflow contract.
- Secrets must come from GitHub Secrets or deployment configuration, never from committed files or logs.

## Workflows

### `.github/workflows/build.yml`
- Runs on manual dispatch, pushes to `main`, and pull requests targeting `main`.
- Runs on `windows-latest`.
- Checks out the repository with the pinned `actions/checkout` action.
- Calls the local `.github/workflows/build-solution-action` composite action.
- Passes `API_KEY` and `PRIVATE_KEY` from GitHub Secrets to tests.
- Uploads the generated installer artifact.

### `.github/workflows/build-solution-action/action.yml`
- Sets up .NET 8.
- Restores dependencies and optionally runs tests.
- Uses Visual Studio tooling, `vswhere`, `DisableOutOfProcBuild`, and `devenv.com` because the Visual Studio Installer project is not an ordinary MSBuild-only project.

### `.github/workflows/update-main-docs.yml`
- Runs on pushes to `main` and manual dispatch.
- Runs on `windows-latest`.
- Sets up Node.js 20 for `.github/workflows/patch-ms.js`.
- Sets up MSBuild, restores NuGet packages, and builds the solution without pre/post-build events.
- Runs `patch-ms.js` to make generated Markdown compatible with GitBook.
- Publishes `Docs` with `validityBase/vbase-github-actions/.github/actions/publish-docs@v1`.
- Publishes to the `main` branch of the central docs repository.
- Uses `DOCS_REPO_ACCESS_TOKEN` for the central docs repository.
