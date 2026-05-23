export class ProblemDetails {
    public readonly additionalData: AdditionalData;
    public readonly code: ApplicationErrorCode;
    public readonly detail: string;
    public readonly instance?: string;

    public constructor(options: ProblemDetailsOptions) {
        this.additionalData = options.additionalData ?? {};
        this.code = options.code;
        this.detail = options.detail;
        this.instance = options.instance;
    }
}

export type ProblemDetailsOptions = {
    additionalData?: AdditionalData;
    code: ApplicationErrorCode;
    detail: string;
    instance?: string;
}

export type AdditionalData = {
    api?: {
        code: string;
        message: string;
        method: string;
        status: number;
        url: string;
    },
    userLockedInMinutes?: number;
    validationErrors?: { [key: string]: string };
}