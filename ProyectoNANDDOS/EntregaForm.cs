using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using PdfDocument = iTextSharp.text.Document;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfPTable = iTextSharp.text.pdf.PdfPTable;
using PdfPCell = iTextSharp.text.pdf.PdfPCell;
using PdfPhrase = iTextSharp.text.Phrase;
using PdfImage = iTextSharp.text.Image;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using PdfBaseColor = iTextSharp.text.BaseColor;
using PdfFont = iTextSharp.text.Font;
using PdfElement = iTextSharp.text.Element;
using PdfPageEventHelper = iTextSharp.text.pdf.PdfPageEventHelper;
using PdfGState = iTextSharp.text.pdf.PdfGState;

namespace ProyectoNANDDOS;

// Modulo para registrar la entrega de equipos y generar comprobantes PDF.
public class EntregaForm : Form
{
    // SECCION: controles de busqueda y datos encontrados.
    private readonly TextBox txtCodigoBusqueda = new();
    private readonly TextBox txtCliente = new();
    private readonly TextBox txtTelefono = new();
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtEquipo = new();
    private readonly TextBox txtProblema = new();
    private readonly TextBox txtRepuestosUsados = new();
    
    // Controles financieros
    private readonly TextBox txtPrecioRepuestos = new();
    private readonly Button btnAgregarProductoExtra = new();
    private readonly TextBox txtCostoServicio = new();
    private readonly TextBox txtCostoTotal = new();
    
    private readonly DateTimePicker dtpFechaEntrega = new();
    private readonly TextBox txtResumen = new();
    private readonly Button btnGenerar = new();
    
    // Control de repuestos extra a descontar al generar la entrega
    private readonly List<(int IdRepuesto, int Cantidad)> repuestosAdescontar = new();
    // Id interno del equipo encontrado. El usuario solo ve codigos visibles.
    private int? equipoId;

    // Modelo interno con todos los datos que necesita el comprobante PDF.
    private sealed class DatosComprobante
    {
        public string CodigoEntrega { get; init; } = string.Empty;
        public string CodigoEquipo { get; init; } = string.Empty;
        public string Cliente { get; init; } = string.Empty;
        public string Telefono { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Equipo { get; init; } = string.Empty;
        public string Problema { get; init; } = string.Empty;
        public string Estado { get; init; } = string.Empty;
        public string Diagnostico { get; init; } = string.Empty;
        public string RepuestosUsados { get; init; } = string.Empty;
        public decimal CostoTotal { get; init; }
        public DateTime FechaEntrega { get; init; }
    }

    public EntregaForm()
    {
        InicializarComponentes();
        ConfigurarInterfazEntrega();
        LimpiarEquipo();
    }

    // SECCION: construccion visual.
    private void InicializarComponentes()
    {
        Text = "Entrega de Equipo";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout general: titulo, busqueda, datos, entrega y boton final.
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(8)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 172));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

        principal.Controls.Add(new Label
        {
            Text = "Entrega de Equipo",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.FromArgb(25, 35, 50)
        }, 0, 0);

        principal.Controls.Add(CrearBarraBusqueda(), 0, 1);
        principal.Controls.Add(CrearDatosEquipo(), 0, 2);
        principal.Controls.Add(CrearDatosEntrega(), 0, 3);

        btnGenerar.Text = "Generar entrega";
        btnGenerar.Dock = DockStyle.Right;
        btnGenerar.Width = 170;
        btnGenerar.BackColor = Color.FromArgb(33, 111, 219);
        btnGenerar.ForeColor = Color.White;
        btnGenerar.FlatStyle = FlatStyle.Flat;
        btnGenerar.FlatAppearance.BorderSize = 0;
        btnGenerar.Click += (_, _) => GenerarEntrega();

        var panelBoton = new Panel { Dock = DockStyle.Fill };
        panelBoton.Controls.Add(btnGenerar);
        principal.Controls.Add(panelBoton, 0, 4);
        Controls.Add(principal);
    }

    // Aplica el estilo Fluent Design a los controles existentes sin alterar la logica de negocio.
    private void ConfigurarInterfazEntrega()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        // Helper recursivo para aplicar estilos automaticamente sin depender de variables locales.
        void AplicarEstiloRecursivo(Control.ControlCollection controles)
        {
            foreach (Control ctrl in controles)
            {
                if (ctrl is GroupBox gb)
                {
                    // Regla estricta: FontStyle.Bold
                    gb.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                    gb.ForeColor = Color.FromArgb(15, 23, 42); // #0F172A
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                    
                    if (txt.ReadOnly)
                    {
                        // Campos de solo lectura (Equipo encontrado)
                        txt.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC
                        txt.ForeColor = Color.FromArgb(51, 65, 85); // #334155
                    }
                    else
                    {
                        // Campos editables (Datos de entrega y Resumen)
                        txt.BackColor = Color.White;
                    }
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    dtp.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                }
                else if (ctrl is NumericUpDown nud)
                {
                    nud.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                }
                else if (ctrl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Cursor = Cursors.Hand;
                    
                    if (btn.Text == "Generar entrega")
                    {
                        btn.BackColor = Color.FromArgb(37, 99, 235); // #2563EB
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                        
                        try
                        {
                            var ruta = Path.Combine(carpetaIconos, "btn_generar.png");
                            if (File.Exists(ruta))
                            {
                                btn.Image = Image.FromFile(ruta);
                                btn.ImageAlign = ContentAlignment.MiddleLeft;
                                btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                                btn.Padding = new Padding(12, 0, 0, 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[NANDDOS] No se pudo cargar el icono 'btn_generar.png': {ex.Message}");
                        }
                    }
                    else if (btn.Text == "Buscar")
                    {
                        btn.BackColor = Color.FromArgb(241, 245, 249); // #F1F5F9
                        btn.ForeColor = Color.FromArgb(15, 23, 42); // #0F172A
                        btn.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                    }
                }
                
                // Recorrer los hijos de forma recursiva
                if (ctrl.HasChildren)
                {
                    AplicarEstiloRecursivo(ctrl.Controls);
                }
            }
        }
        
        AplicarEstiloRecursivo(this.Controls);
    }

    // SECCION: busqueda de equipo por codigo visible.
    private Control CrearBarraBusqueda()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

        txtCodigoBusqueda.Dock = DockStyle.Fill;
        txtCodigoBusqueda.PlaceholderText = "Buscar equipo por código, ejemplo LP-0001";
        txtCodigoBusqueda.CharacterCasing = CharacterCasing.Upper;
        // Permite buscar con Enter.
        txtCodigoBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarEquipo();
            }
        };

        var btnBuscar = new Button
        {
            Text = "Buscar",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White
        };
        btnBuscar.Click += (_, _) => BuscarEquipo();

        panel.Controls.Add(txtCodigoBusqueda, 0, 0);
        panel.Controls.Add(btnBuscar, 1, 0);
        return panel;
    }

    // SECCION: datos autocompletados del equipo.
    private Control CrearDatosEquipo()
    {
        var grupo = new GroupBox
        {
            Text = "Equipo encontrado",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 4 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        PrepararSoloLectura(txtCliente);
        PrepararSoloLectura(txtTelefono);
        PrepararSoloLectura(txtEmail);
        PrepararSoloLectura(txtEquipo);
        PrepararSoloLectura(txtProblema, true);

        panel.Controls.Add(CrearEtiqueta("Cliente"), 0, 0);
        panel.Controls.Add(CrearEtiqueta("Teléfono"), 1, 0);
        panel.Controls.Add(CrearEtiqueta("Email"), 2, 0);
        panel.Controls.Add(txtCliente, 0, 1);
        panel.Controls.Add(txtTelefono, 1, 1);
        panel.Controls.Add(txtEmail, 2, 1);
        panel.Controls.Add(CrearEtiqueta("Equipo"), 0, 2);
        var lblProblema = CrearEtiqueta("Problema");
        panel.Controls.Add(lblProblema, 1, 2);
        panel.SetColumnSpan(lblProblema, 2);
        panel.Controls.Add(txtEquipo, 0, 3);
        panel.Controls.Add(txtProblema, 1, 3);
        panel.SetColumnSpan(txtProblema, 2);

        grupo.Controls.Add(panel);
        return grupo;
    }

    // SECCION: datos que completa el usuario para la entrega.
    private Control CrearDatosEntrega()
    {
        var contenedor = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

        var grupoDatos = new GroupBox { Text = "Datos de entrega y facturación", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var panelDatos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 8 };
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

        PrepararTexto(txtRepuestosUsados, "Repuestos usados (Automático/Manual)", true);
        PrepararSoloLectura(txtPrecioRepuestos);
        txtPrecioRepuestos.Text = "0.00";
        
        btnAgregarProductoExtra.Text = "+ Extra";
        btnAgregarProductoExtra.BackColor = Color.FromArgb(241, 245, 249);
        btnAgregarProductoExtra.FlatStyle = FlatStyle.Flat;
        btnAgregarProductoExtra.FlatAppearance.BorderSize = 0;
        btnAgregarProductoExtra.Dock = DockStyle.Fill;
        btnAgregarProductoExtra.Click += (_, _) => AgregarProductoExtra();

        PrepararTexto(txtCostoServicio, "Ej. 50.00");
        txtCostoServicio.Text = "0.00";
        txtCostoServicio.TextChanged += (_, _) => CalcularTotal();
        txtCostoServicio.KeyPress += (sender, e) => 
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) { e.Handled = true; }
            if ((e.KeyChar == '.') && (((TextBox)sender!).Text.IndexOf('.') > -1)) { e.Handled = true; }
        };

        PrepararSoloLectura(txtCostoTotal);
        txtCostoTotal.Text = "0.00";
        txtCostoTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        txtCostoTotal.ForeColor = Color.FromArgb(127, 29, 29);

        dtpFechaEntrega.Dock = DockStyle.Fill;
        dtpFechaEntrega.Format = DateTimePickerFormat.Short;
        dtpFechaEntrega.Value = DateTime.Today;

        int row = 0;
        var lblRepuestos = CrearEtiqueta("Repuestos usados");
        panelDatos.Controls.Add(lblRepuestos, 0, row);
        panelDatos.SetColumnSpan(lblRepuestos, 3);
        panelDatos.Controls.Add(txtRepuestosUsados, 0, ++row);
        panelDatos.SetColumnSpan(txtRepuestosUsados, 3);

        panelDatos.Controls.Add(CrearEtiqueta("Precio Repuestos"), 0, ++row);
        panelDatos.Controls.Add(CrearEtiqueta("Costo Servicio"), 1, row);
        panelDatos.Controls.Add(CrearEtiqueta("Fecha Entrega"), 2, row);
        
        row++;
        var panelRepuestosLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        panelRepuestosLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        panelRepuestosLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        panelRepuestosLayout.Controls.Add(txtPrecioRepuestos, 0, 0);
        panelRepuestosLayout.Controls.Add(btnAgregarProductoExtra, 1, 0);

        panelDatos.Controls.Add(panelRepuestosLayout, 0, row);
        panelDatos.Controls.Add(txtCostoServicio, 1, row);
        panelDatos.Controls.Add(dtpFechaEntrega, 2, row);

        var lblTotal = CrearEtiqueta("TOTAL A COBRAR:");
        lblTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        panelDatos.Controls.Add(lblTotal, 0, ++row);
        panelDatos.SetColumnSpan(lblTotal, 3);
        panelDatos.Controls.Add(txtCostoTotal, 0, ++row);
        panelDatos.SetColumnSpan(txtCostoTotal, 3);

        grupoDatos.Controls.Add(panelDatos);

        var grupoResumen = new GroupBox { Text = "Resumen", Dock = DockStyle.Fill, Padding = new Padding(12) };
        PrepararSoloLectura(txtResumen, true);
        txtResumen.Multiline = true;
        txtResumen.ScrollBars = ScrollBars.Vertical;
        grupoResumen.Controls.Add(txtResumen);

        contenedor.Controls.Add(grupoDatos, 0, 0);
        contenedor.Controls.Add(grupoResumen, 1, 0);
        return contenedor;
    }

    private void CalcularTotal()
    {
        decimal repuestos = 0;
        decimal servicio = 0;

        decimal.TryParse(txtPrecioRepuestos.Text, out repuestos);
        decimal.TryParse(txtCostoServicio.Text, out servicio);

        decimal total = repuestos + servicio;
        txtCostoTotal.Text = total.ToString("0.00");
    }

    private void AgregarProductoExtra()
    {
        using var modal = new SelectorRepuestosForm();
        if (modal.ShowDialog() == DialogResult.OK && modal.RepuestoSeleccionado != null)
        {
            SelectorRepuestosForm.RepuestoComboItem rep = modal.RepuestoSeleccionado;
            int cantidad = modal.CantidadSeleccionada;
            decimal subtotal = rep.PrecioVenta * cantidad;

            // Agrega al texto de repuestos usados
            string linea = $"{cantidad}x {rep.Nombre} (${rep.PrecioVenta:0.00}) = ${subtotal:0.00}";
            if (string.IsNullOrWhiteSpace(txtRepuestosUsados.Text))
                txtRepuestosUsados.Text = linea;
            else
                txtRepuestosUsados.Text += Environment.NewLine + linea;

            // Suma al textbox
            decimal.TryParse(txtPrecioRepuestos.Text, out decimal actual);
            txtPrecioRepuestos.Text = (actual + subtotal).ToString("0.00");
            CalcularTotal();

            // Cola el descuento para el momento de generar la entrega.
            repuestosAdescontar.Add((rep.Id, cantidad));
        }
    }

    // Etiqueta reutilizable del formulario.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft };
    }

    // Configura campos que muestran informacion sin permitir edicion.
    private static void PrepararSoloLectura(TextBox textBox, bool multilinea = false)
    {
        PrepararTexto(textBox, string.Empty, multilinea);
        textBox.ReadOnly = true;
        textBox.BackColor = Color.White;
    }

    // Configura TextBox normales y multilinea.
    private static void PrepararTexto(TextBox textBox, string placeholder, bool multilinea = false)
    {
        textBox.Dock = DockStyle.Fill;
        textBox.PlaceholderText = placeholder;
        textBox.Multiline = multilinea;
        textBox.ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None;
    }

    // SECCION: busqueda y validacion de equipo.
    private void BuscarEquipo()
    {
        var codigo = txtCodigoBusqueda.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigo))
        {
            MessageBox.Show("Ingresa el código del equipo.", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Busca el equipo por codigo visible y carga cliente, problema y estado.
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT
                e.id,
                e.codigo,
                CONCAT(c.nombres, ' ', c.apellidos) AS cliente,
                c.telefono,
                c.email,
                CONCAT(n.prefijo, ' - ', n.descripcion, ' ', IFNULL(e.marca, ''), ' ', IFNULL(e.modelo, '')) AS equipo,
                e.descripcion_problema AS problema,
                es.nombre AS estado,
                e.repuestos_necesarios
            FROM equipos e
            INNER JOIN clientes c ON c.id = e.cliente_id
            INNER JOIN nomenclaturas n ON n.id = e.nomenclatura_id
            INNER JOIN estados es ON es.id = e.estado_id
            WHERE e.codigo = @codigo
            LIMIT 1;
            """, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);

        if (tabla.Rows.Count == 0)
        {
            LimpiarEquipo();
            MessageBox.Show("No se encontró un equipo con ese código.", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var fila = tabla.Rows[0];
        equipoId = Convert.ToInt32(fila["id"]);
        txtCliente.Text = fila["cliente"].ToString();
        txtTelefono.Text = fila["telefono"].ToString();
        txtEmail.Text = fila["email"].ToString();
        txtEquipo.Text = fila["equipo"].ToString();
        txtProblema.Text = fila["problema"].ToString();
        txtResumen.Clear();
        btnGenerar.Enabled = true;

        // Calcular costo de repuestos reales mediante la BD (usando EntregaDAO)
        txtRepuestosUsados.Text = fila["repuestos_necesarios"]?.ToString() ?? "";
        
        decimal costoRepuestos = EntregaDAO.ObtenerCostoTotalRepuestos(equipoId.Value);
        txtPrecioRepuestos.Text = costoRepuestos.ToString("0.00");
        CalcularTotal();

        // Si ya fue entregado, no duplica entrega y ofrece regenerar PDF.
        if (fila["estado"].ToString() == "Entregado")
        {
            btnGenerar.Enabled = false;
            PreguntarRegenerarComprobante(equipoId.Value);
        }
    }

    // SECCION: crear entrega nueva.
    private void GenerarEntrega()
    {
        if (equipoId is null)
        {
            MessageBox.Show("Primero busca un equipo por código.", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // La transaccion asegura que entrega, estado, PDF y repuestos queden sincronizados.
        using var conexion = ConexionDB.ObtenerConexion();
        using var transaccion = conexion.BeginTransaction();
        string? rutaPdf = null;

        try
        {
            // Doble validacion para evitar entregas duplicadas.
            if (EquipoYaEntregado(equipoId.Value, conexion, transaccion))
            {
                transaccion.Rollback();
                PreguntarRegenerarComprobante(equipoId.Value);
                return;
            }

            var codigoEntrega = GeneradorCodigo.GenerarCodigoEntrega(conexion, transaccion);
            var resumen = CrearResumen(codigoEntrega);
            txtResumen.Text = resumen;

            if (MessageBox.Show($"{resumen}\n\n¿Confirmar entrega?", "Confirmar entrega", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                transaccion.Rollback();
                return;
            }

            rutaPdf = CrearRutaPdf(codigoEntrega);
            GenerarPdf(codigoEntrega, rutaPdf);
            var estadoEntregadoId = ObtenerEstadoEntregadoId(conexion, transaccion);

            // Guarda la entrega con la ruta del PDF generado.
            using var insertar = new MySqlCommand("""
                INSERT INTO entregas (codigo, equipo_id, diagnostico, repuestos_usados, costo_repuestos, costo_servicio, costo_extras, descripcion_extras, total_cobrado, costo_total, fecha_entrega, pdf_path)
                VALUES (@codigo, @equipo_id, @diagnostico, @repuestos_usados, @costo_repuestos, @costo_servicio, @costo_extras, @descripcion_extras, @total_cobrado, @costo_total, @fecha_entrega, @pdf_path);
                """, conexion, transaccion);
            insertar.Parameters.AddWithValue("@codigo", codigoEntrega);
            insertar.Parameters.AddWithValue("@equipo_id", equipoId.Value);
            insertar.Parameters.AddWithValue("@diagnostico", txtProblema.Text.Trim()); // Usamos problema en vez del manual
            insertar.Parameters.AddWithValue("@repuestos_usados", txtRepuestosUsados.Text.Trim());
            
            decimal.TryParse(txtPrecioRepuestos.Text, out decimal repuestos);
            decimal.TryParse(txtCostoServicio.Text, out decimal servicio);
            decimal.TryParse(txtCostoTotal.Text, out decimal total);

            insertar.Parameters.AddWithValue("@costo_repuestos", repuestos);
            insertar.Parameters.AddWithValue("@costo_servicio", servicio);
            insertar.Parameters.AddWithValue("@costo_extras", 0m);
            insertar.Parameters.AddWithValue("@descripcion_extras", string.Empty);
            insertar.Parameters.AddWithValue("@total_cobrado", total);
            insertar.Parameters.AddWithValue("@costo_total", total); // Mantenemos compatibilidad con el schema antiguo si aún existe
            
            insertar.Parameters.AddWithValue("@fecha_entrega", dtpFechaEntrega.Value.Date);
            insertar.Parameters.AddWithValue("@pdf_path", rutaPdf);
            insertar.ExecuteNonQuery();

            // Cambia el estado del equipo a Entregado.
            using var actualizar = new MySqlCommand("UPDATE equipos SET estado_id = @estado_id WHERE id = @equipo_id;", conexion, transaccion);
            actualizar.Parameters.AddWithValue("@estado_id", estadoEntregadoId);
            actualizar.Parameters.AddWithValue("@equipo_id", equipoId.Value);
            actualizar.ExecuteNonQuery();

            // CRÍTICO: Descuenta los repuestos extra agregados en esta entrega.
            foreach (var repExtra in repuestosAdescontar)
            {
                using var cmdStock = new MySqlCommand("UPDATE repuestos SET stock = stock - @cantidad WHERE id = @id;", conexion, transaccion);
                cmdStock.Parameters.AddWithValue("@cantidad", repExtra.Cantidad);
                cmdStock.Parameters.AddWithValue("@id", repExtra.IdRepuesto);
                cmdStock.ExecuteNonQuery();
            }

            transaccion.Commit();
            MessageBox.Show($"Entrega generada correctamente.\n\nPDF: {rutaPdf}", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AbrirPdf(rutaPdf);
            LimpiarFormularioEntrega();
        }
        catch (Exception ex)
        {
            try
            {
                transaccion.Rollback();
            }
            catch
            {
                // La transacción puede estar cerrada si ya se hizo rollback por una entrega existente.
            }

            if (rutaPdf is not null && File.Exists(rutaPdf))
            {
                File.Delete(rutaPdf);
            }

            MessageBox.Show($"No se pudo generar la entrega.\n\n{ex.Message}", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Pregunta si se desea reimprimir un comprobante de una entrega existente.
    private void PreguntarRegenerarComprobante(int idEquipo)
    {
        var respuesta = MessageBox.Show(
            "Este equipo ya fue entregado anteriormente.\n\n¿Desea generar nuevamente el comprobante PDF?",
            "Entrega",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (respuesta != DialogResult.Yes)
        {
            return;
        }

        RegenerarComprobanteExistente(idEquipo);
    }

    // Regenera el PDF usando datos ya guardados, sin crear otra entrega.
    private void RegenerarComprobanteExistente(int idEquipo)
    {
        try
        {
            var datos = ObtenerDatosComprobanteExistente(idEquipo);
            if (datos is null)
            {
                MessageBox.Show("No se encontró una entrega anterior para regenerar el comprobante.", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var rutaPdf = CrearRutaPdf(datos.CodigoEntrega);
            GenerarPdf(datos, rutaPdf);
            ActualizarRutaPdfEntrega(datos.CodigoEntrega, rutaPdf);

            MessageBox.Show($"Comprobante PDF regenerado correctamente.\n\nPDF: {rutaPdf}", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AbrirPdf(rutaPdf);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo regenerar el comprobante PDF.\n\n{ex.Message}", "Entrega", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Recupera datos dinamicos desde MySQL para reimprimir el comprobante.
    private static DatosComprobante? ObtenerDatosComprobanteExistente(int idEquipo)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT
                ent.codigo AS codigo_entrega,
                e.codigo AS codigo_equipo,
                CONCAT(c.nombres, ' ', c.apellidos) AS cliente,
                c.telefono,
                c.email,
                CONCAT(n.prefijo, ' - ', n.descripcion, ' ', IFNULL(e.marca, ''), ' ', IFNULL(e.modelo, '')) AS equipo,
                e.descripcion_problema AS problema,
                es.nombre AS estado,
                ent.diagnostico,
                ent.repuestos_usados,
                ent.costo_total,
                ent.fecha_entrega
            FROM entregas ent
            INNER JOIN equipos e ON e.id = ent.equipo_id
            INNER JOIN clientes c ON c.id = e.cliente_id
            INNER JOIN nomenclaturas n ON n.id = e.nomenclatura_id
            INNER JOIN estados es ON es.id = e.estado_id
            WHERE ent.equipo_id = @equipo_id
            ORDER BY ent.fecha_creacion DESC, ent.id DESC
            LIMIT 1;
            """, conexion);
        comando.Parameters.AddWithValue("@equipo_id", idEquipo);

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);

        if (tabla.Rows.Count == 0)
        {
            return null;
        }

        var fila = tabla.Rows[0];
        return new DatosComprobante
        {
            CodigoEntrega = ObtenerTexto(fila, "codigo_entrega"),
            CodigoEquipo = ObtenerTexto(fila, "codigo_equipo"),
            Cliente = ObtenerTexto(fila, "cliente"),
            Telefono = ObtenerTexto(fila, "telefono"),
            Email = ObtenerTexto(fila, "email"),
            Equipo = ObtenerTexto(fila, "equipo"),
            Problema = ObtenerTexto(fila, "problema"),
            Estado = ObtenerTexto(fila, "estado"),
            Diagnostico = ObtenerTexto(fila, "diagnostico"),
            RepuestosUsados = ObtenerTexto(fila, "repuestos_usados"),
            CostoTotal = Convert.ToDecimal(fila["costo_total"]),
            FechaEntrega = Convert.ToDateTime(fila["fecha_entrega"])
        };
    }

    // Actualiza la ruta del PDF cuando se regenera el comprobante.
    private static void ActualizarRutaPdfEntrega(string codigoEntrega, string rutaPdf)
    {
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("UPDATE entregas SET pdf_path = @pdf_path WHERE codigo = @codigo;", conexion);
        comando.Parameters.AddWithValue("@pdf_path", rutaPdf);
        comando.Parameters.AddWithValue("@codigo", codigoEntrega);
        comando.ExecuteNonQuery();
    }

    // Lee valores de DataRow evitando errores por campos NULL.
    private static string ObtenerTexto(DataRow fila, string columna)
    {
        return fila[columna] == DBNull.Value ? string.Empty : fila[columna]?.ToString() ?? string.Empty;
    }

    // Verifica si ya existe una entrega para el equipo.
    private bool EquipoYaEntregado(int idEquipo, MySqlConnection conexion, MySqlTransaction transaccion)
    {
        using var comando = new MySqlCommand("SELECT COUNT(*) FROM entregas WHERE equipo_id = @equipo_id;", conexion, transaccion);
        comando.Parameters.AddWithValue("@equipo_id", idEquipo);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    // Obtiene el id interno del estado Entregado.
    private static int ObtenerEstadoEntregadoId(MySqlConnection conexion, MySqlTransaction transaccion)
    {
        using var comando = new MySqlCommand("SELECT id FROM estados WHERE nombre = 'Entregado' LIMIT 1;", conexion, transaccion);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // Crea el resumen que se muestra antes de confirmar la entrega.
    private string CrearResumen(string codigoEntrega)
    {
        return
            $"Entrega: {codigoEntrega}\r\n" +
            $"Equipo: {txtCodigoBusqueda.Text.Trim().ToUpperInvariant()}\r\n" +
            $"Cliente: {txtCliente.Text}\r\n" +
            $"Teléfono: {txtTelefono.Text}\r\n" +
            $"Problema reportado: {txtProblema.Text.Trim()}\r\n" +
            $"Repuestos usados: {txtRepuestosUsados.Text.Trim()}\r\n" +
            $"Costo total: Q {(decimal.TryParse(txtCostoTotal.Text, out decimal ct) ? ct : 0):0.00}\r\n" +
            $"Fecha de entrega: {dtpFechaEntrega.Value:dd/MM/yyyy}";
    }

    // Crea la carpeta PDF dentro del proyecto y devuelve la ruta final del comprobante.
    private static string CrearRutaPdf(string codigoEntrega)
    {
        // La carpeta PDF ahora vive en Sistema_NANDDOS/PDF (un nivel arriba de ProyectoNANDDOS).
        var carpetaProyecto = ObtenerCarpetaProyecto();
        var carpeta = Path.Combine(Directory.GetParent(carpetaProyecto)?.FullName ?? carpetaProyecto, "PDF");
        Directory.CreateDirectory(carpeta);
        return Path.Combine(carpeta, $"{codigoEntrega}.pdf");
    }

    // Busca la carpeta real del proyecto para localizar rutas relativas (PDF, iconos, etc.).
    private static string ObtenerCarpetaProyecto()
    {
        var carpeta = new DirectoryInfo(AppContext.BaseDirectory);

        while (carpeta != null)
        {
            if (File.Exists(Path.Combine(carpeta.FullName, "ProyectoNANDDOS.csproj")))
            {
                return carpeta.FullName;
            }

            carpeta = carpeta.Parent;
        }

        return AppContext.BaseDirectory;
    }

    // Toma los datos visibles del formulario para generar un comprobante nuevo.
    private DatosComprobante CrearDatosComprobanteActual(string codigoEntrega)
    {
        return new DatosComprobante
        {
            CodigoEntrega = codigoEntrega,
            CodigoEquipo = txtCodigoBusqueda.Text.Trim().ToUpperInvariant(),
            Cliente = txtCliente.Text,
            Telefono = txtTelefono.Text,
            Email = txtEmail.Text,
            Equipo = txtEquipo.Text,
            Problema = txtProblema.Text,
            Estado = "Entregado",
            Diagnostico = txtProblema.Text.Trim(),
            RepuestosUsados = txtRepuestosUsados.Text.Trim(),
            CostoTotal = decimal.TryParse(txtCostoTotal.Text, out decimal total) ? total : 0,
            FechaEntrega = dtpFechaEntrega.Value.Date
        };
    }

    // Genera PDF para una entrega recien creada.
    private void GenerarPdf(string codigoEntrega, string rutaPdf)
    {
        GenerarPdf(CrearDatosComprobanteActual(codigoEntrega), rutaPdf);
    }

    // Punto unico de generacion PDF, usado por entregas nuevas y reimpresiones.
    private static void GenerarPdf(DatosComprobante datos, string rutaPdf)
    {
        if (UsarDisenoComprobanteReferencia)
        {
            GenerarPdfDisenoReferencia(datos, rutaPdf);
            return;
        }

        using var stream = new FileStream(rutaPdf, FileMode.Create, FileAccess.Write);
        using var documento = new PdfDocument(iTextSharp.text.PageSize.A4, 40, 40, 40, 40);
        iTextSharp.text.pdf.PdfWriter.GetInstance(documento, stream);
        documento.Open();

        var titulo = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
        var normal = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
        documento.Add(new PdfParagraph("Comprobante de Entrega NANDDOS", titulo));
        documento.Add(new PdfParagraph($"Código de entrega: {datos.CodigoEntrega}", normal));
        documento.Add(new PdfParagraph($"Fecha: {datos.FechaEntrega:dd/MM/yyyy}", normal));
        documento.Add(new PdfParagraph(" "));

        var tabla = new PdfPTable(2) { WidthPercentage = 100 };
        tabla.SetWidths(new[] { 30f, 70f });
        AgregarFilaPdf(tabla, "Equipo", datos.CodigoEquipo);
        AgregarFilaPdf(tabla, "Cliente", datos.Cliente);
        AgregarFilaPdf(tabla, "Teléfono", datos.Telefono);
        AgregarFilaPdf(tabla, "Email", datos.Email);
        AgregarFilaPdf(tabla, "Descripción del equipo", datos.Equipo);
        AgregarFilaPdf(tabla, "Problema reportado", datos.Problema);
        AgregarFilaPdf(tabla, "Estado", datos.Estado);
        AgregarFilaPdf(tabla, "Diagnóstico", datos.Diagnostico);
        AgregarFilaPdf(tabla, "Repuestos usados", datos.RepuestosUsados);
        AgregarFilaPdf(tabla, "Costo total", $"Q {datos.CostoTotal:0.00}");
        documento.Add(tabla);

        documento.Add(new PdfParagraph(" "));
        documento.Add(new PdfParagraph("Firma cliente: ________________________________", normal));
        documento.Add(new PdfParagraph("Firma NANDDOS: ________________________________", normal));
        documento.Close();
    }

    // SECCION: diseno profesional del comprobante PDF.
    private static bool UsarDisenoComprobanteReferencia => true;

    // Paleta de colores del comprobante.
    private static readonly PdfBaseColor ColorTextoPrincipal = new(9, 24, 42);
    private static readonly PdfBaseColor ColorBorde = new(17, 32, 54);
    private static readonly PdfBaseColor ColorFilaAlterna = new(235, 235, 235);
    private static readonly PdfBaseColor ColorAzul = new(8, 34, 58);
    private static readonly PdfBaseColor ColorMorado = new(93, 48, 201);
    private static readonly PdfBaseColor ColorBlanco = new(255, 255, 255);
    private static readonly PdfBaseColor ColorNegro = new(0, 0, 0);

    // Construye el PDF con el diseno solicitado en la referencia.
    private static void GenerarPdfDisenoReferencia(DatosComprobante datos, string rutaPdf)
    {
        var rutaLogo = ImagenEmpresa.ObtenerRutaImagen();

        using var stream = new FileStream(rutaPdf, FileMode.Create, FileAccess.Write);
        using var documento = new PdfDocument(iTextSharp.text.PageSize.A4, 37, 37, 24, 34);
        var writer = PdfWriter.GetInstance(documento, stream);
        writer.PageEvent = new EventoComprobantePdf(rutaLogo);

        documento.Open();
        AgregarLogoSuperior(documento, rutaLogo);
        AgregarLineaDecorativa(documento, 5f, 18f);
        AgregarTituloYCaja(documento, datos);
        AgregarTablaComprobante(documento, datos);
        AgregarTextoConformidad(documento);
        AgregarFirmasPdf(documento);
        AgregarPieDePagina(documento);
        documento.Close();
    }

    // Agrega el logo centrado en la parte superior.
    private static void AgregarLogoSuperior(PdfDocument documento, string? rutaLogo)
    {
        var logo = CrearImagenLogoPdf(rutaLogo, soloIcono: false);
        if (logo is not null)
        {
            logo.ScaleToFit(250f, 76f);
            logo.Alignment = PdfElement.ALIGN_CENTER;
            documento.Add(logo);
            return;
        }

        documento.Add(new PdfParagraph("NANDDOS", Fuente(26, PdfFont.BOLD, ColorTextoPrincipal))
        {
            Alignment = PdfElement.ALIGN_CENTER
        });
        documento.Add(new PdfParagraph("FOLLOWING THE INCREDIBLE", Fuente(10, PdfFont.BOLD, ColorTextoPrincipal))
        {
            Alignment = PdfElement.ALIGN_CENTER
        });
    }

    // Agrega la linea azul/morada debajo del encabezado.
    private static void AgregarLineaDecorativa(PdfDocument documento, float alto, float espacioDespues)
    {
        var linea = new PdfPTable(2)
        {
            WidthPercentage = 100,
            SpacingBefore = 8f,
            SpacingAfter = espacioDespues
        };
        linea.SetWidths(new[] { 72f, 28f });
        linea.AddCell(CrearCeldaLinea(ColorAzul, alto));
        linea.AddCell(CrearCeldaLinea(ColorMorado, alto));
        documento.Add(linea);
    }

    // Agrega el titulo y la caja de codigo/fecha.
    private static void AgregarTituloYCaja(PdfDocument documento, DatosComprobante datos)
    {
        var encabezado = new PdfPTable(2)
        {
            WidthPercentage = 100,
            SpacingAfter = 23f
        };
        encabezado.SetWidths(new[] { 70f, 30f });

        var titulo = new PdfPCell(new PdfPhrase("COMPROBANTE DE ENTREGA", Fuente(19.5f, PdfFont.NORMAL, ColorTextoPrincipal)))
        {
            Border = iTextSharp.text.Rectangle.NO_BORDER,
            PaddingLeft = 28f,
            PaddingTop = 10f,
            VerticalAlignment = PdfElement.ALIGN_MIDDLE
        };

        var cajaInterna = new PdfPTable(1) { WidthPercentage = 100 };
        cajaInterna.AddCell(CrearCeldaCajaCodigo("CÓDIGO DE ENTREGA:", Texto(datos.CodigoEntrega)));
        cajaInterna.AddCell(CrearCeldaCajaCodigo("FECHA:", datos.FechaEntrega.ToString("dd/MM/yyyy")));

        var caja = new PdfPCell(cajaInterna)
        {
            BorderColor = new PdfBaseColor(150, 150, 150),
            BorderWidth = 0.8f,
            Padding = 7f,
            MinimumHeight = 44f
        };

        encabezado.AddCell(titulo);
        encabezado.AddCell(caja);
        documento.Add(encabezado);
    }

    // Crea una fila de la caja superior derecha.
    private static PdfPCell CrearCeldaCajaCodigo(string etiqueta, string valor)
    {
        var frase = new PdfPhrase();
        frase.Add(new iTextSharp.text.Chunk(etiqueta + " ", Fuente(10.5f, PdfFont.BOLD, ColorTextoPrincipal)));
        frase.Add(new iTextSharp.text.Chunk(valor, Fuente(10.5f, PdfFont.NORMAL, ColorTextoPrincipal)));

        return new PdfPCell(frase)
        {
            Border = iTextSharp.text.Rectangle.NO_BORDER,
            Padding = 1.5f
        };
    }

    // Crea un tramo de la linea decorativa.
    private static PdfPCell CrearCeldaLinea(PdfBaseColor color, float alto)
    {
        return new PdfPCell
        {
            Border = iTextSharp.text.Rectangle.NO_BORDER,
            BackgroundColor = color,
            FixedHeight = alto
        };
    }

    // Agrega la tabla principal con todos los datos de entrega.
    private static void AgregarTablaComprobante(PdfDocument documento, DatosComprobante datos)
    {
        var tabla = new PdfPTable(2)
        {
            WidthPercentage = 100,
            SplitLate = false,
            SplitRows = true,
            SpacingAfter = 34f
        };
        tabla.SetWidths(new[] { 32f, 68f });

        AgregarFilaPdf(tabla, "Equipo", datos.CodigoEquipo, false);
        AgregarFilaPdf(tabla, "Cliente", datos.Cliente, true);
        AgregarFilaPdf(tabla, "Teléfono", datos.Telefono, false);
        AgregarFilaPdf(tabla, "Email", datos.Email, true);
        AgregarFilaPdf(tabla, "Descripción del equipo", datos.Equipo, false);
        AgregarFilaPdf(tabla, "Problema reportado", datos.Problema, true);
        AgregarFilaPdf(tabla, "Estado", datos.Estado, false);
        AgregarFilaPdf(tabla, "Diagnóstico", datos.Diagnostico, true);
        AgregarFilaPdf(tabla, "Repuestos usados", datos.RepuestosUsados, false);
        AgregarFilaPdf(tabla, "Costo total", $"Q {datos.CostoTotal:0.00}", true);
        documento.Add(tabla);
    }

    // Agrega una fila de la tabla con bordes y fondo alternado.
    private static void AgregarFilaPdf(PdfPTable tabla, string etiqueta, string valor, bool alterna)
    {
        var fondo = alterna ? ColorFilaAlterna : ColorBlanco;
        var celdaEtiqueta = new PdfPCell(new PdfPhrase(etiqueta, Fuente(13, PdfFont.BOLD, ColorTextoPrincipal)))
        {
            PaddingLeft = 9f,
            PaddingRight = 9f,
            PaddingTop = 6f,
            PaddingBottom = 6f,
            BackgroundColor = fondo,
            BorderColor = ColorBorde,
            BorderWidth = 0.85f,
            VerticalAlignment = PdfElement.ALIGN_MIDDLE,
            MinimumHeight = 24f
        };

        var celdaValor = new PdfPCell(new PdfPhrase(Texto(valor), Fuente(13, PdfFont.NORMAL, ColorNegro)))
        {
            PaddingLeft = 9f,
            PaddingRight = 9f,
            PaddingTop = 6f,
            PaddingBottom = 6f,
            BackgroundColor = fondo,
            BorderColor = ColorBorde,
            BorderWidth = 0.85f,
            VerticalAlignment = PdfElement.ALIGN_MIDDLE,
            MinimumHeight = 24f
        };

        tabla.AddCell(celdaEtiqueta);
        tabla.AddCell(celdaValor);
    }

    // Agrega las dos areas de firma.
    private static void AgregarFirmasPdf(PdfDocument documento)
    {
        var firmas = new PdfPTable(2)
        {
            WidthPercentage = 100,
            KeepTogether = true,
            SpacingBefore = 2f
        };
        firmas.SetWidths(new[] { 50f, 50f });
        firmas.AddCell(CrearCeldaFirma("FIRMA DEL CLIENTE", "Firma cliente: ______________________"));
        firmas.AddCell(CrearCeldaFirma("FIRMA DE LA EMPRESA (NANDDOS)", "Firma NANDDOS: ______________________"));
        documento.Add(firmas);
    }

    // Agrega el texto legal de conformidad justo antes del area de firmas.
    private static void AgregarTextoConformidad(PdfDocument documento)
    {
        var colorGris = new PdfBaseColor(80, 80, 80);

        var textoConformidad = new PdfParagraph(
            "Al firmar este documento, acepto que el equipo descrito ha sido revisado en mi presencia, " +
            "se encuentra en las condiciones deseadas, el trabajo solicitado ha sido realizado " +
            "satisfactoriamente y no existen apelaciones ni reclamos posteriores.",
            Fuente(8.5f, PdfFont.NORMAL, colorGris))
        {
            Alignment = PdfElement.ALIGN_JUSTIFIED,
            SpacingBefore = 18f,
            SpacingAfter = 4f
        };
        documento.Add(textoConformidad);
    }

    // Agrega el pie de pagina con la informacion de contacto corporativa de NANDDOS.
    private static void AgregarPieDePagina(PdfDocument documento)
    {
        var colorGris = new PdfBaseColor(90, 90, 90);
        var colorLinea = new PdfBaseColor(200, 200, 200);

        // Linea separadora decorativa antes del pie de pagina
        var lineaSeparadora = new PdfParagraph("_____________________________________________",
            Fuente(9f, PdfFont.NORMAL, colorLinea))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingBefore = 14f,
            SpacingAfter = 6f
        };
        documento.Add(lineaSeparadora);

        // Primera fila de contacto: red social y sitio web
        var lineaWeb = new PdfParagraph(
            "facebook.com/nanddos10  |  nanddos.com",
            Fuente(8f, PdfFont.NORMAL, colorGris))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        documento.Add(lineaWeb);

        // Segunda fila de contacto: celular y correo
        var lineaContacto = new PdfParagraph(
            "cel: 5467 9352  |  ventas@nanddos.com",
            Fuente(8f, PdfFont.NORMAL, colorGris))
        {
            Alignment = PdfElement.ALIGN_CENTER
        };
        documento.Add(lineaContacto);
    }

    // Crea una celda completa de firma.
    private static PdfPCell CrearCeldaFirma(string titulo, string textoFirma)
    {
        var celda = new PdfPCell
        {
            Border = iTextSharp.text.Rectangle.NO_BORDER,
            PaddingLeft = 16f,
            PaddingRight = 16f
        };

        celda.AddElement(new PdfParagraph(titulo, Fuente(12, PdfFont.BOLD, ColorTextoPrincipal))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingAfter = 62f
        });
        celda.AddElement(new PdfParagraph("______________________________", Fuente(12, PdfFont.NORMAL, ColorNegro))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingAfter = 6f
        });
        celda.AddElement(new PdfParagraph(textoFirma, Fuente(11, PdfFont.NORMAL, ColorNegro))
        {
            Alignment = PdfElement.ALIGN_CENTER
        });

        return celda;
    }

    // Crea fuentes Helvetica con tamano, estilo y color indicados.
    private static PdfFont Fuente(float tamano, int estilo, PdfBaseColor color)
    {
        return iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, tamano, estilo, color);
    }

    // Normaliza textos vacios para que el PDF no muestre espacios en blanco.
    private static string Texto(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? "No registrado" : valor.Trim();
    }

    // Carga y recorta el logo para usarlo en encabezado o marca de agua.
    private static PdfImage? CrearImagenLogoPdf(string? rutaLogo, bool soloIcono)
    {
        if (string.IsNullOrWhiteSpace(rutaLogo) || !File.Exists(rutaLogo))
        {
            return null;
        }

        using var original = new Bitmap(rutaLogo);
        var area = ObtenerAreaNoBlanca(original, new Rectangle(0, 0, original.Width, original.Height));
        if (area.Width <= 0 || area.Height <= 0)
        {
            return null;
        }

        if (soloIcono)
        {
            var altoIcono = Math.Max(1, (int)(area.Height * 0.42f));
            var areaSuperior = new Rectangle(area.Left, area.Top, area.Width, altoIcono);
            var areaIcono = ObtenerAreaNoBlanca(original, areaSuperior);
            if (areaIcono.Width > 0 && areaIcono.Height > 0)
            {
                area = areaIcono;
            }
        }

        using var recortada = original.Clone(area, original.PixelFormat);
        using var memoria = new MemoryStream();
        recortada.Save(memoria, ImageFormat.Png);
        return PdfImage.GetInstance(memoria.ToArray());
    }

    // Detecta el area real del logo ignorando bordes blancos.
    private static Rectangle ObtenerAreaNoBlanca(Bitmap imagen, Rectangle limite)
    {
        var izquierda = limite.Right;
        var arriba = limite.Bottom;
        var derecha = limite.Left;
        var abajo = limite.Top;

        for (var y = limite.Top; y < limite.Bottom; y++)
        {
            for (var x = limite.Left; x < limite.Right; x++)
            {
                var color = imagen.GetPixel(x, y);
                if (color.A <= 20 || (color.R > 244 && color.G > 244 && color.B > 244))
                {
                    continue;
                }

                izquierda = Math.Min(izquierda, x);
                arriba = Math.Min(arriba, y);
                derecha = Math.Max(derecha, x);
                abajo = Math.Max(abajo, y);
            }
        }

        return derecha < izquierda || abajo < arriba
            ? Rectangle.Empty
            : Rectangle.FromLTRB(izquierda, arriba, derecha + 1, abajo + 1);
    }

    // Evento de pagina que agrega marca de agua y linea inferior.
    private sealed class EventoComprobantePdf : PdfPageEventHelper
    {
        private readonly string? rutaLogo;

        public EventoComprobantePdf(string? rutaLogo)
        {
            this.rutaLogo = rutaLogo;
        }

        // Se ejecuta al terminar cada pagina del PDF.
        public override void OnEndPage(PdfWriter writer, PdfDocument document)
        {
            AgregarMarcaAgua(writer, document);
            AgregarLineaInferior(writer, document);
        }

        // Dibuja el logo grande y semitransparente sobre la tabla.
        private void AgregarMarcaAgua(PdfWriter writer, PdfDocument document)
        {
            var logo = CrearImagenLogoPdf(rutaLogo, soloIcono: true);
            if (logo is null)
            {
                return;
            }

            try
            {
                logo.ScaleToFit(360f, 255f);
                var x = (document.PageSize.Width - logo.ScaledWidth) / 2f;
                var y = document.PageSize.Height * 0.50f;

                var canvas = writer.DirectContent;
                canvas.SaveState();
                canvas.SetGState(new PdfGState
                {
                    FillOpacity = 0.08f,
                    StrokeOpacity = 0.08f
                });
                logo.SetAbsolutePosition(x, y);
                canvas.AddImage(logo);
                canvas.RestoreState();
            }
            catch
            {
                // Si el logo no puede renderizarse como marca de agua, el PDF sigue siendo legible.
            }
        }

        // Dibuja la linea inferior azul/morada.
        private static void AgregarLineaInferior(PdfWriter writer, PdfDocument document)
        {
            var canvas = writer.DirectContent;
            var ancho = document.PageSize.Width;
            canvas.SaveState();
            canvas.SetColorFill(ColorAzul);
            canvas.Rectangle(0, 0, ancho * 0.72f, 5f);
            canvas.Fill();
            canvas.SetColorFill(ColorMorado);
            canvas.Rectangle(ancho * 0.72f, 0, ancho * 0.28f, 5f);
            canvas.Fill();
            canvas.RestoreState();
        }
    }

    // Version antigua conservada como respaldo si se desactiva el diseno nuevo.
    private static void AgregarFilaPdf(PdfPTable tabla, string etiqueta, string valor)
    {
        var negrita = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
        var normal = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
        tabla.AddCell(new PdfPCell(new PdfPhrase(etiqueta, negrita)) { Padding = 6 });
        tabla.AddCell(new PdfPCell(new PdfPhrase(valor, normal)) { Padding = 6 });
    }

    // Intenta abrir el PDF generado con el visor predeterminado de Windows.
    private static void AbrirPdf(string rutaPdf)
    {
        try
        {
            Process.Start(new ProcessStartInfo(rutaPdf) { UseShellExecute = true });
        }
        catch
        {
            // Si Windows no tiene lector PDF asociado, el comprobante queda guardado en la ruta mostrada.
        }
    }

    // Limpia los datos del equipo encontrado.
    private void LimpiarEquipo()
    {
        equipoId = null;
        txtCliente.Clear();
        txtTelefono.Clear();
        txtEmail.Clear();
        txtEquipo.Clear();
        txtProblema.Clear();
        btnGenerar.Enabled = false;
    }

    // Limpia todo el formulario despues de una entrega correcta.
    private void LimpiarFormularioEntrega()
    {
        LimpiarEquipo();
        txtCodigoBusqueda.Clear();
        txtRepuestosUsados.Clear();
        txtPrecioRepuestos.Text = "0.00";
        txtCostoServicio.Text = "0.00";
        txtCostoTotal.Text = "0.00";
        dtpFechaEntrega.Value = DateTime.Today;
        txtResumen.Clear();
        repuestosAdescontar.Clear();
    }
}
