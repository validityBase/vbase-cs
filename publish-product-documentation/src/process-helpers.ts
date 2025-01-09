const { spawn } = require('node:child_process');

export async function run(cmd: string, args: Array<string>): Promise<void> {

    return new Promise((resolve, reject) => {
        const process = spawn(cmd, args);

        process.stdout.on('data', (data: any) => {
        console.log(`stdout: ${data}`);
        });
        
        process.stderr.on('data', (data:any) => {
        console.error(`stderr: ${data}`);
        });
        
        process.on('close', (code:any) => {
            console.log(`child process exited with code ${code}`);
            if(code !== 0) { reject(); }
            resolve();
        });
    });
}