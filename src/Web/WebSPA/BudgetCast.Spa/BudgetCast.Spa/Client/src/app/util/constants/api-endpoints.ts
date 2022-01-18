
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
      totalPerCampaign: string,
      details: string
    }
  };

  expenses: {
    campaign: {
      all: string,
      search: string
    },
    expenses: {
      get: string,
      add: string;
      searchTags: string
    }
  };
}
