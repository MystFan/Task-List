import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export type NotificationStatus = "error" | "warning" | "success" | "info";

export type AddNotificationPayloadAction = {
    id: string;
    message: string;
    status: NotificationStatus;
};

export type NotificationState = {
    id: string;
    message: string;
    status: NotificationStatus;
};

export const notificationsSlice = createSlice({
    name: "notifications",
    initialState: [] as NotificationState[],
    reducers: {
        addNotification: (state, action: PayloadAction<AddNotificationPayloadAction>): NotificationState[] => {
            const notificationIndex = state.findIndex(notification => notification.id === action.payload.id);
            if (notificationIndex !== -1) {
                state[notificationIndex].message = action.payload.message;
                state[notificationIndex].status = action.payload.status;
                return state;
            }
            state.push({
                id: action.payload.id,
                message: action.payload.message,
                status: action.payload.status
            });
            return state;
        },
        removeAllNotifications: () => {
            return [];
        },
        removeNotification: (state, action: PayloadAction<string>): NotificationState[] => {
            state.splice(state.findIndex(notification => notification.id === action.payload), 1);
            return state;
        },
    }
});

export const {
    addNotification,
    removeAllNotifications,
    removeNotification
} = notificationsSlice.actions;

export default notificationsSlice.reducer;
