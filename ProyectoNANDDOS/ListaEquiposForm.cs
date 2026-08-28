using MySql.Data.MySqlClient;
using System.Data;

namespace ProyectoNANDDOS;

// Modulo para consultar, filtrar y administrar equipos registrados.
public class ListaEquiposForm : Form
{
    // Controles principales de busqueda y tabla.
    private readonly TextBox txtBusqueda = new();
    private readonly ComboBox cmbEstados = new();
    private readonly DataGridView dgvEquipos = new();

    // Botones de accion.
    private readonly Button btnBuscar = new();
    private readonly Button btnCopiarCodigo = new();
    private readonly Button btnCambiarEstado = new();
    private readonly Button btnVerDetalles = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEliminar = new();

    public ListaEquiposForm()
    {
        InicializarComponentes();
        ConfigurarTabla();
        ConfigurarBotones();
        CargarEstados();
        CargarEquipos();
    }

    // SECCION: construccion visual.
    private void InicializarComponentes()
    {
        Text = "Lista de Equipos";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout general: titulo, filtros y tabla de resultados.
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(new Label
        {
            Text = "Lista de Equipos",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.FromArgb(25, 35, 50)
        }, 0, 0);

        // Barra superior con buscador, filtro por estado y acciones.
        var barra = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 7 };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.PlaceholderText = "Buscar por código, cliente, teléfono, marca o problema";
        // Permite buscar presionando Enter.
        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                CargarEquipos();
            }
        };

        cmbEstados.Dock = DockStyle.Fill;
        cmbEstados.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEstados.SelectedIndexChanged += (_, _) => CargarEquipos();

        btnBuscar.Text = "Buscar";
        btnBuscar.Click += (_, _) => CargarEquipos();
        
        btnCopiarCodigo.Text = "Copiar Código";
        btnCopiarCodigo.Click += (_, _) => CopiarCodigoEquipo();
        
        btnCambiarEstado.Text = "Cambiar Estado";
        btnCambiarEstado.Click += (_, _) => CambiarEstadoEquipo();
        
        btnVerDetalles.Text = "Ver Detalles";
        btnVerDetalles.Click += (_, _) => VerDetallesEquipo();
        
        btnEditar.Text = "Editar";
        btnEditar.Click += (_, _) => EditarEquipo();
        
        btnEliminar.Text = "Eliminar";
        btnEliminar.Click += (_, _) => EliminarEquipo();

        barra.Controls.Add(txtBusqueda, 0, 0);
        barra.Controls.Add(cmbEstados, 1, 0);
        barra.Controls.Add(btnBuscar, 2, 0);
        barra.Controls.Add(btnCambiarEstado, 3, 0);
        barra.Controls.Add(btnVerDetalles, 4, 0);
        barra.Controls.Add(btnEditar, 5, 0);
        barra.Controls.Add(btnEliminar, 6, 0);

        // Contenedor que deja el boton Copiar Codigo junto a la tabla.
        var tablaConAcciones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        tablaConAcciones.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
        tablaConAcciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var panelAccionesTabla = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 0, 8, 0)
        };
        btnCopiarCodigo.Dock = DockStyle.Top;
        btnCopiarCodigo.Height = 36;
        panelAccionesTabla.Controls.Add(btnCopiarCodigo);

        tablaConAcciones.Controls.Add(panelAccionesTabla, 0, 0);
        tablaConAcciones.Controls.Add(dgvEquipos, 1, 0);

        // Blindaje de Seguridad RBAC
        btnEditar.Visible = GestorSeguridad.TienePermiso("equipos_editar");
        btnCambiarEstado.Visible = GestorSeguridad.TienePermiso("equipos_editar");
        btnEliminar.Visible = GestorSeguridad.TienePermiso("equipos_eliminar");

        principal.Controls.Add(barra, 0, 1);
        principal.Controls.Add(tablaConAcciones, 0, 2);
        Controls.Add(principal);
    }

    // Aplica el estilo Fluent Design y los iconos locales a los botones de accion.
    private void ConfigurarBotones()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        // Helper local para aplicar estilo a cada boton de forma limpia.
        void AplicarEstilo(Button btn, string archivoIcono, Color fondo, Color texto, Color hover)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9F);
            btn.BackColor = fondo;
            btn.ForeColor = texto;
            btn.FlatAppearance.MouseOverBackColor = hover;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Padding = new Padding(8, 0, 0, 0); // Espacio a la izquierda
            btn.Dock = DockStyle.Fill;

            try
            {
                var ruta = Path.Combine(carpetaIconos, archivoIcono);
                btn.Image = Image.FromFile(ruta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NANDDOS] No se pudo cargar el icono '{archivoIcono}': {ex.Message}");
            }
        }

        var azulAcento = Color.FromArgb(37, 99, 235); // #2563EB
        var azulAcentoHover = Color.FromArgb(29, 78, 216);
        var grisClaro = Color.FromArgb(241, 245, 249); // #F1F5F9
        var grisHover = Color.FromArgb(226, 232, 240); // #E2E8F0
        var grisPizarra = Color.FromArgb(71, 85, 105); // #475569
        var grisPizarraHover = Color.FromArgb(51, 65, 85);
        var rojoSuave = Color.FromArgb(239, 68, 68); // #EF4444
        var rojoSuaveHover = Color.FromArgb(220, 38, 38);
        var textoOscuro = Color.FromArgb(15, 23, 42); // #0F172A

        // Acciones principales (Azul Acento)
        AplicarEstilo(btnBuscar, "btn_buscar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnVerDetalles, "btn_detalles.png", azulAcento, Color.White, azulAcentoHover);

        // Acciones secundarias (Gris Claro)
        AplicarEstilo(btnCambiarEstado, "btn_estado.png", grisClaro, textoOscuro, grisHover);
        AplicarEstilo(btnCopiarCodigo, "btn_copiar.png", grisClaro, textoOscuro, grisHover);
        
        // Ajuste manual de Dock para el boton de copiar codigo segun la estructura.
        btnCopiarCodigo.Dock = DockStyle.Top;
        btnCopiarCodigo.Height = 36;

        // Otras acciones
        AplicarEstilo(btnEditar, "btn_editar.png", grisPizarra, Color.White, grisPizarraHover);
        AplicarEstilo(btnEliminar, "btn_eliminar.png", rojoSuave, Color.White, rojoSuaveHover);
    }

    // Configura la tabla para lectura, seleccion completa y estilo corporativo.
    private void ConfigurarTabla()
    {
        dgvEquipos.Dock = DockStyle.Fill;
        dgvEquipos.AllowUserToAddRows = false;
        dgvEquipos.AllowUserToDeleteRows = false;
        dgvEquipos.AllowUserToOrderColumns = false;
        dgvEquipos.AllowUserToResizeColumns = false;
        dgvEquipos.AllowUserToResizeRows = false;
        dgvEquipos.ReadOnly = true;
        dgvEquipos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvEquipos.MultiSelect = false;
        dgvEquipos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEquipos.RowHeadersVisible = false;

        // Estilo visual general
        dgvEquipos.BackgroundColor = Color.White;
        dgvEquipos.BorderStyle = BorderStyle.None;
        dgvEquipos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvEquipos.GridColor = Color.FromArgb(226, 232, 240); // #E2E8F0 tenues

        // Estilo de Encabezados (Headers)
        dgvEquipos.EnableHeadersVisualStyles = false;
        dgvEquipos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvEquipos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvEquipos.ColumnHeadersHeight = 40;
        dgvEquipos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // #0F172A
        dgvEquipos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvEquipos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
        dgvEquipos.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 23, 42);

        // Estilo de Filas (Rows) y Colores Alternos
        dgvEquipos.RowTemplate.Height = 35;
        dgvEquipos.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        dgvEquipos.DefaultCellStyle.BackColor = Color.White;
        dgvEquipos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC
        
        // Seleccion (Azul suave)
        dgvEquipos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254); // #E0F2FE
        dgvEquipos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42); // #0F172A
    }

    // SECCION: filtros.
    private void CargarEstados()
    {
        cmbEstados.Items.Clear();
        cmbEstados.Items.Add(new EstadoItem(null, "Todos los estados"));

        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("SELECT id, nombre FROM estados ORDER BY id;", conexion);
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            cmbEstados.Items.Add(new EstadoItem(reader.GetInt32("id"), reader.GetString("nombre")));
        }

        cmbEstados.SelectedIndex = 0;
    }

    // SECCION: carga de equipos.
    private void CargarEquipos()
    {
        if (cmbEstados.SelectedItem is null)
        {
            return;
        }

        var estado = (EstadoItem)cmbEstados.SelectedItem;
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT
                e.id,
                e.codigo AS Código,
                CONCAT(c.nombres, ' ', c.apellidos) AS Cliente,
                CONCAT(n.prefijo, ' - ', n.descripcion, ' ', IFNULL(e.marca, ''), ' ', IFNULL(e.modelo, '')) AS Equipo,
                e.descripcion_problema AS Problema,
                es.nombre AS Estado,
                e.fecha_ingreso AS Fecha
            FROM equipos e
            INNER JOIN clientes c ON c.id = e.cliente_id
            INNER JOIN nomenclaturas n ON n.id = e.nomenclatura_id
            INNER JOIN estados es ON es.id = e.estado_id
            WHERE (@estado_id IS NULL OR e.estado_id = @estado_id)
              AND (
                    e.codigo LIKE @busqueda
                 OR CONCAT(c.nombres, ' ', c.apellidos) LIKE @busqueda
                 OR c.telefono LIKE @busqueda
                 OR e.marca LIKE @busqueda
                 OR e.modelo LIKE @busqueda
                 OR e.descripcion_problema LIKE @busqueda
              )
            ORDER BY e.fecha_ingreso DESC, e.codigo DESC;
            """, conexion);
        comando.Parameters.AddWithValue("@estado_id", estado.Id.HasValue ? estado.Id.Value : DBNull.Value);
        comando.Parameters.AddWithValue("@busqueda", $"%{txtBusqueda.Text.Trim()}%");

        // Carga los resultados de MySQL en una tabla temporal.
        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        dgvEquipos.DataSource = tabla;

        if (dgvEquipos.Columns["id"] is DataGridViewColumn columnaId)
        {
            columnaId.Visible = false;
        }
        if (dgvEquipos.Columns["Fecha"] is DataGridViewColumn columnaFecha)
        {
            columnaFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        ConfigurarColumnasEquipos();
    }

    // Define el orden y proporcion visual de las columnas.
    private void ConfigurarColumnasEquipos()
    {
        ConfigurarColumna("Código", 0, 14);
        ConfigurarColumna("Cliente", 1, 20);
        ConfigurarColumna("Equipo", 2, 22);
        ConfigurarColumna("Problema", 3, 24);
        ConfigurarColumna("Estado", 4, 16);
        ConfigurarColumna("Fecha", 5, 14);
    }

    // Aplica configuracion solo si la columna existe.
    private void ConfigurarColumna(string nombre, int orden, float peso)
    {
        if (dgvEquipos.Columns[nombre] is not DataGridViewColumn columna)
        {
            return;
        }

        columna.DisplayIndex = orden;
        columna.FillWeight = peso;
        columna.MinimumWidth = 80;
        columna.Resizable = DataGridViewTriState.False;
    }

    // Devuelve el id interno del equipo seleccionado.
    private int? ObtenerEquipoSeleccionado()
    {
        if (dgvEquipos.CurrentRow?.DataBoundItem is not DataRowView fila)
        {
            MessageBox.Show("Selecciona un equipo.", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        return Convert.ToInt32(fila["id"]);
    }

    // Obtiene el codigo visible del equipo seleccionado para copiarlo.
    private string? ObtenerCodigoEquipoSeleccionado()
    {
        if (dgvEquipos.CurrentRow?.DataBoundItem is not DataRowView fila)
        {
            return null;
        }

        foreach (DataColumn columna in fila.Row.Table.Columns)
        {
            if (string.Equals(columna.ColumnName, "Código", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(columna.ColumnName, "Codigo", StringComparison.OrdinalIgnoreCase))
            {
                return fila.Row[columna]?.ToString();
            }
        }

        return fila.Row.Table.Columns.Count > 1 ? fila[1]?.ToString() : null;
    }

    // SECCION: copiar codigo visible.
    private void CopiarCodigoEquipo()
    {
        var codigo = ObtenerCodigoEquipoSeleccionado();
        if (string.IsNullOrWhiteSpace(codigo))
        {
            MessageBox.Show("Seleccione un equipo primero", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Clipboard.SetText(codigo);
        MessageBox.Show("Código copiado correctamente", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // SECCION: editar datos del equipo.
    private void EditarEquipo()
    {
        var id = ObtenerEquipoSeleccionado();
        if (id is null)
        {
            return;
        }

        var datos = ObtenerEquipo(id.Value);
        using var formulario = CrearFormularioEdicion(datos, out var txtMarca, out var txtModelo, out var txtSerial, out var txtProblema, out var dgvRepuestos);

        if (formulario.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(txtProblema.Text))
        {
            MessageBox.Show("La descripción del problema es obligatoria.", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Concatenar repuestos con cantidades en formato "2x RAM, 1x SSD".
        var partes = new List<string>();
        foreach (DataGridViewRow fila in dgvRepuestos.Rows)
        {
            var nombre = fila.Cells["Descripcion"].Value?.ToString() ?? "";
            var cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
            partes.Add($"{cantidad}x {nombre}");
        }
        var repuestosConcatenados = string.Join(", ", partes);


        // Actualiza solo datos del equipo, no el estado.
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            UPDATE equipos
            SET marca = @marca,
                modelo = @modelo,
                serial = @serial,
                descripcion_problema = @problema,
                repuestos_necesarios = @repuestos
            WHERE id = @id;
            """, conexion);
        comando.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
        comando.Parameters.AddWithValue("@modelo", txtModelo.Text.Trim());
        comando.Parameters.AddWithValue("@serial", txtSerial.Text.Trim());
        comando.Parameters.AddWithValue("@problema", txtProblema.Text.Trim());
        comando.Parameters.AddWithValue("@repuestos", repuestosConcatenados);
        comando.Parameters.AddWithValue("@id", id.Value);
        comando.ExecuteNonQuery();

        CargarEquipos();
        MessageBox.Show("Equipo actualizado correctamente.", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // SECCION: cambiar estado.
    private void CambiarEstadoEquipo()
    {
        var id = ObtenerEquipoSeleccionado();
        if (id is null)
        {
            return;
        }

        var datos = ObtenerEstadoEquipo(id.Value);
        var estados = ObtenerEstados();
        using var formulario = CrearFormularioCambiarEstado(datos, estados, out var cmbNuevoEstado);

        if (formulario.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (cmbNuevoEstado.SelectedItem is not EstadoItem nuevoEstado || nuevoEstado.Id is null)
        {
            MessageBox.Show("Selecciona el nuevo estado.", "Cambiar estado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Guarda el nuevo estado en la base y recarga la tabla.
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("UPDATE equipos SET estado_id = @estado_id WHERE id = @id;", conexion);
        comando.Parameters.AddWithValue("@estado_id", nuevoEstado.Id.Value);
        comando.Parameters.AddWithValue("@id", id.Value);
        comando.ExecuteNonQuery();

        CargarEquipos();
        MessageBox.Show("Estado actualizado correctamente.", "Cambiar estado", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // SECCION: ver detalle completo.
    private void VerDetallesEquipo()
    {
        var id = ObtenerEquipoSeleccionado();
        if (id is null)
        {
            return;
        }

        var datos = ObtenerDetalleEquipo(id.Value);
        using var formulario = new DetalleEquipoForm(datos);
        formulario.ShowDialog(this);
    }

    // SECCION: eliminar equipo.
    private void EliminarEquipo()
    {
        var id = ObtenerEquipoSeleccionado();
        if (id is null)
        {
            return;
        }

        if (MessageBox.Show("¿Eliminar este equipo y sus entregas asociadas?", "Lista de equipos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        // Elimina primero entregas relacionadas y luego el equipo.
        using var conexion = ConexionDB.ObtenerConexion();
        using var transaccion = conexion.BeginTransaction();
        try
        {
            using var eliminarEntregas = new MySqlCommand("DELETE FROM entregas WHERE equipo_id = @id;", conexion, transaccion);
            eliminarEntregas.Parameters.AddWithValue("@id", id.Value);
            eliminarEntregas.ExecuteNonQuery();

            using var eliminarEquipo = new MySqlCommand("DELETE FROM equipos WHERE id = @id;", conexion, transaccion);
            eliminarEquipo.Parameters.AddWithValue("@id", id.Value);
            eliminarEquipo.ExecuteNonQuery();

            transaccion.Commit();
            CargarEquipos();
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            MessageBox.Show($"No se pudo eliminar el equipo.\n\n{ex.Message}", "Lista de equipos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Obtiene los datos editables de un equipo.
    private static DataRow ObtenerEquipo(int id)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT
                e.codigo,
                e.marca,
                e.modelo,
                e.serial,
                e.descripcion_problema,
                e.repuestos_necesarios,
                CONCAT(n.prefijo, ' - ', n.descripcion) AS tipo_equipo
            FROM equipos e
            INNER JOIN nomenclaturas n ON n.id = e.nomenclatura_id
            WHERE e.id = @id;
            """, conexion);
        comando.Parameters.AddWithValue("@id", id);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        return tabla.Rows[0];
    }

    // Obtiene el estado actual para mostrarlo en el modal de cambio.
    private static DataRow ObtenerEstadoEquipo(int id)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT e.codigo, e.estado_id, es.nombre AS estado_actual
            FROM equipos e
            INNER JOIN estados es ON es.id = e.estado_id
            WHERE e.id = @id;
            """, conexion);
        comando.Parameters.AddWithValue("@id", id);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        return tabla.Rows[0];
    }

    // Obtiene la informacion completa para la ventana de detalles.
    private static DataRow ObtenerDetalleEquipo(int id)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT
                e.codigo,
                CONCAT(c.nombres, ' ', c.apellidos) AS cliente,
                c.telefono,
                c.email,
                CONCAT(n.prefijo, ' - ', n.descripcion) AS tipo_equipo,
                e.marca,
                e.modelo,
                e.serial,
                e.descripcion_problema,
                es.nombre AS estado,
                e.fecha_ingreso,
                e.repuestos_necesarios,
                ent.diagnostico,
                ent.fecha_entrega,
                ent.costo_total
            FROM equipos e
            INNER JOIN clientes c ON c.id = e.cliente_id
            INNER JOIN nomenclaturas n ON n.id = e.nomenclatura_id
            INNER JOIN estados es ON es.id = e.estado_id
            LEFT JOIN entregas ent ON ent.id = (
                SELECT ent2.id
                FROM entregas ent2
                WHERE ent2.equipo_id = e.id
                ORDER BY ent2.fecha_creacion DESC, ent2.id DESC
                LIMIT 1
            )
            WHERE e.id = @id;
            """, conexion);
        comando.Parameters.AddWithValue("@id", id);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        return tabla.Rows[0];
    }

    // Lista todos los estados disponibles desde MySQL.
    private static List<EstadoItem> ObtenerEstados()
    {
        var estados = new List<EstadoItem>();
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("SELECT id, nombre FROM estados ORDER BY id;", conexion);
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            estados.Add(new EstadoItem(reader.GetInt32("id"), reader.GetString("nombre")));
        }

        return estados;
    }

    // Crea la ventana modal para editar informacion del equipo.
    private static Form CrearFormularioEdicion(
        DataRow datos,
        out TextBox txtMarca,
        out TextBox txtModelo,
        out TextBox txtSerial,
        out TextBox txtProblema,
        out DataGridView dgvRepuestosUtilizados)
    {
        var formulario = new Form
        {
            Text = $"Editar equipo {datos["codigo"]}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(560, 680),
            MinimumSize = new Size(560, 680),
            Font = new Font("Segoe UI", 10F)
        };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 2,
            RowCount = 14
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        txtMarca = CrearTextBox(datos["marca"].ToString());
        txtModelo = CrearTextBox(datos["modelo"].ToString());
        txtSerial = CrearTextBox(datos["serial"].ToString());
        txtProblema = CrearTextBox(datos["descripcion_problema"].ToString(), true);

        var lblCodigo = CrearEtiqueta($"Código: {datos["codigo"]}");
        panel.Controls.Add(lblCodigo, 0, 0);
        panel.SetColumnSpan(lblCodigo, 2);
        var lblTipo = CrearEtiqueta($"Tipo: {datos["tipo_equipo"]}");
        panel.Controls.Add(lblTipo, 0, 1);
        panel.SetColumnSpan(lblTipo, 2);
        panel.Controls.Add(CrearEtiqueta("Marca"), 0, 2);
        panel.Controls.Add(CrearEtiqueta("Modelo"), 1, 2);
        panel.Controls.Add(txtMarca, 0, 3);
        panel.Controls.Add(txtModelo, 1, 3);
        panel.Controls.Add(CrearEtiqueta("Serial"), 0, 4);
        panel.Controls.Add(txtSerial, 0, 5);
        panel.SetColumnSpan(txtSerial, 2);
        var lblProblema = CrearEtiqueta("Problema");
        panel.Controls.Add(lblProblema, 0, 6);
        panel.SetColumnSpan(lblProblema, 2);
        panel.Controls.Add(txtProblema, 0, 7);
        panel.SetColumnSpan(txtProblema, 2);

        // SECCION: selector multiple de repuestos.
        var lblRepuestos = CrearEtiqueta("Repuestos utilizados");
        panel.Controls.Add(lblRepuestos, 0, 8);
        panel.SetColumnSpan(lblRepuestos, 2);

        // Panel con ComboBox + NumericUpDown + boton Agregar + boton Quitar.
        var panelSelector = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Margin = Padding.Empty
        };
        panelSelector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panelSelector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        panelSelector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        panelSelector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));

        var cmb = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat
        };

        var nud = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F),
            Minimum = 1,
            Value = 1
        };

        var btnAgregar = new Button
        {
            Text = "+",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAgregar.FlatAppearance.BorderSize = 0;

        var btnQuitar = new Button
        {
            Text = "-",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(239, 68, 68), // #EF4444
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnQuitar.FlatAppearance.BorderSize = 0;

        panelSelector.Controls.Add(cmb, 0, 0);
        panelSelector.Controls.Add(nud, 1, 0);
        panelSelector.Controls.Add(btnAgregar, 2, 0);
        panelSelector.Controls.Add(btnQuitar, 3, 0);
        panel.Controls.Add(panelSelector, 0, 9);
        panel.SetColumnSpan(panelSelector, 2);

        // DataGridView para los repuestos seleccionados.
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 9F),
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "CodigoRepuesto", HeaderText = "Código", FillWeight = 30 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descripcion", HeaderText = "Descripción", FillWeight = 50 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cantidad", FillWeight = 20 });

        panel.Controls.Add(dgv, 0, 10);
        panel.SetColumnSpan(dgv, 2);

        // Cargar repuestos del inventario en el ComboBox.
        CargarRepuestosComboBox(cmb);

        // Precargar repuestos existentes si ya hay datos guardados.
        var repuestosExistentes = datos["repuestos_necesarios"]?.ToString() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(repuestosExistentes))
        {
            foreach (var parte in repuestosExistentes.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(parte)) continue;

                int cantidad = 1;
                string nombreLimpio = parte;

                // Extraer formato "2x Nombre"
                int indiceX = parte.IndexOf("x ");
                if (indiceX > 0 && int.TryParse(parte.Substring(0, indiceX), out int cantParceada))
                {
                    cantidad = cantParceada;
                    nombreLimpio = parte.Substring(indiceX + 2).Trim();
                }

                // Busqueda inversa para recuperar el CODIGO real de la base de datos
                string codigo = RepuestoDAO.BuscarCodigoPorNombreAproximado(nombreLimpio);
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    codigo = "N/A";
                }

                dgv.Rows.Add(codigo, nombreLimpio, cantidad);
            }
        }

        // Evento: redirigir a Inventario si selecciona "OTROS".
        cmb.SelectedIndexChanged += (_, _) =>
        {
            if (cmb.SelectedItem is RepuestoItem item && item.Id == -1)
            {
                using var inventario = new InventarioForm();
                inventario.Width += 250;
                inventario.ShowDialog();
                CargarRepuestosComboBox(cmb);
            }
        };

        // Evento: agregar repuesto al DataGridView.
        btnAgregar.Click += (_, _) =>
        {
            if (cmb.SelectedItem is not RepuestoItem seleccionado || seleccionado.Id <= 0)
            {
                return;
            }

            int cantidadAgregar = (int)nud.Value;

            // Validar stock inicial (antes de intentar la BD).
            if (cantidadAgregar > seleccionado.StockDisponible)
            {
                MessageBox.Show($"Stock insuficiente. Solo hay {seleccionado.StockDisponible} unidades disponibles.",
                    "Repuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoLimpio = seleccionado.Codigo;

            // Descuento inmediato en base de datos.
            bool exito = RepuestoDAO.DescontarStock(codigoLimpio, cantidadAgregar);
            
            if (exito)
            {
                bool existe = false;

                // Buscar duplicados para sumar cantidad visualmente.
                foreach (DataGridViewRow fila in dgv.Rows)
                {
                    if (fila.Cells["CodigoRepuesto"].Value.ToString().Trim() == codigoLimpio)
                    {
                        int cantidadActual = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                        fila.Cells["Cantidad"].Value = cantidadActual + cantidadAgregar;
                        existe = true;
                        break;
                    }
                }

                if (!existe)
                {
                    dgv.Rows.Add(codigoLimpio, seleccionado.NombreLimpio, cantidadAgregar);
                }

                CargarRepuestosComboBox(cmb);
                nud.Value = 1;
            }
            else
            {
                MessageBox.Show("Error Crítico: No se pudo descontar el repuesto del inventario.",
                    "Fallo de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        // Evento: quitar repuesto del DataGridView y devolver al stock.
        btnQuitar.Click += (_, _) =>
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un repuesto de la tabla para quitarlo.",
                    "Repuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filaSeleccionada = dgv.SelectedRows[0];
            string cod = filaSeleccionada.Cells["CodigoRepuesto"].Value.ToString().Trim();

            if (cod == "N/A" || string.IsNullOrWhiteSpace(cod))
            {
                // Si el codigo no es valido, simplemente removemos 1 cantidad o toda la fila (no afecta BD)
                int cantidadActual = Convert.ToInt32(filaSeleccionada.Cells["Cantidad"].Value);
                if (cantidadActual > 1)
                {
                    filaSeleccionada.Cells["Cantidad"].Value = cantidadActual - 1;
                }
                else
                {
                    dgv.Rows.RemoveAt(filaSeleccionada.Index);
                }
                return;
            }

            bool exito = RepuestoDAO.AumentarStock(cod, 1);
            if (exito)
            {
                int cantidadActual = Convert.ToInt32(filaSeleccionada.Cells["Cantidad"].Value);
                if (cantidadActual > 1)
                {
                    filaSeleccionada.Cells["Cantidad"].Value = cantidadActual - 1;
                }
                else
                {
                    dgv.Rows.RemoveAt(filaSeleccionada.Index);
                }
                
                CargarRepuestosComboBox(cmb);
            }
            else
            {
                MessageBox.Show("Error Crítico: No se pudo devolver el repuesto al inventario. Verifica que el código exista en la base de datos.", 
                    "Fuga de Inventario Prevenida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 46 };
        var btnGuardar = new Button { Text = "Guardar", DialogResult = DialogResult.OK, Width = 100, Height = 34 };
        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 100, Height = 34 };
        botones.Controls.Add(btnGuardar);
        botones.Controls.Add(btnCancelar);

        formulario.Controls.Add(panel);
        formulario.Controls.Add(botones);
        formulario.AcceptButton = btnGuardar;
        formulario.CancelButton = btnCancelar;

        // Asignar el DataGridView al parametro de salida despues de las lambdas.
        dgvRepuestosUtilizados = dgv;
        return formulario;
    }

    // Carga el ComboBox con los repuestos disponibles en inventario (stock > 0) mas la opcion OTROS.
    private static void CargarRepuestosComboBox(ComboBox cmb)
    {
        cmb.Items.Clear();
        var repuestos = RepuestoDAO.ObtenerConStock();

        foreach (var r in repuestos)
        {
            cmb.Items.Add(new RepuestoItem(r.IdRepuesto, r.Codigo, r.Nombre, $"{r.Codigo} - {r.Nombre} (Stock: {r.Stock})", r.Stock));
        }

        // Opcion especial para registrar repuestos nuevos al vuelo.
        cmb.Items.Add(new RepuestoItem(-1, "", "", "--- OTROS (Agregar Nuevo) ---", 0));

        if (cmb.Items.Count > 0)
        {
            cmb.SelectedIndex = 0;
        }
    }

    // Objeto para almacenar id, codigo, nombre limpio, nombre visible y stock de un repuesto en el ComboBox.
    private sealed class RepuestoItem
    {
        public RepuestoItem(int id, string codigo, string nombreLimpio, string nombreMostrar, int stockDisponible)
        {
            Id = id;
            Codigo = codigo;
            NombreLimpio = nombreLimpio;
            NombreMostrar = nombreMostrar;
            StockDisponible = stockDisponible;
        }

        public int Id { get; }
        public string Codigo { get; }
        public string NombreLimpio { get; }
        public string NombreMostrar { get; }
        public int StockDisponible { get; }

        public override string ToString()
        {
            return NombreMostrar;
        }
    }

    // Crea la ventana modal para seleccionar un nuevo estado.
    private static Form CrearFormularioCambiarEstado(DataRow datos, List<EstadoItem> estados, out ComboBox cmbNuevoEstado)
    {
        var formulario = new Form
        {
            Text = $"Cambiar estado {datos["codigo"]}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(420, 240),
            MinimumSize = new Size(420, 240),
            Font = new Font("Segoe UI", 10F)
        };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(22),
            ColumnCount = 1,
            RowCount = 6
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        panel.Controls.Add(CrearEtiqueta("Estado actual"), 0, 0);
        panel.Controls.Add(new TextBox
        {
            Text = datos["estado_actual"].ToString(),
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BackColor = Color.White
        }, 0, 1);

        cmbNuevoEstado = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbNuevoEstado.Items.AddRange(estados.Cast<object>().ToArray());

        var estadoActual = Convert.ToInt32(datos["estado_id"]);
        for (var i = 0; i < cmbNuevoEstado.Items.Count; i++)
        {
            if (cmbNuevoEstado.Items[i] is EstadoItem item && item.Id == estadoActual)
            {
                cmbNuevoEstado.SelectedIndex = i;
                break;
            }
        }

        panel.Controls.Add(CrearEtiqueta("Nuevo estado"), 0, 2);
        panel.Controls.Add(cmbNuevoEstado, 0, 3);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnGuardar = new Button { Text = "Guardar", DialogResult = DialogResult.OK, Width = 100, Height = 34 };
        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 100, Height = 34 };
        botones.Controls.Add(btnGuardar);
        botones.Controls.Add(btnCancelar);
        panel.Controls.Add(botones, 0, 5);

        formulario.Controls.Add(panel);
        formulario.AcceptButton = btnGuardar;
        formulario.CancelButton = btnCancelar;
        return formulario;
    }

    // TextBox reutilizable para modales.
    private static TextBox CrearTextBox(string? texto, bool multilinea = false)
    {
        return new TextBox
        {
            Text = texto ?? string.Empty,
            Dock = DockStyle.Fill,
            Multiline = multilinea,
            ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None
        };
    }

    // Etiqueta reutilizable para modales.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft };
    }

    // Objeto simple para mostrar estados en ComboBox conservando su id.
    private sealed class EstadoItem
    {
        public EstadoItem(int? id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public int? Id { get; }
        public string Nombre { get; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
