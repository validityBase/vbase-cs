import * as core from '@actions/core';
import { Constants } from './constants';
import * as fs from 'fs';

const env = process.env as any;

export async function copyDocs(): Promise<string> {
    // copy the markdown files from the build directory to the docs repository
    
    let docsSubDirectory = core.getInput('target-docs-path');
    if(!docsSubDirectory) {
        console.log('No target-docs-path provided. We will use the current repository name as a docs sub-directory.');
        docsSubDirectory = env.GITHUB_REPOSITORY.split('/')[1];
    }
    const sourceDirectory = core.getInput('source-docs-path') + "/";
    const targetDirectory = `${Constants.MainDocsDirectory}/${docsSubDirectory}`;
    console.log(`Copying the files from ${sourceDirectory} to ${targetDirectory}...`);

    if (!fs.existsSync(targetDirectory)) {
        fs.mkdirSync(targetDirectory);
    }
    // copy all files recursively
    fs.cpSync(sourceDirectory, targetDirectory, {recursive: true, filter: (src: string) => {
        // we use this filter to log the files being copied
        console.log(`Copying ${src}`);
        return true; // copy all files
    }});

    return targetDirectory;
}