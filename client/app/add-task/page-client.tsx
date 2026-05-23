"use client";
import { useState } from "react";
import { Grid, Card } from "@mui/material";
import Loader from "@adList/components/Loader/Loader";
import TaskForm, { TaskValues } from "@adList/components/TaskForm/TaskForm";
import axios from "axios";
import * as luxon from "luxon";
import { useRouter } from 'next/navigation';
import { useDispatch } from "react-redux";
import { UpdateSmartTask } from "@adList/app/api/update-task/route";
import { isMutationError, ProblemDetails } from "@adList/utils/errors";
import { addNotification } from "@adList/store/slices/notifications-slice";
import { addError } from "@adList/store/slices/errors-slice";
import { Notifications } from "@adList/components/Notifications/Notifications";

export default function PageClient() {
    const router = useRouter();
    const dispatch = useDispatch();
    const [loading, setLoading] = useState(false);

    const onSubmit = async (values: TaskValues) => {
        setLoading(true);

        try {
            await axios.post("/api/create-task", {
                title: values.title,
                description: values.description,
                dueDate: luxon.DateTime.fromJSDate(values.dueDate!).toISO(),
            } as UpdateSmartTask);
        } catch (error) {
            if (axios.isAxiosError(error)) {
                if (isMutationError(error.response, "InvalidApiRequest")) {
                    const details: ProblemDetails = error.response.data as ProblemDetails;
                    dispatch(addNotification({
                        id: "add-task-error",
                        message: Object.values(details.additionalData.validationErrors ?? []).join("\n"),
                        status: "error"
                    }));
                } else {
                    dispatch(addError(error.response!.data));
                }

                return;
            }
        } finally {
            setLoading(false);
        }

        await router.push("/");
    };

    return (
        <Grid container rowSpacing={1} columnSpacing={{ xs: 1, sm: 2, md: 3 }}>
            <Grid size={12}>
                <Card sx={{ marginTop: "1rem", textAlign: "center" }}><h1>Add Task</h1></Card>
                <Notifications />
                <TaskForm
                    onSubmit={onSubmit}
                    isEditMode={false}
                />
            </Grid>
            <Loader open={loading} />
        </Grid>
    )
}