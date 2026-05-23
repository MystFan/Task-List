export function nameof<T>(name: Extract<keyof T, string>): string;
export function nameof<T>(ctor: T & Function): string;
export function nameof<T>(ctorOrName: Extract<keyof T, string> | T & Function) {
    if (typeof ctorOrName === "function") {
        return ctorOrName.prototype.constructor.name;
    }
    return ctorOrName;
}