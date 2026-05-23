import { Form, Formik } from "formik";
import * as Yup from "yup";
import { Box, Button, TextField, FormControl, InputLabel, Select, MenuItem, Grid, FormHelperText } from "@mui/material";
import { DateTimePicker } from "@mui/x-date-pickers";
import { nameof } from "@adList/utils/nemeof";
import { DateTime } from "luxon";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterLuxon } from "@mui/x-date-pickers/AdapterLuxon";

export type TaskFormProps = {
    title?: string;
    description?: string | null;
    dueDate?: Date | null;
    status?: string | null;
    isEditMode?: boolean;
    onSubmit: (values: TaskValues) => void | Promise<void>;
    onCancel?: () => void;
}

export type TaskValues = {
    title: string;
    description?: string | null;
    dueDate?: Date | null;
    status?: string | null;
}

const titleMaxLength = 200;
const descriptionMaxLength = 2000;

export default function TaskForm(props: TaskFormProps) {
    const schema: Yup.ObjectSchema<TaskValues> = Yup.object({
        title: Yup.string()
            .label("Title")
            .max(titleMaxLength, `Title must be at most ${titleMaxLength} characters`)
            .required("Title is required"),
        description: Yup.string()
            .nullable()
            .label("Description")
            .max(descriptionMaxLength, `Description must be at most ${descriptionMaxLength} characters`),
        dueDate: Yup.date()
            .label("Due date")
            .nullable(),
        status: Yup.string()
            .label("Status")
            .nullable()
    });

    return (
        <LocalizationProvider dateAdapter={AdapterLuxon}>
            <Formik
                initialValues={{
                    title: props.title ? props.title : "",
                    description: props.description,
                    dueDate: props.dueDate ? props.dueDate : null,
                    status: props.status ? props.status : "Incomplete"
                }}
                onSubmit={props.onSubmit}
                validationSchema={schema}
            >
                {formik => (
                    <Box>
                        <Form>
                            <Grid container rowSpacing={1} columnSpacing={{ xs: 1, sm: 2, md: 3 }}>
                                <Grid size={{ xs: 12, sm: 12, md: 6, xl: 6 }}>
                                    <FormControl fullWidth>
                                        <TextField
                                            error={formik.touched.title && Boolean(formik.errors.title)}
                                            autoFocus
                                            required
                                            margin="dense"
                                            id={TaskFormTestIds.titleInput}
                                            name={nameof<TaskValues>("title")}
                                            label="Title"
                                            type="text"
                                            fullWidth
                                            variant="outlined"
                                            slotProps={{
                                                input: {
                                                    // @ts-expect-error Jest testing
                                                    "data-testid": TaskFormTestIds.titleInput,
                                                    "aria-label": "Title"
                                                }
                                            }}
                                            value={formik.values.title}
                                            onBlur={(e) => {
                                                formik.handleBlur(nameof<TaskValues>("title"))(e);
                                            }}
                                            onChange={e => {
                                                formik.setFieldValue(nameof<TaskValues>("title"), e.target.value, true);
                                            }}
                                        />
                                        <FormHelperText data-testid={TaskFormTestIds.titleHelpText} error={formik.touched.title && Boolean(formik.errors.title)}>
                                            {formik.touched.title && formik.errors.title
                                                ? formik.errors.title
                                                : ""}
                                        </FormHelperText>
                                    </FormControl>
                                </Grid>
                                <Grid size={{ xs: 12, sm: 12, md: 6, xl: 6 }}>
                                    <FormControl fullWidth>
                                        <TextField
                                            error={formik.touched.description && Boolean(formik.errors.description)}
                                            margin="dense"
                                            id={TaskFormTestIds.descriptionInput}
                                            name={nameof<TaskValues>("description")}
                                            label="Description"
                                            multiline
                                            rows={4}
                                            fullWidth
                                            variant="outlined"
                                            slotProps={{
                                                input: {
                                                    // @ts-expect-error Jest testing
                                                    "data-testid": TaskFormTestIds.descriptionInput,
                                                    "aria-label": "Description"
                                                }
                                            }}
                                            value={formik.values.description}
                                            onBlur={(e) => {
                                                formik.handleBlur(nameof<TaskValues>("description"))(e);
                                            }}
                                            onChange={e => {
                                                formik.setFieldValue(nameof<TaskValues>("description"), e.target.value, true);
                                            }}
                                        />
                                        <FormHelperText data-testid={TaskFormTestIds.descriptionHelpText} error={formik.touched.description && Boolean(formik.errors.description)}>
                                            {formik.touched.description && formik.errors.description
                                                ? formik.errors.description
                                                : ""}
                                        </FormHelperText>
                                    </FormControl>
                                </Grid>
                                <Grid size={{ xs: 12, sm: 12, md: 6, xl: 6 }}>
                                    <FormControl sx={{ marginTop: "0.5rem", marginBottom: "1rem" }} fullWidth>
                                        <DateTimePicker
                                            format="dd/MM/yyyy HH:mm:ss"
                                            label="Due date"
                                            slotProps={{
                                                textField: {
                                                    id: TaskFormTestIds.dueDateInput,
                                                    // @ts-expect-error Jest testing
                                                    "data-testid": TaskFormTestIds.dueDateInput,
                                                    "aria-label": "Due date",
                                                    margin: "dense",
                                                    name: nameof<TaskValues>("dueDate"),
                                                    error: formik.touched.dueDate && Boolean(formik.errors.dueDate),
                                                    onBlur: (e) => {
                                                        formik.handleBlur(nameof<TaskValues>("dueDate"))(e);
                                                    }
                                                }
                                            }}
                                            value={formik.values.dueDate ? DateTime.fromJSDate(formik.values.dueDate!) : null}
                                            onChange={(value) => {
                                                formik.setFieldValue(nameof<TaskValues>("dueDate"), value ? value.toJSDate() : null, true);
                                            }}
                                        />
                                    </FormControl>
                                </Grid>
                                <Grid size={{ xs: 12, sm: 12, md: 6, xl: 6 }}>
                                    {props.isEditMode &&
                                        <FormControl sx={{ marginTop: "1rem", marginBottom: "1rem" }} fullWidth>
                                            <InputLabel>Status</InputLabel>
                                            <Select
                                                id={TaskFormTestIds.statusSelect}
                                                label="Status"
                                                data-testid={TaskFormTestIds.statusSelect}
                                                disabled={formik.values.status === "Completed"}
                                                value={formik.values.status}
                                                slotProps={{
                                                    input: {
                                                        // @ts-expect-error Jest testing
                                                        "data-testid": TaskFormTestIds.statusInput,
                                                        "aria-label": "Status"
                                                    }
                                                }}
                                                onChange={(e) => {
                                                    formik.setFieldValue(nameof<TaskValues>("status"), e.target.value, true);
                                                }}
                                                onBlur={(e) => {
                                                    formik.handleBlur(nameof<TaskValues>("status"))(e);
                                                }}
                                            >
                                                <MenuItem data-testid={TaskFormTestIds.statusSelectOptionIncomplete} value="Incomplete">Incomplete</MenuItem>
                                                <MenuItem data-testid={TaskFormTestIds.statusSelectOptionCompleted} value="Completed">Completed</MenuItem>
                                            </Select>
                                        </FormControl>
                                    }
                                </Grid>
                                <Box sx={{ display: "flex", justifyContent: "end" }}>
                                    <Button data-testid={TaskFormTestIds.saveButton} type="submit" variant="contained">Save</Button>
                                </Box>
                            </Grid>
                        </Form>
                    </Box>
                )}
            </Formik>
        </LocalizationProvider >
    );
}

export const TaskFormTestIds = {
    titleInput: "task-form-title",
    titleHelpText: "task-form-title-help-text",
    descriptionInput: "task-form-description",
    descriptionHelpText: "task-form-description-help-text",
    dueDateInput: "task-form-due-date",
    dueDateHelpText: "task-form-dueDate-help-text",
    statusSelect: "task-form-status",
    statusSelectOptionIncomplete: "task-form-status-incomplete",
    statusSelectOptionCompleted: "task-form-status-completed",
    statusInput: "task-form-status-input",
    saveButton: "task-form-save-button"
};