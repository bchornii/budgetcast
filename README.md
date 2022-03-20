
# budgetcast 
.NET Core application, based on a simplified microservices architecture and Docker containers.

## Goals
 - Expenses tracking
 - Expense predictions
 - Organizing campaings for savings
 - View/Sharing history of expenses

## Architecture overview

This application is cross-platform at the server and client-side, thanks to .NET 6 services capable of running on Linux or Windows containers depending on your Docker host plus any browser for the client web apps. The architecture proposes a microservice oriented architecture implementation with multiple autonomous microservices (each one owning its own data/db) and implementing different approaches within each microservice (simple CRUD vs. DDD/CQRS patterns) using HTTP as the communication protocol between the client apps and the microservices and in the _next iterations_ asynchronous communication for data updates propagation across multiple services based on Integration Events and an Event Bus (a light message broker, to choose between RabbitMQ or Azure Service Bus/Azure Queue Storage).

To achieve runtime notifications when significant changes in the system occur and avoid constantly polling back-end services for such a changes, the system employs WebSocket connection communication channel. SignalR framework has been choosen to abstract any communication/transport low level details and be able to focus on business logic implementation only. 

Azure SignalR Service integration was added in order to avoid potential problems with performance, scalability and availability of the solutions based on persistent client connections (aka. web sockets). These issues are handler for us with a 99.9% SLA.

![budgetcast-containers-diagram](https://user-images.githubusercontent.com/16306082/159163472-61edfea9-d634-4ce5-af6a-ce81d98a32d7.jpg)

## Code details 
 #### CRUD
 Identity management service uses CRUD approach due to the lack of its own domain and using approach Conformist, where it follows ASP.NET Identity terms and concepts.
 
 #### CQS/CQRS
 Expenses service uses concept of Commands and Queries at application layer to separate operations and models used for different purposes (read/write).
 
 #### DDD
 Expenses service uses DDD patterns at domain layer to model its domain.
 
 #### Clean architecture
 The fundamental rule is that all code can depend on layers more central, but code cannot depend on layers further out from the core. In other words, all coupling is toward the center.

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
    - MS Sql Server
    - Azure SDK
    - Seq
    
 - **UI**
    - Asp.Net 6 (as prod hosting web server)
    - Serilog
    - Angular 13
    - Angular Material 13
    - Bootstrap 4
    - moment
    
 - **Web Status UI**
    - Asp.Net 6
      - Asp.Net Core Health Checks UI 6
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
  - GOOGLE CREDENTIALS [to use OAth2.0 for user delegated authorization]
    - GOOGLE_CLIENT_SECRET=[google client secret]
    - GOOGLE_CLIENT_ID=[google client id]

  - FACECBOOK CREDENTIALS [to use OAth2.0 for user delegated authorization]
    - FACEBOOK_CLIENT_SECRET=[facebook client secret]
    - FACEBOOK_CLIENT_ID=[facebook client id]

  - EMAIL PARAMS [for individual user account confrmation]
     - EMAIL_FROM=[service email box address]
     - EMAIL_PASSWORD=[service email box password]
     
  - BASE ENV VARIABLES
    - HOST_BASE_ADDR=localhost (as default)
    
  - IDENTITY MICROSERVICE
    - IDENTITY_DB_PORT=5433
    - IDENTITY_API_PORT=5100
    - IDENTITY_API_HTTPS_PORT=5140
    
  - EXPENSES MICROSERVICE
    - EXPENSES_API_PORT = 5101
    - EXPENSES_DB_PORT=5433
    - EXPENSES_AZBUS_CONNSTR=[azure service bus connection string - send/listen]
    
  - NOTIFICATION MICROSERVICE
    - NOTIFICATIONS_API_PORT=5102
    - NOTIFICATIONS_API_HTTPS_PORT=5142
    - NOTIFICATIONS_SIGNALR_CONNSTR=[azure signalR connection string]
    - NOTIFICATIONS_AZBUS_CONNSTR=[azure service bus connection string - send/listen/manage]
    
  - WEB UI HOST MICROSERVICE
    - UI_PORT=5200
    - UI_HTTPS_PORT=5240
    
  - HEALTH CHECKS MICROSERVICE [web ui with health check information about the system]
    - HEALTHCHECKS_UI_PORT=5201
    - HEALTHCHECKS_UI_HTTPS_PORT=5241
 
  - SEQ [logs storage]
    - SEQ_PORT=5341
```
