using MySql.Data.MySqlClient;
using System.Data;

namespace ProyectoNANDDOS;

// Modulo para consultar, editar y eliminar clientes.
public class ClientesForm : Form
{
    // Controles principales del modulo.
    private readonly TextBox txtBusqueda = new();
    private readonly DataGridView dgvClientes = new();

    // Botones de accion.
    private readonly Button btnBuscar = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEliminar = new();

    public ClientesForm()
    {
        InicializarComponentes();
        ConfigurarTablaClientes();
        ConfigurarBotonesClientes();
        CargarClientes();
    }

    // SECCION: construccion visual.
    private void InicializarComponentes()
    {
        Text = "Clientes";
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
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(new Label
        {
            Text = "Clientes",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.FromArgb(25, 35, 50)
        }, 0, 0);

        // Barra superior con buscador y botones de accion.
        var barra = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.CharacterCasing = CharacterCasing.Lower;
        txtBusqueda.PlaceholderText = "Buscar por código, nombre o teléfono";
        // Permite buscar al presionar Enter.
        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                CargarClientes();
            }
        };

        btnBuscar.Text = "Buscar";
        btnBuscar.Click += (_, _) => CargarClientes();
        
        btnEditar.Text = "Editar";
        btnEditar.Click += (_, _) => EditarCliente();
        
        btnEliminar.Text = "Eliminar";
        btnEliminar.Click += (_, _) => EliminarCliente();

        barra.Controls.Add(txtBusqueda, 0, 0);
        barra.Controls.Add(btnBuscar, 1, 0);
        barra.Controls.Add(btnEditar, 2, 0);
        barra.Controls.Add(btnEliminar, 3, 0);

        // Blindaje de Seguridad RBAC
        btnEditar.Visible = GestorSeguridad.TienePermiso("clientes_editar");
        btnEliminar.Visible = GestorSeguridad.TienePermiso("clientes_editar");

        principal.Controls.Add(barra, 0, 1);
        principal.Controls.Add(dgvClientes, 0, 2);
        Controls.Add(principal);
    }

    // Aplica el estilo Fluent Design y los iconos locales a los botones de accion de Clientes.
    private void ConfigurarBotonesClientes()
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
        var grisPizarra = Color.FromArgb(71, 85, 105); // #475569
        var grisPizarraHover = Color.FromArgb(51, 65, 85);
        var rojoSuave = Color.FromArgb(239, 68, 68); // #EF4444
        var rojoSuaveHover = Color.FromArgb(220, 38, 38);

        AplicarEstilo(btnBuscar, "btn_buscar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnEditar, "btn_editar.png", grisPizarra, Color.White, grisPizarraHover);
        AplicarEstilo(btnEliminar, "btn_eliminar.png", rojoSuave, Color.White, rojoSuaveHover);
    }

    // Configura la tabla dgvClientes para lectura, seleccion completa y estilo corporativo.
    private void ConfigurarTablaClientes()
    {
        dgvClientes.Dock = DockStyle.Fill;
        dgvClientes.AllowUserToAddRows = false;
        dgvClientes.AllowUserToDeleteRows = false;
        dgvClientes.AllowUserToOrderColumns = false;
        dgvClientes.AllowUserToResizeColumns = false;
        dgvClientes.AllowUserToResizeRows = false;
        dgvClientes.ReadOnly = true;
        dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvClientes.MultiSelect = false;
        dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvClientes.RowHeadersVisible = false;

        // Estilo visual general
        dgvClientes.BackgroundColor = Color.White;
        dgvClientes.BorderStyle = BorderStyle.None;
        dgvClientes.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvClientes.GridColor = Color.FromArgb(226, 232, 240); // #E2E8F0

        // Estilo de Encabezados (Headers)
        dgvClientes.EnableHeadersVisualStyles = false;
        dgvClientes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvClientes.ColumnHeadersHeight = 40;
        dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // #0F172A
        dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        
        // REGLA CRITICA: Uso exclusivo de FontStyle.Bold
        dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgvClientes.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 23, 42);

        // Estilo de Filas (Rows) y Colores Alternos
        dgvClientes.RowTemplate.Height = 35;
        dgvClientes.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        dgvClientes.DefaultCellStyle.BackColor = Color.White;
        dgvClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC
        
        // Seleccion (Azul suave)
        dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254); // #E0F2FE
        dgvClientes.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42); // #0F172A
    }

    // SECCION: carga de datos.
    private void CargarClientes()
    {
        using var conexion = ConexionDB.ObtenerConexion();
        var busqueda = txtBusqueda.Text.Trim().ToLower();
        using var comando = new MySqlCommand("""
            SELECT id, codigo AS Código, nombres AS Nombres, apellidos AS Apellidos, email AS Email, telefono AS Teléfono
            FROM clientes
            WHERE LOWER(codigo) LIKE @busqueda
               OR LOWER(CONCAT(nombres, ' ', apellidos)) LIKE @busqueda
               OR telefono LIKE @busqueda
            ORDER BY nombres, apellidos;
            """, conexion);
        comando.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");

        // Llena un DataTable y lo muestra en el DataGridView.
        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        dgvClientes.DataSource = tabla;

        if (dgvClientes.Columns["id"] is DataGridViewColumn columnaId)
        {
            columnaId.Visible = false;
        }

        ConfigurarColumnasClientes();
    }

    // Define orden y proporcion visual de columnas.
    private void ConfigurarColumnasClientes()
    {
        ConfigurarColumna("Código", 0, 14);
        ConfigurarColumna("Nombres", 1, 22);
        ConfigurarColumna("Apellidos", 2, 22);
        ConfigurarColumna("Email", 3, 24);
        ConfigurarColumna("Teléfono", 4, 16);
    }

    // Aplica configuracion a una columna si existe en la tabla.
    private void ConfigurarColumna(string nombre, int orden, float peso)
    {
        if (dgvClientes.Columns[nombre] is not DataGridViewColumn columna)
        {
            return;
        }

        columna.DisplayIndex = orden;
        columna.FillWeight = peso;
        columna.MinimumWidth = 80;
        columna.Resizable = DataGridViewTriState.False;
    }

    // Devuelve el id interno del cliente seleccionado.
    private int? ObtenerClienteSeleccionado()
    {
        if (dgvClientes.CurrentRow?.DataBoundItem is not DataRowView fila)
        {
            MessageBox.Show("Selecciona un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        return Convert.ToInt32(fila["id"]);
    }

    // SECCION: editar cliente.
    private void EditarCliente()
    {
        var id = ObtenerClienteSeleccionado();
        if (id is null)
        {
            return;
        }

        var datos = ObtenerCliente(id.Value);
        using var formulario = CrearFormularioEdicion(datos, out var txtNombres, out var txtApellidos, out var txtEmail, out var txtTelefono);

        if (formulario.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(txtNombres.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            MessageBox.Show("Nombres y apellidos son obligatorios.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Guarda los cambios del cliente en MySQL.
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            UPDATE clientes
            SET nombres = @nombres, apellidos = @apellidos, email = @email, telefono = @telefono
            WHERE id = @id;
            """, conexion);
        comando.Parameters.AddWithValue("@nombres", txtNombres.Text.Trim());
        comando.Parameters.AddWithValue("@apellidos", txtApellidos.Text.Trim());
        comando.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
        comando.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
        comando.Parameters.AddWithValue("@id", id.Value);
        comando.ExecuteNonQuery();

        CargarClientes();
        MessageBox.Show("Cliente actualizado correctamente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // SECCION: eliminar cliente.
    private void EliminarCliente()
    {
        var id = ObtenerClienteSeleccionado();
        if (id is null)
        {
            return;
        }

        // No se elimina si tiene equipos asociados.
        using var conexion = ConexionDB.ObtenerConexion();
        using var contar = new MySqlCommand("SELECT COUNT(*) FROM equipos WHERE cliente_id = @id;", conexion);
        contar.Parameters.AddWithValue("@id", id.Value);
        var equipos = Convert.ToInt32(contar.ExecuteScalar());

        if (equipos > 0)
        {
            MessageBox.Show("No se puede eliminar un cliente con equipos registrados.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("¿Eliminar este cliente?", "Clientes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        using var eliminar = new MySqlCommand("DELETE FROM clientes WHERE id = @id;", conexion);
        eliminar.Parameters.AddWithValue("@id", id.Value);
        eliminar.ExecuteNonQuery();
        CargarClientes();
    }

    // Obtiene los datos actuales de un cliente para editarlo.
    private static DataRow ObtenerCliente(int id)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("SELECT codigo, nombres, apellidos, email, telefono FROM clientes WHERE id = @id;", conexion);
        comando.Parameters.AddWithValue("@id", id);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);
        return tabla.Rows[0];
    }

    // Crea una ventana pequena para editar datos basicos del cliente.
    private static Form CrearFormularioEdicion(DataRow datos, out TextBox txtNombres, out TextBox txtApellidos, out TextBox txtEmail, out TextBox txtTelefono)
    {
        var formulario = new Form
        {
            Text = $"Editar cliente {datos["codigo"]}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(440, 330),
            MinimumSize = new Size(440, 330),
            Font = new Font("Segoe UI", 10F)
        };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 1,
            RowCount = 9
        };

        txtNombres = CrearTextBox(datos["nombres"].ToString());
        txtApellidos = CrearTextBox(datos["apellidos"].ToString());
        txtEmail = CrearTextBox(datos["email"].ToString());
        txtTelefono = CrearTextBox(datos["telefono"].ToString());

        panel.Controls.Add(CrearEtiqueta("Nombres"), 0, 0);
        panel.Controls.Add(txtNombres, 0, 1);
        panel.Controls.Add(CrearEtiqueta("Apellidos"), 0, 2);
        panel.Controls.Add(txtApellidos, 0, 3);
        panel.Controls.Add(CrearEtiqueta("Email"), 0, 4);
        panel.Controls.Add(txtEmail, 0, 5);
        panel.Controls.Add(CrearEtiqueta("Teléfono"), 0, 6);
        panel.Controls.Add(txtTelefono, 0, 7);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnGuardar = new Button { Text = "Guardar", DialogResult = DialogResult.OK, Width = 100, Height = 34 };
        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 100, Height = 34 };
        botones.Controls.Add(btnGuardar);
        botones.Controls.Add(btnCancelar);
        panel.Controls.Add(botones, 0, 8);

        formulario.AcceptButton = btnGuardar;
        formulario.CancelButton = btnCancelar;
        formulario.Controls.Add(panel);
        return formulario;
    }

    // TextBox estandar para formularios de edicion.
    private static TextBox CrearTextBox(string? texto)
    {
        return new TextBox { Text = texto ?? string.Empty, Dock = DockStyle.Fill, MaxLength = 120 };
    }

    // Etiqueta estandar para formularios de edicion.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft };
    }
}
