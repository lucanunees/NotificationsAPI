# NotificationsAPI

API para gerenciamento e envio de notificações com arquitetura limpa em camadas.

## 🏗️ Arquitetura

O projeto segue o padrão de **Arquitetura Limpa em Camadas**, garantindo separação de responsabilidades, testabilidade e manutenibilidade.

### 📂 Estrutura de Diretórios

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
