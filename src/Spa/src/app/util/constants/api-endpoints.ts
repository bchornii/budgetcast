import { environment } from "src/environments/environment";

const baseUrl = environment.baseUrl;

export const dashboard = {

    account: {
        isAuthenticated: `${baseUrl}/account/isAuthenticated`,
        signInWithGoogle: `${baseUrl}/account/signInWithGoogle`,
        signInWithFacebook: `${baseUrl}/account/signInWithFacebook`,
        login: `${baseUrl}/account/login`,
        logout: `${baseUrl}/account/logout`,
        check: `${baseUrl}/account/check`,
        updateProfile: `${baseUrl}/account/updateProfile`,
        register: `${baseUrl}/account/register`,
        forgotPassword: `${baseUrl}/account/forgotPassword`,
        resetPassword: `${baseUrl}/account/resetPassword`
    },

    receipt: {
        addBasic: `${baseUrl}/receipt/addBasic`,  
        basicReceipts: `${baseUrl}/receipt/basicReceipts`,
        totalPerCampaign: `${baseUrl}/receipt/total/{{campaignName}}`
    },
    

    campaign: {
        all: `${baseUrl}/campaigns`,
        search: `${baseUrl}/campaigns/search`
    },
    
    tags: {
        search: `${baseUrl}/tags/search`
    }
};