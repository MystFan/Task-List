import "@testing-library/jest-dom";
import MasterNav, { MasterNavTestIds } from "@adList/components/MasterNav/MasterNav";
import { renderWithProviders } from "@adList/utils/tests/render-with-providers";
import userEvent, { UserEvent } from "@testing-library/user-event";
import { axe, toHaveNoViolations } from "jest-axe";
import { within } from "@testing-library/react";
import { useRouter, redirect } from "next/navigation";
import { authUser } from "@adList/jest.setup";

const router = {
    push: jest.fn(),
    replace: jest.fn(),
    refresh: jest.fn(),
    back: jest.fn(),
    pathname: "/"
};

(useRouter as jest.Mock).mockReturnValue(router);

describe("MasterNav", () => {
    expect.extend(toHaveNoViolations);
    let user: UserEvent;

    beforeEach(() => {
        router.pathname = "/";
    });

    beforeAll(() => {
        user = userEvent.setup();
    });

    describe("Rendering", () => {
        it("renders the navigation bar", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            expect(getByTestId(MasterNavTestIds.appBar)).toBeInTheDocument();
        });

        it("renders ModeSwitch component", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            expect(getByTestId(MasterNavTestIds.modeSwitch)).toBeInTheDocument();
        });

        it("drawer is closed by default", () => {
            const { queryByRole } = renderWithProviders(<MasterNav />);

            expect(queryByRole("presentation")).not.toBeInTheDocument();
        });
    });


    describe("Navigation", () => {
        it("renders Tasks button", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const tasksButton = getByTestId(MasterNavTestIds.tasksButton);

            expect(tasksButton).toHaveTextContent("Tasks")
            expect(tasksButton).toBeInTheDocument();
        });

        it("renders Add Task button", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const addTaskButton = getByTestId(MasterNavTestIds.addTaskButton);

            expect(addTaskButton).toHaveTextContent("Add Task")
            expect(addTaskButton).toBeInTheDocument();
        });

        it("renders Logout button", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const logoutButton = getByTestId(MasterNavTestIds.logoutButton);

            expect(logoutButton).toHaveTextContent("Logout")
            expect(logoutButton).toBeInTheDocument();
        });

        it("navigates to home when Tasks button is clicked", async () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const tasksButton = getByTestId(MasterNavTestIds.tasksButton);
            await user.click(tasksButton);

            expect(router.push).toHaveBeenCalledWith("/");
        });

        it("navigates to logout when Logout button is clicked", async () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const logoutButton = getByTestId(MasterNavTestIds.logoutButton);
            await user.click(logoutButton);

            expect(redirect).toHaveBeenCalledWith("/auth/logout");
        });
    });

    describe("User Authentication", () => {
        it("displays user name chip when user is authenticated", () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const userName = getByTestId(MasterNavTestIds.usernameChip);

            expect(userName).toHaveTextContent("John Doe");
            expect(userName).toBeInTheDocument();
        });

        it("displays user avatar when user is authenticated", async () => {
            const { getByTestId } = renderWithProviders(<MasterNav />);

            const avatar = (await within(getByTestId(MasterNavTestIds.userAvatar)).findAllByAltText("John Doe"))[0];

            expect(avatar).toBeInTheDocument();
            expect(avatar).toHaveAttribute("src", authUser.user?.picture);
        });
    });

    describe("Accessibility", () => {
        it("should have no accessibility violations", async () => {
            const { container } = renderWithProviders(<MasterNav />);

            const results = await axe(container);

            expect(results).toHaveNoViolations();
        });
    });
})