import { cloneDocsRepository } from './git-helpers';
import * as core from '@actions/core';

console.log('Publishing user documentation to the central docs repository...');

const env = process.env as any;
core.exportVariable('GIT_COMMITTER_NAME', env.GITHUB_ACTOR);
cloneDocsRepository()
    .then(() => {
        console.log('Cloning the docs repository done.');
    })
    .then(() => {
        console.log('Publishing user documentation is done.');
    });

// // copy the markdown files from the build directory to the docs repository
// console.log(process.env);
// let docsSubDirectory = core.getInput('target-docs-path');
// if(!docsSubDirectory) {
//     console.log('No target-docs-path provided. We will use the current repository name as a docs sub-directory.');
//     docsSubDirectory = env.GITHUB_REPOSITORY.split('/')[1];
// }
// const sourceDirectory = core.getInput('source-docs-path');
// console.log(`Copying the markdown files from ${sourceDirectory} to ${docsSubDirectory}...`);
// const targetDirectory = `main-docs/${docsSubDirectory}`;
// if (!fs.existsSync(targetDirectory)) {
//     fs.mkdirSync(targetDirectory);
// }
// // copy all files recursively
// fs.cp(sourceDirectory, targetDirectory, {recursive: true}, (err:any) => {
//     console.error(err);
// });

// // commit and push the changes to the docs repository
