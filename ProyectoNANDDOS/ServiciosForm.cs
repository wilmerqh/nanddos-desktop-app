using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Módulo de gestión del catálogo de servicios con estilo Fluent Design corporativo.
public class ServiciosForm : Form
{
    // Controles principales.
    private readonly TextBox txtBusqueda = new();
    private readonly DataGridView dgvServicios = new();

    // Campos del editor.
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtDescripcion = new();
    private readonly TextBox txtPrecio = new();

    // Botones de acción.
    private readonly Button btnBuscar = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnGuardar = new();
    private readonly Button btnEliminar = new();

    // ID del servicio seleccionado para edición (-1 = nuevo).
    private int idServicioActual = -1;

    public ServiciosForm()
    {
        InicializarComponentes();
        ConfigurarTabla();
        ConfigurarBotones();
        AplicarBlindajeRBAC();
        CargarServicios();
    }

    // SECCION: construcción visual principal.
    private void InicializarComponentes()
    {
        Text = "Servicios";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout general: titulo, barra, tabla, editor.
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(8)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));  // Titulo
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));  // Barra de acciones
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Tabla
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 130)); // Editor

        // Titulo.
        principal.Controls.Add(new Label
        {
            Text = "Catálogo de Servicios",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        }, 0, 0);

        // Barra de acciones.
        principal.Controls.Add(CrearBarraAcciones(), 0, 1);

        // Tabla.
        dgvServicios.Dock = DockStyle.Fill;
        dgvServicios.SelectionChanged += (_, _) => CargarSeleccion();
        principal.Controls.Add(dgvServicios, 0, 2);

        // Editor inferior.
        principal.Controls.Add(CrearPanelEditor(), 0, 3);

        Controls.Add(principal);
    }

    // Crea la barra de búsqueda y botones superiores.
    private TableLayoutPanel CrearBarraAcciones()
    {
        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.PlaceholderText = "Buscar por nombre o descripción...";
        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) CargarServicios();
        };

        btnBuscar.Text = "Buscar";
        btnBuscar.Dock = DockStyle.Fill;
        btnBuscar.Margin = new Padding(4);
        btnBuscar.Click += (_, _) => CargarServicios();

        btnNuevo.Text = "Nuevo";
        btnNuevo.Dock = DockStyle.Fill;
        btnNuevo.Margin = new Padding(4);
        btnNuevo.Click += (_, _) => LimpiarEditor();

        btnEliminar.Text = "Eliminar";
        btnEliminar.Dock = DockStyle.Fill;
        btnEliminar.Margin = new Padding(4);
        btnEliminar.Click += (_, _) => EliminarServicio();

        barra.Controls.Add(txtBusqueda, 0, 0);
        barra.Controls.Add(btnBuscar, 1, 0);
        barra.Controls.Add(btnNuevo, 2, 0);
        barra.Controls.Add(btnEliminar, 3, 0);

        return barra;
    }

    // Crea el panel inferior de edición con los campos y el botón Guardar.
    private Panel CrearPanelEditor()
    {
        var panelEditor = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(241, 245, 249), // #F1F5F9
            Padding = new Padding(10)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // Label Nombre
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));   // txtNombre
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));  // Label Descripción
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));   // txtDescripcion
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // btnGuardar
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        // Fila 1: Nombre y Descripción.
        layout.Controls.Add(CrearEtiqueta("Nombre:"), 0, 0);
        txtNombre.Dock = DockStyle.Fill;
        txtNombre.Margin = new Padding(4);
        layout.Controls.Add(txtNombre, 1, 0);

        layout.Controls.Add(CrearEtiqueta("Descripción:"), 2, 0);
        txtDescripcion.Dock = DockStyle.Fill;
        txtDescripcion.Margin = new Padding(4);
        layout.Controls.Add(txtDescripcion, 3, 0);

        // Fila 2: Precio y botón Guardar.
        layout.Controls.Add(CrearEtiqueta("Precio:"), 0, 1);
        txtPrecio.Dock = DockStyle.Fill;
        txtPrecio.Margin = new Padding(4);
        txtPrecio.PlaceholderText = "0.00";
        // Validación: solo permite números y punto decimal.
        txtPrecio.KeyPress += (_, e) =>
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
            // Solo permite un punto o coma decimal.
            if ((e.KeyChar == '.' || e.KeyChar == ',') && txtPrecio.Text.Contains('.'))
            {
                e.Handled = true;
            }
        };
        layout.Controls.Add(txtPrecio, 1, 1);

        btnGuardar.Text = "Guardar";
        btnGuardar.Dock = DockStyle.Fill;
        btnGuardar.Margin = new Padding(4);
        btnGuardar.Click += (_, _) => GuardarServicio();
        layout.Controls.Add(btnGuardar, 4, 0);
        layout.SetRowSpan(btnGuardar, 2);

        panelEditor.Controls.Add(layout);
        return panelEditor;
    }

    // Helper para crear etiquetas del editor.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(4)
        };
    }

    // Aplica estilos Fluent Design a los botones.
    private void ConfigurarBotones()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

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

        var azulAcento = Color.FromArgb(37, 99, 235);
        var azulAcentoHover = Color.FromArgb(29, 78, 216);
        var verdeExito = Color.FromArgb(22, 163, 74);
        var verdeExitoHover = Color.FromArgb(21, 128, 61);
        var grisPizarra = Color.FromArgb(71, 85, 105);
        var grisPizarraHover = Color.FromArgb(51, 65, 85);
        var rojoSuave = Color.FromArgb(239, 68, 68);
        var rojoSuaveHover = Color.FromArgb(220, 38, 38);

        AplicarEstilo(btnBuscar, "btn_buscar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnNuevo, "btn_guardar.png", grisPizarra, Color.White, grisPizarraHover);
        AplicarEstilo(btnGuardar, "btn_guardar.png", verdeExito, Color.White, verdeExitoHover);
        AplicarEstilo(btnEliminar, "btn_eliminar.png", rojoSuave, Color.White, rojoSuaveHover);
    }

    // Configura el DataGridView con estilo Fluent Design corporativo.
    private void ConfigurarTabla()
    {
        dgvServicios.AllowUserToAddRows = false;
        dgvServicios.AllowUserToDeleteRows = false;
        dgvServicios.AllowUserToOrderColumns = false;
        dgvServicios.AllowUserToResizeColumns = false;
        dgvServicios.AllowUserToResizeRows = false;
        dgvServicios.ReadOnly = true;
        dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvServicios.MultiSelect = false;
        dgvServicios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvServicios.RowHeadersVisible = false;

        dgvServicios.BackgroundColor = Color.White;
        dgvServicios.BorderStyle = BorderStyle.None;
        dgvServicios.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvServicios.GridColor = Color.FromArgb(226, 232, 240);

        dgvServicios.EnableHeadersVisualStyles = false;
        dgvServicios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvServicios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvServicios.ColumnHeadersHeight = 40;
        dgvServicios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
        dgvServicios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvServicios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgvServicios.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 23, 42);

        dgvServicios.RowTemplate.Height = 35;
        dgvServicios.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
        dgvServicios.DefaultCellStyle.BackColor = Color.White;
        dgvServicios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvServicios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
        dgvServicios.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
    }

    // SECCION: Blindaje de Seguridad RBAC.
    private void AplicarBlindajeRBAC()
    {
        btnNuevo.Visible = GestorSeguridad.TienePermiso("servicios_editar");
        btnGuardar.Visible = GestorSeguridad.TienePermiso("servicios_editar");
        btnEliminar.Visible = GestorSeguridad.TienePermiso("servicios_eliminar");
    }

    // SECCION: carga de datos.
    private void CargarServicios()
    {
        try
        {
            var tabla = ServicioDAO.ObtenerServicios(txtBusqueda.Text);
            dgvServicios.DataSource = tabla;

            // Ocultar columna de ID y renombrar las visibles.
            if (dgvServicios.Columns["id_servicio"] is DataGridViewColumn colId)
            {
                colId.Visible = false;
            }
            if (dgvServicios.Columns["nombre"] is DataGridViewColumn colNombre)
            {
                colNombre.HeaderText = "Nombre";
                colNombre.FillWeight = 30;
            }
            if (dgvServicios.Columns["descripcion"] is DataGridViewColumn colDesc)
            {
                colDesc.HeaderText = "Descripción";
                colDesc.FillWeight = 45;
            }
            if (dgvServicios.Columns["precio"] is DataGridViewColumn colPrecio)
            {
                colPrecio.HeaderText = "Precio";
                colPrecio.FillWeight = 25;
                colPrecio.DefaultCellStyle.Format = "C2"; // Formato moneda
                colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar los servicios.\n\n{ex.Message}",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Carga los datos del servicio seleccionado en el editor.
    private void CargarSeleccion()
    {
        if (dgvServicios.CurrentRow?.DataBoundItem is not DataRowView fila)
        {
            return;
        }

        idServicioActual = Convert.ToInt32(fila["id_servicio"]);
        txtNombre.Text = fila["nombre"]?.ToString() ?? "";
        txtDescripcion.Text = fila["descripcion"]?.ToString() ?? "";

        // Casteo seguro de DECIMAL para el precio.
        var valorPrecio = fila["precio"];
        if (valorPrecio != null && valorPrecio != DBNull.Value)
        {
            txtPrecio.Text = Convert.ToDecimal(valorPrecio).ToString("0.00");
        }
        else
        {
            txtPrecio.Text = "0.00";
        }
    }

    // Limpia el editor para crear un nuevo servicio.
    private void LimpiarEditor()
    {
        idServicioActual = -1;
        txtNombre.Clear();
        txtDescripcion.Clear();
        txtPrecio.Clear();
        dgvServicios.ClearSelection();
        txtNombre.Focus();
    }

    // SECCION: guardar (insertar o actualizar).
    private void GuardarServicio()
    {
        // Validación de campos obligatorios.
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("El campo Nombre es obligatorio.",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNombre.Focus();
            return;
        }

        // Validación y parseo seguro del precio.
        string textoPrecio = txtPrecio.Text.Trim().Replace(",", ".");
        if (string.IsNullOrWhiteSpace(textoPrecio))
        {
            textoPrecio = "0";
        }

        if (!decimal.TryParse(textoPrecio, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal precio))
        {
            MessageBox.Show("El precio ingresado no es válido. Use formato numérico (ej: 150.50).",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPrecio.Focus();
            return;
        }

        try
        {
            if (idServicioActual == -1)
            {
                // Insertar nuevo.
                ServicioDAO.InsertarServicio(txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio);
                MessageBox.Show("Servicio registrado correctamente.",
                    "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Actualizar existente.
                ServicioDAO.ActualizarServicio(idServicioActual, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio);
                MessageBox.Show("Servicio actualizado correctamente.",
                    "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CargarServicios();
            LimpiarEditor();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar el servicio.\n\n{ex.Message}",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // SECCION: eliminar servicio.
    private void EliminarServicio()
    {
        if (idServicioActual == -1)
        {
            MessageBox.Show("Selecciona un servicio de la tabla para eliminarlo.",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var resultado = MessageBox.Show(
            $"¿Estás seguro de eliminar el servicio \"{txtNombre.Text}\"?",
            "Confirmar Eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (resultado != DialogResult.Yes) return;

        try
        {
            ServicioDAO.EliminarServicio(idServicioActual);
            MessageBox.Show("Servicio eliminado correctamente.",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarServicios();
            LimpiarEditor();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar el servicio.\n\n{ex.Message}",
                "Servicios", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
