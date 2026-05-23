import { createSlice, nanoid, PayloadAction } from "@reduxjs/toolkit";
import { isMutationError } from "@adList/utils/errors";

export type AddErrorPayloadAction = {
    code: ApplicationErrorCode;
    id: string;
    status?: number;
};

export type ErrorState = {
    code: ApplicationErrorCode;
    id: string;
    status?: number;
};

export const errorsSlice = createSlice({
    name: "errors",
    initialState: [] as ErrorState[],
    reducers: {
        addError: {
            reducer: (state, action: PayloadAction<AddErrorPayloadAction>): ErrorState[] => {
                state.push({
                    code: action.payload.code,
                    id: nanoid(),
                    status: action.payload.status
                });
                return state;
            },
            prepare: (error: unknown) => {
                let code: ApplicationErrorCode;
                let status: number | undefined;

                if (isMutationError(error)) {
                    code = error.data.code;
                    status = error.status;
                } else {
                    code = "UnexpectedError";
                }

                return {
                    payload: {
                        code,
                        status
                    } as AddErrorPayloadAction
                }
            }
        },
        removeError: (state, action: PayloadAction<string>): ErrorState[] => {
            state.splice(state.findIndex(error => error.id === action.payload), 1);
            return state;
        },
    }
});

export const {
    addError,
    removeError
} = errorsSlice.actions;

export default errorsSlice.reducer;
