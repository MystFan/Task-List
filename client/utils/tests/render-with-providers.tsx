import { install } from "resize-observer";
import { render, type RenderOptions } from "@testing-library/react";
import { type PropsWithChildren } from "react";
import UserProvider from "@adList/providers/user-provider";
import { ThemeProvider } from "@mui/material/styles";
import theme from "@adList/components/theme";
import { AppStore, RootState, setupStore } from "@adList/store/store";
import { AppWrapper } from "@adList/components/AppWrapper/AppWrapper";

interface ExtendedRenderOptions extends Omit<RenderOptions, "queries"> {
    preloadedState?: Partial<RootState>
    store?: AppStore
}

export function renderWithProviders(ui: React.ReactElement, {
    preloadedState = {},
    store = setupStore(preloadedState),
    ...renderOptions
}: ExtendedRenderOptions = {}) {
    function TestAppWrapper({ children }: PropsWithChildren<{}>) {
        return (
            <AppWrapper store={store}>
                <UserProvider>
                    {children}
                </UserProvider>
            </AppWrapper>
        );
    }

    install();

    return render(ui, { wrapper: TestAppWrapper, ...renderOptions });
}