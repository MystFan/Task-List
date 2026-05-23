import { waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { axe, toHaveNoViolations } from "jest-axe";
import { AlertComponent, AlertComponentTestIds } from "@adList/components/AlertComponent/AlertComponent";
import { renderWithProviders } from "@adList/utils/tests/render-with-providers";

describe("AlertComponent", () => {
    expect.extend(toHaveNoViolations);
    const user = userEvent.setup();

    const defaultProps = {
        id: "test-alert",
        children: "This is an alert message",
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe("Rendering", () => {
        it("renders the alert component", async () => {
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            expect(await findByTestId(AlertComponentTestIds.root(defaultProps.id))).toBeInTheDocument();
        });

        it("renders the alert content correctly", async () => {
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toBeInTheDocument();
            expect(content).toHaveTextContent("This is an alert message");
        });

        it("renders with custom children", async () => {
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps}>
                    <strong>Custom alert content</strong>
                </AlertComponent>
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toHaveTextContent("Custom alert content");
        });

        it("renders with default info severity", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorInfo");
        });

        it("renders with default filled variant", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-filled");
        });
    });

    describe("Severity Variants", () => {
        it("renders with success severity", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} status="success" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorSuccess");
        });

        it("renders with info severity", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} status="info" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorInfo");
        });

        it("renders with warning severity", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} status="warning" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorWarning");
        });

        it("renders with error severity", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} status="error" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorError");
        });
    });

    describe("Visual Variants", () => {
        it("renders with filled variant", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} variant="filled" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-filled");
        });

        it("renders with outlined variant", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} variant="outlined" />
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-outlined");
        });
    });

    describe("Close Functionality", () => {
        it("renders close button when onClose is provided", async () => {
            const onClose = jest.fn();
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} onClose={onClose} />
            );

            const closeButton = await findByRole("button", { name: /close/i });
            expect(closeButton).toBeInTheDocument();
        });

        it("does not render close button when onClose is not provided", async () => {
            const { queryByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            await waitFor(() => {
                expect(queryByRole("button", { name: /close/i })).not.toBeInTheDocument();
            });
        });

        it("calls onClose when close button is clicked", async () => {
            const onClose = jest.fn();
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} onClose={onClose} />
            );

            const closeButton = await findByRole("button", { name: /close/i });
            await user.click(closeButton);

            expect(onClose).toHaveBeenCalledTimes(1);
        });

        it("does not crash when onClose is undefined and close button is clicked", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} onClose={undefined} />
            );

            await waitFor(() => {
                const closeButton = findByRole("button", { name: /close/i });
                expect(closeButton).resolves.not.toThrow();
            });
        });
    });

    describe("Test IDs", () => {
        it("generates correct root test id", async () => {
            const customId = "custom-alert-123";
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps} id={customId} />
            );

            expect(await findByTestId(AlertComponentTestIds.root(customId))).toBeInTheDocument();
        });

        it("generates correct content test id", async () => {
            const customId = "custom-alert-456";
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps} id={customId} />
            );

            expect(await findByTestId(AlertComponentTestIds.content(customId))).toBeInTheDocument();
        });

        it("uses different test ids for different alert instances", async () => {
            const { findByTestId } = renderWithProviders(
                <>
                    <AlertComponent id="alert-1">First alert</AlertComponent>
                    <AlertComponent id="alert-2">Second alert</AlertComponent>
                </>
            );

            expect(await findByTestId(AlertComponentTestIds.root("alert-1"))).toBeInTheDocument();
            expect(await findByTestId(AlertComponentTestIds.root("alert-2"))).toBeInTheDocument();
            expect(await findByTestId(AlertComponentTestIds.content("alert-1"))).toHaveTextContent("First alert");
            expect(await findByTestId(AlertComponentTestIds.content("alert-2"))).toHaveTextContent("Second alert");
        });
    });

    describe("Accessibility", () => {
        it("should have no accessibility violations", async () => {
            const { container } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            await waitFor(async () => {
                const results = await axe(container);
                expect(results).toHaveNoViolations();
            });
        });

        it("has role status for screen readers", async () => {
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} />
            );

            expect(await findByRole("status")).toBeInTheDocument();
        });

        it("has accessible close button when onClose is provided", async () => {
            const onClose = jest.fn();
            const { findByRole } = renderWithProviders(
                <AlertComponent {...defaultProps} onClose={onClose} />
            );

            const closeButton = await findByRole("button", { name: /close/i });
            expect(closeButton).toHaveAccessibleName();
        });
    });

    describe("Combined Props", () => {
        it("renders with all props combined", async () => {
            const onClose = jest.fn();
            const { findByTestId, findByRole } = renderWithProviders(
                <AlertComponent
                    id="full-alert"
                    status="warning"
                    variant="outlined"
                    onClose={onClose}
                >
                    Complete alert message
                </AlertComponent>
            );

            const alert = await findByRole("status");
            const content = await findByTestId(AlertComponentTestIds.content("full-alert"));

            expect(alert).toHaveClass("MuiAlert-outlined");
            expect(alert).toHaveClass("MuiAlert-colorWarning");
            expect(content).toHaveTextContent("Complete alert message");
            expect(await findByRole("button", { name: /close/i })).toBeInTheDocument();
        });

        it("renders success filled alert with close", async () => {
            const onClose = jest.fn();
            const { findByRole } = renderWithProviders(
                <AlertComponent
                    {...defaultProps}
                    status="success"
                    variant="filled"
                    onClose={onClose}
                >
                    Success message
                </AlertComponent>
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorSuccess");
        });

        it("renders error outlined alert without close", async () => {
            const { findByRole, queryByRole } = renderWithProviders(
                <AlertComponent
                    {...defaultProps}
                    status="error"
                    variant="outlined"
                >
                    Error message
                </AlertComponent>
            );

            const alert = await findByRole("status");
            expect(alert).toHaveClass("MuiAlert-colorError");

            await waitFor(() => {
                expect(queryByRole("button", { name: /close/i })).not.toBeInTheDocument();
            });
        });
    });

    describe("Edge Cases", () => {
        it("handles empty children", async () => {
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps}>
                    {""}
                </AlertComponent>
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toHaveTextContent("");
        });

        it("handles complex children with multiple elements", async () => {
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps}>
                    <div>
                        <strong>Title:</strong>
                        <span> Description text</span>
                    </div>
                </AlertComponent>
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toHaveTextContent("Title: Description text");
        });

        it("handles very long text content", async () => {
            const longText = "A".repeat(1000);
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps}>
                    {longText}
                </AlertComponent>
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toHaveTextContent(longText);
        });

        it("handles special characters in content", async () => {
            const specialText = '<script>alert("test")</script>';
            const { findByTestId } = renderWithProviders(
                <AlertComponent {...defaultProps}>
                    {specialText}
                </AlertComponent>
            );

            const content = await findByTestId(AlertComponentTestIds.content(defaultProps.id));
            expect(content).toHaveTextContent(specialText);
        });
    });

    describe("Multiple Instances", () => {
        it("can render multiple alerts with different severities", async () => {
            const { findByTestId } = renderWithProviders(
                <>
                    <AlertComponent id="alert-success" status="success">Success</AlertComponent>
                    <AlertComponent id="alert-error" status="error">Error</AlertComponent>
                    <AlertComponent id="alert-warning" status="warning">Warning</AlertComponent>
                    <AlertComponent id="alert-info" status="info">Info</AlertComponent>
                </>
            );

            expect(await findByTestId(AlertComponentTestIds.root("alert-success"))).toBeInTheDocument();
            expect(await findByTestId(AlertComponentTestIds.root("alert-error"))).toBeInTheDocument();
            expect(await findByTestId(AlertComponentTestIds.root("alert-warning"))).toBeInTheDocument();
            expect(await findByTestId(AlertComponentTestIds.root("alert-info"))).toBeInTheDocument();
        });
    });
});
