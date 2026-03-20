📚 MyBookCloud: Event-Driven Book Management System

	Професійний мікросервісний проєкт, побудований на базі .NET 7, Angular, MassTransit та SignalR. Система демонструє сучасні підходи до побудови масштабованих додатків з використанням черг повідомлень та оновлень у реальному часі.

🚀 Основні можливості
	Event-Driven Architecture: Використання RabbitMQ для асинхронної обробки та збагачення даних.
	Real-time Updates: SignalR миттєво оновлює стан книг на клієнті без перезавантаження сторінки.
	Background Processing: Окремий Worker-сервіс для інтеграції з Google Books API.
	Clean Architecture: Чіткий розподіл на шари (Application, Domain, Persistence, Infrastructure).
	Full Dockerization: Весь стек розгортається однією командою.

🛠 Технологічний стек
	Backend: .NET 7 (C#), Entity Framework Core (PostgreSQL).
	Frontend: Angular 16+ (SPA).
	Messaging: MassTransit + RabbitMQ.
	Real-time: SignalR.
	Containerization: Docker & Docker Compose.

📦 Як запустити проєкт (Docker)
	Проєкт налаштований для максимально швидкого старту ("One-Click Experience"). Вам знадобиться лише встановлений Docker Desktop.
	Клонуйте репозиторій:
	bash
	git clone https://github.com
	cd MyBookCloud

	Запустіть всю систему:
	bash
	docker-compose up --build

	Відкрийте додаток у браузері:
	Frontend: http://localhost:49375
	API Swagger: http://localhost:7126/swagger
	RabbitMQ Management: http://localhost:15672 (guest/guest)
👤 Дані для входу (Default User)
	При першому запуску система автоматично створює адміністратора (Seed Data) для тестування:
	Email: admin@test.com
	Password: admin123
⚙️ Технічні деталі (Deep Dive)
	Database: Використовується PostgreSQL. Міграції та початкові дані застосовуються автоматично при старті API контейнера.
	Infrastructure as Code: Усі зв'язки між сервісами (БД, RabbitMQ, API) налаштовані через внутрішню мережу Docker.
	Security: Використовується JWT-авторизація з підтримкою SignalR (передача токена через Query String для WebSockets).