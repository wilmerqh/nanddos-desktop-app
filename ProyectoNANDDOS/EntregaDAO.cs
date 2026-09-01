using System;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

public static class EntregaDAO
{
    public static decimal ObtenerCostoTotalRepuestos(int idEquipo)
    {
        decimal total = 0;
        string repuestosNecesarios = "";

        // 1. Obtenemos el texto de repuestos usados del equipo
        using (var conexion = ConexionDB.ObtenerConexion())
        {
            using var cmd = new MySqlCommand("SELECT repuestos_necesarios FROM equipos WHERE id = @id", conexion);
            cmd.Parameters.AddWithValue("@id", idEquipo);
            var res = cmd.ExecuteScalar();
            if (res != null && res != DBNull.Value)
            {
                repuestosNecesarios = res.ToString() ?? "";
            }
        }

        if (string.IsNullOrWhiteSpace(repuestosNecesarios)) return 0;

        // 2. Parseamos el texto y buscamos el precio en la tabla repuestos.
        // Asumimos que el formato es algo como "2x Pantalla"
        var partes = repuestosNecesarios.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        using (var conexion = ConexionDB.ObtenerConexion())
        {
            foreach (var parte in partes)
            {
                var p = parte.Trim();
                if (string.IsNullOrEmpty(p)) continue;

                // Extraer cantidad y nombre usando Regex (Ej: "2x Pantalla")
                var match = Regex.Match(p, @"^(\d+)x\s+(.+)$");
                if (match.Success)
                {
                    int cantidad = int.Parse(match.Groups[1].Value);
                    string nombreRepuesto = match.Groups[2].Value.Trim();

                    // Quitamos la parte del precio si el string viene del modo nuevo con " = $..."
                    var matchPrecio = Regex.Match(nombreRepuesto, @"^(.+?)\s*\(\$");
                    if (matchPrecio.Success)
                    {
                        nombreRepuesto = matchPrecio.Groups[1].Value.Trim();
                    }

                    using var cmdPrecio = new MySqlCommand("SELECT COALESCE(precio_venta, 0) FROM repuestos WHERE nombre = @nombre LIMIT 1", conexion);
                    cmdPrecio.Parameters.AddWithValue("@nombre", nombreRepuesto);
                    var precioRes = cmdPrecio.ExecuteScalar();
                    
                    if (precioRes != null && precioRes != DBNull.Value)
                    {
                        decimal precio = Convert.ToDecimal(precioRes);
                        total += (cantidad * precio);
                    }
                }
            }
        }

        return total;
    }
}

