import { ApplicationError, isServerSideApiError } from "@adList/utils/errors";
import { ProblemDetails } from "./problem-details";
import { NextRequest, NextResponse } from "next/server";

export type RouteHandlerContext = {
    params?: Promise<Record<string, string>>;
};

type NextHandler = (
    req: NextRequest,
    context: RouteHandlerContext
) => Promise<NextResponse>;

export const withApiErrorHandler = (handler: NextHandler) => {
    return async (req: NextRequest, context: RouteHandlerContext) => {
        try {
            return await handler(req, context);
        } catch (error: any) {
            let problemDetails: ProblemDetails;
            let status: number;

            if (isServerSideApiError(error)) {
                status = error.code === "ECONNREFUSED"
                    ? 503
                    : error.response?.status ?? 500;

                let code: ApplicationErrorCode;

                switch (error.response?.data.code) {
                    case "UserNotFound":
                        code = "UserNotFound";
                        break;
                    case "InvalidRequest":
                        code = "InvalidApiRequest";
                        break;
                    case "TaskAlreadyCompleted":
                        code = "TaskAlreadyCompleted";
                        break;
                    default:
                        code = "UnexpectedError";
                        break;
                }

                problemDetails = new ProblemDetails({
                    additionalData: {
                        ...error.response?.data.additionalData,
                        api: {
                            code: error.response?.data.code!,
                            message: error.response?.data.message!,
                            method: error.config?.method!,
                            status: error.response?.status!,
                            url: error.config?.url ? new URL(error.config?.url).pathname : ""
                        }
                    },
                    code,
                    detail: error.message,
                    instance: req.url
                });
            } else if (error instanceof ApplicationError) {
                status = 400;

                problemDetails = new ProblemDetails({
                    code: error.code,
                    detail: error.message,
                    instance: req.url
                });
            } else {
                status = 500;

                problemDetails = new ProblemDetails({
                    code: "UnexpectedError",
                    detail: error.message,
                    instance: req.url
                });
            }

            console.error(`${error.stack} ${JSON.stringify(problemDetails, null, 2)}`);

            return NextResponse.json(problemDetails, { status: status });
        }
    };
}
