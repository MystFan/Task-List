"use client";
import { useColorScheme } from "@mui/material/styles";
import Box from "@mui/material/Box";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";

export default function ModeSwitch() {
    const { mode, setMode } = useColorScheme();
    if (!mode) {
        return null;
    }

    return (
        <Box
            data-testid={ModeSwitchTestIds.wrapper}
            sx={{
                display: "flex",
                justifyContent: "flex-end",
                mt: 0,
                p: 1,
            }}
        >
            <FormControl>
                <InputLabel id="mode-select-label">Mode</InputLabel>
                <Select
                    labelId="mode-select-label"
                    id="mode-select"
                    size="small"
                    value={mode}
                    onChange={(event) => setMode(event.target.value as typeof mode)}
                    label="Theme"
                >
                    <MenuItem value="system">System</MenuItem>
                    <MenuItem value="light">Light</MenuItem>
                    <MenuItem value="dark">Dark</MenuItem>
                </Select>
            </FormControl>
        </Box>
    )
}

export const ModeSwitchTestIds = {
    wrapper: "modes-witch-wrapper"
};