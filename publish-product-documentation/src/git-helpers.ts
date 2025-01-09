import * as core from '@actions/core';
import { run } from './process-helpers';
import { Constants } from './constants';

const docsRepoAccessToken = core.getInput('docs-repo-access-token');
const docsRepository = core.getInput('target-repository');

export async function cloneDocsRepository(): Promise<void> {
    console.log(`Cloning the docs repository: "${docsRepository}"...`);
    await run("git", ["clone", `https://${docsRepoAccessToken}@github.com/${docsRepository}.git`, Constants.MainDocsDirectory], null);
    console.log('Cloning the docs repository done.');
}

export async function commitAndPushDocsRepository(productDocsSubDirectory: string): Promise<void> {
    console.log('Committing and pushing the changes to the docs repository...');
    
    await run("git", ["config", "user.name", "github-actions[bot]"], Constants.MainDocsDirectory);
    await run("git", ["config", "user.email", "github-actions[bot]@users.noreply.github.com"], Constants.MainDocsDirectory);
    await run("git", ["add", productDocsSubDirectory], Constants.MainDocsDirectory);
    await run("git", ["diff-index", "--quiet", "HEAD"], Constants.MainDocsDirectory)
        .then(() => {
            // no changes
            console.log('No changes in the docs repository.');
        })
        .catch(async () => {
            // there are changes
            console.log('Committing the changes to the docs repository...');
            console.log(process.env);
            // git commit -m "Update vbase-py-samples documentation from automated build"
            // git push https://$DOCS_BUILD_PAT@github.com/validityBase/docs.git main
            //await run("git", ["commit", "-m", `Update ${productDocsSubDirectory} documentation from automated build`], Constants.MainDocsDirectory);
            //await run("git", ["push", `https://${docsRepoAccessToken}@github.com/${docsRepository}.git`, ], Constants.MainDocsDirectory);

        });
}