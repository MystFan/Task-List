import { waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { axe, toHaveNoViolations } from "jest-axe";
import ConfirmDialog, { ConfirmDialogTestIds } from "@adList/components/ConfirmDialog/ConfirmDialog";
import { renderWithProviders } from "@adList/utils/tests/render-with-providers";

describe("ConfirmDialog", () => {
    expect.extend(toHaveNoViolations);
    const user = userEvent.setup();
    const defaultProps = {
        id: "confirm-dialog",
        open: true,
        title: "Confirm Action",
        confirm: "Confirm",
        content: "Are you sure you want to proceed?",
        handleSave: jest.fn(),
        handleClose: jest.fn(),
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe("Rendering", () => {
        it("renders the dialog when open is true", async () => {
            const { findByRole } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            expect(await findByRole("dialog")).toBeInTheDocument();
        });

        it("does not render the dialog when open is false", async () => {
            const { queryByRole } = renderWithProviders(<ConfirmDialog {...defaultProps} open={false} />);

            expect(queryByRole("dialog")).not.toBeInTheDocument();
        });

        it("renders the dialog when open prop is undefined (defaults to false)", async () => {
            const { queryByRole } = renderWithProviders(<ConfirmDialog {...defaultProps} open={undefined} />);

            expect(queryByRole("dialog")).not.toBeInTheDocument();
        });

        it("renders the title correctly", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            expect(await findByTestId(ConfirmDialogTestIds.title(defaultProps.id))).toBeInTheDocument();
        });

        it("renders the content text correctly", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const content = await findByTestId(ConfirmDialogTestIds.content(defaultProps.id));

            expect(content).toBeInTheDocument();
            expect(content).toHaveTextContent(defaultProps.content);
        });

        it("renders empty content when content prop is not provided", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} content={undefined} />);

            const content = await findByTestId(ConfirmDialogTestIds.content(defaultProps.id));

            expect(content).toHaveTextContent("")
        });

        it("renders the confirm button with correct text", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const confirmButton = await findByTestId(ConfirmDialogTestIds.confirmButton(defaultProps.id));

            expect(confirmButton).toBeInTheDocument();
            expect(confirmButton).toHaveTextContent(defaultProps.confirm);
        });

        it("renders the cancel button with correct text", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const cancelButton = await findByTestId(ConfirmDialogTestIds.cancelButton(defaultProps.id));

            expect(cancelButton).toBeInTheDocument();
            expect(cancelButton).toHaveTextContent("Cancel");
        });

        it("renders the close icon button", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const closeIcon = await findByTestId(ConfirmDialogTestIds.closeIcon(defaultProps.id));

            expect(closeIcon).toBeInTheDocument();
        });
    });

    describe("Children Rendering", () => {
        it("renders children content when provided", async () => {
            const { findByTestId } = renderWithProviders(
                <ConfirmDialog {...defaultProps}>
                    <div data-testid="custom-content">Custom Content</div>
                </ConfirmDialog>
            );

            expect(await findByTestId("custom-content")).toBeInTheDocument();
        });

        it("renders both content text and children", async () => {
            const { findByTestId } = renderWithProviders(
                <ConfirmDialog {...defaultProps}>
                    <div data-testid="custom-content">Custom Content</div>
                </ConfirmDialog>
            );

            expect(await findByTestId("custom-content")).toBeInTheDocument()
            expect(await findByTestId(ConfirmDialogTestIds.content(defaultProps.id))).toBeInTheDocument()
        });
    });

    describe("User Interactions", () => {
        it("calls handleSave when confirm button is clicked", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const confirmButton = await findByTestId(ConfirmDialogTestIds.confirmButton(defaultProps.id));
            await user.click(confirmButton);

            expect(defaultProps.handleSave).toHaveBeenCalledTimes(1);
        });

        it("calls handleClose when cancel button is clicked", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const cancelButton = await findByTestId(ConfirmDialogTestIds.cancelButton(defaultProps.id));
            await user.click(cancelButton);

            expect(defaultProps.handleClose).toHaveBeenCalledTimes(1);
        });

        it("calls handleClose when close icon is clicked", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const closeIcon = await findByTestId(ConfirmDialogTestIds.closeIcon(defaultProps.id));
            await user.click(closeIcon);

            expect(defaultProps.handleClose).toHaveBeenCalledTimes(1);
        });

        it("does not call handleSave when cancel is clicked", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const cancelButton = await findByTestId(ConfirmDialogTestIds.cancelButton(defaultProps.id));
            await user.click(cancelButton);

            expect(defaultProps.handleSave).not.toHaveBeenCalled();
        });

        it("does not call handleClose when confirm is clicked", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            const confirmButton = await findByTestId(ConfirmDialogTestIds.confirmButton(defaultProps.id));
            await user.click(confirmButton);

            expect(defaultProps.handleClose).not.toHaveBeenCalled();
        });
    });

    describe("Optional Handlers", () => {
        it("does not crash when handleSave is not provided", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} handleSave={undefined} />);

            const confirmButton = await findByTestId(ConfirmDialogTestIds.confirmButton(defaultProps.id));
            await user.click(confirmButton);

            // Should not throw error
            expect(confirmButton).toBeInTheDocument();
        });

        it("does not crash when handleClose is not provided", async () => {
            const { findByTestId } = renderWithProviders(<ConfirmDialog {...defaultProps} handleClose={undefined} />);

            const cancelButton = await findByTestId(ConfirmDialogTestIds.cancelButton(defaultProps.id));
            await user.click(cancelButton);

            // Should not throw error
            expect(cancelButton).toBeInTheDocument();
        });
    });

    describe("Accessibility", () => {
        it("should have no accessibility violations", async () => {
            const { container } = renderWithProviders(<ConfirmDialog {...defaultProps} />);

            await waitFor(async () => {
                const results = await axe(container);

                expect(results).toHaveNoViolations();
            });
        });
    });
});
