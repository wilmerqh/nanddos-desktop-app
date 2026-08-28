using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Capa de acceso a datos para las estadísticas del Dashboard (Panel de Control).
public static class DashboardDAO
{
    // Obtiene el conteo de equipos agrupados por su estado actual.
    // Retorna un diccionario donde la clave es el estado y el valor es la cantidad de equipos.
    public static Dictionary<string, int> ObtenerEstadisticasEquipos()
    {
        var estadisticas = new Dictionary<string, int>();

        try
        {
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT es.nombre AS estado, COUNT(e.id) as cantidad 
                FROM equipos e
                INNER JOIN estados es ON e.estado_id = es.id
                GROUP BY es.nombre
                ORDER BY FIELD(es.nombre, 'En Diagnóstico', 'En Reparación', 'En Espera de Repuestos', 'Terminado/Listo', 'Entregado', 'Inactivo', 'Cancelado');
                """, conexion);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                string estado = lector.GetString("estado");
                int cantidad = lector.GetInt32("cantidad");
                estadisticas[estado] = cantidad;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener las estadísticas del Dashboard.\n\n{ex.Message}", ex);
        }

        return estadisticas;
    }
}
