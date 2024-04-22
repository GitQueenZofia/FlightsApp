Flight Management System using ASP .NET core

Description:
Flight Management System API developed using ASP.NET Core. 
It enables users to manage flight information, including browsing, adding, updating, and deleting flights. 
The API incorporates various features such as authentication, data validation, ORM integration with SQLite and unit testing.

Features Implemented:
- Flight Management API: Implements CRUD operations for managing flights.
- Authentication: Utilizes JWT token-based authentication to secure API endpoints.
- Data Validation: Ensures correctness and consistency of input data through validation mechanisms.
- ORM Integration with SQLite: Utilizes Entity Framework Core with SQLite for database operations, simplifying data access.
- Unit Tests: Includes unit tests for AuthController and FlightController to verify the reliability and correctness of API functionality.
UserManager has been mocked for testing AuthController.

Additional info:
- To access the API endpoints, users must first register and then log in.
Use the /register endpoint to register a new user and /login to log in.
- Aircraft types are represented as an enum in the API. When specifying the aircraft type, use one of the following enum values:
0, 1, 2, 3 (orresponding aicraft types: Embraer, Boeing, Airbus, Other respectively).
- The database contains sample flights for demonstration purposes.
