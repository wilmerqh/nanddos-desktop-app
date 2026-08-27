using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Acceso a datos para las tablas 'cargos', 'permisos' y la pivote 'cargo_permiso'.
public static class CargoDAO
{
    // Retorna todos los cargos registrados en el sistema.
    public static List<Cargo> ObtenerCargos()
    {
        var lista = new List<Cargo>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT id_cargo, nombre, descripcion, protegido
                FROM cargos
                ORDER BY nombre;
                """, conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Cargo
                {
                    IdCargo = lector.GetInt32("id_cargo"),
                    Nombre = lector.GetString("nombre"),
                    Descripcion = lector.IsDBNull(lector.GetOrdinal("descripcion"))
                        ? string.Empty
                        : lector.GetString("descripcion"),
                    Protegido = lector.GetBoolean("protegido")
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la lista de cargos.\n\n{ex.Message}", ex);
        }
        return lista;
    }

    // Retorna todos los permisos disponibles en el sistema.
    public static List<Permiso> ObtenerTodosLosPermisos()
    {
        var lista = new List<Permiso>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT id_permiso, nombre_interno, descripcion, modulo
                FROM permisos
                ORDER BY modulo, descripcion;
                """, conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Permiso
                {
                    IdPermiso = lector.GetInt32("id_permiso"),
                    NombreInterno = lector.GetString("nombre_interno"),
                    Descripcion = lector.IsDBNull(lector.GetOrdinal("descripcion"))
                        ? string.Empty
                        : lector.GetString("descripcion"),
                    Modulo = lector.IsDBNull(lector.GetOrdinal("modulo"))
                        ? string.Empty
                        : lector.GetString("modulo")
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la lista de permisos.\n\n{ex.Message}", ex);
        }
        return lista;
    }

    // Retorna los IDs de permisos asignados a un cargo especifico.
    public static List<int> ObtenerPermisosPorCargo(int idCargo)
    {
        var ids = new List<int>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT id_permiso
                FROM cargo_permiso
                WHERE id_cargo = @id_cargo;
                """, conexion);
            comando.Parameters.AddWithValue("@id_cargo", idCargo);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                ids.Add(lector.GetInt32("id_permiso"));
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener los permisos del cargo {idCargo}.\n\n{ex.Message}", ex);
        }
        return ids;
    }

    // Guarda un cargo (nuevo o existente) y sincroniza sus permisos en una transaccion.
    // Estrategia: DELETE todos los permisos anteriores + INSERT de los nuevos marcados.
    public static void GuardarCargoYPermisos(Cargo cargo, List<int> idsPermisos)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            int idCargo;

            if (cargo.IdCargo == 0)
            {
                // Cargo nuevo: INSERT y obtener el ID generado.
                using var cmdInsertar = new MySqlCommand("""
                    INSERT INTO cargos (nombre, descripcion, protegido)
                    VALUES (@nombre, @descripcion, @protegido);
                    SELECT LAST_INSERT_ID();
                    """, conexion, transaccion);
                cmdInsertar.Parameters.AddWithValue("@nombre", cargo.Nombre);
                cmdInsertar.Parameters.AddWithValue("@descripcion", cargo.Descripcion);
                cmdInsertar.Parameters.AddWithValue("@protegido", false); // Los cargos nuevos no son protegidos.

                idCargo = Convert.ToInt32(cmdInsertar.ExecuteScalar());
            }
            else
            {
                // Cargo existente: UPDATE.
                idCargo = cargo.IdCargo;

                using var cmdActualizar = new MySqlCommand("""
                    UPDATE cargos
                    SET nombre = @nombre, descripcion = @descripcion
                    WHERE id_cargo = @id_cargo;
                    """, conexion, transaccion);
                cmdActualizar.Parameters.AddWithValue("@nombre", cargo.Nombre);
                cmdActualizar.Parameters.AddWithValue("@descripcion", cargo.Descripcion);
                cmdActualizar.Parameters.AddWithValue("@id_cargo", idCargo);
                cmdActualizar.ExecuteNonQuery();
            }

            // Eliminar TODOS los permisos anteriores de este cargo.
            using var cmdBorrar = new MySqlCommand(
                "DELETE FROM cargo_permiso WHERE id_cargo = @id_cargo;",
                conexion, transaccion);
            cmdBorrar.Parameters.AddWithValue("@id_cargo", idCargo);
            cmdBorrar.ExecuteNonQuery();

            // Insertar los permisos marcados uno por uno.
            foreach (int idPermiso in idsPermisos)
            {
                using var cmdPermiso = new MySqlCommand(
                    "INSERT INTO cargo_permiso (id_cargo, id_permiso) VALUES (@id_cargo, @id_permiso);",
                    conexion, transaccion);
                cmdPermiso.Parameters.AddWithValue("@id_cargo", idCargo);
                cmdPermiso.Parameters.AddWithValue("@id_permiso", idPermiso);
                cmdPermiso.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            throw new Exception($"Error al guardar el cargo y sus permisos.\n\n{ex.Message}", ex);
        }
    }

    // Elimina un cargo y sus relaciones de permisos (solo si no es protegido).
    public static void EliminarCargo(int idCargo)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            // Verificar que no sea protegido.
            using var cmdVerificar = new MySqlCommand(
                "SELECT protegido FROM cargos WHERE id_cargo = @id_cargo;",
                conexion, transaccion);
            cmdVerificar.Parameters.AddWithValue("@id_cargo", idCargo);
            var protegido = cmdVerificar.ExecuteScalar();

            if (protegido is not null && Convert.ToBoolean(protegido))
            {
                throw new Exception("No se puede eliminar un cargo protegido del sistema.");
            }

            // Verificar que no haya usuarios asignados a este cargo.
            using var cmdUsuarios = new MySqlCommand(
                "SELECT COUNT(*) FROM usuarios WHERE id_cargo = @id_cargo;",
                conexion, transaccion);
            cmdUsuarios.Parameters.AddWithValue("@id_cargo", idCargo);
            int cantidadUsuarios = Convert.ToInt32(cmdUsuarios.ExecuteScalar());

            if (cantidadUsuarios > 0)
            {
                throw new Exception(
                    $"No se puede eliminar este cargo porque tiene {cantidadUsuarios} usuario(s) asignado(s).\n" +
                    "Reasigne los usuarios a otro cargo antes de eliminarlo.");
            }

            // Eliminar permisos asociados.
            using var cmdPermisos = new MySqlCommand(
                "DELETE FROM cargo_permiso WHERE id_cargo = @id_cargo;",
                conexion, transaccion);
            cmdPermisos.Parameters.AddWithValue("@id_cargo", idCargo);
            cmdPermisos.ExecuteNonQuery();

            // Eliminar el cargo.
            using var cmdEliminar = new MySqlCommand(
                "DELETE FROM cargos WHERE id_cargo = @id_cargo;",
                conexion, transaccion);
            cmdEliminar.Parameters.AddWithValue("@id_cargo", idCargo);
            cmdEliminar.ExecuteNonQuery();

            transaccion.Commit();
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            throw new Exception($"Error al eliminar el cargo.\n\n{ex.Message}", ex);
        }
    }
}
