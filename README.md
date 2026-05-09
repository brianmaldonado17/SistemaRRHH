Sistema de Gestión de Recursos Humanos (SistemaRRHH)

Este sistema integral, es un proyecto universitario, para la gestión de talento humano, control de nóminas y generación de prestaciones laborales. El proyecto fue desarrollado bajo una arquitectura robusta y segura, pensado para servir como base escalable.

/*****************************************************************************************************************************************************************/
/*****************************************************************************************************************************************************************/

*Características Principales
**Módulo de Seguridad: Autenticación basada en cookies con soporte para múltiples roles (Administrador, RRHH, Colaborador).

**Gestión de Personal: Altas, bajas y reactivación de colaboradores con control de historial.

**Motor Financiero: Cálculo automatizado de nóminas mensuales y prestaciones (Bono 14, Aguinaldo, Indemnización).

**Portal del Colaborador: Dashboard personal con indicadores de antigüedad y descarga de boletas de pago en PDF.

**Seguridad: Implementación de variables de entorno y Secretos de Usuario para proteger credenciales de base de datos y servicios de correo.

/*****************************************************************************************************************************************************************/
/*****************************************************************************************************************************************************************/

*Tecnologías Utilizadas
**Backend: ASP.NET Core 8.0 (MVC)

**Lenguaje: C#

**Acceso a Datos: Dapper (Micro-ORM para alto rendimiento)

**Base de Datos: MySQL Server 9.4

**Frontend: Razor Views, Bootstrap 5 y Bootstrap Icons

**Servicios: SMTP para notificaciones por correo electrónico

/*****************************************************************************************************************************************************************/
/*****************************************************************************************************************************************************************/

*Configuración Inicial
Para ejecutar este proyecto en un entorno local, sigue estos pasos:

**Base de Datos:
Ejecuta el script Script_SIRRHH_Final.sql incluido en la raíz para generar el esquema y los datos base.

**Secretos de Usuario:
Para evitar exponer credenciales, el sistema utiliza User Secrets. Configura tu cadena de conexión y las claves de correo en tu archivo secrets.json local:

JSON
{
  "ConnectionStrings": {
    "ConexionMySql": "Server=localhost;Database=SIRRHH;Uid=tu_usuario;Pwd=tu_clave;"
  },
  "ConfiguracionEmail": {
    "PasswordApp": "tu_clave_de_aplicacion_gmail"
  }
}

**appsettings.json
Actualiza la ConfiguracionEmail con los datos correspondientes al correo que se usará para el servicio SMTP.

**Compilación:
Abre la solución SistemaRRHH.sln en Visual Studio y restaura los paquetes NuGet.

/*****************************************************************************************************************************************************************/
/*****************************************************************************************************************************************************************/

*Estructura del Proyecto (MVC)
**Controllers: Lógica de navegación y procesamiento de solicitudes.

**Models: Definición de objetos de negocio y ViewModels.

**Datos: Clase de conexión y acceso a la base de datos.

**Views: Interfaz de usuario estructurada por módulos.
