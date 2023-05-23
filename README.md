# BankWebApp

This solution contains three projects: 

A WebApp that allows Cashiers to manage Clients and admins to oversee and manage users.

A WebApi that allows a user to authenticate and request account data for this user and admin to oversee and request data for any user.

A DirtyMoneyDetector when run will check the latest transactions in the database and write a json and txt log file to bin/debug.


## Demo

WebApp: https://bankease.azurewebsites.net


## WebApp

The Bank Ease web application built with Razor Pages as the frontend and an SQL database as the backend. 

### Getting Started
Prerequisites

    .NET Core 3.1 or later.
    An IDE such as Visual Studio or VSCode.
    A database management system such as SQL Server.
    BankAppData.Bak
    
Setup
Restore the database from BankAppData.Bak backup
https://aspcodeprod.blob.core.windows.net/school-dev/BankAppDatav2%20(1).bak
Run the application VS

Testing Credentials <br />
Admin <br />
username: testme@admin.com password: Testme123# <br />
Cashier <br />
username: testme@cashier.com password: Testme123# <br />

### Architecture


BankWebApp:
contains the Razor Pages for the frontend of the application.
BankWebApp utilizes ASP.NET for user management and authentication. 

DataAccessLibrary: 
This library contains the database context and all associated entity classes and migrations.
Its primary responsibility is to interact with the database and provide CRUD (create, read, update, delete) operations for the other parts of the application.

ServicesLibrary: 
This library contains business logic for the application, such as how to create, read, update, and delete users, clients, and accounts. It also includes functionality for displaying data such as clients, users, transactions, payments, loans, and cards.
Its purpose is to abstract away any database-specific code and provide a simple interface for the frontend or other parts of the application to interact with.

UtilityLibrary: 
This library contains a series helper classes and methods that are used throughout the application, such as string manipulation, object comparer, errorcodes, sorting columns and search functionality.

### Features

Cashier can perform CRUD operations for Clients and Accounts. In addition to basic CRUD functionality, Cashier can manage transfers, deposits and withdrawals.
Cashiers can also view client data such as transactions, payments, loans and cards.

Admin can perform CRUD operations for Clients and Users, manage roles and reset passwords.



## WebApi

This API provides endpoints for Retrieving client information from the database.
### Getting Started
Prerequisites

    .NET Core 3.1 or later.
    An IDE such as Visual Studio or VSCode.
    A database management system such as SQL Server.

Authentication

User credentials are created on startup and for testing purposes you can authenticate a user with: name: {FirstName} password: {userId}

View self endpoint is only avilable to the authorized User.

View any endpoints are only available to Admin role.

To access these endpoints, you need to include an Authorization header in your request with a valid JWT token.


Endpoints
GET /client

Retrieves client information from the database for the currently authorized user.
Request

    Method: GET
    URL: /client

Response

    Status code: 200
    Body: Customer object

GET /client-account-transactions/{accountId}/{limit}/{offset}

Retrieves a list of transactions for the account with the specified account ID for the currently authorized user, with limits and offsets, from the database.
Request

    Method: GET
    URL: /client-account-transactions/{accountId}/{limit}/{offset}
    Path parameters:
        accountId: ID of the account to retrieve transactions for
        limit: Maximum number of transactions to retrieve
        offset: Number of transactions to skip before starting to retrieve results

Response

    Status code: 200
    Body: List of Transaction objects

GET /client/{id}

Retrieves information for a client with the specified ID from the database. This endpoint is only accessible to admins.
Request

    Method: GET
    URL: /client/{id}
    Path parameters:
        id: ID of the client to retrieve information for

Response

    Status code: 200
    Body: Customer object

GET /client-account-transactions/{customerId}/{accountId}/{limit}/{offset}

Retrieves a list of transactions for the specified client account from the database, with a specified limit and offset. This endpoint is only accessible to admins.
Request

    Method: GET
    URL: /client-account-transactions/{customerId}/{accountId}/{limit}/{offset}
    Path parameters:
        customerId: ID of the customer whose account transactions to retrieve
        accountId: ID of the account to retrieve transactions for
        limit: Maximum number of transactions to retrieve
        offset: Number of transactions to skip before starting to retrieve results

Response

    Status code: 200
    Body: List of Transaction objects

Authorization

All endpoints require authorization. Users with the "User" role can access the /client and /client-account-transactions/{accountId}/{limit}/{offset} endpoints, while users with the "Admin" role can access all endpoints. Access to these endpoints is controlled using the [Authorize] attribute and the Roles property.




## DirtyMoneyDetector

This console application detects suspicious transactions and logs them in JSON/TXT format and saves the log files in /bin/debug.

DirtyMoneyDetector initializes the database context, creates instances of CustomerService and AccountService, creates a folder structure for the logs, retrieves log data using an instance of LogHandler, and then serializes the log data to both JSON and TXT files using an instance of LogFileStructurer.

LogFileStructurer contains two methods, 

CreateFolderStructure()
Creates a folder structure with the name "DirtyMoneyDetector_LogFiles" and subfolders with the name "Date-" followed by the current date.

SerializeObjectsToJsonFileAndTxt()
Serializes the provided transactions, logs the data to both JSON and TXT files. Logs are separated by country "Log-yyyy-MM-dd-country-{Country} .json/.txt".

Modifying LogFileStructurer

The log structure can be modified by changing the Log-yyyy-MM-dd-country-{"Value"} to any property of the logged item keep in mind that the valuse must be present in the object for this to work.
