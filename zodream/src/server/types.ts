export const DEFAULT_ENTRY = 'main.z';
export const FILE_REGEX = /\.z$/;
export const ROOT_REGEX = /[root=([^])]/;

export enum PROPERTY_OPEN {
    PUBLIC,
    PRIVATE,
}

export enum PROPERTY_TYPE {
    PROPERTY,
    STATIC,
    CONST
}

export enum PROPERTY_EDIT {
    READONLY,
    MUT
}