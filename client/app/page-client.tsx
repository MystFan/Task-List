"use client";
import { useState, useCallback, useMemo, useRef } from "react";
import { redirect } from "next/navigation";
import * as luxon from "luxon";
import { useColorScheme } from "@mui/material/styles";
import {
    AllCommunityModule,
    GridReadyEvent,
    IDatasource,
    IGetRowsParams,
    ValueFormatterParams,
    ICellRendererParams,
    ClientSideRowModelModule
} from "ag-grid-community";
import { AgGridProvider, AgGridReact } from "ag-grid-react";
import { Box, Button, Chip } from "@mui/material";
import axios from "axios";
import ConfirmDialog from "@adList/components/ConfirmDialog/ConfirmDialog";
import Loader from "@adList/components/Loader/Loader";
import { Notifications } from "@adList/components/Notifications/Notifications";
import { GetTaskCommandResponse } from "@adList/http";
import { isMutationError, ProblemDetails } from "@adList/utils/errors";
import { addNotification } from "@adList/store/slices/notifications-slice";
import { addError } from "@adList/store/slices/errors-slice";
import { useDispatch } from "react-redux";

const modules = [AllCommunityModule, ClientSideRowModelModule];

export default function PageClient() {
    const { mode } = useColorScheme();
    const dispatch = useDispatch();
    const [loading, setLoading] = useState(true);
    const [deleteTaskId, setDeleteTaskId] = useState<number | undefined>();
    const [completeTaskId, setCompleteTaskId] = useState<number | undefined>();
    const gridRef = useRef<AgGridReact<GetTaskCommandResponse>>(null);

    const columnDefs = useMemo(() => [
        { field: "title", width: 150 },
        { field: "description", width: 150 },
        { field: "authorName", headerName: "Author", width: 150 },
        {
            field: "createdAt",
            headerName: "Created At",
            width: 170,
            valueFormatter: (params: ValueFormatterParams) => {
                return params.value ? luxon.DateTime.fromISO(params.value).toFormat("dd/MM/yyyy HH:mm:ss") : "";
            }
        },
        {
            field: "dueDate",
            headerName: "Due Date",
            width: 170,
            valueFormatter: (params: ValueFormatterParams) => {
                return params.value ? luxon.DateTime.fromISO(params.value).toFormat("dd/MM/yyyy HH:mm:ss") : "";
            }
        },
        {
            headerName: "Status",
            field: "completionStatus",
            cellRenderer: (params: ICellRendererParams<GetTaskCommandResponse>) => {
                if (loading) {
                    return <></>;
                }

                return (
                    <Box sx={{ display: "flex", alignItems: "center" }}>
                        {params.data?.completionStatus === "Incomplete" &&
                            <Chip label={params.data?.completionStatus} color="primary" />
                        }
                        {params.data?.completionStatus === "Completed" &&
                            <Chip label={params.data?.completionStatus} color="success" />
                        }
                    </Box>
                )
            },
            width: 120,
            cellStyle: {
                display: "flex",
                alignItems: "center"
            }
        },
        {
            headerName: "Actions",
            field: "actions",
            width: 220,
            cellRenderer: (params: ICellRendererParams<GetTaskCommandResponse>) => {
                if (loading) {
                    return <></>;
                }

                return (
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                        {params.data?.completionStatus !== "Completed" &&
                            <Button variant="contained" size="small" onClick={e => {
                                e.preventDefault();
                                setCompleteTaskId(params.data!.id)
                            }}>
                                Complete
                            </Button>
                        }
                        <Button variant="contained" size="small" onClick={e => {
                            e.preventDefault();
                            redirect("/edit-task/" + params.data!.id)
                        }}>
                            Edit
                        </Button>
                        <Button variant="contained" size="small" onClick={e => {
                            e.preventDefault();
                            setDeleteTaskId(params.data!.id);
                        }}>
                            Delete
                        </Button>
                    </Box>
                )
            },
            sortable: false,
            filter: false,
            cellStyle: {
                display: "flex",
                alignItems: "center"
            }
        },
    ], [loading, setDeleteTaskId]);

    const dataSource: IDatasource = useMemo(() => {
        const dataSource: IDatasource = {
            getRows: async (params: IGetRowsParams) => {
                const { startRow, endRow, sortModel } = params;

                const response = await axios.post("/api/get-tasks", {
                    startRow, endRow, sortModel
                });

                setLoading(false);

                params.successCallback(response.data.tasks, response.data.totalCount);
            }
        };

        return dataSource;
    }, []);

    const onGridReady = useCallback((event: GridReadyEvent) => {
        event.api.sizeColumnsToFit();
    }, []);

    return (
        <AgGridProvider modules={modules}>
            <Notifications />
            <Box data-ag-theme-mode={
                mode === "system"
                    ? window.matchMedia("(prefers-color-scheme: dark)").matches
                        ? "dark"
                        : "light"
                    : mode}
                sx={{ height: "500px", width: "100%", marginTop: "10px" }}>
                <AgGridReact
                    ref={gridRef}
                    columnDefs={columnDefs}
                    onGridReady={onGridReady}
                    rowModelType="infinite"
                    cacheBlockSize={50}
                    pagination={false}
                    datasource={dataSource}
                />
            </Box>
            <Loader open={loading} />
            <ConfirmDialog
                id="confirm-delete-dialog"
                open={!!deleteTaskId}
                title="Confirm Delete"
                content="Are you sure you want to delete"
                confirm="Delete"
                handleClose={() => setDeleteTaskId(undefined)}
                handleSave={async () => {
                    if (deleteTaskId && gridRef?.current) {
                        setLoading(true);

                        try {
                            await axios.delete("/api/delete-task/" + deleteTaskId);
                        } catch (error) {
                            if (axios.isAxiosError(error)) {
                                if (isMutationError(error.response, "InvalidApiRequest")) {
                                    const details: ProblemDetails = error.response.data as ProblemDetails;
                                    dispatch(addNotification({
                                        id: "edit-task-error",
                                        message: Object.values(details.additionalData.validationErrors ?? []).join("\n"),
                                        status: "error"
                                    }));
                                } else {
                                    dispatch(addError(error.response!.data));
                                }

                                return;
                            }
                        } finally {
                            gridRef?.current.api.refreshInfiniteCache();

                            setDeleteTaskId(undefined);
                            setLoading(false);
                        }
                    }
                }}
            />
            <ConfirmDialog
                id="confirm-complete-dialog"
                open={!!completeTaskId}
                title="Confirm Complete"
                content="Are you sure you want to complete the task"
                confirm="Complete"
                handleClose={() => setCompleteTaskId(undefined)}
                handleSave={async () => {
                    if (completeTaskId && gridRef?.current) {
                        setLoading(true);

                        try {
                            await axios.put("/api/complete-task/" + completeTaskId);
                        } catch (error) {
                            if (axios.isAxiosError(error)) {
                                if (isMutationError(error.response, "TaskAlreadyCompleted")) {
                                    dispatch(addNotification({
                                        id: "complete-task-error",
                                        message: "Invalid completion status transition",
                                        status: "error"
                                    }));
                                } else {
                                    dispatch(addError(error.response!.data));
                                }

                                return;
                            }
                        } finally {
                            gridRef?.current.api.refreshInfiniteCache();

                            setCompleteTaskId(undefined);
                            setLoading(false);
                        }
                    }
                }}
            />
        </AgGridProvider>
    );
}
