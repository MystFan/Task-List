import { NextRequest, NextResponse } from "next/server";
import { auth0 } from "@adList/auth/auth0";
import { DeleteSmartTaskCommand } from "@adList/http";
import { createApi } from "@adList/http/client/api-client";
import { RouteHandlerContext, withApiErrorHandler } from "@adList/utils/errors";

export const DELETE = withApiErrorHandler(async (_: NextRequest, context: RouteHandlerContext) => {
    const params = await context.params;

    const session = await auth0.getSession();

    await createApi(session).deleteTaskDelete({ id: Number(params!.id) } as DeleteSmartTaskCommand);

    return NextResponse.json({});
});