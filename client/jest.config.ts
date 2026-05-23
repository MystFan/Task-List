import type { Config } from "jest"
import nextJest from "next/jest.js"

const createJestConfig = nextJest({
    // Provide the path to your Next.js app to load next.config.js and .env files in your test environment
    dir: "./",
})

const config: Config = {
    clearMocks: true,
    modulePathIgnorePatterns: ["<rootDir>/.next/"],
    setupFilesAfterEnv: ["<rootDir>/jest.setup.ts"],
    testEnvironment: "jest-environment-jsdom",
    // See https://github.com/mswjs/msw/issues/1786#issuecomment-1782559851
    testEnvironmentOptions: {
        customExportConditions: [""],
    },
    moduleNameMapper: {
        "^@adList/(.*)$": "<rootDir>/$1"
    }
}

// createJestConfig is exported this way to ensure that next/jest can load the Next.js config which is async
export default createJestConfig(config)