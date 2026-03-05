# 🔔 NotificationsAPI — FIAP Cloud Games (FCG)

API de Notificações do FIAP Cloud Games (FCG) — Envio de e-mails de boas-vindas e confirmações de compra via RabbitMQ.

**Stack**: .NET 8 | ASP.NET Core | RabbitMQ | Clean Architecture

---

## 📁 Estrutura do Projeto

````````
NotificationsAPI.Domain/
├── Entities/
│   └── Notification.cs          # Entidade de notificação
├── Events/
│   ├── UserCreatedEvent.cs      # Evento de usuário criado
│   └── PaymentProcessedEvent.cs # Evento de pagamento
└── Interfaces/
    ├── IEmailSender.cs          # Contrato para envio de e-mail
    ├── INotificationRepository.cs
    └── IEventHandler.cs         # Contrato para handlers de evento


NotificationsAPI.Application/
├── UseCases/
│   ├── SendWelcomeEmailUseCase.cs
│   └── SendPurchaseConfirmationUseCase.cs
├── EventHandlers/
│   ├── UserCreatedEventHandler.cs   # Consome UserCreatedEvent
│   └── PaymentProcessedEventHandler.cs # Consome PaymentProcessedEvent
├── DTOs/
│   ├── UserCreatedEventDto.cs
│   ├── PaymentProcessedEventDto.cs
│   └── SendEmailDto.cs
├── UseCases/
│   ├── SendWelcomeEmailUseCase.cs
│   └── SendPurchaseConfirmationUseCase.cs
└── EventHandlers/
    ├── UserCreatedEventHandler.cs   # Consome UserCreatedEvent
    └── PaymentProcessedEventHandler.cs # Consome PaymentProcessedEvent├── UseCases/
│   ├── SendWelcomeEmailUseCase.cs
│   └── SendPurchaseConfirmationUseCase.cs
└── EventHandlers/
    ├── UserCreatedEventHandler.cs   # Consome UserCreatedEvent
    └── PaymentProcessedEventHandler.cs # Consome PaymentProcessedEvent



NotificationsAPI.Infrastructure/
├── RabbitMQ/
│   ├── RabbitMQConsumer.cs      # Consumidor genérico
│   ├── RabbitMQPublisher.cs     # Publicador (futuro)
│   └── RabbitMQSettings.cs      # Configurações
├── Email/
│   └── EmailService.cs          # Implementação de IEmailSender
├── Repositories/
│   └── NotificationRepository.cs # Persistência
└── DependencyInjection.cs       # Registros de DI


NotificationsAPI/
├── Program.cs                   # Configuração de inicialização
├── appsettings.json             # Credenciais RabbitMQ
├── Controllers/
│   └── NotificationsController.cs # Endpoints (opcional)
└── HostedServices/
    └── RabbitMQConsumerHostedService.cs # Serviço de fundo

````````

### 🎯 Responsabilidades de Cada Camada

#### **Domain** (Núcleo - Sem Dependências)
- Entidades de negócio
- Interfaces (contratos)
- Exceções customizadas
- Lógica de domínio pura
- **Dependências**: Nenhuma

#### **Application** (Orquestração)
- Use cases (aplicações)
- DTOs (Data Transfer Objects)
- Mappers
- Validações
- Orquestração de fluxos
- **Dependências**: Domain

#### **Infrastructure** (Implementação Técnica)
- Implementação de repositórios
- Acesso a banco de dados (Entity Framework Core)
- Serviços externos
- Configuração de injeção de dependência
- **Dependências**: Domain, Application

#### **API** (Apresentação)
- Controllers
- Configuração do middleware
- Endpoints REST
- Program.cs
- **Dependências**: Application, Infrastructure

### 🔄 Fluxo de Dependência

```mermaid
graph TD;
    A[Domain] <-- B[Application];
    B <-- C[Infrastructure];
    C <-- D[API];
```

**Regra importante**: As dependências devem sempre apontar para o centro, nunca para fora.

## 🛠️ Tecnologias

- **.NET 8.0** - Framework
- **ASP.NET Core** - Web API
- **Nullable Reference Types** - Segurança de tipos
- **Implicit Usings** - Redução de boilerplate

## 🚀 Como Começar

### Pré-requisitos
- .NET 8.0 SDK instalado
- Visual Studio 2026 ou VS Code

### Restaurar Dependências

````````
Producer (Origem)
    ↓
    ├─→ Exchange (Distribuidor)
            ↓
            ├─→ Queue (Fila)
                    ↓
                    Consumer (Consumidor)

````````

┌─────────────────────────────────────────────────────────┐
│                    NotificationsAPI                      │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   RabbitMQ   │  │   RabbitMQ   │  │   RabbitMQ   │  │
│  │  Consumer 1  │  │  Consumer 2  │  │  Consumer 3  │  │
│  │ (UserCreated)│  │(PaymentProc) │  │    (Other)   │  │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  │
│         │                 │                 │           │
│  ┌──────▼─────────────────▼─────────────────▼───────┐  │
│  │              Application Layer                     │  │
│  │  ┌──────────────┐  ┌──────────────────────┐       │  │
│  │  │ UseCases     │  │ EventHandlers        │       │  │
│  │  │ - SendWelcome│  │ - UserCreatedHandler │       │  │
│  │  │ - SendConfirm│  │ - PaymentHandler     │       │  │
│  │  └──────────────┘  └──────────────────────┘       │  │
│  └──────┬──────────────────────────────────────────┬──┘  │
│         │                                          │     │
│  ┌──────▼──────────────┐  ┌────────────────────────▼──┐ │
│  │ Domain Layer        │  │ Infrastructure Layer      │ │
│  │ ┌────────────────┐  │  │ ┌────────────────────┐   │ │
│  │ │ Entities       │  │  │ │ EmailService       │   │ │
│  │ │ - Notification │  │  │ │ - SendAsync()      │   │ │
│  │ │                │  │  │ │ RabbitMQ Client    │   │ │
│  │ │ Interfaces     │  │  │ │ - Subscribe()      │   │ │
│  │ │ - IEmailSender │  │  │ │ - Publish()        │   │ │
│  │ │ - INotificationRep
│  │ └────────────────┘  │  │ └────────────────────┘   │ │
│  └─────────────────────┘  └───────────────────────────┘ │
│                                                           │
└─────────────────────────────────────────────────────────┘

````````

Quando: Um usuário é criado em outro serviço
Evento: UserCreatedEvent
Estrutura:
{
    "userId": "guid",
    "userName": "string",
    "userEmail": "string",
    "createdAt": "datetime"
}

Ação na NotificationsAPI:
✉️ Enviar e-mail de boas-vindas
   - Subject: "Bem-vindo ao nosso serviço!"
   - Body: "Olá {userName}! Suas credenciais foram criadas."
   - Destinatário: {userEmail}

Log no Console:
📝 "Email de boas-vindas enviado para {userEmail}"

Persistência:
💾 Salvar registro de notificação (Notification)



Quando: Um pagamento é processado em outro serviço
Evento: PaymentProcessedEvent
Estrutura:
{
    "paymentId": "guid",
    "userId": "guid",
    "userEmail": "string",
    "amount": "decimal",
    "status": "Approved | Declined",
    "processedAt": "datetime"
}

Ação na NotificationsAPI:
✉️ APENAS se status == "Approved"
   - Subject: "Compra confirmada!"
   - Body: "Olá! Sua compra de R$ {amount} foi confirmada."
   - Destinatário: {userEmail}

✉️ Se status == "Declined" (opcional, futuro)
   - Subject: "Sua compra foi recusada"
   - Body: "Desculpe, sua compra não foi processada."

Log no Console:
📝 "Email de confirmação de compra enviado para {userEmail}"
ou
📝 "Pagamento recusado - sem e-mail enviado"

Persistência:
💾 Salvar registro de notificação (Notification)

````````

Serviço A (User Service)
  ↓
  Usuário criado
  ↓
  Publica: UserCreatedEvent
  ↓
  RabbitMQ (Exchange → Queue)
  ↓
  NotificationsAPI (Consumer)
  ↓
  UserCreatedEventHandler
  ↓
  UseCase: SendWelcomeEmailUseCase
  ↓
  EmailService.SendAsync()
  ↓
  📝 Log no console
  💾 Persistir notificação


Serviço B (Payment Service)
  ↓
  Pagamento processado
  ↓
  Status = "Approved"
  ↓
  Publica: PaymentProcessedEvent
  ↓
  RabbitMQ (Exchange → Queue)
  ↓
  NotificationsAPI (Consumer)
  ↓
  PaymentProcessedEventHandler
  ↓
  Verifica: if (status == "Approved")
  ↓
  UseCase: SendPurchaseConfirmationUseCase
  ↓
  EmailService.SendAsync()
  ↓
  📝 Log no console
  💾 Persistir notificação

````````

---

## 🐳 Como Executar com Docker

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução

### Passo 1 — Subir os containers

Na raiz do projeto, execute:

````````
docker-compose up -d
````````

Isso irá:
- Baixar as imagens necessárias
- Criar e iniciar os containers em modo détaché (background)

Isso irá subir **dois containers**:

| Serviço        | Porta(s)                     | Descrição                           |
|----------------|------------------------------|-------------------------------------|
| `rabbitmq`     | `5672` (AMQP) / `15672` (UI) | Broker de mensageria                |
| `notifications-api` | `8080` (HTTP)           | API de notificações (.NET 8)        |

### Passo 2 — Verificar se o RabbitMQ está pronto

Acesse o painel de gerenciamento do RabbitMQ:

- **URL**: http://localhost:15672
- **Usuário**: `guest`
- **Senha**: `guest`

Aguarde até que as filas `user-created-queue` e `payment-processed-queue` apareçam na aba **Queues** (são criadas automaticamente pelo consumidor da API).

### Passo 3 — Acessar o Swagger

- **URL**: http://localhost:8080/swagger/V1/swagger.json
- **Swagger UI**: http://localhost:8080/swagger/index.html

### Passo 4 — Verificar os logs
docker-compose logs -f notifications-api

Você deve ver:
🚀 Iniciando consumidores RabbitMQ... 🐇 Consumidor iniciado na fila: 'user-created-queue' 🐇 Consumidor iniciado na fila: 'payment-processed-queue' ✅ Todos os consumidores RabbitMQ estão ativos!

### Passo 5 — Parar os containers
docker-compose down
````````

## 📨 Testando com Payloads via RabbitMQ Management UI

### Publicar mensagem na fila `user-created-queue`

1. Acesse http://localhost:15672 → Aba **Queues** → `user-created-queue`
2. Expanda a seção **Publish message**
3. Em **Properties**, adicione: `content_type = application/json`
4. Em **Payload**, cole o JSON abaixo e clique em **Publish message**:

```
{
    "userId": "1",
    "userName": "João Silva",
    "userEmail": "joao.silva@example.com",
    "createdAt": "2023-10-10T10:00:00Z"
}
```

### Publicar mensagem na fila `payment-processed-queue` (Aprovado)

1. Acesse http://localhost:15672 → Aba **Queues** → `payment-processed-queue`
2. Expanda **Publish message**, defina `content_type = application/json`  
3. Cole o payload e publique:
{ "paymentId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890", 
  "userId": "d3b07384-d9a0-4e9b-8a0d-3e3b07384d9a", 
  "userEmail": "lucas.nunes@email.com", 
  "amount": 199.90, 
  "status": "Approved", 
  "processedAt": "2026-03-05T10:05:00Z" 
}