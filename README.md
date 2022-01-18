
# budgetcast 
.NET Core application, based on a simplified microservices architecture and Docker containers.

## Goals
 - Expenses tracking
 - Expense predictions
 - Organizing campaings for savings
 - View/Sharing history of expenses

## Code details
 #### CQRS
 Commands and queries are clearly separated in the application layer.
 
 #### DDD
 The DDD patterns can be checked in the domain layer.
 
 #### Onion
 The fundamental rule is that all code can depend on layers more central, but code cannot 
 depend on layers further out from the core. In other words, all coupling is toward the center.

## Tech stack
 - **Back-End**
    - Asp.Net 6
      - Asp.Net Identity 6
          - Individual User Accounts 
          - External providers (Google, Facebook)
      - MediatR
      - Entity Framework Core 6
      - Autofac
      - Automapper
      - FluentValidation
      - Polly
      - Swagger
      - Serilog
    - MsSql Server
    - MongoDB
      - MongoDB.Driver 2.10
    - Seq
    
 - **UI**
    - Asp.Net 6 (as prod hosting web server)
    - Serilog
    - Angular 13
    - Angular Material 13
    - Bootstrap 4
    - moment
    
 - **Web Status UI**
    - Asp.Net Core 3.1
      - Asp.Net Core Health Checks UI 3.1
    - Serilog

## Hosting platform
 - Docker containers
 
## Run locally  
 
 #### Docker
 - docker-compose.yml: This file contains the definition of all images needed for running application.
 - docker-compose.override.yml: This file contains the base configuration for all images of the previous file. 
 
 Usually these two files are using together:
 ```
 docker-compose up -f docker-compose.yml -f docker-compose.override.yml
 ```
 
 #### HTTPS
 Development HTTPS certificate `budgetcast.pfx` is being used to secure communication.
  
 #### The following environments variables must be set:
 ```
  - GOOGLE CREDENTIALS (from google console where you register you app with OAuth)
    - GOOGLE_CLIENT_SECRET=?
    - GOOGLE_CLIENT_ID=?

  - FACECBOOK CREDENTIALS (from fb console where you register you app with OAuth)
    - FACEBOOK_CLIENT_SECRET=?
    - FACEBOOK_CLIENT_ID=?

  - EMAIL PARAMS (used for individual user account confrmation)
     - EMAIL_FROM=?
     - EMAIL_PASSWORD=?

  - LOCAL ENV PARAMS
    - HOST_BASE_ADDR=localhost (as default)

  - LOCAL TCP PORTs
    - BUDGETCAST_DB_PORT=27017
    - IDENTITY_DB_PORT=5433
    - UI_PORT=5200
    - UI_HTTPS_PORT=5240
    - HEALTHCHECKS_UI_PORT=5201
    - HEALTHCHECKS_UI_HTTPS_PORT=5241
    - DASHBOARD_API_PORT=5100
    - DASHBOARD_API_HTTPS_PORT=5140
    - SEQ_PORT=5341
```
