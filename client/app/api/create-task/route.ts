import { NextRequest, NextResponse } from "next/server";
import { auth0 } from "@adList/auth/auth0";
import { CreateSmartTaskCommand } from "@adList/http";
import { createApi } from "@adList/http/client/api-client";
import { withApiErrorHandler } from "@adList/utils/errors";

export type CreateSmartTask = {
    title: string;
    description?: string;
    dueDate?: string;
}

export const POST = withApiErrorHandler(async (req: NextRequest) => {
    const payload: CreateSmartTask = await req.json();

    const session = await auth0.getSession();

    await createApi(session).createTaskPost({
        title: payload.title,
        description: payload.description,
        dueDate: payload.dueDate
    } as CreateSmartTaskCommand);

    return NextResponse.json({});
});