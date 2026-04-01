# GoldenGems Backend API

API REST desarrollada con ASP.NET Core siguiendo **Clean Architecture** para la plataforma GoldenGems de comercio de joyería.

## Tecnologias

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (PostgreSQL)
- JWT Authentication
- Swagger/OpenAPI
- xUnit + Moq + FluentAssertions (Testing)

## Arquitectura

El proyecto sigue **Clean Architecture** con separacion estricta de capas:

```
src/
├── GoldenGems.Domain/            # Entidades, Value Objects, Interfaces de repositorio
│   ├── Entities/
│   │   ├── Security/             # User, Role, UserRole, Module, Form, Action, RoleAction
│   │   ├── People/               # Person, Contact, DocumentType, Region
│   │   ├── Business/             # Company, Product, ProductImage, ProductType, UserPreference
│   │   ├── Chat/                 # Conversation, Message
│   │   └── Payment/              # Commission
│   └── Interfaces/               # Contratos de repositorios (IRepository<T>, IUserRepository, etc.)
│
├── GoldenGems.Application/       # Logica de negocio, DTOs, Services
│   ├── Common/                   # ApiResponse<T>, BaseService
│   ├── DTOs/                     # Data Transfer Objects por dominio
│   ├── Interfaces/               # Contratos de servicios
│   ├── Models/                   # TokenResult, PaginationParams, etc.
│   ├── Services/                 # Implementaciones de servicios
│   │   ├── Auth/                 # AuthService, UserValidationService
│   │   ├── Admin/                # RoleService, ModuleService, FormService, ActionService, etc.
│   │   ├── Business/             # CompanyService, ProductService, ProductTypeService, etc.
│   │   ├── Chat/                 # ChatService
│   │   ├── Payment/              # CommissionService
│   │   └── People/               # PersonService, ContactService
│   └── Validators/               # Validadores
│
├── GoldenGems.Infrastructure/    # EF Core, Repositorios, Autenticacion
│   ├── Authentication/           # JwtTokenService, PasswordHasherService
│   ├── Data/                     # DbContext, Configurations (Fluent API)
│   ├── Migrations/               # Migraciones de EF Core
│   └── Repositories/             # Implementaciones de repositorios
│
└── GoldenGems.API/               # Controllers, Middleware, Program.cs
    ├── Controllers/              # Endpoints REST
    └── Middleware/                # GlobalExceptionMiddleware

tests/
└── GoldenGems.Application.Tests/ # Pruebas unitarias
    └── Services/
        ├── Auth/                 # AuthServiceTests (17 tests)
        ├── Admin/                # RoleServiceTests (7 tests)
        └── Business/             # CompanyServiceTests (7 tests)
```

## Requisitos

- .NET SDK 10.0
- PostgreSQL 13+
- Visual Studio Code o Visual Studio 2022+

## Instalacion y Configuracion

1. Clona el repositorio y entra a la carpeta:

```bash
cd GgBackEnd
```

2. Restaura dependencias:

```bash
dotnet restore
```

3. Configura la conexion a base de datos en `src/GoldenGems.API/appsettings.json`.

4. Aplica migraciones:

```bash
dotnet ef database update --project src/GoldenGems.Infrastructure --startup-project src/GoldenGems.API
```

5. Ejecuta la API:

```bash
dotnet run --project src/GoldenGems.API
```

## URLs locales

- HTTP: http://localhost:5233
- Swagger UI: http://localhost:5233 (en desarrollo)

## Base de datos y migraciones

```bash
# Crear migracion
dotnet ef migrations add NombreMigracion --project src/GoldenGems.Infrastructure --startup-project src/GoldenGems.API

# Aplicar migraciones
dotnet ef database update --project src/GoldenGems.Infrastructure --startup-project src/GoldenGems.API
```

## Configuracion

### CORS

La politica actual permite cualquier origen, metodo y header en desarrollo. Ajustar para produccion en `src/GoldenGems.API/Program.cs`.

### Autenticacion JWT

1. Configura valores por defecto (Issuer, Audience, expiracion) en `src/GoldenGems.API/appsettings.json`.
2. Expone los secretos mediante variables de entorno o `dotnet user-secrets`:
   - `JWT_SECRET`
   - `JWT_ISSUER`
   - `JWT_AUDIENCE`
   - `JWT_ACCESS_TOKEN_MINUTES`
3. Los controladores decorados con `[Authorize]` validan automaticamente el token.

### Secrets

No subas credenciales al repositorio. Usa variables de entorno o User Secrets para datos sensibles.

## Endpoints disponibles

### Health Check
| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/health` | Verifica estado de la API |

### Autenticacion (`/api/auth`)
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| POST | `/api/auth/register` | Publico | Registro de usuario |
| POST | `/api/auth/login` | Publico | Inicio de sesion (email o username) |
| POST | `/api/auth/create` | Admin | Creacion de usuario por admin |

### Administracion
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| CRUD | `/api/roles` | Admin | Gestion de roles |
| CRUD | `/api/modules` | Admin | Gestion de modulos |
| CRUD | `/api/forms` | Admin | Gestion de formularios |
| CRUD | `/api/actions` | Admin | Gestion de acciones |
| CRUD | `/api/document-types` | Admin | Tipos de documento |
| GET | `/api/regions` | Auth | Regiones y departamentos |

### Negocio
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| CRUD | `/api/companies` | Auth | Gestion de empresas |
| CRUD | `/api/products` | Auth | Gestion de productos |
| CRUD | `/api/product-types` | Auth | Tipos de producto |
| CRUD | `/api/product-images` | Auth | Imagenes de productos |
| GET | `/api/catalog` | Publico | Catalogo con filtros, orden y paginacion |
| CRUD | `/api/preferences` | Auth | Preferencias de usuario |

### Chat y Negociacion
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| POST | `/api/chat/start` | Auth | Iniciar conversacion |
| GET | `/api/chat/my` | Auth | Mis conversaciones |
| POST | `/api/chat/{id}/message` | Auth | Enviar mensaje |
| POST | `/api/chat/{id}/offer` | Auth | Oferta de precio |
| POST | `/api/chat/{id}/accept` | Auth | Aceptar precio |
| POST | `/api/chat/{id}/reject` | Auth | Rechazar precio |
| POST | `/api/chat/{id}/close` | Auth | Cerrar conversacion |

### Personas
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| CRUD | `/api/persons` | Auth | Gestion de personas |
| CRUD | `/api/contacts` | Auth | Informacion de contacto |

### Pagos
| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| CRUD | `/api/commissions` | Admin | Gestion de comisiones |

## Pruebas Unitarias

El proyecto incluye **31 pruebas unitarias** usando xUnit, Moq y FluentAssertions.

```bash
# Ejecutar todas las pruebas
dotnet test tests/GoldenGems.Application.Tests

# Ejecutar con detalle
dotnet test tests/GoldenGems.Application.Tests --verbosity normal
```

### Cobertura actual

| Servicio | Tests | Escenarios cubiertos |
|----------|-------|---------------------|
| AuthService | 17 | Registro, Login, Creacion por admin, validaciones de unicidad, rehash de password |
| RoleService | 7 | CRUD de roles, validaciones de nombre, duplicados |
| CompanyService | 7 | Registro de empresas, validacion NIT/nombre, CRUD, soft delete |

## Comandos utiles

```bash
dotnet build                                    # Compilar
dotnet run --project src/GoldenGems.API         # Ejecutar
dotnet test                                      # Ejecutar tests
dotnet publish -c Release                        # Publicar
dotnet add package NombreDelPaquete              # Agregar paquete
```

## Licencia

Proyecto privado - GoldenGems © 2026
