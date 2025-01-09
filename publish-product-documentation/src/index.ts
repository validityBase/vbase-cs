import path from 'path';
import { cloneDocsRepository, commitAndPushDocsRepository } from './git-helpers';
import { copyDocs, preprocessMdsInDirectory } from './md-helpers';
import { Constants } from './constants';

console.log('Publishing user documentation to the central docs repository...');

cloneDocsRepository()
    .then(() => {
        return copyDocs();
    })
    .then((prodDocsDirectoryInTheMainDocs) => {
        return preprocessMdsInDirectory(path.join(Constants.MainDocsDirectory, prodDocsDirectoryInTheMainDocs))
            .then(() => prodDocsDirectoryInTheMainDocs);
    })
    .then((prodDocsDirectoryInTheMainDocs) => {
        return commitAndPushDocsRepository(prodDocsDirectoryInTheMainDocs);
    })
    .then(() => {
        console.log('Publishing user documentation is done.');
    });



// // commit and push the changes to the docs repository
