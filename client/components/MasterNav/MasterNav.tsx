import * as React from "react";
import { styled, alpha } from "@mui/material/styles";
import Box from "@mui/material/Box";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Container from "@mui/material/Container";
import Divider from "@mui/material/Divider";
import MenuItem from "@mui/material/MenuItem";
import Drawer from "@mui/material/Drawer";
import MenuIcon from "@mui/icons-material/Menu";
import MenuList from "@mui/material/MenuList";
import Avatar from "@mui/material/Avatar";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import ModeSwitch, { ModeSwitchTestIds } from "@adList/components/ModeSwitch/ModeSwitch";
import Image from "next/image"
import { redirect, useRouter } from "next/navigation";
import { Chip } from "@mui/material";
import { useAuthUser } from "@adList/providers/user-provider";

const StyledToolbar = styled(Toolbar)(({ theme }) => ({
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    flexShrink: 0,
    borderRadius: `calc(${theme.shape.borderRadius}px + 8px)`,
    backdropFilter: "blur(24px)",
    border: "1px solid",
    borderColor: (theme.vars || theme).palette.divider,
    backgroundColor: theme.vars
        ? `rgba(${theme.vars.palette.background.defaultChannel} / 0.4)`
        : alpha(theme.palette.background.default, 0.4),
    boxShadow: (theme.vars || theme).shadows[1],
    padding: "8px 12px",
}));

export default function MasterNav() {
    const router = useRouter();
    const [open, setOpen] = React.useState(false);

    const { user } = useAuthUser();

    const toggleDrawer = (newOpen: boolean) => () => {
        setOpen(newOpen);
    };

    return (
        <AppBar
            position="relative"
            enableColorOnDark
            data-testid={MasterNavTestIds.appBar}
            sx={{
                boxShadow: 0,
                bgcolor: "transparent",
                backgroundImage: "none",
                mt: "calc(var(--template-frame-height, 0px) + 28px)",
            }}
        >
            <Container maxWidth="lg">
                <StyledToolbar variant="dense" disableGutters>
                    <Box sx={{ flexGrow: 1, display: "flex", alignItems: "center", gap: 1, px: 0 }}>
                        <Box sx={{ display: { xs: "none", md: "flex" }, alignItems: "center", gap: 1 }}>
                            <Button data-testid={MasterNavTestIds.tasksButton} variant="outlined" color="info" size="medium" onClick={() => router.push("/")}>
                                Tasks
                            </Button>
                            <Button data-testid={MasterNavTestIds.addTaskButton} variant="outlined" color="info" size="medium" onClick={() => router.push("/add-task")}>
                                Add Task
                            </Button>
                        </Box>
                    </Box>
                    <Box
                        sx={{
                            display: { xs: "none", md: "flex" },
                            gap: 1,
                            alignItems: "center",
                        }}
                    >
                        <Button data-testid={MasterNavTestIds.logoutButton} variant="outlined" color="info" size="medium" onClick={() => redirect("/auth/logout")}>
                            Logout
                        </Button>
                        {user && <Chip data-testid={MasterNavTestIds.usernameChip} label={user.name} />}
                        {user && <Avatar data-testid={MasterNavTestIds.userAvatar} alt={user.name} src={user.picture} />}
                        <ModeSwitch />
                    </Box>
                    <Box sx={{ display: { xs: "flex", md: "none" }, gap: 1 }}>
                        <IconButton aria-label="Menu button" onClick={toggleDrawer(true)}>
                            <MenuIcon />
                        </IconButton>
                        <Drawer
                            anchor="top"
                            open={open}
                            onClose={toggleDrawer(false)}
                            slotProps={{
                                paper: {
                                    sx: {
                                        top: "var(--template-frame-height, 0px)",
                                    },
                                },
                            }}
                        >
                            <Box sx={{ p: 2, backgroundColor: "background.default" }}>
                                <Box
                                    sx={{
                                        display: "flex",
                                        justifyContent: "flex-end",
                                    }}
                                >
                                    <IconButton onClick={toggleDrawer(false)}>
                                        <CloseRoundedIcon />
                                    </IconButton>
                                </Box>
                                <MenuList>
                                    {user &&
                                        <Box
                                            sx={{
                                                display: "flex",
                                                flexDirection: "row",
                                                flexWrap: "wrap",
                                                alignItems: "center",
                                                gap: 1,
                                                justifyContent: "flex-end",
                                            }}
                                        >
                                            <Chip label={user.name} />
                                            <Avatar alt={user.name} src={user.picture} />
                                            <ModeSwitch />
                                        </Box>}
                                    <MenuItem onClick={() => router.push("/")}>Tasks</MenuItem>
                                    <MenuItem onClick={() => router.push("/add-task")}>Add Task</MenuItem>
                                    <Divider sx={{ my: 3 }} />
                                    <MenuItem onClick={() => redirect("/auth/logout")}>Logout</MenuItem>
                                </MenuList>
                            </Box>
                        </Drawer>
                    </Box>
                </StyledToolbar>
            </Container>
        </AppBar >
    );
}

export const MasterNavTestIds = {
    appBar: "master-nav-app-bar",
    logoImage: "master-nav-logo-image",
    modeSwitch: ModeSwitchTestIds.wrapper,
    tasksButton: "master-nav-tasks-button",
    addTaskButton: "master-nav-add-task-button",
    logoutButton: "master-nav-logout-button",
    usernameChip: "master-nav-username-chip",
    userAvatar: "master-nav-user-avatar",
    menuDrawer: "master-nav-menu-drawer"
};