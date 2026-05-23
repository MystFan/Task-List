import { CircularProgress, Backdrop, Box } from "@mui/material";
import { PropsWithChildren } from "react";

export type LoaderProps = {
    open: boolean;
    size?: "small" | "medium" | "large";
    position?: "relative" | "absolute";
}

export default function Loader(props: PropsWithChildren<LoaderProps>) {
    const position = props.position || "absolute";
    let size: string;
    switch (props.size) {
        case "small":
            size = "2rem";
            break;
        case "large":
            size = "4rem";
            break;
        default: size = "3rem";
            break;
    }

    return (
        <>
            {position === "absolute" &&
                <>
                    {props.children}
                    <Backdrop
                        open={props.open}
                        sx={{
                            position: { position },
                            zIndex: (theme) => theme.zIndex.modal + 1,
                            color: "#fff",
                        }}
                    >
                        <CircularProgress size={size} color="inherit" />
                    </Backdrop>
                </>
            }
            {position === "relative" &&
                <Box sx={{ position: position }}>
                    {props.children}

                    <Backdrop
                        open={props.open}
                        sx={{
                            position: "absolute",
                            zIndex: (theme) => theme.zIndex.modal + 1,
                            color: "#fff",
                        }}
                    >
                        <CircularProgress size={size} color="inherit" />
                    </Backdrop>
                </Box>
            }
        </>
    );
}
