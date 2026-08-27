namespace ProyectoNANDDOS;

// Gestor centralizado de seguridad y permisos del sistema NANDDOS.
// Evalua si el usuario autenticado tiene acceso a una funcionalidad especifica.
public static class GestorSeguridad
{
    // Verifica si el usuario actual tiene un permiso especifico.
    // Si el usuario es SuperAdministrador, SIEMPRE retorna true (acceso total).
    // Si no, busca el nombre_interno del permiso en la lista cargada en SesionActual.
    public static bool TienePermiso(string nombrePermiso)
    {
        // El SuperAdministrador tiene acceso irrestricto a todo el sistema.
        if (SesionActual.EsSuperAdministrador)
        {
            return true;
        }

        // Verifica si el permiso existe en la lista cargada desde la base de datos.
        return SesionActual.Permisos.Contains(nombrePermiso);
    }
}
