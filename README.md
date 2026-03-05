# ⚡ FlowBoard

> SaaS de gerenciamento de tarefas multi-tenant com ASP.NET Core, Clean Architecture e JWT.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=flat-square&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat-square&logo=jsonwebtokens)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

## 📋 Sobre o Projeto

FlowBoard é uma aplicação SaaS completa de gerenciamento de tarefas no estilo Kanban. Cada empresa possui seu próprio ambiente isolado (multi-tenancy), com controle de acesso por perfil (Admin/Member) e autenticação via JWT.

O projeto foi construído com foco em **arquitetura limpa**, **boas práticas de segurança** e **escalabilidade**, simulando um ambiente de produção real.

---

## 🚀 Funcionalidades

- ✅ **Multi-tenancy** — isolamento completo de dados por empresa
- ✅ **Autenticação JWT** — stateless, com claims de tenant e role
- ✅ **Kanban** — tarefas organizadas em A Fazer / Em Progresso / Concluído
- ✅ **CRUD completo** — projetos e tarefas com criação, edição e exclusão
- ✅ **Gestão de usuários** — Admin pode cadastrar novos membros
- ✅ **Registro de empresa** — onboarding completo com slug único
- ✅ **BCrypt** — senhas sempre armazenadas com hash seguro
- ✅ **Frontend responsivo** — HTML/CSS/JS puro, sem dependências

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, com dependências apontando sempre para o núcleo:

```
FlowBoard/
├── FlowBoard.Domain/          # Entidades e enums (sem dependências externas)
│   ├── Entities/
│   │   ├── Tenant.cs
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   └── TaskItem.cs
│   └── Enums/
│       ├── UserRole.cs
│       └── TaskItemStatus.cs
│
├── FlowBoard.Application/     # Casos de uso, DTOs e interfaces
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│
├── FlowBoard.Infrastructure/  # EF Core, repositórios, banco de dados
│   ├── Data/
│   ├── Migrations/
│   └── Repositories/
│
├── FlowBoard.API/             # Controllers, JWT, middleware, DI
│   ├── Controllers/
│   ├── Middleware/
│   ├── Services/
│   └── Program.cs
│
└── frontend/                  # Interface web (HTML/CSS/JS)
    ├── index.html
    ├── dashboard.html
    ├── register.html
    └── app.js
```

---

## 🛢️ Modelo de Dados

```
Tenants          Users              Projects         Tasks
─────────        ──────────         ─────────        ──────────
Id (PK)    ←─── TenantId (FK)  ←── TenantId (FK)    TenantId (FK)
Slug (UQ)        Id (PK)            Id (PK)     ←─── ProjectId (FK)
Name             Email (UQ*)        Name             Id (PK)
                 PasswordHash       Description      Title
                 Role (enum)                         Status (enum)

* Unique por (TenantId, Email)
```

---

## 🔐 Fluxo de Autenticação

```
1. Usuário informa: slug da empresa + email + senha
2. API resolve o Tenant pelo Slug
3. Busca o User filtrando por Email + TenantId
4. BCrypt.Verify(senha, hash) → valida senha
5. Gera JWT com claims: { userId, tenantId, role }
6. Todas as requisições seguintes usam o token
7. TenantId extraído do token — nunca do body
```

---

## 🧰 Tecnologias

| Camada         | Tecnologia                    |
|----------------|-------------------------------|
| Backend        | ASP.NET Core 9                |
| ORM            | Entity Framework Core 9       |
| Banco de Dados | SQL Server                    |
| Autenticação   | JWT Bearer + BCrypt           |
| Documentação   | Swagger / OpenAPI             |
| Frontend       | HTML5 + CSS3 + JavaScript     |
| Fontes         | Syne + DM Sans (Google Fonts) |

---

## ⚙️ Como Rodar Localmente

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (local ou Express)

### 1. Clone o repositório
```bash
git clone https://github.com/rbarboteu/FlowBoard.git
cd FlowBoard
```

### 2. Configure a connection string

Edite `FlowBoard.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FlowBoardDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "sua-chave-secreta-com-32-caracteres-minimo",
    "Issuer": "FlowBoard",
    "Audience": "FlowBoard"
  }
}
```

### 3. Execute as migrations
```bash
dotnet ef database update --project FlowBoard.Infrastructure --startup-project FlowBoard.API
```

### 4. Suba a API
```bash
dotnet run --project FlowBoard.API
```

### 5. Abra o frontend

Abra `frontend/index.html` no navegador ou acesse `http://localhost:5291/swagger` para explorar a API.

---

## 📡 Endpoints da API

### Auth
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/login` | Login com slug + email + senha |
| POST | `/api/auth/register-tenant` | Cadastro de nova empresa |
| POST | `/api/auth/register-user` | Cadastro de usuário (Admin) |

### Projects
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/projects` | Lista projetos do tenant |
| POST | `/api/projects` | Cria novo projeto |
| PUT | `/api/projects/{id}` | Edita projeto |
| DELETE | `/api/projects/{id}` | Remove projeto |

### Tasks
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tasks/project/{id}` | Lista tarefas do projeto |
| POST | `/api/tasks` | Cria nova tarefa |
| PATCH | `/api/tasks/{id}/status` | Atualiza status |
| DELETE | `/api/tasks/{id}` | Remove tarefa |

### Users
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/users` | Lista usuários do tenant |

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<p align="center">Desenvolvido por <a href="https://github.com/rbarboteu">rbarboteu</a></p>
