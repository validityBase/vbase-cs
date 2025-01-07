module.exports = async ({sourceFolder, mainDocsFolder, targetFolder, context}) => {
    // const fs = require('fs');
    // const path = require('path');


    // if (!targetFolder) {
    //     console.log('No target folder specified. We will use repository name as a default value.');
    //     targetFolder = context.repo.repo.split('/')[1];
    // }

    // targetFolder = path.join(mainDocsFolder, targetFolder);
    // console.log(`Copying markdown files from "${sourceFolder}" to "${targetFolder}"`);

    // // ensure target folder exists
    // if (!fs.existsSync(targetFolder)) {
    //     fs.mkdirSync(targetFolder, {recursive: true});
    // }

    // // copy all files recursively
    // fs.cp(sourceFolder, targetFolder, {recursive: true}, (err) => {
    //     console.error(err);
    // });
}