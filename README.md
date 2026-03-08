# ⚡ FlowBoard

> SaaS de gerenciamento de tarefas multi-tenant com ASP.NET Core, Clean Architecture e JWT.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=flat-square&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Railway-4169E1?style=flat-square&logo=postgresql)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat-square&logo=jsonwebtokens)
![Railway](https://img.shields.io/badge/API-Railway-0B0D0E?style=flat-square&logo=railway)
![Vercel](https://img.shields.io/badge/Frontend-Vercel-000000?style=flat-square&logo=vercel)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

## 🌐 Demo ao Vivo

| Serviço | URL |
|---------|-----|
| 🖥️ Frontend | [flow-board-rouge.vercel.app](https://flow-board-rouge.vercel.app) |
| ⚙️ API | [flowboard-production-5220.up.railway.app](https://flowboard-production-5220.up.railway.app) |
| 🩺 Health Check | [/api/health](https://flowboard-production-5220.up.railway.app/api/health) |

---

## 📋 Sobre o Projeto

FlowBoard é uma aplicação SaaS completa de gerenciamento de tarefas no estilo Kanban. Cada empresa possui seu próprio ambiente isolado (multi-tenancy), com controle de acesso por perfil (Admin/Member) e autenticação via JWT.

O projeto foi construído com foco em **arquitetura limpa**, **boas práticas de segurança** e **escalabilidade**, simulando um ambiente de produção real — com deploy completo em nuvem.

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
- ✅ **Deploy em nuvem** — API no Railway, frontend no Vercel, banco PostgreSQL

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

| Camada          | Tecnologia                    |
|-----------------|-------------------------------|
| Backend         | ASP.NET Core 9                |
| ORM             | Entity Framework Core 9       |
| Banco (local)   | SQL Server                    |
| Banco (nuvem)   | PostgreSQL (Railway)          |
| Autenticação    | JWT Bearer + BCrypt           |
| Documentação    | Swagger / OpenAPI             |
| Frontend        | HTML5 + CSS3 + JavaScript     |
| Fontes          | Syne + DM Sans (Google Fonts) |
| Deploy API      | Railway (Docker + .NET 9)     |
| Deploy Frontend | Vercel                        |

---

## ☁️ Deploy

### Infraestrutura

```
GitHub ──push──► Railway (build Docker) ──► API .NET 9
                       │
                       └──► PostgreSQL (Railway internal network)

GitHub ──push──► Vercel ──► Frontend estático
```

### Como funciona o deploy da API

A API é deployada via **Dockerfile** no Railway. A cada push na branch `main`, o Railway reconstrói a imagem automaticamente.

O `Program.cs` detecta automaticamente o banco correto:
- **Localmente**: usa SQL Server via `appsettings.json`
- **Em produção**: recebe a `DATABASE_URL` do Railway e converte para o formato Npgsql automaticamente

As migrations do EF Core rodam automaticamente no startup via `db.Database.Migrate()`, criando todas as tabelas no PostgreSQL sem nenhuma intervenção manual.

### Variáveis de ambiente (Railway)

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__DefaultConnection` | URL de conexão PostgreSQL |
| `Jwt__Secret` | Chave secreta para assinatura JWT |
| `Jwt__Issuer` | Issuer do token JWT |
| `Jwt__Audience` | Audience do token JWT |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish FlowBoard.API/FlowBoard.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
CMD ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet FlowBoard.API.dll
```

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

Copie o arquivo de exemplo:
```bash
cp FlowBoard.API/appsettings.Example.json FlowBoard.API/appsettings.json
```

Edite `FlowBoard.API/appsettings.json` com suas configurações:
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

> ⚠️ O arquivo `appsettings.json` está no `.gitignore` e nunca é commitado.

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

### Health
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/health` | Verifica se a API está no ar |

---

## 🔒 Segurança

- Senhas armazenadas com **BCrypt** (nunca em texto puro)
- JWT com expiração de **7 dias**
- `appsettings.json` no `.gitignore` — segredos nunca vão para o repositório
- `appsettings.Example.json` com valores de placeholder para referência
- TenantId sempre extraído do token JWT — nunca aceito do body da requisição
- CORS configurado para aceitar qualquer origem (adequado para demo)


<p align="center">Desenvolvido por <a href="https://github.com/rbarboteu">rbarboteu</a></p>
