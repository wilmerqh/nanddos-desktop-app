# ==============================================================================
# SISTEMA DE ESCRITORIO: ControlLabITERN (Versión Completa con Seguridad OWASP)
# Ejecuta el Inicio de Sesión con Bcrypt (OWASP A02) y el Dashboard de 3 Módulos
# (Equipos, Áreas e Inventario) con Sentencias Preparadas (OWASP A03)
# ==============================================================================

# Importamos el módulo login_vista que inicia el flujo completo de autenticación segura
from login_vista import iniciar_login  # Función que abre la ventana de Login con validación Bcrypt

# Si ejecutamos este archivo directamente, arrancamos el sistema desde la ventana de Login
if __name__ == "__main__":  # Comprobamos ejecución principal
    # Iniciamos la aplicación abriendo el Login seguro
    iniciar_login()  # Abre Login -> Valida con bcrypt -> Abre Dashboard (Equipos, Áreas, Inventario)
