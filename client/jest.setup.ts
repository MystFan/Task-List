import "@testing-library/jest-dom";
import "cross-fetch/polyfill";

import { TextEncoder } from "util";
import { AuthUserInterface } from "@adList/providers/user-provider";

global.TextEncoder = TextEncoder;
global.window.matchMedia = jest.fn(() => ({
    addEventListener: jest.fn(),
    removeEventListener: jest.fn()
} as unknown as MediaQueryList));

jest.mock("next/navigation", () => ({
    ...jest.requireActual('next/navigation'),
    useRouter: jest.fn(),
    redirect: jest.fn()
}));

export const authUser: AuthUserInterface = {
    user: {
        email: "rock.metal.crusader@gmail.com",
        email_verified: true,
        name: "John Doe",
        nickname: "Mock User",
        picture: "https://media.gettyimages.com/id/127379452/photo/kitty.jpg?s=612x612&w=0&k=20&c=wQg0MwSqrqfI3XwMXdDV8dcdY2SGYXvbwOtMvqLWQjI=",
        sub: "auth0|69eb24bc9b5cfe37fbc54db6"
    },
    isLoading: false,
    invalidate: jest.fn()
};

jest.mock("@auth0/nextjs-auth0/client", () => ({
    useUser: jest.fn().mockReturnValue(authUser)
}));

Object.defineProperty(window, "matchMedia", {
    writable: true,
    value: (query: string) => ({
        matches: false,
        media: query,
        onchange: null,
        addListener: jest.fn(), // deprecated but still used in some libs
        removeListener: jest.fn(),
        addEventListener: jest.fn(),
        removeEventListener: jest.fn(),
        dispatchEvent: jest.fn(),
    }),
})