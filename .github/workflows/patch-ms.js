const fs = require('fs');
const path = require('path');
const dir = 'Docs';
fs.readdirSync(dir).forEach(file => {
    if (path.extname(file) === '.md') {
        const filePath = path.join(dir, file);
        console.log('Processing: ' + filePath);

        const data = fs.readFileSync(filePath, 'utf8');
        const result = data
            // fix TOC
            .replace(/\[([^\]]+)\]\(#([^ ]+) '([^']+)'\)/gm, "[$1](#|||$2||| '$3')")
            // fix anchors
            .replace(/\<a name='([^']+)'\>\<\/a\>[\r\n]+(#+)\ (.+)/gm, '$2 $3 <a name="|||$1|||" id="|||$1|||" href="#|||$1|||"></a>')
            // at this point all refs are outlined in the format |||anchor||| we need to URL escape and lowercase them
            .replace(/\|\|\|([^|]+)\|\|\|/gm, (_, p1) => {
                if(p1.startsWith('http')) return p1; // ignore external links
                console.log('\tEscaping: ' + p1);

                let refStr = p1; 
                [',','`',"'",'"','(',')','{','}', '-', '#'].forEach(function(letter){
                    refStr = refStr.replaceAll(letter, '');
                })
                refStr = refStr.toLowerCase()
                return encodeURIComponent(refStr);
            });

        fs.writeFileSync(filePath, result, 'utf8');
    }
});
