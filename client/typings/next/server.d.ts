import "next";
import { SessionData } from "@auth0/nextjs-auth0/types";

declare module "next/server" {
    export interface NextRequest {
        session: SessionData | null
    }
}