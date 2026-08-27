using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Capa de acceso a datos para la tabla 'usuarios'.
public static class UsuarioDAO
{
    // Retorna todos los usuarios del sistema junto con el nombre de su cargo asignado.
    public static List<Usuario> ObtenerUsuarios()
    {
        var lista = new List<Usuario>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT 
                    u.id_usuario, 
                    u.nombre_completo, 
                    u.usuario, 
                    u.id_cargo, 
                    c.nombre AS nombre_cargo, 
                    u.es_superadministrador, 
                    u.activo
                FROM usuarios u
                LEFT JOIN cargos c ON u.id_cargo = c.id_cargo
                ORDER BY u.nombre_completo;
                """, conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Usuario
                {
                    IdUsuario = lector.GetInt32("id_usuario"),
                    NombreCompleto = lector.GetString("nombre_completo"),
                    Username = lector.GetString("usuario"),
                    IdCargo = lector.IsDBNull(lector.GetOrdinal("id_cargo")) ? 0 : lector.GetInt32("id_cargo"),
                    NombreCargo = lector.IsDBNull(lector.GetOrdinal("nombre_cargo")) ? "Sin Cargo" : lector.GetString("nombre_cargo"),
                    EsSuperAdmin = lector.GetBoolean("es_superadministrador"),
                    Activo = lector.GetBoolean("activo")
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la lista de usuarios.\n\n{ex.Message}", ex);
        }
        return lista;
    }

    // Guarda un usuario (INSERT si es nuevo, UPDATE si ya existe).
    // Evita sobrescribir la contrasena si el campo viene vacio en una actualizacion.
    public static void GuardarUsuario(Usuario user)
    {
        using var conexion = ConexionDB.ObtenerConexion();

        // 1. Validar que el username no este duplicado.
        using var cmdVerificar = new MySqlCommand(
            "SELECT COUNT(*) FROM usuarios WHERE usuario = @username AND id_usuario != @idUsuario;", 
            conexion);
        cmdVerificar.Parameters.AddWithValue("@username", user.Username);
        cmdVerificar.Parameters.AddWithValue("@idUsuario", user.IdUsuario);
        
        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());
        if (count > 0)
        {
            throw new Exception($"El nombre de usuario '{user.Username}' ya está en uso. Por favor, elige otro.");
        }

        // 2. Insertar o Actualizar.
        if (user.IdUsuario == 0)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new Exception("La contraseña es obligatoria para usuarios nuevos.");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var cmdInsertar = new MySqlCommand("""
                INSERT INTO usuarios (nombre_completo, usuario, password_hash, id_cargo, es_superadministrador, activo)
                VALUES (@nombre, @usuario, @hash, @idCargo, @super, @activo);
                """, conexion);
            cmdInsertar.Parameters.AddWithValue("@nombre", user.NombreCompleto);
            cmdInsertar.Parameters.AddWithValue("@usuario", user.Username);
            cmdInsertar.Parameters.AddWithValue("@hash", hash);
            cmdInsertar.Parameters.AddWithValue("@idCargo", user.IdCargo);
            cmdInsertar.Parameters.AddWithValue("@super", user.EsSuperAdmin);
            cmdInsertar.Parameters.AddWithValue("@activo", user.Activo);
            
            cmdInsertar.ExecuteNonQuery();
        }
        else
        {
            // Update: si hay password, actualizar todo. Si no, ignorar password.
            bool actualizarPassword = !string.IsNullOrWhiteSpace(user.Password);
            
            string sql = actualizarPassword 
                ? """
                  UPDATE usuarios 
                  SET nombre_completo = @nombre, 
                      usuario = @usuario, 
                      password_hash = @hash, 
                      id_cargo = @idCargo, 
                      es_superadministrador = @super, 
                      activo = @activo 
                  WHERE id_usuario = @idUsuario;
                  """
                : """
                  UPDATE usuarios 
                  SET nombre_completo = @nombre, 
                      usuario = @usuario, 
                      id_cargo = @idCargo, 
                      es_superadministrador = @super, 
                      activo = @activo 
                  WHERE id_usuario = @idUsuario;
                  """;

            using var cmdActualizar = new MySqlCommand(sql, conexion);
            cmdActualizar.Parameters.AddWithValue("@nombre", user.NombreCompleto);
            cmdActualizar.Parameters.AddWithValue("@usuario", user.Username);
            cmdActualizar.Parameters.AddWithValue("@idCargo", user.IdCargo);
            cmdActualizar.Parameters.AddWithValue("@super", user.EsSuperAdmin);
            cmdActualizar.Parameters.AddWithValue("@activo", user.Activo);
            cmdActualizar.Parameters.AddWithValue("@idUsuario", user.IdUsuario);

            if (actualizarPassword)
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                cmdActualizar.Parameters.AddWithValue("@hash", hash);
            }

            cmdActualizar.ExecuteNonQuery();
        }
    }

    // Cambia el estado de un usuario (Soft Delete o Activacion).
    public static void CambiarEstadoUsuario(int idUsuario, bool activo)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "UPDATE usuarios SET activo = @activo WHERE id_usuario = @idUsuario;", 
                conexion);
            comando.Parameters.AddWithValue("@activo", activo);
            comando.Parameters.AddWithValue("@idUsuario", idUsuario);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al cambiar el estado del usuario.\n\n{ex.Message}", ex);
        }
    }
}
