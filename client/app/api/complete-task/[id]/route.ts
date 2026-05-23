import { NextRequest, NextResponse } from "next/server";
import { CompleteSmartTaskCommand } from "@adList/http";
import { createApi } from "@adList/http/client/api-client";
import { RouteHandlerContext, withApiErrorHandler } from "@adList/utils/errors";
import { withApiSessionHandler } from "@adList/utils/with-api-session-handler";

export const PUT = withApiErrorHandler(
    withApiSessionHandler(async (req: NextRequest, context: RouteHandlerContext) => {
        const params = await context.params;

        await createApi(req.session).completeTaskPut({ id: Number(params!.id) } as CompleteSmartTaskCommand);

        return NextResponse.json({});
    })
);