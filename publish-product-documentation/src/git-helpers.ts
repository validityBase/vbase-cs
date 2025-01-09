import * as core from '@actions/core';
import { run } from './process-helpers';

const docsRepoAccessToken = core.getInput('docs-repo-access-token');
const docsRepository = core.getInput('target-repository');

export async function cloneDocsRepository() {
    console.log(`Cloning the docs repository "${docsRepository}"...`);
    await run("git", ["clone", `https://${docsRepoAccessToken}@github.com/${docsRepository}.git`, "main-docs"]);
    console.log('Cloning the docs repository done.');
}