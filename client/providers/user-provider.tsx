"use client"

import { createContext, useContext, PropsWithChildren } from "react"
import { useUser } from "@auth0/nextjs-auth0/client"
import { User } from "@auth0/nextjs-auth0/types";

export interface AuthUserInterface {
    user?: User | null;
    isLoading: boolean;
    error?: Error | null;
    invalidate: () => Promise<User | undefined>;
}

const UserContext = createContext<AuthUserInterface>(null!);

export default function UserProvider(props: PropsWithChildren) {
    const auth = useUser();

    return (
        <UserContext.Provider value={auth}>
            {props.children}
        </UserContext.Provider>
    )
}

export function useAuthUser() {
    return useContext(UserContext)
}