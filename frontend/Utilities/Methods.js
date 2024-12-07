const NUMBER = "number";
const STRING = "string";
const OBJECT = "object";
const FUNCTION = "function";

export function isNumber(x) {
    return typeof x === NUMBER;
}

export function isString(x) {
    return typeof x === STRING;
}

export function isObject(x) {
    return typeof x === OBJECT;
}

export function isFunction(x) {
    return typeof x === FUNCTION;
}