Prerequisites

    • For the API project, install .NET 8 SDK and configure global environment variable
    • For the database install SQL and SQL Management
    • Set a database username and password for testing
    • Install IDE Visual Studio 2022 or Visual Studio Code, depending on your preference
        ○ Note: The following steps are a guide for Visual Studio 2022 and Visual Studio Code IDEs

Steps to run the project

    1. Clone the project from the repository
    2. Download and install NuGet package library dependencies for each project (API, Service, DAL)
    3. Project API: Modify the connection string in appsettings and secret keys to reflect the server, username and password values ​​established in the prerequisites
    4. DAL Project:
        a. Open a power shell terminal in the example project path ./RapidPay.DAL/
        b. Delete "Migrations" folder from project's root path
        c. Run following commands:
            i. Create initial migrations: dotnet ef migrations add InitialCreate --project RapidPay.DAL.csproj --startup-project ../RapidPay.API/RapidPay.API.csproj
            ii. Update data base: dotnet ef database update --project RapidPay.DAL.csproj --startup-project ../RapidPay.API/RapidPay.API.csproj
        d. The above commands will create the following changes:
            i. A new "Migrations" folder will be created with its corresponding files
            ii. RapidPay database will be created in SQL with the respective tables
    5. Set Startup Project to RapidPay.API and run it, the project is configured to create both test users and their respective roles when running for the first time
    6. Test users:
        a. User: admin | Password: Admin@1234 |Assigned Role: "Admin"
        b. User: testuser | Password: Password123! |Assigned Role: "User"
		
Steps to run the project (Non-Windows users) unsing VS Code

	1. Clone the project from the repository
	2. Open a terminal in each project root path and run the following command "dotnet restore". This command will restore the dependencies and tools of a project.
	3. Project API: Modify the connection string in appsettings and secret keys to reflect the server, username and password values ​​established in the prerequisites
	4. DAL Project:
        a. Open a power shell terminal in the example project path ./RapidPay.DAL/
        b. Delete "Migrations" folder from project's root path
        c. Run following commands:
            i. Create initial migrations: dotnet ef migrations add InitialCreate --project RapidPay.DAL.csproj --startup-project ../RapidPay.API/RapidPay.API.csproj
            ii. Update data base: dotnet ef database update --project RapidPay.DAL.csproj --startup-project ../RapidPay.API/RapidPay.API.csproj
        d. The above commands will create the following changes:
            i. A new "Migrations" folder will be created with its corresponding files
            ii. RapidPay database will be created in SQL with the respective tables
    5. Open a new terminal on RapidPay.API path and run following command "dotnet run", the project is configured to create both test users and their respective roles when running for the first time.
    6. Test users:
		a. User: admin | Password: Admin@1234 |Assigned Role: "Admin"
		b. User: testuser | Password: Password123! |Assigned Role: "User"


Project architecture

The solution is divided into 4 projects which have the following functionalities

	1. RapidPay.API: This directory contains the API project. The API project is responsible for exposing endpoints that can be consumed by clients (e.g., web, mobile apps). It acts as the entry point for the application, handling HTTP requests, routing, and returning responses.
	2. RapidPay.Service: This directory contains the Service project. The Service layer handles the business logic of the application. It processes data received from the API and interacts with the Data Access Layer to perform CRUD and custom operations.
	3. RapidPay.DAL: This directory contains the Data Access Layer (DAL) project. The DAL is responsible for interacting with the database. It includes the context and entities that represent the database schema and perform operations.
	4. RapidPay.Test: This directory contains the test project. The Test project includes unit tests to ensure that the application functions correctly and meets the specified requirements.
	5. RapidPay.Shared: This directory contains Shared functionalities that can be used for all other projects. It contains a file logger which creates a folder and file called "logs" to record errors or certain processes, this folder will be created at the root of API project.

This structure follows a common layered architecture pattern, which helps in separating concerns, improving code maintainability, and making it easier to test and manage different parts of the application independently. Including a dedicated test project also ensures that you can maintain high code quality and verify that changes do not introduce regressions.


Project Notes:

For the initial value in the fee calculation there is a hardcoded value of 0.5, which is found in RapidPay.Services/Helper/UFE.cs in a global variable at the beginning of the class