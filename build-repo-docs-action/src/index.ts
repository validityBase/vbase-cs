const { exec } = require('child_process');
import * as core from '@actions/core';

console.log('Publishing user documentation to the central docs repository...');

const docsRepoAccessToken = core.getInput('docs-repo-access-token');
const docsRepository = core.getInput('docs-repository');

// checkout the docs repository
console.log(`Cloning the docs repository ${docsRepository}...`);
exec(`git clone https://${docsRepoAccessToken}@github.com/${docsRepository}.git main-docs`);
exec("cd main-docs");
console.log('Cloning the docs repository done.');

// copy the markdown files from the build directory to the docs repository

// commit and push the changes to the docs repository
