import { Alert } from "@mui/material";

type AlertComponentProps = {
    id: string;
    status?: "success" | "info" | "warning" | "error";
    variant?: "outlined" | "filled";
    children: React.ReactNode;
    onClose?: () => unknown;
}

export function AlertComponent(props: AlertComponentProps) {
    return (
        <Alert
            data-testid={AlertComponentTestIds.root(props.id)}
            role="status"
            severity={props.status ?? "info"}
            variant={props.variant ?? "filled"}
            onClose={props.onClose ? () => props.onClose?.() : undefined}
            sx={{ marginTop: "1rem", marginBottom: "1rem" }}
        >
            <span data-testid={AlertComponentTestIds.content(props.id)}>
                {props.children}
            </span>
        </Alert>
    );
}

export const AlertComponentTestIds = {
    content: (id: string) => `alert-content-${id}`,
    root: (id: string) => `alert-root-${id}`
};
