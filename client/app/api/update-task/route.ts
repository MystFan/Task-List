import { NextRequest, NextResponse } from "next/server";
import { auth0 } from "@adList/auth/auth0";
import { CompletionStatus, UpdateSmartTaskCommand } from "@adList/http";
import { createApi } from "@adList/http/client/api-client";
import { withApiErrorHandler } from "@adList/utils/errors";

export type UpdateSmartTask = {
    id: number;
    title: string;
    description?: string;
    dueDate?: string;
    status?: string;
}

export const PUT = withApiErrorHandler(async (req: NextRequest) => {
    const payload: UpdateSmartTask = await req.json();

    const session = await auth0.getSession();

    await createApi(session).updateTaskPut({
        id: payload.id,
        title: payload.title,
        description: payload.description,
        dueDate: payload.dueDate,
        completionStatus: payload.status === "Completed" ? CompletionStatus.Completed : CompletionStatus.Pending
    } as UpdateSmartTaskCommand);

    return NextResponse.json({});
});