# MyBookCloud

A full-stack application with ASP.NET Core Web API backend and Angular frontend.

## Project Structure

- **Backend**: ASP.NET Core Web API with WeatherForecast controller
- **Frontend**: Angular application in `ClientApp` folder

## Prerequisites

- .NET 7.0 SDK
- Node.js (version 16 or higher)
- npm

## Running the Application

### 1. Start the Backend API

```bash
cd MyBookCloud
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:7126
- HTTP: http://localhost:5194
- Swagger UI: https://localhost:7126/swagger

### 2. Start the Angular Frontend

```bash
cd MyBookCloud/ClientApp
npm install
ng serve
```

The Angular app will be available at:
- http://localhost:4200

## Features

- **Weather Forecast API**: Returns 5-day weather forecast with random data
- **Angular Frontend**: Displays weather forecast in a responsive, modern UI
- **CORS Configuration**: Properly configured to allow Angular app to access the API
- **Error Handling**: Graceful error handling with user-friendly messages
- **Responsive Design**: Mobile-friendly layout with modern styling

## API Endpoints

- `GET /api/weatherforecast` - Returns weather forecast data

## Development Notes

- The Angular app uses the classic module-based approach (not standalone components)
- CORS is configured to allow requests from `http://localhost:4200`
- The frontend automatically refreshes data and handles loading states
