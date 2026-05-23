import { Configuration, ConfigurationParameters, AdListWebApiFactory } from "@adList/http";
import { SessionData } from "@auth0/nextjs-auth0/types";

export function createApi(session: SessionData | null) {
    const configParams: ConfigurationParameters = {
        basePath: process.env.ADLIST_API,
        accessToken: session?.tokenSet.idToken
    };

    const apiConfig = new Configuration(configParams);

    return AdListWebApiFactory(apiConfig);
}
