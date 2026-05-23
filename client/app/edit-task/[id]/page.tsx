import { auth0 } from "@adList/auth/auth0";
import { createApi } from "@adList/http/client/api-client";
import PageClient from "./page-client";
import { GetSmartTaskQueryResponse } from "@adList/http";
import { AxiosResponse } from "axios";

export default async function Page({ params }: { params: Promise<{ id: string }> }) {
    const { id } = await params;

    const session = await auth0.getSession();

    const response: AxiosResponse<GetSmartTaskQueryResponse> = await createApi(session).getTaskGet({ params: { id: id } });

    return (
        <PageClient
            id={response.data.id!}
            title={response.data.title!}
            description={response.data.description}
            dueDate={response.data.dueDate}
            status={response.data.completionStatus}
        />
    );
}
