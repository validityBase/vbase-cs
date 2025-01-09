import * as core from '@actions/core';
import { Constants } from './constants';
import * as fs from 'fs';
import path from 'path';

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

    return docsSubDirectory;
}

export async function preprocessMdsInDirectory(directory: string): Promise<void> {
    // iterate over all markdown files in the directory and preprocess them
    console.log(`Preprocessing markdown files in ${directory}...`);

    const files = getFiles(directory);

    for (let i = 0; i < files.length; i++) {

        if(path.extname(files[i]) !== '.md') {
            console.log(`Skipping ${files[i]}`);
            continue;
        }

        console.log(`Preprocessing ${files[i]}...`);
        let content = fs.readFileSync(files[i], 'utf8');
        content = 'PREPROCESSED\r\n' + content;
        fs.writeFileSync(files[i], content);
    }
}

function getFiles(dir: string): Array<string> {
    const fsEntries = fs.readdirSync(dir, { withFileTypes: true });
    let res: string[] = [];
    for (let i = 0; i < fsEntries.length; i++) {
      if (fsEntries[i].isDirectory()) {
        res = res.concat(getFiles(path.join(dir, fsEntries[i].name)));
      } else {
        res = res.concat([path.join(dir, fsEntries[i].name)]);
      }
    }
    return res;
  }