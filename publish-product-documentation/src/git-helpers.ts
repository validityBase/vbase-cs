import * as core from '@actions/core';
import { run } from './process-helpers';
import { Constants } from './constants';

const docsRepoAccessToken = core.getInput('docs-repo-access-token');
const docsRepository = core.getInput('target-repository');

export async function cloneDocsRepository() {
    await run('echo', ["$PWD"], null);
    console.log(`Cloning the docs repository: "${docsRepository}"...`);
    await run("git", ["clone", `https://${docsRepoAccessToken}@github.com/${docsRepository}.git`, Constants.MainDocsDirectory], null);
    console.log('Cloning the docs repository done.');
}

export async function commitAndPushDocsRepository(productDocsFolderToAdd: string) {
    console.log('Committing and pushing the changes to the docs repository...');
    await run('echo', ["$PWD"], Constants.MainDocsDirectory);
    await run("git", ["config", "user.name", "github-actions[bot]"], Constants.MainDocsDirectory);
    await run("git", ["config", "user.email", "github-actions[bot]@users.noreply.github.com"], Constants.MainDocsDirectory);
    await run("git", ["add", productDocsFolderToAdd], Constants.MainDocsDirectory);
    var diffOutput = await run("git", ["diff-index", "--quiet", "HEAD"], Constants.MainDocsDirectory);
    console.log(`diffOutput: ${diffOutput}`);
}