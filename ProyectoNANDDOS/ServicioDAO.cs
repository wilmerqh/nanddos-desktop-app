using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Capa de acceso a datos CRUD para la tabla 'servicios'.
public static class ServicioDAO
{
    // Devuelve todos los servicios como DataTable para enlazar al DataGridView.
    public static DataTable ObtenerServicios(string busqueda = "")
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT id_servicio, nombre, descripcion, precio
                FROM servicios
                WHERE LOWER(nombre) LIKE @busqueda
                   OR LOWER(descripcion) LIKE @busqueda
                ORDER BY nombre;
                """, conexion);
            comando.Parameters.AddWithValue("@busqueda", $"%{busqueda.Trim().ToLower()}%");

            var tabla = new DataTable();
            using var adaptador = new MySqlDataAdapter(comando);
            adaptador.Fill(tabla);
            return tabla;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la lista de servicios.\n\n{ex.Message}", ex);
        }
    }

    // Inserta un nuevo servicio en la base de datos.
    public static void InsertarServicio(string nombre, string descripcion, decimal precio)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                INSERT INTO servicios (nombre, descripcion, precio)
                VALUES (@nombre, @descripcion, @precio);
                """, conexion);
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@descripcion", descripcion);
            comando.Parameters.Add("@precio", MySqlDbType.Decimal).Value = precio;
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al insertar el servicio.\n\n{ex.Message}", ex);
        }
    }

    // Actualiza un servicio existente por su ID.
    public static void ActualizarServicio(int idServicio, string nombre, string descripcion, decimal precio)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                UPDATE servicios
                SET nombre = @nombre, descripcion = @descripcion, precio = @precio
                WHERE id_servicio = @id;
                """, conexion);
            comando.Parameters.AddWithValue("@id", idServicio);
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@descripcion", descripcion);
            comando.Parameters.Add("@precio", MySqlDbType.Decimal).Value = precio;
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al actualizar el servicio.\n\n{ex.Message}", ex);
        }
    }

    // Elimina un servicio por su ID.
    public static void EliminarServicio(int idServicio)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "DELETE FROM servicios WHERE id_servicio = @id;", conexion);
            comando.Parameters.AddWithValue("@id", idServicio);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar el servicio.\n\n{ex.Message}", ex);
        }
    }
}
