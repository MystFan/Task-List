import { auth0 } from "@adList/auth/auth0";
import { NextRequest, NextResponse } from "next/server";

export type RouteHandlerContext = {
    params?: Promise<Record<string, string>>;
};

type NextHandler = (
    req: NextRequest,
    context: RouteHandlerContext
) => Promise<NextResponse>;

export const withApiSessionHandler = (handler: NextHandler) => {
    return async (req: NextRequest, context: RouteHandlerContext) => {
        const session = await auth0.getSession();
        req.session = session;
        return await handler(req, context);
    };
}
