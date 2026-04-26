# Multiply by 5 Checker

A full-stack application that checks whether a number is divisible by 5.

## Projects

### Backend – MultiplyApi
ASP.NET Core 8 REST API
- Single endpoint: `POST /api/multiply/check`
- Rate limiting: 10 requests per minute per client
- Built with: .NET 8, ASP.NET Core RateLimiting, IMemoryCache

### Frontend – multiply-ui
Angular application
- Input field + result display
- Countdown timer when rate limit is exceeded
- HTTP call duration logged to dev tools

## Running the Project

### Backend
```bash
cd MultiplyApi
dotnet run
```
API available at: `https://localhost:5000`
Swagger UI at: `https://localhost:5000/swagger`

### Frontend
```bash
cd multiply-ui
npm install
ng serve
```
App available at: `http://localhost:4200`
