namespace ProyectoNANDDOS;

// Modelo que representa un registro de la tabla 'usuarios' con datos cruzados de 'cargos'.
public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    
    // Equivale al campo 'usuario' en la base de datos (username de login).
    public string Username { get; set; } = string.Empty;
    
    // Propiedad transitoria para enviar la contrasena en texto plano al DAO para ser hasheada.
    // No se puebla al consultar usuarios por seguridad.
    public string Password { get; set; } = string.Empty;
    
    public int IdCargo { get; set; }
    
    // Propiedad calculada proveniente del JOIN con la tabla cargos.
    public string NombreCargo { get; set; } = string.Empty;
    
    public bool EsSuperAdmin { get; set; }
    
    public bool Activo { get; set; } = true;
}
