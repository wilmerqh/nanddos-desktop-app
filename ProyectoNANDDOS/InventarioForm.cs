using System.Data;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Modulo de inventario de repuestos con estilo Fluent Design corporativo.
public class InventarioForm : Form
{
    // Barra de busqueda.
    private readonly TextBox txtBusqueda = new();

    // Tabla de datos.
    private readonly DataGridView dgvInventario = new();

    // Botones de accion.
    private readonly Button btnAgregarNuevo = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEliminar = new();
    private readonly Button btnBuscar = new();

    public InventarioForm()
    {
        InicializarComponentes();
        ConfigurarTablaInventario();
        ConfigurarBotonesInventario();
        CargarInventario();
    }

    // SECCION: construccion visual principal.
    private void InicializarComponentes()
    {
        Text = "Inventario de Repuestos";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout general: titulo, barra de acciones y tabla.
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));   // Titulo
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));   // Barra de acciones
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Tabla

        // Titulo principal.
        principal.Controls.Add(new Label
        {
            Text = "Inventario de Repuestos",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42) // #0F172A
        }, 0, 0);

        // Barra de acciones con busqueda y botones.
        principal.Controls.Add(CrearBarraAcciones(), 0, 1);

        // Tabla de inventario.
        dgvInventario.Dock = DockStyle.Fill;
        principal.Controls.Add(dgvInventario, 0, 2);

        Controls.Add(principal);
    }

    // Crea la barra de busqueda y botones de accion.
    private TableLayoutPanel CrearBarraAcciones()
    {
        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            Padding = new Padding(0)
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Buscador
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); // btnBuscar
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // btnAgregarNuevo
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); // btnEditar
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); // btnEliminar

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.Font = new Font("Segoe UI", 11F);
        txtBusqueda.BorderStyle = BorderStyle.FixedSingle;
        txtBusqueda.PlaceholderText = "Buscar por código o nombre...";
        txtBusqueda.Margin = new Padding(4, 8, 4, 4);
        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
                CargarInventario();
        };

        btnBuscar.Text = "Buscar";
        btnBuscar.Dock = DockStyle.Fill;
        btnBuscar.Margin = new Padding(4);
        btnBuscar.Click += (_, _) => CargarInventario();

        btnAgregarNuevo.Text = "Agregar Nuevo";
        btnAgregarNuevo.Dock = DockStyle.Fill;
        btnAgregarNuevo.Margin = new Padding(4);
        btnAgregarNuevo.Click += (_, _) => 
        {
            using var modal = new RepuestoModalForm();
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CargarInventario();
            }
        };

        btnEditar.Text = "Editar";
        btnEditar.Dock = DockStyle.Fill;
        btnEditar.Margin = new Padding(4);
        btnEditar.Click += (_, _) => 
        {
            if (dgvInventario.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un repuesto de la tabla para editarlo.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvInventario.SelectedRows[0];
            
            var rep = new Repuesto
            {
                IdRepuesto = Convert.ToInt32(fila.Cells["id_repuesto"].Value),
                Codigo = fila.Cells["codigo"]?.Value?.ToString() ?? "",
                Nombre = fila.Cells["nombre"]?.Value?.ToString() ?? "",
                Categoria = fila.Cells["categoria"]?.Value?.ToString() ?? "",
                Stock = Convert.ToInt32(fila.Cells["stock"].Value),
                PrecioCosto = Convert.ToDecimal(fila.Cells["precio_costo"].Value),
                PrecioVenta = Convert.ToDecimal(fila.Cells["precio_venta"].Value),
                FechaIngreso = Convert.ToDateTime(fila.Cells["fecha_ingreso"].Value)
            };

            using var modal = new RepuestoModalForm(rep);
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CargarInventario();
            }
        };

        btnEliminar.Text = "Eliminar";
        btnEliminar.Dock = DockStyle.Fill;
        btnEliminar.Margin = new Padding(4);
        btnEliminar.Click += (_, _) => EliminarRepuesto();

        // Blindaje de Seguridad RBAC
        btnAgregarNuevo.Visible = GestorSeguridad.TienePermiso("inventario_editar");
        btnEditar.Visible = GestorSeguridad.TienePermiso("inventario_editar");
        btnEliminar.Visible = GestorSeguridad.TienePermiso("inventario_editar");

        barra.Controls.Add(txtBusqueda, 0, 0);
        barra.Controls.Add(btnBuscar, 1, 0);
        barra.Controls.Add(btnAgregarNuevo, 2, 0);
        barra.Controls.Add(btnEditar, 3, 0);
        barra.Controls.Add(btnEliminar, 4, 0);

        return barra;
    }

    // Aplica el estilo Fluent Design y los iconos locales a los botones.
    private void ConfigurarBotonesInventario()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        // Helper local para aplicar estilo a cada boton.
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
            btn.Padding = new Padding(8, 0, 0, 0);

            try
            {
                var ruta = Path.Combine(carpetaIconos, archivoIcono);
                if (File.Exists(ruta))
                    btn.Image = Image.FromFile(ruta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NANDDOS] No se pudo cargar el icono '{archivoIcono}': {ex.Message}");
            }
        }

        var azulAcento = Color.FromArgb(37, 99, 235);       // #2563EB
        var azulAcentoHover = Color.FromArgb(29, 78, 216);
        var grisPizarra = Color.FromArgb(71, 85, 105);       // #475569
        var grisPizarraHover = Color.FromArgb(51, 65, 85);
        var rojoSuave = Color.FromArgb(239, 68, 68);         // #EF4444
        var rojoSuaveHover = Color.FromArgb(220, 38, 38);

        AplicarEstilo(btnAgregarNuevo, "btn_guardar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnEditar, "btn_editar.png", grisPizarra, Color.White, grisPizarraHover);
        AplicarEstilo(btnEliminar, "btn_eliminar.png", rojoSuave, Color.White, rojoSuaveHover);
        AplicarEstilo(btnBuscar, "btn_buscar.png", azulAcento, Color.White, azulAcentoHover);
    }

    // Configura el DataGridView con estilo Fluent Design corporativo.
    private void ConfigurarTablaInventario()
    {
        dgvInventario.AllowUserToAddRows = false;
        dgvInventario.AllowUserToDeleteRows = false;
        dgvInventario.AllowUserToOrderColumns = false;
        dgvInventario.AllowUserToResizeColumns = false;
        dgvInventario.AllowUserToResizeRows = false;
        dgvInventario.ReadOnly = true;
        dgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvInventario.MultiSelect = false;
        dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvInventario.RowHeadersVisible = false;

        // Estilo visual general.
        dgvInventario.BackgroundColor = Color.White;
        dgvInventario.BorderStyle = BorderStyle.None;
        dgvInventario.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvInventario.GridColor = Color.FromArgb(226, 232, 240); // #E2E8F0

        // Estilo de encabezados.
        dgvInventario.EnableHeadersVisualStyles = false;
        dgvInventario.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvInventario.ColumnHeadersHeight = 40;
        dgvInventario.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);   // #0F172A
        dgvInventario.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvInventario.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgvInventario.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 23, 42);

        // Estilo de filas y colores alternos.
        dgvInventario.RowTemplate.Height = 35;
        dgvInventario.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        dgvInventario.DefaultCellStyle.BackColor = Color.White;
        dgvInventario.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC

        // Color de seleccion (azul suave con texto oscuro).
        dgvInventario.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);   // #E0F2FE
        dgvInventario.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);       // #0F172A
    }

    // SECCION: logica de datos.

    // Carga todos los repuestos o filtra por texto de busqueda.
    private void CargarInventario()
    {
        try
        {
            var texto = txtBusqueda.Text.Trim();
            var tabla = string.IsNullOrEmpty(texto)
                ? RepuestoDAO.ObtenerTodos()
                : RepuestoDAO.Buscar(texto);

            dgvInventario.DataSource = tabla;

            // Renombrar encabezados a español.
            if (dgvInventario.Columns.Count > 0)
            {
                RenombrarColumnas();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar el inventario.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Renombra las columnas del DataGridView a textos legibles en español.
    private void RenombrarColumnas()
    {
        var nombres = new Dictionary<string, string>
        {
            { "id_repuesto", "ID" },
            { "codigo", "Código" },
            { "nombre", "Nombre" },
            { "categoria", "Categoría" },
            { "stock", "Stock" },
            { "precio_costo", "Precio Costo" },
            { "precio_venta", "Precio Venta" },
            { "fecha_ingreso", "Fecha de Ingreso" }
        };

        foreach (DataGridViewColumn col in dgvInventario.Columns)
        {
            if (nombres.TryGetValue(col.Name, out var nombre))
                col.HeaderText = nombre;
        }

        // Ocultar la columna ID al usuario.
        if (dgvInventario.Columns.Contains("id_repuesto"))
            dgvInventario.Columns["id_repuesto"].Visible = false;
    }

    // Elimina el repuesto seleccionado previa confirmacion.
    private void EliminarRepuesto()
    {
        try
        {
            if (dgvInventario.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un repuesto de la tabla para eliminarlo.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvInventario.SelectedRows[0];
            int idRepuesto = Convert.ToInt32(fila.Cells["id_repuesto"].Value);

            var result = MessageBox.Show(
                "¿Está seguro de eliminar este repuesto? Esta acción no se puede deshacer.",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RepuestoDAO.Eliminar(idRepuesto);
                MessageBox.Show("Repuesto eliminado correctamente.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarInventario();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar el repuesto.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
