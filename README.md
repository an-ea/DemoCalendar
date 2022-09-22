# Demo Calendar
Demo Calendar is a demo project written using a microservices approach. It provides basic calendar functionality with authentication/authorization.

**Microservices:**
- Calendar.Api is a REST API with Swagger.
- IdentityServer implements Single sign-on.
- WebClient is a Web MVC application.
- SQL Server.

## Getting Started
### Docker:
```powershell
docker-compose build
docker-compose up
```
URLs:
```
Web Client: http://host.docker.internal:7002
API: http://host.docker.internal:7001
```

### or Visual Studio:
Startup projects:
- Calendar.Api
- WebClient
- IdentityServer

In debug mode the application uses the in-memory database.

URLs:
```
Web Client: http://localhost:7002
API: http://localhost:7001
```

### Credentials:
- Login: demo
- Password: demo

**Used libraries:**
- AutoFixture
- AutoMapper
- FluentAssertions
- FluentValidation
- IdentityServer4
- MediatR
- EntityFrameworkCore
- Moq
- Polly
- Refit
- Swashbuckle