import * as fs from "fs";
import * as path from "path";
import ClassNode from "../nodes/class";
import InterfaceNode from "../nodes/interface";

interface IFileProperties {
    root?: string;
}

export default class File {

    public properties: IFileProperties = {};

    public path: string = __filename;

    public node: ClassNode | InterfaceNode | null = null;

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
        const name = path.basename(file);
        const content = fs.readFileSync(file).toString();
        if (name.charAt(0) === '_') {
            box.node = InterfaceNode.parse(name.substr(1), content);
        } else {
            box.node = ClassNode.parse(name, content);
        }
        return box;
    }
}