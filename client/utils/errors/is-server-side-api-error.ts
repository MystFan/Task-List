import { ExceptionReasonCode } from "@adList/http";
import { AxiosError } from "axios";

export type ServerSideApiProblemDetails = {
    additionalData: { [key: string]: unknown },
    code: ExceptionReasonCode;
    message: string;
    status: number;
}

export function isServerSideApiError(error: unknown): error is AxiosError<ServerSideApiProblemDetails> {
    return error instanceof AxiosError;
}
