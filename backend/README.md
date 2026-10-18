# Financy — backend

ASP.NET Core 8 Web API for Financy. Setup, configuration and the API reference are in the [main README](../README.md).

```bash
cp Financy.API/appsettings.example.json Financy.API/appsettings.json   # then set JwtSettings:SecretKey
dotnet ef database update --project Financy.Infrastructure --startup-project Financy.API
dotnet run --project Financy.API   # http://localhost:5198/swagger
```
