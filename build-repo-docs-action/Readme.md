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
2. Make the changes in the corresponding TypeScript (TS) file in the `src` folder.  
3. Run the following command to compile the TS file into a JS file: `npm run build`. This will update the `index.js` file in the action root folder.  
4. Commit the changes and push them to the repository.  
