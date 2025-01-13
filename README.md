
## Run natively for easier debug
1. Install Dapr and `dapr init`
2. Open a terminal
```
tye run
 ```
2. Attach a debugger to the process

## Run by docker-compose
1. Open a terminal
```
docker-compose up -d --build
```
---
## List of service ports
- http://localhost:5000: API Gateway
- http://localhost:5100: Identity Service / AuthServer
- http://localhost:5200: Event Service
- http://localhost:5300: Game Service
- http://localhost:5400: Payment Service

## List of extension ports
- http://localhost:5432: PostgreSQL
- http://localhost:6379: Redis (Cache & PubSub)
- http://localhost:9411: Zipkin
- http://localhost:9200: Elasticsearch
- http://localhost:5601: Kibana

Swagger endpoints are available for each service. For example, `http://localhost:5200/swagger/index.html` will show the Event Service API documentation.

---
## List of endpoints
- The general format of the endpoints is `{API_Gateway_URL}/{service}/api/1/{user_role}/{action}`.<br/>
For example, `http://localhost:5000/EventService/api/1/Admin/GetEvents` will return a list of events.
- The user role is used to determine the permission of the user. The available roles are `Admin`, `Player`, and `CounterPart`.
- List of SignalR endpoints:
  - `{API_Gateway}/EventService/ws/PlayerNotification`: for player notification
  - `{API_Gateway}/GameService/ws/QuizSession`: for quiz session
