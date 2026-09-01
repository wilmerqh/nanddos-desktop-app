using System.Data;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Acceso a datos CRUD para la tabla 'repuestos'.
// Sigue el mismo patron ADO.NET clasico usado en ClientesForm, ListaEquiposForm, etc.
public static class RepuestoDAO
{
    // Devuelve todos los repuestos ordenados por fecha de ingreso (mas recientes primero).
    public static DataTable ObtenerTodos()
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT
                    id_repuesto,
                    codigo,
                    nombre,
                    categoria,
                    stock,
                    precio_costo,
                    precio_venta,
                    fecha_ingreso
                FROM repuestos
                ORDER BY fecha_ingreso DESC;
                """, conexion);

            var tabla = new DataTable();
            using var adaptador = new MySqlDataAdapter(comando);
            adaptador.Fill(tabla);
            return tabla;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la lista de repuestos.\n\n{ex.Message}", ex);
        }
    }

    // Devuelve los repuestos que tienen stock disponible (stock > 0).
    public static List<Repuesto> ObtenerConStock()
    {
        var lista = new List<Repuesto>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT id_repuesto, codigo, nombre, categoria, stock, precio_venta
                FROM repuestos
                WHERE stock > 0
                ORDER BY nombre;
                """, conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Repuesto
                {
                    IdRepuesto = lector.GetInt32("id_repuesto"),
                    Codigo = lector.GetString("codigo"),
                    Nombre = lector.GetString("nombre"),
                    Categoria = lector.GetString("categoria"),
                    Stock = lector.GetInt32("stock"),
                    PrecioVenta = lector.GetDecimal("precio_venta")
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] Error al obtener repuestos con stock: {ex.Message}");
        }

        return lista;
    }

    // Busca un repuesto por su codigo unico. Devuelve null si no existe.
    public static Repuesto? BuscarPorCodigo(string codigo)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT
                    id_repuesto,
                    codigo,
                    nombre,
                    categoria,
                    stock,
                    precio_costo,
                    precio_venta,
                    fecha_ingreso
                FROM repuestos
                WHERE codigo = @codigo
                LIMIT 1;
                """, conexion);

            comando.Parameters.AddWithValue("@codigo", codigo);

            using var lector = comando.ExecuteReader();
            if (!lector.Read())
                return null;

            return MapearRepuesto(lector);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al buscar el repuesto con código '{codigo}'.\n\n{ex.Message}", ex);
        }
    }

    // Inserta un nuevo repuesto en la base de datos.
    public static void Insertar(Repuesto repuesto)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                INSERT INTO repuestos
                    (codigo, nombre, categoria, stock, precio_costo, precio_venta, fecha_ingreso)
                VALUES
                    (@codigo, @nombre, @categoria, @stock, @precio_costo, @precio_venta, @fecha_ingreso);
                """, conexion);

            AgregarParametros(comando, repuesto);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al insertar el repuesto '{repuesto.Nombre}'.\n\n{ex.Message}", ex);
        }
    }

    // Actualiza un repuesto existente (stock, precios, nombre, categoria).
    public static void Actualizar(Repuesto repuesto)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                UPDATE repuestos SET
                    codigo        = @codigo,
                    nombre        = @nombre,
                    categoria     = @categoria,
                    stock         = @stock,
                    precio_costo  = @precio_costo,
                    precio_venta  = @precio_venta,
                    fecha_ingreso = @fecha_ingreso
                WHERE id_repuesto = @id_repuesto;
                """, conexion);

            AgregarParametros(comando, repuesto);
            comando.Parameters.AddWithValue("@id_repuesto", repuesto.IdRepuesto);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al actualizar el repuesto '{repuesto.Nombre}'.\n\n{ex.Message}", ex);
        }
    }

    // Elimina un repuesto por su id.
    public static void Eliminar(int idRepuesto)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "DELETE FROM repuestos WHERE id_repuesto = @id_repuesto;", conexion);

            comando.Parameters.AddWithValue("@id_repuesto", idRepuesto);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar el repuesto con ID {idRepuesto}.\n\n{ex.Message}", ex);
        }
    }

    // Busca repuestos cuyo nombre o codigo contengan el texto indicado.
    public static DataTable Buscar(string texto)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT
                    id_repuesto,
                    codigo,
                    nombre,
                    categoria,
                    stock,
                    precio_costo,
                    precio_venta,
                    fecha_ingreso
                FROM repuestos
                WHERE nombre LIKE @texto OR codigo LIKE @texto
                ORDER BY nombre ASC;
                """, conexion);

            comando.Parameters.AddWithValue("@texto", $"%{texto}%");

            var tabla = new DataTable();
            using var adaptador = new MySqlDataAdapter(comando);
            adaptador.Fill(tabla);
            return tabla;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al buscar repuestos con el texto '{texto}'.\n\n{ex.Message}", ex);
        }
    }

    // Mapea una fila del lector a un objeto Repuesto.
    private static Repuesto MapearRepuesto(MySqlDataReader lector)
    {
        return new Repuesto
        {
            IdRepuesto   = lector.GetInt32("id_repuesto"),
            Codigo       = lector.GetString("codigo"),
            Nombre       = lector.GetString("nombre"),
            Categoria    = lector.GetString("categoria"),
            Stock        = lector.GetInt32("stock"),
            PrecioCosto  = lector.GetDecimal("precio_costo"),
            PrecioVenta  = lector.GetDecimal("precio_venta"),
            FechaIngreso = lector.GetDateTime("fecha_ingreso")
        };
    }

    // Agrega los parametros comunes de un repuesto al comando SQL.
    private static void AgregarParametros(MySqlCommand comando, Repuesto repuesto)
    {
        comando.Parameters.AddWithValue("@codigo", repuesto.Codigo);
        comando.Parameters.AddWithValue("@nombre", repuesto.Nombre);
        comando.Parameters.AddWithValue("@categoria", repuesto.Categoria);
        comando.Parameters.AddWithValue("@stock", repuesto.Stock);
        comando.Parameters.AddWithValue("@precio_costo", repuesto.PrecioCosto);
        comando.Parameters.AddWithValue("@precio_venta", repuesto.PrecioVenta);
        comando.Parameters.AddWithValue("@fecha_ingreso", repuesto.FechaIngreso);
    }

    // Descuenta una unidad del stock de un repuesto por su id.
    public static void DescontarStock(int idRepuesto, int cantidad = 1)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "UPDATE repuestos SET stock = stock - @cantidad WHERE id_repuesto = @id_repuesto AND stock >= @cantidad;",
                conexion);

            comando.Parameters.AddWithValue("@id_repuesto", idRepuesto);
            comando.Parameters.AddWithValue("@cantidad", cantidad);

            int filasAfectadas = comando.ExecuteNonQuery();
            if (filasAfectadas == 0)
            {
                throw new Exception("No se pudo descontar el stock. Es posible que no haya unidades disponibles.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al descontar stock del repuesto con ID {idRepuesto}.\n\n{ex.Message}", ex);
        }
    }

    // Aumenta el stock de un repuesto (devolucion de inventario).
    public static bool AumentarStock(int idRepuesto, int cantidad)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "UPDATE repuestos SET stock = stock + @cantidad WHERE id_repuesto = @id_repuesto;",
                conexion);

            comando.Parameters.AddWithValue("@id_repuesto", idRepuesto);
            comando.Parameters.AddWithValue("@cantidad", cantidad);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al aumentar stock del repuesto con ID {idRepuesto}: {ex.Message}");
            return false;
        }
    }

    // Aumenta el stock de un repuesto por codigo (devolucion de inventario).
    public static bool AumentarStock(string codigo, int cantidad)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "UPDATE repuestos SET stock = stock + @cantidad WHERE codigo = @codigo;",
                conexion);

            comando.Parameters.AddWithValue("@codigo", codigo);
            comando.Parameters.AddWithValue("@cantidad", cantidad);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al aumentar stock del repuesto con codigo {codigo}: {ex.Message}");
            return false;
        }
    }

    // Descuenta stock de un repuesto por codigo.
    public static bool DescontarStock(string codigo, int cantidad)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "UPDATE repuestos SET stock = stock - @cantidad WHERE codigo = @codigo AND stock >= @cantidad;",
                conexion);

            comando.Parameters.AddWithValue("@codigo", codigo);
            comando.Parameters.AddWithValue("@cantidad", cantidad);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al descontar stock del repuesto con codigo {codigo}: {ex.Message}");
            return false;
        }
    }

    // Busca un repuesto por aproximacion de nombre y retorna su ID (0 si no se encuentra).
    public static int BuscarIdPorNombreAproximado(string nombreAproximado)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "SELECT id_repuesto FROM repuestos WHERE LOWER(nombre) LIKE CONCAT('%', LOWER(@nombre), '%') LIMIT 1;",
                conexion);

            comando.Parameters.AddWithValue("@nombre", nombreAproximado);

            var resultado = comando.ExecuteScalar();
            if (resultado != null && int.TryParse(resultado.ToString(), out int id))
            {
                return id;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al buscar repuesto por nombre '{nombreAproximado}': {ex.Message}");
        }
        return 0;
    }

    // Busca un repuesto por aproximacion de nombre y retorna su CODIGO (string vacio si no se encuentra).
    public static string BuscarCodigoPorNombreAproximado(string nombreAproximado)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "SELECT codigo FROM repuestos WHERE LOWER(nombre) LIKE CONCAT('%', LOWER(@nombre), '%') LIMIT 1;",
                conexion);

            comando.Parameters.AddWithValue("@nombre", nombreAproximado);

            var resultado = comando.ExecuteScalar();
            if (resultado != null)
            {
                return resultado.ToString() ?? "";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al buscar codigo de repuesto por nombre '{nombreAproximado}': {ex.Message}");
        }
        return "";
    }

    // Obtiene una lista de prefijos únicos que ya existen en la base de datos (Ej. 'RM', 'SSD').
    public static List<string> ObtenerPrefijosExistentes()
    {
        var prefijos = new List<string>();
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "SELECT DISTINCT SUBSTRING_INDEX(codigo, '-', 1) AS prefijo FROM repuestos WHERE codigo LIKE '%-%';",
                conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                var prefijo = lector["prefijo"]?.ToString();
                if (!string.IsNullOrWhiteSpace(prefijo))
                {
                    prefijos.Add(prefijo);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NANDDOS] Error al obtener prefijos: {ex.Message}");
        }
        return prefijos;
    }

    // Genera automáticamente el siguiente código correlativo para un prefijo dado (Ej. RM-0001).
    public static string GenerarSiguienteCodigo(string prefijo)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "SELECT codigo FROM repuestos WHERE codigo LIKE @prefijo_like ORDER BY codigo DESC LIMIT 1;",
                conexion);

            comando.Parameters.AddWithValue("@prefijo_like", prefijo + "-%");

            var resultado = comando.ExecuteScalar()?.ToString();

            if (string.IsNullOrWhiteSpace(resultado))
            {
                return $"{prefijo}-0001";
            }

            var partes = resultado.Split('-');
            if (partes.Length == 2 && int.TryParse(partes[1], out int numeroFina))
            {
                return $"{prefijo}-{(numeroFina + 1).ToString("D4")}";
            }
            
            // Fallback en caso de que el código tenga un formato inesperado
            return $"{prefijo}-0001";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al generar el código para el prefijo '{prefijo}'.\n\n{ex.Message}", ex);
        }
    }

    // Verifica si ya existe un repuesto con un nombre igual o muy similar.
    public static bool ExisteNombreSimilar(string nombre)
    {
        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand(
                "SELECT COUNT(*) FROM repuestos WHERE LOWER(nombre) LIKE CONCAT('%', LOWER(@nombre), '%');",
                conexion);

            comando.Parameters.AddWithValue("@nombre", nombre.Trim());

            int count = Convert.ToInt32(comando.ExecuteScalar());
            return count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NANDDOS] Error al verificar similitud de nombre: {ex.Message}");
            return false; // Ante la duda, permitimos continuar
        }
    }
}
