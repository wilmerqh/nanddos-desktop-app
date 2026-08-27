namespace ProyectoNANDDOS;

// Modelo que representa un registro de la tabla 'cargos'.
public class Cargo
{
    public int IdCargo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Protegido { get; set; }
}

// Modelo que representa un registro de la tabla 'permisos'.
public class Permiso
{
    public int IdPermiso { get; set; }
    public string NombreInterno { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
}
