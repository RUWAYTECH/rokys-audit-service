# Rokys Audit Service

Sistema de auditoría para gestión de roles, empresas y configuraciones de auditoría.

## Requisitos Previos

- .NET SDK 8.0 o superior
- SQL Server 16.0 o superior
- Java 21.0.4 (para Liquibase)
- Liquibase 4.29.2

## Configuración Inicial

### 1. Base de Datos

El proyecto utiliza Liquibase para la gestión de migraciones de base de datos.

#### Configurar Liquibase

1. Navegar al directorio de base de datos:
```bash
cd Rokys.Audit.DataBase
```

2. Configurar el archivo `liquibase.properties` con la cadena de conexión a tu base de datos.

3. Ejecutar las migraciones:
```bash
liquibase update
```

### 2. Configuración de la Aplicación

Actualizar las cadenas de conexión y configuraciones en `appsettings.json` o `appsettings.Development.json` según el entorno.

## Levantar el Proyecto

### Opción 1: Desde Visual Studio / Visual Studio Code

1. Abrir la solución `Rokys.Audit.Services.sln`
2. Establecer `Rokys.Audit.WebAPI` como proyecto de inicio
3. Presionar F5 o hacer clic en "Run"

### Opción 2: Desde la Terminal

```bash
# Navegar al directorio del proyecto API
cd Rokys.Audit.WebAPI

# Restaurar dependencias
dotnet restore

# Ejecutar el proyecto
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Compilar para Publicación

### Compilación para Producción

```bash
# Desde la raíz del proyecto
dotnet build --configuration Release

# O compilar todo el solution
dotnet build Rokys.Audit.Services.sln --configuration Release
```

### Publicar la Aplicación

```bash
# Publicar el proyecto API
cd Rokys.Audit.WebAPI
dotnet publish --configuration Release --output ./publish

# Los archivos publicados estarán en ./publish
```

### Publicar con configuración específica

```bash
# Publicar para un runtime específico (ejemplo: Linux x64)
dotnet publish --configuration Release --runtime linux-x64 --self-contained false --output ./publish

# Publicar self-contained (incluye el runtime de .NET)
dotnet publish --configuration Release --runtime linux-x64 --self-contained true --output ./publish
```

## Estructura del Proyecto

- `Rokys.Audit.WebAPI` - API principal
- `Rokys.Audit.Services` - Lógica de negocio
- `Rokys.Audit.Model` - Modelos de datos
- `Rokys.Audit.DTOs` - Objetos de transferencia de datos
- `Rokys.Audit.Infrastructure` - Repositorios e interfaces
- `Rokys.Audit.DataBase` - Scripts de base de datos y migraciones Liquibase

## Documentación API

Una vez levantado el proyecto, acceder a Swagger UI en:
```
https://localhost:5001/swagger
```

## Notas Adicionales

- Asegurarse de que SQL Server esté ejecutándose antes de levantar el proyecto
- Verificar que las migraciones de Liquibase se hayan ejecutado correctamente
- Revisar los logs en caso de errores de inicio