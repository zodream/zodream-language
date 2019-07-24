import * as fs from "fs";
import * as path from "path";
import { DEFAULT_ENTRY, FILE_REGEX } from "../types";
import File from "./file";

interface IFiles {
    [path: string]: File;
}

export default class Program {
    public entry: string = __filename; // 入口文件

    public root: string = __dirname; // 根目录

    protected files: IFiles = {};

    constructor(root: string, file?: string) {
        this.setRoot(root, file);
        this.verifyRoot();
    }

    /**
     * getFile
     */
    public getFile(file: string): File {
        file = path.resolve(this.root, file);
        if (this.files.hasOwnProperty(file)) {
            return this.files[file];
        }
        return this.files[file] = File.parse(file);
    }

    /**
     * 验证项目根目录，允许在入口文件中设置根目录
     */
    private verifyRoot() {
        if (!this.entry || !fs.existsSync(this.entry)) {
            return;
        }
        const file = this.getFile(this.entry);
        if (!file.hasProperty('root')) {
            return;
        }
        this.root = file.getRootPath() as string;
    }

    /**
     * 根据启动路径设置根目录及入口文件
     * @param file 
     */
    private setRoot(root: string, file?: string) {
        if (file) {
            this.root = root;
            this.entry = path.resolve(root, file);
            return;
        }
        const state = fs.statSync(root);
        if (state.isFile()) {
            this.entry = root;
            this.root = path.dirname(root);
            return;
        }
        if (!state.isDirectory()) {
            return;
        }
        this.root = root;
        this.entry = this.getEntry(root);
    }

    private getEntry(root: string): string {
        let file = path.resolve(root, DEFAULT_ENTRY);
        if (fs.existsSync(file)) {
            return file;
        }
        const files = fs.readdirSync(root);
        for (const item of files) {
            if (FILE_REGEX.test(item)) {
                return path.resolve(root, item);
            }
        }
        throw new Error("entry not found");
    }
}