📚 MyBookCloud – Event-Driven Book Management System

MyBookCloud is a microservices-based application built with .NET 7, Angular, MassTransit, and SignalR.
It demonstrates modern approaches to building scalable systems using event-driven architecture, message queues, and real-time updates.

The application allows users to manage their personal library by adding books with reading status (Reading / Completed / Want to Read) and personal ratings.
When a book is added via ISBN, the system automatically fetches metadata (such as cover and page count) from Google Books API and updates the UI in real time.

🚀 Key Features

🧠 Event-Driven Architecture
Uses RabbitMQ for asynchronous processing and data enrichment

⚡ Real-Time Updates
SignalR instantly updates book data on the client without page reload

📖 Book Management
Add books with status (Reading / Completed / Want to Read) and ratings

🔎 ISBN Integration
Automatically retrieves book cover and page count via Google Books API

🔄 Background Processing
Dedicated Worker service for external API integration

🧱 Clean Architecture / DDD
Clear separation of layers (Application, Domain, Persistence, Infrastructure)

🐳 Full Dockerization
Entire system runs with a single command

🛠 Tech Stack

Backend:

.NET 7 (C#)

Entity Framework Core (PostgreSQL)

Frontend:

Angular 16+ (SPA)

Messaging:

MassTransit + RabbitMQ

Real-Time:

SignalR

Infrastructure:

Docker & Docker Compose

📦 Running the Project (Docker)

The project is designed for a fast “one-click” setup.
You only need Docker Desktop installed.

Clone the repository
git clone https://github.com/yourname/MyBookCloud
cd MyBookCloud
Run the entire system
docker-compose up --build
🌐 Access the Application

Frontend: http://localhost:49375

API (Swagger): http://localhost:7126/swagger

RabbitMQ Management: http://localhost:15672
 (guest / guest)

👤 Default Credentials

The system seeds a default admin user on first run:

Email: admin@test.com

Password: admin123

⚙️ Technical Details (Deep Dive)

Database: PostgreSQL with automatic migrations and seed data on startup

Infrastructure as Code: All services (API, DB, RabbitMQ) are configured via Docker internal networking

Security: JWT-based authentication with SignalR support (token passed via query string for WebSockets)

💡 Notes

Book metadata is enriched asynchronously via external APIs

UI updates happen in real time using SignalR

Fallback mechanisms can be applied if external APIs do not return full data (e.g., missing covers)
