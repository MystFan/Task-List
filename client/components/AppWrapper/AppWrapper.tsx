"use client";
import { useMemo, PropsWithChildren } from "react";
import { Provider } from "react-redux";
import { AppStore, setupStore } from "@adList/store";
import { AppRouterCacheProvider } from "@mui/material-nextjs/v16-appRouter";
import { ThemeProvider } from "@mui/material/styles";
import theme from "@adList/components/theme";
import UserProvider from "@adList/providers/user-provider";

export type AppWrapperProps = {
    children: React.ReactNode;
    store?: AppStore;
}

export function AppWrapper(props: PropsWithChildren<AppWrapperProps>) {
    const store = useMemo(() => props.store ? props.store : setupStore(), [props.store]);

    return (
        <AppRouterCacheProvider options={{ enableCssLayer: true }}>
            <ThemeProvider theme={theme}>
                <Provider store={store}>
                    <UserProvider>
                        {props.children}
                    </UserProvider>
                </Provider>
            </ThemeProvider>
        </AppRouterCacheProvider>
    );
}