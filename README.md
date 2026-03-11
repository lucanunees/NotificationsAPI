# 🔔 NotificationsAPI — FIAP Cloud Games (FCG)

API de Notificações do **FIAP Cloud Games (FCG)** — responsável pelo envio de e-mails de boas-vindas e confirmações de compra, consumindo eventos via **RabbitMQ** e persistindo registros em **SQL Server** com **Entity Framework Core**.

---

## 📌 Contexto da Aplicação

O FIAP Cloud Games é uma plataforma de jogos em nuvem composta por microsserviços. A **NotificationsAPI** é o serviço responsável por:

- **Consumir eventos** publicados por outros serviços (User Service, Payment Service) via filas do RabbitMQ
- **Enviar e-mails** de boas-vindas quando um usuário é criado
- **Enviar confirmações de compra** quando um pagamento é aprovado
- **Persistir** todas as notificações enviadas no banco de dados SQL Server

A API opera como um **worker assíncrono** que escuta filas do RabbitMQ em background via `BackgroundService`.

### Fluxos suportados

| Fila | Evento | Ação |
|---|---|---|
| `user-created-queue` | Usuário criado | Envia e-mail de boas-vindas e persiste no banco |
| `payment-processed-queue` | Pagamento aprovado | Envia confirmação de compra e persiste no banco |
| `payment-processed-queue` | Pagamento recusado | Apenas loga no console (sem persistência) |

---

## 🏗️ Arquitetura — Clean Architecture

O projeto segue os princípios da **Clean Architecture**, separando responsabilidades em 4 camadas com dependências apontando sempre para o centro (Domain).

### Camadas

| Camada | Projeto | Responsabilidade | Dependências |
|---|---|---|---|
| **Domain** | `NotificationsAPI.Domain` | Entidades de negócio (`Notification`, `NotificationType`), interfaces (`IEmailSender`, `INotificationRepository`, `IEventHandler<T>`). Nenhuma lógica técnica. | Nenhuma |
| **Application** | `NotificationsAPI.Application` | Use cases (`SendWelcomeEmailUseCase`, `SendPurchaseConfirmationUseCase`), event handlers que orquestram o fluxo, e DTOs de entrada. | Domain |
| **Infrastructure** | `NotificationsAPI.Infrastructure` | Implementações concretas: `NotificationDbContext` (EF Core), `NotificationRepository`, `RabbitMQConsumer`, `EmailService` e registro de DI. | Domain, Application |
| **API** | `NotificationsAPI` | `Program.cs` (startup), `RabbitMQConsumerHostedService` (background service), controllers e configuração do middleware. | Application, Infrastructure |

### Fluxo interno de uma mensagem

O caminho que uma mensagem percorre desde a fila até o banco de dados:

| Etapa | Classe | Camada | O que faz |
|---|---|---|---|
| 1 | `RabbitMQConsumer<T>` | Infrastructure | Recebe a mensagem da fila e deserializa o JSON |
| 2 | `IEventHandler<T>.HandleAsync()` | Application | Delega o processamento ao use case correspondente |
| 3 | `UseCase.ExecuteAsync()` | Application | Monta o e-mail e cria a entidade `Notification` |
| 4 | `IEmailSender.SendAsync()` | Infrastructure | Envia o e-mail (simulado no console) |
| 5 | `INotificationRepository.AddAsync()` | Infrastructure | Insere o registro no SQL Server via EF Core |
| 6 | `BasicAckAsync()` | Infrastructure | Confirma o processamento e remove a mensagem da fila |

---

## 📁 Estrutura de Pastas
NotificationsAPI/                              ← raiz do repositório ├── Dockerfile                                 # Build multi-stage da API ├── docker-compose.yml                         # Orquestração dos 3 containers ├── src/ │   ├── NotificationsAPI/                      # Camada de Apresentação (API) │   │   ├── Program.cs │   │   ├── appsettings.json │   │   ├── Controllers/ │   │   │   └── NotificationsController.cs │   │   └── HostedServices/ │   │       └── RabbitMQConsumerHostedService.cs │   │ │   ├── NotificationsAPI.Application/          # Camada de Aplicação │   │   ├── UseCases/ │   │   │   ├── SendWelcomeEmailUseCase.cs │   │   │   └── SendPurchaseConfirmationUseCase.cs │   │   ├── EventHandlers/ │   │   │   ├── UserCreatedEventHandler.cs │   │   │   └── PaymentProcessedEventHandler.cs │   │   └── DTOs/ │   │       ├── UserCreatedEventDto.cs │   │       └── PaymentProcessedEventDto.cs │   │ │   ├── NotificationsAPI.Domain/               # Camada de Domínio │   │   ├── Entities/ │   │   │   └── Notification.cs │   │   └── Interfaces/ │   │       ├── IEmailSender.cs │   │       ├── INotificationRepository.cs │   │       └── IEventHandler.cs │   │ │   └── NotificationsAPI.Infrastructure/       # Camada de Infraestrutura │       ├── Persistence/ │       │   └── NotificationDbContext.cs │       ├── Migrations/ │       │   └── ..._InitialCreate.cs │       ├── Repositories/ │       │   └── NotificationRepository.cs │       ├── RabbitMQ/ │       │   ├── RabbitMQConsumer.cs │       │   ├── RabbitMQPublisher.cs │       │   └── RabbitMQSettings.cs │       ├── Email/ │       │   └── EmailService.cs │       └── DependencyInjection/ │           └── DependencyInjection.cs

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API e Hosted Services |
| Entity Framework Core | 8.0 | ORM, migrations e persistência |
| SQL Server | 2022 | Banco de dados relacional |
| RabbitMQ | 3.x | Message broker (protocolo AMQP) |
| RabbitMQ.Client | 7.2.1 | Client .NET para RabbitMQ (API assíncrona) |
| Docker / Docker Compose | 3.8 | Containerização e orquestração |
| Swagger / OpenAPI | — | Documentação interativa da API |

---

## 🐳 Docker

### Dockerfile — O que faz

O `Dockerfile` na raiz do repositório utiliza **multi-stage build** para gerar uma imagem otimizada. O processo é dividido em 4 estágios:

| Estágio | Imagem Base | O que faz |
|---|---|---|
| **base** | `mcr.microsoft.com/dotnet/aspnet:8.0` (~220MB) | Define a imagem de runtime leve usada na imagem final |
| **build** | `mcr.microsoft.com/dotnet/sdk:8.0` (~900MB) | Copia os `.csproj`, restaura os pacotes NuGet e compila o projeto em modo Release |
| **publish** | Herda do `build` | Executa `dotnet publish` para gerar os binários otimizados para produção |
| **final** | Herda do `base` | Copia apenas os binários publicados (sem SDK, sem código fonte). Resultado: imagem leve de ~220MB |

### docker-compose.yml — O que faz

O `docker-compose.yml` orquestra **3 serviços** que se comunicam pela mesma rede Docker interna (`notifications-network`):

| Serviço | Imagem | Portas | Descrição |
|---|---|---|---|
| **sql-server** | `mcr.microsoft.com/mssql/server:2022-latest` | `1433` | Banco de dados SQL Server. O volume `sqlserver-data` persiste os dados entre reinicializações. |
| **rabbitmq** | `rabbitmq:3-management` | `5672` (AMQP), `15672` (UI) | Message broker com painel de gerenciamento web. |
| **notifications-api** | Build local via `Dockerfile` | `8080` | API .NET 8. Aguarda SQL e RabbitMQ ficarem saudáveis antes de iniciar. |

### Como a comunicação funciona

| Aspecto | Detalhe |
|---|---|
| **Rede interna** | Todos os containers rodam na rede `notifications-network`. Eles se enxergam pelo nome do serviço (ex: `sql-server`, `rabbitmq`). |
| **Variáveis de ambiente** | As variáveis definidas no `docker-compose.yml` sobrescrevem automaticamente os valores do `appsettings.json` (ex: `ConnectionStrings__ConnectionString`). |
| **Healthcheck** | O `depends_on` com `condition: service_healthy` garante que a API só inicia quando SQL Server e RabbitMQ já aceitam conexões. |
| **Migrations automáticas** | No startup da API, `db.Database.Migrate()` cria o database `fcg_notifications_db` e a tabela `Notifications` automaticamente. |
| **Persistência** | O volume `sqlserver-data` garante que os dados do SQL Server não são perdidos ao parar os containers. |

---

## 🚀 Como Executar

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e rodando
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (apenas para desenvolvimento local)

### Subir com Docker Compose (recomendado)

Na raiz do repositório
comando: docker-compose up --build

Aguarde até ver no terminal:
sql-server        | SQL Server is now ready for client connections rabbitmq          | Server startup complete notifications-api | info: Applying migration '20260311043114_InitialCreate' notifications-api | 🚀 Iniciando consumidores RabbitMQ... notifications-api | 🐇 Consumidor iniciado na fila: 'user-created-queue' notifications-api | 🐇 Consumidor iniciado na fila: 'payment-processed-queue' notifications-api | ✅ Todos os consumidores RabbitMQ estão ativos!

### URLs de Acesso

| Serviço | URL | Credenciais |
|---|---|---|
| **Swagger UI** | http://localhost:8080/swagger | — |
| **RabbitMQ Management** | http://localhost:15672 | `guest` / `guest` |
| **SQL Server** | `localhost,1433` | `sa` / `OcP2020123` |

### Comandos Docker Úteis

| Comando | O que faz |
|---|---|
| `docker-compose up --build` | Sobe todos os containers com rebuild |
| `docker-compose up --build -d` | Sobe em background (detached) |
| `docker logs -f notifications-api` | Mostra logs da API em tempo real |
| `docker-compose down` | Para todos os containers |
| `docker-compose down -v` | Para e apaga o volume do banco (reset completo) |
| `docker-compose up --build notifications-api` | Rebuild apenas da API |

---

## 📨 Testando o Fluxo Completo

### 1. Publicar mensagem — E-mail de boas-vindas

Acesse o [RabbitMQ Management](http://localhost:15672) → **Queues** → `user-created-queue` → **Publish message**.

Em **Payload**, cole:
{ "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "userName": "Lucas Nunes", "userEmail": "lucas@teste.com", "createdAt": "2026-03-11T10:00:00Z" }

### 2. Publicar mensagem — Confirmação de compra (aprovado)

Na fila `payment-processed-queue` → **Publish message**:

{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "purchaseId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "amount": 59.99,
  "currency": "BRL",
  "status": "Approved",
  "processedAt": "2026-03-11T10:05:00Z"
}

### 3. Publicar mensagem — Confirmação de compra (recusado)

Na fila `payment-processed-queue` → **Publish message**:

{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "purchaseId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "amount": 59.99,
  "currency": "BRL",
  "status": "Rejected",
  "processedAt": "2026-03-11T10:07:00Z"
}

> Neste caso a API loga `⏭️ Pagamento recusado - nenhum e-mail enviado` e não persiste no banco.

### 4. Verificar no banco de dados
docker exec -it sql-server /opt/mssql-tools18/bin/sqlcmd 
-S localhost -U sa -P "OcP2020123" -C -d fcg_notifications_db 
-Q "SELECT Id, UserEmail, Subject, Type, IsSent FROM Notifications"
