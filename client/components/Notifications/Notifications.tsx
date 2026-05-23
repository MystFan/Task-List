"use client";
import { useEffect } from "react";
import { RootState } from "@adList/store";
import { useDispatch, useSelector } from "react-redux";
import { removeAllNotifications, removeNotification } from "@adList/store/slices/notifications-slice";
import { AlertComponent, AlertComponentTestIds } from "@adList/components/AlertComponent/AlertComponent";

export function Notifications() {
    const dispatch = useDispatch();
    const notifications = useSelector((state: RootState) => state.notifications);

    useEffect(() => {
        dispatch(removeAllNotifications());
        return () => {
            dispatch(removeAllNotifications());
        };
    }, [dispatch]);

    return (
        <>
            {notifications.map(notification => (
                <AlertComponent
                    key={notification.id}
                    id={`${notification.id}`}
                    status={notification.status}
                    onClose={() => {
                        dispatch(removeNotification(notification.id));
                    }}
                >
                    {notification.message}
                </AlertComponent>)
            )}
        </>
    );
}

export const NotificationsTestIds = {
    alert: {
        root: (id: string) => AlertComponentTestIds.root(id)
    }
};
