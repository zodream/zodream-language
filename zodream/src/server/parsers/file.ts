import * as fs from "fs";
import * as path from "path";

interface IFileProperties {
    root?: string;
}

export default class File {

    public properties: IFileProperties = {};

    public path: string = __filename;

    /**
     * hasProperty
     */
    public hasProperty(name: string): boolean {
        return this.properties.hasOwnProperty(name);
    }

    /**
     * getRootPath
     */
    public getRootPath(): string| undefined {
        if (!this.hasProperty('root') || !this.properties['root']) {
            return;
        }
        return path.resolve(this.path, this.properties['root']);
    }

    /**
     * parse
     */
    public static parse(file: string): File {
        const box = new File();
        box.path = file;
        const content = fs.readFileSync(file).toString();
        return box;
    }
}