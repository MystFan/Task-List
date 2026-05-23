import { PropsWithChildren } from "react";
import { styled } from "@mui/material/styles";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, DialogContentText } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

type ConfirmDialogProps = {
    id?: string;
    open?: boolean;
    title: string;
    confirm: string;
    content?: string;
    handleSave?: () => void
    handleClose?: () => void
}

const BootstrapDialog = styled(Dialog)(({ theme }) => ({
    "& .MuiDialogContent-root": {
        padding: theme.spacing(2),
    },
    "& .MuiDialogActions-root": {
        padding: theme.spacing(1),
    },
}));

export default function ConfirmDialog(props: PropsWithChildren<ConfirmDialogProps>) {
    const id = props.id || "confirm-dialog";

    return (
        <Dialog
            id={id}
            data-testid={ConfirmDialogTestIds.root(id)}
            open={props.open ?? false}
            closeAfterTransition={true}
            onClose={props.handleClose}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
            role="dialog"
        >
            <BootstrapDialog
                onClose={props.handleClose}
                aria-labelledby="customized-dialog-title"
                open={props.open ?? false}
            >
                <DialogTitle data-testid={ConfirmDialogTestIds.title(id)} sx={{ m: 0, p: 2 }} id="customized-dialog-title">
                    {props.title}
                </DialogTitle>
                <IconButton
                    aria-label="close"
                    onClick={props.handleClose}
                    sx={(theme) => ({
                        position: "absolute",
                        right: 8,
                        top: 8,
                        color: theme.palette.grey[500]
                    })}
                >
                    <CloseIcon data-testid={ConfirmDialogTestIds.closeIcon(id)} />
                </IconButton>
                <DialogContent dividers>
                    <DialogContentText data-testid={ConfirmDialogTestIds.content(id)}>
                        {props.content ?? ""}
                    </DialogContentText>
                    {props.children}
                </DialogContent>
                <DialogActions>
                    <Button data-testid={ConfirmDialogTestIds.cancelButton(id)} variant="outlined" onClick={props.handleClose} autoFocus>
                        Cancel
                    </Button>
                    <Button data-testid={ConfirmDialogTestIds.confirmButton(id)} variant="contained" onClick={props.handleSave}>
                        {props.confirm}
                    </Button>
                </DialogActions>
            </BootstrapDialog>
        </Dialog>
    );
}

export const ConfirmDialogTestIds = {
    root: (id: string) => `confirm-dialog-root-${id}`,
    title: (id: string) => `confirm-dialog-title-${id}`,
    content: (id: string) => `confirm-dialog-content-${id}`,
    confirmButton: (id: string) => `confirm-dialog-confirm-button-${id}`,
    cancelButton: (id: string) => `confirm-dialog-cancel-button-${id}`,
    closeIcon: (id: string) => `confirm-dialog-close-icon-${id}`
};