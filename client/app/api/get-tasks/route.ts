import { NextRequest, NextResponse } from "next/server";
import { auth0 } from "@adList/auth/auth0";
import { GetTasksCommand, GetTasksCommandResponse, SortDirection } from "@adList/http";
import { createApi } from "@adList/http/client/api-client";
import { AxiosResponse } from "axios";
import { withApiErrorHandler } from "@adList/utils/errors";
import {
    SortModelItem
} from "ag-grid-community";

export const POST = withApiErrorHandler(async (request: NextRequest) => {
    const body = await request.json();

    const session = await auth0.getSession();

    const response: AxiosResponse<GetTasksCommandResponse> = await createApi(session).getTasksPost({
        startIndex: body.startRow,
        endIndex: body.endRow,
        sorts: body.sortModel.map((sort: SortModelItem) => {
            return { name: sort.colId, direction: sort.sort === "asc" ? SortDirection.Asc : SortDirection.Desc }
        })
    } as GetTasksCommand);

    return NextResponse.json({
        tasks: response.data.tasks,
        totalCount: response.data.totalCount
    });
});