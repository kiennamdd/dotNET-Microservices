# dotNET-Microservices
- An E-Commerce application, designed towards microservices architecture. <br>
- Database used are: PostgresSQL, MS-SQL, MongoDb
- Each backend service is built using ASP.NET Core WebApi (.NET 6). Each service performs an CRUD for entities and works with database using EntityFramework and repository pattern.<br>
- Asynchronous communication between services (send/receive integration events) is implemented using RabbitMQ with MassTransit.<br>
- Clean Architecture implemented for Cart and Order service. Moreover, Order service is developed with CQRS pattern and Event-driven design.<br>
- Stripe API (https://stripe.com) is integrated for managing checkout session, retrieving payment event in realtime.<br>
- Before providing APIs to frontend, an API Gateway is implemented with Ocelot.
- An simple website is built with ASP.Net Core MVC to consume APIs of backend services using HTTP Request.
- All project modules are containerized with Docker and managed with Docker Compose.
## Run project
### Prerequisites:
- Docker installed
- To ensure checkout feature works, you must set your api key provided by Stripe for STRIPE_API_KEY in stripe_cli.env, and also in appsettings.json for Order.API, Discount.API modules.
- To ensure email service works, you must add your SMTP server and email account details to the appsettings.json at Email.API module.
### Setup and Run:
1. Clone project
```
git clone https://github.com/kiennamdd/dotNET-Microservices.git
```
2. Go to project folder
```
cd dotNET-Microservices/src
```
3. Run with Docker Compose (make sure Docker is running)
```
docker compose up -d
```
## References
Before working on this project, I have read and referenced the following projects:
1. https://github.com/bhrugen/Mango
2. https://github.com/aspnetrun/run-aspnetcore-microservices
3. https://github.com/ardalis/CleanArchitecture
4. https://github.com/dotnet-architecture/eShopOnContainers
