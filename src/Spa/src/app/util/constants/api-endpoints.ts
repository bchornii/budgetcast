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
        register: `${baseUrl}/account/register`,
        forgotPassword: `${baseUrl}/account/forgotPassword`,
        resetPassword: `${baseUrl}/account/resetPassword`
    },

    profile: {
        update: `${baseUrl}/profile/update`,
        uploadImg: `${baseUrl}/profile/uploadImg`
    },

    receipt: {
        addBasic: `${baseUrl}/receipt/addBasic`,  
        basicReceipts: `${baseUrl}/receipt/basicReceipts`,
        totalPerCampaign: `${baseUrl}/receipt/total/{{campaignName}}`,
        details: `${baseUrl}/receipt/{{id}}/details`
    },
    

    campaign: {
        all: `${baseUrl}/campaigns`,
        search: `${baseUrl}/campaigns/search`
    },
    
    tags: {
        search: `${baseUrl}/tags/search`
    }
};