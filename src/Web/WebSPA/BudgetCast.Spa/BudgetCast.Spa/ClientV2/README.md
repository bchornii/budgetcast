# EoiPortal

## Project structure
```
/src
├── app
│   ├── core
│   │   ├── auth
│   │   │   ├── guards
│   │   │   │   ├── auth-guard.ts
│   │   │   │   └── auth-guard.spec.ts
│   │   │   ├── models
│   │   │   │   └── auth.model.ts
│   │   │   ├── pages
│   │   │   │   ├── login
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── directives/
│   │   │   │   │   ├── models/
│   │   │   │   │   ├── pipes/
│   │   │   │   │   ├── services/
│   │   │   │   │   ├── login.html
│   │   │   │   │   ├── login.scss
│   │   │   │   │   ├── login.spec.ts
│   │   │   │   │   └── login.ts
│   │   │   │   └── register/
│   │   │   ├── services
│   │   │   │   ├── auth-store.ts
│   │   │   │   └── auth-store.spec.ts
│   │   │   └── auth.routes.ts
│   │   ├── interceptors
│   │   │   ├── auth-interceptor.ts
│   │   │   └── auth-interceptor.spec.ts
│   │   ├── home/
│   │   └── services
│   │       ├── notification-api.ts
│   │       └── notification-api.spec.ts
│   ├── modules
│   │   ├── about-goverment/
│   │   ├── agriculture/
│   │   ├── bussiness-development/
│   │   ├── education-and-training/
│   │   ├── employment/
│   │   │   └── application/
│   │   ├── energy/
│   │   ├── fish-and-wildlife/
│   │   └── moving-to-alberta/
│   ├── shared
│   │   ├── components/
│   │   ├── pipes/
│   │   └── utils/
│   ├── app.config.ts
│   ├── app.html
│   ├── app.routes.ts
│   ├── app.scss
│   ├── app.spec.ts
│   └── app.ts
├── index.html
├── main.ts
└── styles.scss
```

### Naming
 - Separate words within a file name with hyphens (-). For example, a component named UserProfile has a file name user-profile.ts.
 - Components, Directives and Services do not use a suffix anymore, both for the class and the file name.
 - Pipes, Guards, Resolvers, Interceptors and Modules keep the suffix on the class name but the suffix is now separated with a - instead of a ., like auth-guard.ts instead of auth.guard.ts.

### `core` folder
The code specific to a non-business feature. A non-business feature is a feature that is not specific to the business domain of the application. 

Some features might be as simple as a service or a directive. To simplify your structure, you might not want to create a dedicated folder for them in the `src/app/core` folder. In this situation, place the code directly at the root of the folder, in a folder based on its technical shape (`services`, `directives`, `pipes`, `guards`, `interceptors`).

#### `auth` non-business feature. 

While being specific to the authentication feature, some code is shared between the different pages of the feature, or even more. The user model and the authentication service are shared between the **different pages of the auth feature**.
The authentication guard is shared between the **different pages of the whole application**. So, these would go under `auth/services` and `auth/guards`.

The `auth/pages` folders are used to store the routed component and all the related and specific code of a page. At the root of the folder, you’ll find the routed component, and more folders for content shared between the different pages of the feature:
 - `components` for the components used in the pages
 - `services` for the services used in the pages
 - `models` for the models used in the pages
 - `directives` for the directives used in the pages
 - `pipes` for the pipes used in the pages

### `features` folder
A business feature is a feature that is specific to the business domain of the application. Rather than including all features in a single folder, such as `src/app/features` you can group them by domain, in nested folders:
```
   ├── modules
   │   ├── employment/
           └── application/
```

Besides this meta organization, features use the same logic as the core folder:
 - a dedicated folder for each feature
 - a dedicated folder for the pages of the feature
 - more folders for content shared between the different pages of the feature

 You won't have any at the root of the `src/app/modules` folder. It's all about business isolated features. If you need to share some code between multiple features of a same domain, put it at the root of the domain folder, such as `employment` in this case.
```
   ├── modules
   │   ├── employment/
   │       ├── application/
   │       └── employment.model.ts
```

### `shared` folder
The shared code is the code that is shared between the different features of the application. There are two kinds of code being shared between features:
 - the one including some business logic
 - the one that doesn’t.

Let’s take a notification component as an example. Even if the data to fill it comes from a business feature, the notification should not care. If it expects a title and a message, it should not care about the origin of the title and the message. And its Signal input() should be named title and message, not productTitle and productMessage.

Such a component is perfectly suited to be placed in the `src/app/shared` folder. Such components, pipes and some util functions are the perfect example of _dumb_ shared code.

The _smart_ shared code is the code that includes some business logic. Where to put it as it’s both shared and business specific? Place this content in the related feature folder in the `src/app/modules` folder. That's quite common for a feature to include some part of another one.

It can be about Shared UI content, like a product-card component, or shared business logic, like a product-api or a product.model. Placing it in the shared folder will quickly lead to have too much content there, making it hard to maintain.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

