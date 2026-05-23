import { waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { axe, toHaveNoViolations } from "jest-axe";
import TaskForm, { TaskFormProps, TaskFormTestIds } from "@adList/components/TaskForm/TaskForm";
import { renderWithProviders } from "@adList/utils/tests/render-with-providers";

describe("TaskForm", () => {
    expect.extend(toHaveNoViolations);
    const user = userEvent.setup();

    const defaultProps: TaskFormProps = {
        onSubmit: jest.fn(),
        onCancel: jest.fn(),
        isEditMode: true
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe("Rendering", () => {
        it("renders the form with all fields", () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            expect(getByTestId(TaskFormTestIds.titleInput)).toBeInTheDocument();
            expect(getByTestId(TaskFormTestIds.descriptionInput)).toBeInTheDocument();
            expect(getByTestId(TaskFormTestIds.dueDateInput)).toBeInTheDocument();
            expect(getByTestId(TaskFormTestIds.statusSelect)).toBeInTheDocument();
            expect(getByTestId(TaskFormTestIds.saveButton)).toBeInTheDocument();
        });

        it("renders with initial values when provided", () => {
            const props: TaskFormProps = {
                ...defaultProps,
                title: "Test Task",
                description: "Test Description",
                dueDate: new Date("2025-12-31"),
                status: "Incomplete"
            };

            const { getByTestId } = renderWithProviders(<TaskForm {...props} />);

            expect(getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput)).toHaveValue(props.title!);
            expect(getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput)).toHaveValue(props.description!);
        });

        it("renders with empty values when no initial values provided", async () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput) as HTMLInputElement;
            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput) as HTMLInputElement;

            expect(titleInput.value).toBe("");
            expect(descriptionInput.value).toBe("");
        });

        it("does not render status field when not in edit mode", async () => {
            const { queryByTestId } = renderWithProviders(<TaskForm {...defaultProps} isEditMode={false} />);

            await waitFor(() => {
                expect(queryByTestId(TaskFormTestIds.statusSelect)).not.toBeInTheDocument();
            });
        });

        it("renders status field when in edit mode", async () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} isEditMode={true} />);

            await waitFor(() => {
                expect(getByTestId(TaskFormTestIds.statusSelect)).toBeInTheDocument();
            });
        });

        it("renders save button", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const saveButton = await findByTestId(TaskFormTestIds.saveButton);

            expect(saveButton).toBeInTheDocument();
            expect(saveButton).toHaveAttribute("type", "submit");
        });
    });

    describe("Form Validation", () => {
        it("shows error when title is empty and field is touched", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = await findByTestId(TaskFormTestIds.titleInput);
            await user.click(titleInput);
            await user.tab(); // Blur the field

            const error = await findByTestId(TaskFormTestIds.titleHelpText);
            expect(error).toHaveTextContent("Title is required");
        });

        it("shows error when title exceeds max length", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = await findByTestId(TaskFormTestIds.titleInput);
            const longTitle = "a".repeat(201); // Exceeds 200 character limit

            await user.click(titleInput);
            await user.paste(longTitle);
            await user.tab();

            const error = await findByTestId(TaskFormTestIds.titleHelpText);
            expect(error).toHaveTextContent("Title must be at most 200 characters");
        });

        it("shows error when description exceeds max length", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const descriptionInput = await findByTestId(TaskFormTestIds.descriptionInput);
            const longDescription = "a".repeat(2001); // Exceeds 2000 character limit

            await user.click(descriptionInput);
            await user.paste(longDescription);
            await user.tab();

            const error = await findByTestId(TaskFormTestIds.descriptionHelpText);
            expect(error).toHaveTextContent("Description must be at most 2000 characters");
        });

        it("does not show error for valid title", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = await findByTestId(TaskFormTestIds.titleInput);
            await user.click(titleInput);
            await user.type(titleInput, "Valid Title");
            await user.tab();

            await waitFor(async () => {
                const error = await findByTestId(TaskFormTestIds.titleHelpText);
                expect(error).toHaveTextContent("");
            });
        });

        it("allows empty description", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const descriptionInput = await findByTestId(TaskFormTestIds.descriptionInput);
            await user.click(descriptionInput);
            await user.tab();

            await waitFor(async () => {
                const error = await findByTestId(TaskFormTestIds.descriptionHelpText);
                expect(error).toHaveTextContent("");
            });
        });
    });

    describe("User Interactions", () => {
        it("updates title field when user types", async () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput) as HTMLInputElement;
            await user.click(titleInput);
            await user.type(titleInput, "New Task");

            expect(titleInput.value).toBe("New Task");
        });

        it("updates description field when user types", async () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput) as HTMLInputElement;
            await user.click(descriptionInput);
            await user.type(descriptionInput, "Task description");

            expect(descriptionInput.value).toBe("Task description");
        });

        it("calls onSubmit with form values when form is submitted", async () => {
            const onSubmit = jest.fn();
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} onSubmit={onSubmit} />
            );

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput) as HTMLInputElement;
            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput) as HTMLInputElement;
            const saveButton = getByTestId(TaskFormTestIds.saveButton);

            await user.click(titleInput);
            await user.type(titleInput, "Test Task");
            await user.click(descriptionInput);
            await user.type(descriptionInput, "Test Description");
            await user.click(saveButton);

            await waitFor(() => {
                expect(onSubmit).toHaveBeenCalledTimes(1);
                expect(onSubmit).toHaveBeenCalledWith(
                    expect.objectContaining({
                        title: "Test Task",
                        description: "Test Description",
                    }),
                    expect.anything()
                );
            });
        });

        it("does not call onSubmit when form has validation errors", async () => {
            const onSubmit = jest.fn();
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} onSubmit={onSubmit} />
            );

            const saveButton = getByTestId(TaskFormTestIds.saveButton);
            await user.click(saveButton);

            await waitFor(() => {
                expect(onSubmit).not.toHaveBeenCalled();
            });
        });

        it("clears title field when user deletes text", async () => {
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} title="Initial Title" />
            );

            const titleInput = await getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput) as HTMLInputElement;
            await user.clear(titleInput);

            expect(titleInput).toHaveValue("");
        });
    });

    describe("Edit Mode", () => {
        it("shows status dropdown in edit mode", async () => {
            const { findByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} isEditMode={true} status="Incomplete" />
            );

            expect(await findByTestId(TaskFormTestIds.statusSelect)).toBeInTheDocument();
        });

        it("allows changing status from Incomplete to Completed", async () => {
            const { findByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} isEditMode={true} status="Incomplete" />
            );

            const statusSelect = within(await findByTestId(TaskFormTestIds.statusSelect)).getAllByRole("combobox")[0] as HTMLDivElement;
            await user.click(statusSelect);

            const completeOption = await findByTestId(TaskFormTestIds.statusSelectOptionCompleted);
            await user.click(completeOption);

            await waitFor(async () => {
                expect(statusSelect).toHaveTextContent("Completed");
            });
        });

        it("disables status dropdown when status is Completed", async () => {
            const { findByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} isEditMode={true} status="Completed" />
            );

            const statusInput = await findByTestId(TaskFormTestIds.statusInput)
            expect(statusInput).toBeDisabled();
        });

        it("defaults status to Incomplete when not provided", async () => {
            const { findByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} isEditMode={true} />
            );

            const statusSelect = within(await findByTestId(TaskFormTestIds.statusSelect)).getAllByRole("combobox")[0] as HTMLDivElement;
            expect(statusSelect).toHaveTextContent("Incomplete");
        });
    });

    describe("Date Picker", () => {
        it("renders date picker field", async () => {
            const { findByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            expect(await findByTestId(TaskFormTestIds.dueDateInput)).toBeInTheDocument();
        });

        it("displays initial due date when provided", async () => {
            const dueDate = new Date("2025-12-31T10:30:00");
            const { findByDisplayValue } = renderWithProviders(
                <TaskForm {...defaultProps} dueDate={dueDate} />
            );

            await waitFor(async () => {
                const dateInput = await findByDisplayValue(/31\/12\/2025/);
                expect(dateInput).toBeInTheDocument();
            });
        });
    });

    describe("Form Submission", () => {
        it("submits form with all field values", async () => {
            const onSubmit = jest.fn();
            const dueDate = new Date("2025-12-31T10:30:00");
            const { getByTestId, findByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} dueDate={dueDate} onSubmit={onSubmit} isEditMode={true} />
            );

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput) as HTMLInputElement;
            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput) as HTMLInputElement;
            const saveButton = getByTestId(TaskFormTestIds.saveButton);
            const statusSelect = within(await findByTestId(TaskFormTestIds.statusSelect)).getAllByRole("combobox")[0] as HTMLDivElement;

            await user.click(titleInput);
            await user.type(titleInput, "Complete Task");
            await user.click(descriptionInput);
            await user.type(descriptionInput, "Full description");

            await user.click(statusSelect);
            const completeOption = await findByTestId(TaskFormTestIds.statusSelectOptionCompleted);
            await user.click(completeOption);

            await user.click(saveButton);

            await waitFor(() => {
                expect(onSubmit).toHaveBeenCalledWith(
                    expect.objectContaining({
                        title: "Complete Task",
                        description: "Full description",
                        status: "Completed",
                        dueDate: dueDate
                    }),
                    expect.anything()
                );
            });
        });
    });

    describe("Initial Values", () => {
        it("uses provided title as initial value", async () => {
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} title="Initial Task" />
            );

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput);

            expect(titleInput).toHaveValue("Initial Task");
        });

        it("uses empty string when title is not provided", async () => {
            const { getByTestId } = renderWithProviders(<TaskForm {...defaultProps} />);

            const titleInput = getByTestId(TaskFormTestIds.titleInput).querySelector("#" + TaskFormTestIds.titleInput);

            expect(titleInput).toHaveValue("");
        });

        it("uses provided description as initial value", async () => {
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} description="Initial description" />
            );

            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput);

            expect(descriptionInput).toHaveValue("Initial description");
        });

        it("handles null description", async () => {
            const { getByTestId } = renderWithProviders(
                <TaskForm {...defaultProps} description={null} />
            );

            const descriptionInput = getByTestId(TaskFormTestIds.descriptionInput).querySelector("#" + TaskFormTestIds.descriptionInput);
            expect(descriptionInput).toHaveValue("");
        });
    });

    describe("Accessibility", () => {
        it("should have no accessibility violations", async () => {
            const props: TaskFormProps = {
                ...defaultProps,
                title: "Test Task",
                description: "Test Description",
                dueDate: new Date("2025-12-31"),
                status: "Incomplete",
                isEditMode: true
            };
            const { container } = renderWithProviders(<TaskForm {...props} />);

            const results = await axe(container);

            expect(results).toHaveNoViolations();
        });
    });
});
