# GoldenGems Backend API

API REST desarrollada con ASP.NET Core para el proyecto GoldenGems.

## 🚀 Tecnologías

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (PostgreSQL)
- Swagger/OpenAPI

## 📁 Estructura del Proyecto

```
GoldenGemsBackEnd/
├── Controllers/       # Controladores de la API
├── Models/           # Modelos de datos
├── Services/         # Lógica de negocio
├── Repositories/     # Acceso a datos
├── DTOs/            # Data Transfer Objects
├── Data/            # Contexto de base de datos
├── Middleware/      # Middleware personalizado
└── Program.cs       # Punto de entrada de la aplicación
```

## ✅ Requisitos

- .NET SDK 10.0
- PostgreSQL 13+ (o compatible)
- Visual Studio Code o Visual Studio 2022

## 🛠️ Instalación y Configuración

1. Entra a la carpeta del proyecto:
    - GoldenGemsBackEnd

2. Restaura dependencias:
    - dotnet restore

3. Configura la conexión a base de datos en [GoldenGemsBackEnd/appsettings.json](GoldenGemsBackEnd/appsettings.json).

4. Aplica migraciones:
    - dotnet ef database update

5. Ejecuta la API:
    - dotnet run

## 🌐 URLs locales

- HTTPS: https://localhost:7XXX
- HTTP: http://localhost:5XXX
- Swagger UI (dev): raíz del sitio

## 🗃️ Base de datos y migraciones

- Crear migración:
  - dotnet ef migrations add NombreMigracion
- Aplicar migraciones:
  - dotnet ef database update

## 🔧 Configuración

### CORS
La política actual permite cualquier origen, método y header en desarrollo. Ajusta la política en [GoldenGemsBackEnd/Program.cs](GoldenGemsBackEnd/Program.cs) para producción.

### Swagger
En desarrollo, Swagger UI se sirve en la raíz de la aplicación.

### Autenticación JWT
1. Configura valores por defecto (Issuer, Audience, expiración) en [GoldenGemsBackEnd/appsettings.json](GoldenGemsBackEnd/appsettings.json) pero deja `SecretKey` vacío.
2. (Opcional) Crea un archivo `appsettings.Local.json` copiando el template [GoldenGemsBackEnd/appsettings.Local.json.example](GoldenGemsBackEnd/appsettings.Local.json.example); este archivo ya está ignorado por git y se puede usar para secretos locales.
3. Expón los secretos reales mediante variables de entorno o `dotnet user-secrets` usando los nombres:
  - `JWT_SECRET`
  - `JWT_ISSUER`
  - `JWT_AUDIENCE`
  - `JWT_ACCESS_TOKEN_MINUTES`
4. Si una variable existe, sobrescribe al valor del archivo; de lo contrario la app lanza una excepción para evitar arrancar sin clave.
5. Los servicios `AuthService` y `JwtTokenService` se registran en [GoldenGemsBackEnd/Program.cs](GoldenGemsBackEnd/Program.cs) junto a la configuración de `JwtBearer`, por lo que cualquier controlador decorado con `[Authorize]` validará automáticamente el token.
6. La clase `ApiResponse<T>` estandariza las respuestas para login y registro.

### Secrets
No subas credenciales. Usa variables de entorno o User Secrets para datos sensibles y deja valores de ejemplo en [GoldenGemsBackEnd/appsettings.json](GoldenGemsBackEnd/appsettings.json).

## 🧪 Endpoints disponibles

### Health Check
- GET /api/health

### Autenticación
- POST /api/auth/register → Registra un usuario y devuelve `AuthResponseDto` + JWT.
- POST /api/auth/login → Valida credenciales (email o username) y emite un nuevo JWT.

Ambos endpoints aceptan/retornan el envoltorio `ApiResponse<T>` y están implementados en [GoldenGemsBackEnd/Controllers/AuthController.cs](GoldenGemsBackEnd/Controllers/AuthController.cs). La lógica de negocio reside en [GoldenGemsBackEnd/Services/Auth/AuthService.cs](GoldenGemsBackEnd/Services/Auth/AuthService.cs) y el token se genera mediante [GoldenGemsBackEnd/Services/Auth/JwtTokenService.cs](GoldenGemsBackEnd/Services/Auth/JwtTokenService.cs).

## 📦 Comandos útiles

- dotnet build
- dotnet test
- dotnet publish -c Release
- dotnet add package NombreDelPaquete

## 📄 Licencia

Proyecto privado - GoldenGems © 2026
# GgBackEnd
