import errorsReducer from "./slices/errors-slice";
import notificationsReducer from "./slices/notifications-slice";
import { combineReducers, configureStore } from "@reduxjs/toolkit";

const rootReducer = combineReducers({
    errors: errorsReducer,
    notifications: notificationsReducer
});

export const setupStore = (preloadedState?: Partial<RootState>) => {
    const store = configureStore({
        devTools: process.env.NODE_ENV === "development",
        middleware: (getDefaultMiddleware) => getDefaultMiddleware(),
        preloadedState,
        reducer: rootReducer,
    });

    return store;
};

export type RootState = ReturnType<typeof rootReducer>;
export type AppStore = ReturnType<typeof setupStore>;
