## 🚀 Como Executar

### 1. Pré-requisitos
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### 2. Clone e Configure
```bash
git clone [seu-repositorio]
cd DeveloperStore.Sales
```

### 3. Restaurar Dependências
```bash
dotnet restore
```

### 4. Executar a API
```bash
cd src/DeveloperStore.Sales.Api
dotnet run
```

### 5. Acessar a Documentação
- **Swagger UI**: http://localhost:5000
- **API Health Check**: http://localhost:5000/health

## 📊 Endpoints da API

### 🛒 Vendas
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/sales` | Criar nova venda |
| `GET` | `/api/sales` | Listar vendas (com filtros) |
| `GET` | `/api/sales/{id}` | Buscar venda por ID |
| `GET` | `/api/sales/by-number/{number}` | Buscar por número |
| `PUT` | `/api/sales/{id}` | Atualizar venda |
| `DELETE` | `/api/sales/{id}` | Cancelar venda |

### 📦 Itens da Venda
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/sales/{saleId}/items` | Adicionar item |
| `PUT` | `/api/sales/{saleId}/items/{itemId}` | Atualizar item |
| `DELETE` | `/api/sales/{saleId}/items/{itemId}` | Cancelar item |

## 📝 Exemplo de Uso

### Criar Nova Venda
```json
POST /api/sales
{
  "saleNumber": "SALE-2024-001",
  "saleDate": "2024-01-15T10:30:00Z",
  "customer": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "João Silva",
    "email": "joao@email.com",
    "document": "12345678901"
  },
  "branch": {
    "id": "987fcdeb-51a2-43d1-9c45-123456789abc",
    "name": "Loja Centro",
    "address": "Rua Principal, 123",
    "city": "São Paulo",
    "state": "SP"
  },
  "items": [
    {
      "product": {
        "id": "456e7890-e89b-12d3-a456-426614174001",
        "name": "Notebook Dell",
        "description": "Notebook Dell Inspiron 15",
        "category": "Eletrônicos",
        "sku": "DELL-NB-001"
      },
      "quantity": 5,
      "unitPrice": 2500.00
    }
  ]
}
```

### Resposta
```json
{
  "id": "789abc12-3def-4567-8901-234567890abc",
  "saleNumber": "SALE-2024-001",
  "saleDate": "2024-01-15T10:30:00Z",
  "customer": { ... },
  "branch": { ... },
  "totalAmount": 11250.00,
  "isCancelled": false,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null,
  "items": [
    {
      "id": "item-123",
      "product": { ... },
      "quantity": 5,
      "unitPrice": 2500.00,
      "discountPercentage": 0.10,
      "discountAmount": 1250.00,
      "subTotal": 12500.00,
      "totalAmount": 11250.00,
      "isCancelled": false
    }
  ]
}
```

## 🧪 Executar Testes

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test tests/DeveloperStore.Sales.Domain.Tests/

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Configurações

### Logging
O projeto usa **Serilog** para logging estruturado:
- Console output para desenvolvimento
- Configurável via `appsettings.json`

### Validação
Utiliza **FluentValidation** para validação robusta:
- Validação automática nos controllers
- Mensagens de erro amigáveis
- Validação customizada por DTO

### Eventos
Sistema de eventos integrado:
- Dispatch automático após operações
- Handlers específicos por tipo de evento
- Logging detalhado de eventos

## 💡 Diferenciais Implementados

### ✅ Arquitetura DDD Completa
- Separação clara de responsabilidades
- Domain Events para comunicação
- Padrão External Identities
- Rich Domain Models

### ✅ Sistema de Eventos Robusto
- Publisher de eventos configurável
- Handlers específicos por evento
- Logging estruturado de eventos
- Preparado para Message Brokers

### ✅ Validações Avançadas
- FluentValidation integrado
- Validações de negócio no domínio
- Mensagens de erro detalhadas

### ✅ Documentação Swagger Completa
- API documentada automaticamente
- Exemplos de request/response
- Metadados detalhados

### ✅ Testes Unitários
- Cobertura das regras de negócio
- Testes de validação
- Assertions fluentes com FluentAssertions

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - API REST
- **Entity Framework Core** - ORM (preparado)
- **FluentValidation** - Validação
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação
- **xUnit** - Testes unitários
- **FluentAssertions** - Assertions para testes

## 📚 Próximos Passos

### Para Produção
1. **Banco de Dados**: Configurar SQL Server ou PostgreSQL
2. **Message Broker**: Integrar RabbitMQ ou Azure Service Bus
3. **Autenticação**: Implementar JWT/OAuth2
4. **Cache**: Adicionar Redis para performance
5. **Monitoramento**: Application Insights ou similar

### Melhorias
1. **CQRS**: Separar comandos e consultas
2. **Event Sourcing**: Histórico completo de eventos
3. **API Versioning**: Versionamento da API
4. **Rate Limiting**: Controle de taxa de requisições

## 👥 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Reques
