# Stock Price Alert Service

A real-time stock price monitoring and alerting service built with **C#, ASP.NET Core** and SignalR. The system simulates stock price movements, allows users to set price alerts, and delivers real-time notifications when alert conditions are triggered.

## 🎯 Features

- **Real-time Stock Price Simulation**: Simulates stock price movements fluctating by -2% to 2% every 15 seconds
- **Price Alert System**: Set "above" or "below" threshold alerts for any stock symbol
- **Instant Notifications**: WebSocket-based real-time alerts using SignalR
- **User Management**: Multi-user support with isolated alert management
- **Caching**: Redis-based caching for optimized performance. For simplicity of testing this code base is using InMemory Distributed cache however this can be easily changed to redis with a one liner through dependency injection
- **Clean Architecture**: Modular design following SOLID principles
- **Testing**: Unit and integration tests included
- **Docker Support**: Full containerization with docker-compose
- **Web Interface**: Simple HTML dashboard for managing alerts and viewing prices

## 🏗️ Architecture

The solution follows **Clean Architecture** principles with clear separation of concerns:

```
├── StockPriceMonitoring.Api/          		# Web API and Controllers. This project also hosts a background worker that periodically simulates stock price changes and updates connected clients
├── StockPriceMonitoring.Core/         		# Domain Models and Interfaces
├── StockPriceMonitoring.Repositories/		# Data Access and External Services
├── StockPriceMonitoring.Application/  		# Business Logic and Services
└── StockPriceMonitoring.Tests/        		# Unit and Integration Tests
```

### Technology Stack

- **Framework**: ASP.NET Core 8
- **Database**: Entity Framework Core with MySql Server
- **Caching**: Redis-based caching for optimized performance
- **Real-time Communication**: SignalR (WebSockets)
- **Background Processing**: Hosted background services
- **Testing**: xUnit, Moq, In-Memory Database
- **Containerization**: Docker and Docker Compose

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- MySql Server (or use Docker)
- Redis (or use Docker) or In-memory distributed cache for test simplicity

### Option 1: Run with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd StockPriceMontirong
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

3. **Access the application**
   - **Web Interface**: http://localhost/index.html
   - **API Documentation**: http://localhost/swagger
   - **SignalR Hub**: http://localhost/notificationhub

### Option 2: Run Locally

1. **Setup Database and Redis**
   ```bash
   # Start MySQL and Redis (using Docker)
   docker run -d --name mysql -e MYSQL_ROOT_PASSWORD=Password123! -e MYSQL_DATABASE=StockAlertDb -p 3306:3306 mysql:8.0
   docker run -d --name redis -p 6379:6379 redis:7-alpine
   ```

2. **Update Connection Strings**
   
   Update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=StockAlertDb;Uid=root;Pwd=Password123!;AllowPublicKeyRetrieval=True;UseSSL=False;",
       "RedisConnection": "localhost:6379"
     }
   }
   ```

3. **Build and Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project StockAlertService.Api
   ```

   ```

## 🎮 Usage

### Web Interface

1. **Open your browser** to http://localhost/index.html
2. **Enter a User ID** (e.g., "peter_borg") and click "Connect"
3. **View live stock prices** updating in real-time
4. **Create price alerts**:
   - Enter stock symbol (AAPL, GOOGL, MSFT, TSLA, AMZN)
   - Set price threshold
   - Choose "Above" or "Below"
   - Click "Create Alert"
5. **Watch for notifications** when alerts trigger at the bottom of the page 

### API Endpoints

#### Stock Prices
```bash
# Get all current prices
GET /api/stockprices

# Get specific stock price
GET /api/stockprices/{symbol}
```

#### Alerts
```bash
# Create an alert
POST /api/alerts
{
  "userId": "peter_borg",
  "symbol": "AAPL",
  "threshold": 200.00,
  "type": 0  // 0 = Above, 1 = Below
}

# Get user's alerts
GET /api/alerts/user/{userId}

# Delete an alert
DELETE /api/alerts/{alertId}/user/{userId}
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter Category=Unit

# Integration tests only
dotnet test --filter Category=Integration
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter Category=Unit

# Integration tests only
dotnet test --filter Category=Integration
```

### Test Coverage
The test suite includes:
- **Service Layer Tests**: Business logic validation
- **Repository Tests**: Data access verification
- **Controller Tests**: API endpoint validation
- **Integration Tests**: End-to-end scenarios

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | MySQL connection string | See appsettings.json |
| `ConnectionStrings__RedisConnection` | Redis connection string | localhost:6379 |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Development |

### Stock Symbols

The system simulates prices for these default symbols:
- **AAPL** (Apple) 
- **GOOGL** (Alphabet)
- **MSFT** (Microsoft)
- **TSLA** (Tesla)
- **AMZN** (Amazon) 

### Price Update Frequency

- **Background Service**: Updates prices every 15 seconds
- **Price Variation**: ±2% maximum change per update
- **Cache TTL**: 30 seconds for price data

## 🏛️ Architecture Details

### Clean Architecture Layers

#### 1. Core Layer (`StockPriceMonitoring.Core`)
- **Domain Models**: `StockPrice`, `PriceAlert`, `AlertNotification`
- **Enumerations**: 'AlertType', 'AlertStatus'

#### 2. Application Layer (`StockPriceMonitoring.Services and StockPriceMonitoring.Repositories`)
- **Interfaces**: Repositories and Service interface contracts
- **Repositories**: Data access implementation
- **Services**: Business logic implementation
- **External Services**: Caching, third-party integrations
- **Background Services**: Price simulation and alert checking
- **SignalR Hubs**: Real-time communication
- **Dependencies**: Core layer 

#### 3. Infrastructure Layer (`StockPricingMonitoring.Repositories.EF`)
- **Repositories**: Data access implementation
- **Database Context**: Entity Framework configuration
- **Dependencies**: Core layer

#### 4. API Layer (`StockPricingMonitoring.Api`)
- **Controllers**: HTTP endpoints
- **Configuration**: Dependency injection, middleware
- **Static Files**: Web interface
- **Dependencies**: Application and Infrastructure layers

### Design Patterns Used

- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling
- **Background Service Pattern**: Continuous processing
- **Observer Pattern**: Real-time notifications via SignalR
- **Strategy Pattern**: Alert type handling

## 📈 Scalability Considerations

### Current Architecture Benefits
- **Stateless API**: Easy horizontal scaling (adding more nodes to handle volumes)
- **Background Processing**: Decoupled from API requests
- **Caching Layer**: Reduced database load
- **Event-Driven**: SignalR for real-time updates

### Scaling Strategies

#### Database Optimization
```sql
-- Recommended indexes for performance
CREATE INDEX IX_PriceAlerts_Symbol_Status ON PriceAlerts (Symbol, Status);
CREATE INDEX IX_PriceAlerts_UserId ON PriceAlerts (UserId);
CREATE INDEX IX_StockPrices_Symbol_Timestamp ON StockPrices (Symbol, Timestamp DESC);
```

#### Horizontal Scaling
- **Load Balancer**: Distribute API requests
- **Database Clustering**: MySQL Master-Slave 
- **Redis Cluster**: Scale caching layer
- **Message Queue**: Decouple alert processing (RabbitMQ/Azure Service Bus)

#### Performance Optimizations
- **Connection Pooling**: Database connections
- **Bulk Operations**: Batch alert processing
- **CDN**: Static content delivery
- **API Rate Limiting**: Prevent abuse

### Bottlenecks and Mitigation

#### Single Background Service Limitation

The current architecture uses a single background service instance to update stock prices every 15 seconds. While this works well for development and small-scale deployments, it becomes a bottleneck in production environments with multiple application instances. If one instance fails, price updates stop entirely, and the system cannot leverage horizontal scaling for this critical component.

**Mitigation Strategies**: Implement distributed locks using Redis or a database-based locking mechanism to ensure only one instance processes price updates at a time, while allowing failover to other instances. Another approach could be to use a message queue system like RabbitMQ where price update jobs can be distributed across multiple worker instances. If cloud computing is an option we can have these running as Azurre Functions or AWS Lambda which provide automatic scaling for background processing tasks.

#### Database Query Performance

As the number of alerts grows into thousands or millions, the alert checking queries can become a significant performance bottleneck. The current implementation loads all active alerts into memory and checks them against current prices, which doesn't scale well with large datasets. Additionally, frequent database reads for price history and alert validation can overwhelm the MySQL server.

**Mitigation Strategies**: Implement proper database indexing on frequently queried columns such as Symbol, Status, and UserId. Consider partitioning the PriceAlerts table by symbol or date ranges to improve query performance. Introduce read replicas to distribute query load away from the primary database, and implement query optimization techniques like batching and pagination. Another option would be a CQRS pattern where read operations use optimized data stores different from the transactional database.

#### SignalR Memory Consumption

Each SignalR connection consumes server memory, and with thousands of concurrent users, this can lead to memory pressure and potential out-of-memory exceptions. The problem is compounded when users leave connections open without properly disconnecting, leading to memory leaks over time. Additionally, broadcasting price updates to all connected clients can create CPU and network bottlenecks.

**Mitigation Strategies**: Implement connection limits per user and total connection capping to prevent memory exhaustion. Sticky sessions can be used in load-balanced environments to ensure consistent connection routing. For high-scale scenarios, a cloud service such as Azure SignalR Service can be used, which offloads connection management and scaling concerns. Another improvment would be that price updates are pushed only to users that subscribe to them so that they only receive updates for symbols they're actively monitoring.

#### Alert Processing Latency

As the volume of alerts increases, processing each alert individually can introduce significant latency, especially when multiple alerts trigger simultaneously. The current synchronous processing model can block the background service, delaying subsequent price updates and alert checks. Network calls for notifications and database updates for triggered alerts can further compound these delays.

**Mitigation Strategies**: Implement batch processing to handle multiple alerts in single operations, reducing database round trips and improving throughput. Another great improvement would be to use asynchronous message queues to decouple alert processing from price updates, allowing the system to handle high-volume scenarios gracefully.


## 🔒 Production Considerations

### Security
- [ ] **Authentication**: Add JWT/OAuth integration
- [ ] **Authorization**: Role-based access control
- [ ] **Rate Limiting**: Prevent API abuse
- [ ] **Input Validation**: Comprehensive data validation
- [ ] **HTTPS**: Force secure connections
- [ ] **CORS**: Configure proper origins

### Monitoring
- [ ] **Application Insights**: Performance monitoring
- [ ] **Health Checks**: System health endpoints
- [ ] **Metrics**: Custom performance counters
- [ ] **Alerting**: System failure notifications

### Reliability
- [ ] **Circuit Breaker**: Handle service failures
- [ ] **Zero-Downtime Deployment**: Blue-green deployments



### Debug Mode

### MySQL Tables

The application uses these main tables:

#### StockPrices
```sql
CREATE TABLE StockPrices (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Symbol VARCHAR(10) NOT NULL,
    Price DECIMAL(18,4) NOT NULL,
    Timestamp DATETIME(6) NOT NULL,
    DayOpen DECIMAL(18,4) NOT NULL,
    DayHigh DECIMAL(18,4) NOT NULL,
    DayLow DECIMAL(18,4) NOT NULL,
    INDEX IX_StockPrices_Symbol_Timestamp (Symbol, Timestamp DESC)
);
```

#### PriceAlerts
```sql
CREATE TABLE PriceAlerts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(100) NOT NULL,
    Symbol VARCHAR(10) NOT NULL,
    Threshold DECIMAL(18,4) NOT NULL,
    Type INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME(6) NOT NULL,
    TriggeredAt DATETIME(6) NULL,
    TriggeredPrice DECIMAL(18,4) NULL,
    INDEX IX_PriceAlerts_Symbol_Status (Symbol, Status),
    INDEX IX_PriceAlerts_UserId (UserId)
);
```

Enable detailed logging in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.SignalR": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "StockAlertService": "Debug"
    }
  }
}
```

