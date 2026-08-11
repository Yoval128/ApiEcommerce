# .NET Core, API RESTful, SQL Server, JWT, Mapster, Azure y desarrollo backend profesional con C#

## Descripción

Este proyecto consiste en el desarrollo de una **API REST profesional utilizando .NET 8 y C#**, con el objetivo de aplicar buenas prácticas de desarrollo backend y construir una aplicación robusta, segura, mantenible y escalable. El proyecto se desarrolla como parte de un proceso de aprendizaje práctico, simulando un entorno de trabajo real.

A lo largo del desarrollo se implementarán funcionalidades comunes en aplicaciones empresariales, como la gestión de entidades, operaciones CRUD, persistencia de datos con **SQL Server** y **Entity Framework Core**, autenticación mediante **JWT**, autorización basada en roles, versionado de la API, paginación, carga de archivos y otras características orientadas a construir un backend moderno.

Para mejorar la separación entre las entidades de persistencia y los objetos utilizados por la API, se utilizará **Mapster** para realizar el mapeo entre entidades y DTOs, evitando conversiones manuales innecesarias y favoreciendo un código más limpio y mantenible.

El proyecto también seguirá principios de **arquitectura limpia y organización modular del código**, utilizando patrones como el **Repositorio** para mantener una estructura mantenible y escalable.

La API contará con documentación interactiva mediante **Swagger/OpenAPI** y se realizarán pruebas de los endpoints utilizando **Postman**, incluyendo colecciones organizadas para facilitar su consumo y validación.

Finalmente, la aplicación será preparada para un entorno de producción utilizando **Microsoft Azure**, integrando servicios como **Azure App Service** para el despliegue de la API y **Azure SQL Database** para la persistencia de datos. De esta manera, el proyecto cubre el ciclo completo desde el desarrollo local hasta la publicación de un backend accesible desde Internet.

## Objetivos

* Desarrollar una API REST profesional con .NET 8 y C#.
* Implementar operaciones CRUD para las diferentes entidades del sistema.
* Integrar SQL Server mediante Entity Framework Core.
* Utilizar **Mapster** para realizar el mapeo entre entidades y DTOs.
* Aplicar autenticación mediante JSON Web Tokens (JWT).
* Implementar autorización basada en roles.
* Implementar validaciones, manejo de errores y respuestas estructuradas.
* Incorporar versionado de la API.
* Implementar paginación.
* Incorporar carga y gestión de archivos.
* Documentar la API mediante Swagger/OpenAPI.
* Crear y mantener colecciones de Postman para probar y consumir los endpoints.
* Aplicar patrones y buenas prácticas de arquitectura backend.
* Separar responsabilidades para facilitar el mantenimiento y evolución del proyecto.
* Preparar la aplicación para un entorno de producción.
* Desplegar la API mediante **Azure App Service**.
* Utilizar **Azure SQL Database** como base de datos en la nube.
* Configurar la comunicación entre la API desplegada y la base de datos de Azure.
* Aplicar buenas prácticas de configuración y seguridad para un entorno productivo.

## Tecnologías

### Backend

* .NET 8
* ASP.NET Core Web API
* C#
* Entity Framework Core
* SQL Server
* Mapster

### Seguridad

* JWT (JSON Web Tokens)
* Autenticación
* Autorización basada en roles
* CORS
* HTTPS

### Arquitectura y desarrollo

* Arquitectura limpia
* Patrón Repository
* DTOs
* Validaciones
* Manejo de excepciones
* Versionado de API
* Paginación

### Documentación y pruebas

* Swagger / OpenAPI
* Postman
* Colecciones para pruebas y consumo de la API

### Cloud y despliegue

* Microsoft Azure
* Azure App Service
* Azure SQL Database
* Azure Portal
* Configuración mediante Application Settings
* Firewall y conectividad entre servicios de Azure

### Control de versiones

* Git
* GitHub

## Resultado esperado

Al finalizar el proyecto se contará con una **API REST desarrollada con .NET 8**, estructurada siguiendo buenas prácticas de desarrollo backend, con persistencia mediante Entity Framework Core, autenticación y autorización mediante JWT, transformación de datos mediante Mapster, documentación mediante Swagger/OpenAPI y pruebas mediante Postman.

Además, la aplicación estará preparada para ejecutarse en un entorno de producción utilizando **Microsoft Azure**, con la API alojada en **Azure App Service** y la información persistida en **Azure SQL Database**.

El proyecto permitirá comprender no solo el desarrollo de una API, sino también el proceso completo de construcción, configuración, pruebas y despliegue de un backend moderno desarrollado con C# y .NET.
