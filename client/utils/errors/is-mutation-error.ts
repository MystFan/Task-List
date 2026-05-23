import { AdditionalData, ProblemDetails } from "@adList/utils/errors/problem-details";
import { nameof } from "@adList/utils/nemeof";
import { isServerSideApiError } from "./is-server-side-api-error";
import { AxiosError } from "axios";

export type MutationError = {
    data: ProblemDetails;
    status: number;
}

export type MutationErrorWithAdditionalDataEnsured<K extends keyof AdditionalData> = MutationError & {
    data: ProblemDetails & { additionalData: Required<Pick<AdditionalData, K>> }
}

export function isMutationError(error: any): error is MutationError;
export function isMutationError(error: any, code?: ApplicationErrorCode): error is MutationError;
export function isMutationError(error: any, code: "InvalidApiRequest"): error is MutationErrorWithAdditionalDataEnsured<"validationErrors">;
export function isMutationError(error: any, code?: ApplicationErrorCode): error is MutationError {
    return !code || (error.data && (error.data as ProblemDetails).code === code);
}

export function isMutationFieldError<TModel>(error: any, fieldName: keyof TModel):
    error is MutationErrorWithAdditionalDataEnsured<"validationErrors"> & { validationErrors: { [key in keyof TModel]: string } } {
    return isMutationError(error, "InvalidApiRequest") && !!error.data.additionalData.validationErrors[fieldName as string];
}
