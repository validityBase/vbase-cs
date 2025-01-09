import { cloneDocsRepository, commitAndPushDocsRepository } from './git-helpers';
import { copyDocs } from './md-helpers';


console.log('Publishing user documentation to the central docs repository...');

cloneDocsRepository()
    .then(() => {
        return copyDocs();
    })
    .then((prodDocsDirectoryInTheMainDocs) => {
        return commitAndPushDocsRepository(prodDocsDirectoryInTheMainDocs);
    })
    .then(() => {
        console.log('Publishing user documentation is done.');
    });



// // commit and push the changes to the docs repository
