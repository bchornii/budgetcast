namespace BudgetCast.Gateways.Bff;

public class BffOptions
    {
        /// <summary>
        /// Base path for management endpoints. Defaults to "/bff".
        /// </summary>
        public PathString ManagementBasePath { get; set; } = "/bff";

        /// <summary>
        /// Login individual endpoint
        /// </summary>
        public PathString LoginIndividualPath => ManagementBasePath.Add("/login/individual");
        
        /// <summary>
        /// Logout individual endpoint
        /// </summary>
        public PathString LogoutIndividualPath => ManagementBasePath.Add("/logout/individual");
        
        /// <summary>
        /// User endpoint
        /// </summary>
        public PathString UserPath => ManagementBasePath.Add("/user-info");
    }