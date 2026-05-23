import { auth0 } from "@adList/auth/auth0";
import { redirect } from "next/navigation";
import MasterLayout from "@adList/components/MasterLayout/MasterLayout";
import { PropsWithChildren } from "react";
import { AppWrapper } from "@adList/components/AppWrapper/AppWrapper";

export default async function Template(props: PropsWithChildren) {
    const session = await auth0.getSession();

    if (!session?.user) {
        redirect("/auth/login");
    }

    return (
        <AppWrapper>
            <MasterLayout>{props.children}</MasterLayout>;
        </AppWrapper>
    )
}