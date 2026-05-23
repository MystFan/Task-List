export class ApplicationError extends Error {
    constructor(public readonly code: ApplicationErrorCode, message: string = "") {
        super(message);
    }
}
