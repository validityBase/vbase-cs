## Overall Convention for Documentation Publishing

1. The user-facing documentation should be stored in the `docs` folder of the product repository.
The documentation should be written in Markdown format.
1. The internal documentation should be stored in the dev-docs folder of the product repository. It'll not be published to the central documentation repository.
1. Each product repository should have a workflow for documentation publishing. The workflow should be based on this action. Here is an example of the workflow YAML file:
``` yaml
name: Update the Main Docs Repository

on:
  push:
    branches:
      - main

jobs:
  update-main-docs:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      - name: Publish Documents
        uses: validityBase/docs/publish-product-documentation@main
        with:
          docs-repo-access-token: ${{ secrets.DOCS_REPO_ACCESS_TOKEN }} #source-docs-path: # optional - default is 'docs'
          #target-docs-path: # optional - default is the name of the current repository
          #target-repository: # optional - default is 'validityBase/docs'
          #target-repository-branch: # optional - default is the branch name of the current product branch
          #preprocess-plant-uml: # optional - default is 'true'
    
```
1. The diagrams in the documentation should be created using PlantUML. The source code for the diagrams should be enclosed in:\
\`\`\` plantuml(Diagram Name)\
 DIAGRAM CODE GOES HERE\
\`\`\`
1. To view the diagrams in the development environment, please use a corresponding plugin. For example, for Visual Studio Code, you can use the [Markdown Plantuml Preview](https://marketplace.visualstudio.com/items?itemName=myml.vscode-markdown-plantuml-preview) extension.
1. During the document publishing process, this action will automatically replace all PlantUML code blocks with the corresponding image links to make them visible in the end-user browser.

# Purpose of this Custom Action  
This action is created to simplify the process of publishing user documentation from the product repository to the central vBase documentation repository.  

It copies the Markdown files from the `Docs` folder of the product repository to the folder named after the product repository in the central vBase documentation repository.  

## PlantUML Diagrams Preprocessing  
Besides copying the files, it also preprocesses the PlantUML diagrams in the Markdown files. Specifically, it performs the following tasks:  
- Searches for diagram code blocks defined using the pattern:  
\`\`\` plantuml\
 DIAGRAM CODE GOES HERE\
\`\`\`
- Then it compiles the diagram code into an actual image using the PlantUML online service.  
- Finally, it replaces the diagram code with an image entry, so the diagram can be viewed in a browser.  

## How to Make Changes to the Action  
1. Install Node.js and npm on your development machine.  
1. Make the changes in the corresponding TypeScript (TS) file in the `src` folder.  
1. Run the following command to compile the TS file into a JS file: `npm run build`. This will update the `index.js` file in the action root folder.  
1. Commit the changes and push them to the repository.  
