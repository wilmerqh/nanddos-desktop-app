namespace ProyectoNANDDOS;

// Almacena los datos del usuario autenticado durante toda la ejecucion de la aplicacion.
// Esta clase funciona como un "contexto de sesion" en memoria.
public static class SesionActual
{
    // Datos basicos del usuario autenticado.
    public static int IdUsuario { get; set; }
    public static string NombreCompleto { get; set; } = string.Empty;
    public static string Username { get; set; } = string.Empty;
    public static int IdCargo { get; set; }
    public static bool EsSuperAdministrador { get; set; }

    // Lista de nombres internos de los permisos asignados al usuario.
    // Se carga al iniciar sesion desde los permisos del cargo y los permisos individuales.
    public static List<string> Permisos { get; set; } = new();

    // Reinicia todos los valores de la sesion a su estado por defecto.
    // Debe llamarse al cerrar sesion o al salir de la aplicacion.
    public static void LimpiarSesion()
    {
        IdUsuario = 0;
        NombreCompleto = string.Empty;
        Username = string.Empty;
        IdCargo = 0;
        EsSuperAdministrador = false;
        Permisos = new List<string>();
    }
}
