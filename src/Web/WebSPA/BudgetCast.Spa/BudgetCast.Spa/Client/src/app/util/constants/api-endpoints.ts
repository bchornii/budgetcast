
export interface Endpoints {
  dashboard: {
    account: {
      isAuthenticated: string,
      signInWithGoogle: string,
      signInWithFacebook: string,
      login: string,
      logout: string,
      check: string,
      updateProfile: string,
      register: string,
      forgotPassword: string,
      resetPassword: string
    },
    receipt: {
      addBasic: string,
      basicReceipts: string,
      totalPerCampaign: string,
      details: string
    },
    campaign: {
      all: string,
      search: string
    },
    tags: {
      search: string
    }
  };
}
