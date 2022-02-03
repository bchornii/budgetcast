
export interface Endpoints {
  identity: {
    account: {
      register: string,
      update: string,
      emailConfirmation: string,
      passwordForgot: string,
      passwordReset: string,
      isAuthenticated: string
    },

    signIn: {
      google: string,
      facebook: string,
      individual: string,      
    },

    signOut: {
      all: string
    }
  }

  expenses: {
    campaign: {
      all: string,
      search: string,
      totals: string
    },
    expenses: {
      get: string,
      add: string;
      searchTags: string,
      details: string
    }
  };

  notifications: {
    all: string;
  }
}
