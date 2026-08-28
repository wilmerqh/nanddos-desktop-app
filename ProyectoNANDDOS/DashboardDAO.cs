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
        // Lista Maestra de Estados
        var estadisticas = new Dictionary<string, int>()
        {
            { "RECIBIDO", 0 },
            { "EN DIAGNÓSTICO", 0 },
            { "ESPERANDO REPUESTO", 0 },
            { "EN REPARACIÓN", 0 },
            { "TERMINADO", 0 },
            { "ENTREGADO", 0 }
        };

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
                string estadoBd = lector.GetString("estado").Trim().ToUpper();
                int cantidad = lector.GetInt32("cantidad");
                
                if (estadisticas.ContainsKey(estadoBd)) 
                {
                    estadisticas[estadoBd] = cantidad;
                } 
                else 
                {
                    estadisticas.Add(estadoBd, cantidad);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener las estadísticas del Dashboard.\n\n{ex.Message}", ex);
        }

        return estadisticas;
    }
}
