using MySql.Data.MySqlClient;
using System.Data;

namespace ProyectoNANDDOS;

// Flujo guiado para buscar cliente, registrar cliente nuevo y registrar equipo.
public class RegistrarEquipoForm : Form
{
    // SECCION: controles de busqueda y resultado de cliente.
    private readonly TextBox txtBuscarCliente = new();
    private readonly DataGridView dgvClientes = new();
    private readonly Panel panelResultado = new();
    private readonly Panel panelFormulario = new();
    private readonly Panel panelScroll = new();
    private readonly RowStyle filaTablaClientes = new(SizeType.Absolute, 0);
    private readonly Button btnRegistrarNuevoEquipo = new();
    private readonly Button btnNuevoCliente = new();

    // Campos visibles del cliente encontrado o nuevo en el resultado.
    private readonly TextBox txtNombresEncontrado = new();
    private readonly TextBox txtApellidosEncontrado = new();
    private readonly TextBox txtTelefonoEncontrado = new();
    private readonly TextBox txtEmailEncontrado = new();

    // Campos usados para guardar un cliente nuevo.
    private readonly TextBox txtNombresNuevo = new();
    private readonly TextBox txtApellidosNuevo = new();
    private readonly TextBox txtTelefonoNuevo = new();
    private readonly TextBox txtEmailNuevo = new();
    private readonly GroupBox grupoClienteNuevo = new();
    private readonly GroupBox grupoClienteResumen = new();
    private readonly Label lblClienteResumen = new();

    // SECCION: datos del equipo que se va a registrar.
    private readonly ComboBox cmbTipoEquipo = new();
    private readonly DateTimePicker dtpFechaIngreso = new();
    private readonly TextBox txtMarca = new();
    private readonly TextBox txtModelo = new();
    private readonly TextBox txtSerial = new();
    private readonly TextBox txtProblema = new();
    private readonly ComboBox cmbRepuestosInventario = new();
    private readonly NumericUpDown nudCantidadRepuesto = new() { Minimum = 1, Value = 1, Width = 60 };
    private readonly DataGridView dgvRepuestosUtilizados = new();

    // Estado interno del formulario.
    private TableLayoutPanel? principal;
    private int? clienteSeleccionadoId;
    private bool registrandoClienteNuevo;
    private bool capturandoClienteNuevoEnResultado;

    public RegistrarEquipoForm()
    {
        InicializarComponentes();
        ConfigurarInterfazRegistro();
        CargarNomenclaturas();
        MostrarEstadoInicial();
    }

    // SECCION: construccion principal del formulario.
    private void InicializarComponentes()
    {
        Text = "Registrar Equipo";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Panel con scroll para evitar que el contenido se corte.
        panelScroll.Dock = DockStyle.Fill;
        panelScroll.AutoScroll = true;
        panelScroll.BackColor = BackColor;
        panelScroll.Padding = new Padding(8);

        // Contenedor vertical de encabezado, busqueda, resultado y formulario.
        principal = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 4,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            MinimumSize = new Size(760, 0)
        };
        principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        principal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        principal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        principal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        principal.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Ajusta el ancho interno para que el scroll vertical funcione correctamente.
        void AjustarAncho()
        {
            var anchoDisponible = panelScroll.ClientSize.Width - panelScroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth;
            principal.Width = Math.Max(760, anchoDisponible);
            ActualizarScroll();
        }

        panelScroll.Resize += (_, _) => AjustarAncho();

        principal.Controls.Add(CrearEncabezado(), 0, 0);
        principal.Controls.Add(CrearPanelBusqueda(), 0, 1);
        principal.Controls.Add(CrearPanelResultado(), 0, 2);
        principal.Controls.Add(CrearPanelFormulario(), 0, 3);

        panelScroll.Controls.Add(principal);
        Controls.Add(panelScroll);
        VincularCamposClienteNuevo();
        AjustarAncho();
    }

    // Aplica el estilo Fluent Design a los controles existentes de forma automatica y recursiva.
    private void ConfigurarInterfazRegistro()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        // Helper recursivo para aplicar estilos sin depender de variables locales
        void AplicarEstiloRecursivo(Control.ControlCollection controles)
        {
            foreach (Control ctrl in controles)
            {
                if (ctrl is Label lbl)
                {
                    if (lbl.Text == "Buscar Cliente")
                    {
                        // Regla estricta: FontStyle.Bold
                        lbl.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
                        lbl.ForeColor = Color.FromArgb(15, 23, 42); // #0F172A
                    }
                    else if (lbl.Text == "Registro guiado de clientes y equipos")
                    {
                        lbl.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
                        lbl.ForeColor = Color.FromArgb(100, 116, 139); // #64748B
                    }
                }
                else if (ctrl is GroupBox gb)
                {
                    gb.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                    gb.ForeColor = Color.FromArgb(15, 23, 42); // #0F172A
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
                    
                    if (txt.ReadOnly)
                    {
                        txt.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC
                        txt.ForeColor = Color.FromArgb(51, 65, 85); // #334155
                    }
                    else
                    {
                        txt.BackColor = Color.White;
                    }
                }
                else if (ctrl is Button btn)
                {
                    if (btn.Text == "Buscar")
                    {
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.Cursor = Cursors.Hand;
                        btn.BackColor = Color.FromArgb(37, 99, 235); // #2563EB
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                        
                        try
                        {
                            var ruta = Path.Combine(carpetaIconos, "btn_buscar.png");
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
                            System.Diagnostics.Debug.WriteLine($"[NANDDOS] Error cargando icono 'btn_buscar.png': {ex.Message}");
                        }
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

    // SECCION: encabezado superior.
    private Control CrearEncabezado()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            Margin = new Padding(0, 0, 0, 8)
        };
        panel.Controls.Add(new Label
        {
            Text = "Buscar Cliente",
            Dock = DockStyle.Left,
            Width = 320,
            Font = new Font("Segoe UI Semibold", 20F),
            ForeColor = Color.FromArgb(25, 35, 50),
            TextAlign = ContentAlignment.MiddleLeft
        });
        panel.Controls.Add(new Label
        {
            Text = "Registro guiado de clientes y equipos",
            Dock = DockStyle.Right,
            Width = 360,
            ForeColor = Color.FromArgb(84, 96, 112),
            TextAlign = ContentAlignment.MiddleRight
        });
        return panel;
    }

    // SECCION: Paso 1 - busqueda de cliente.
    private Control CrearPanelBusqueda()
    {
        var grupo = CrearGrupo("Paso 1 - Buscar Cliente");
        grupo.MinimumSize = new Size(0, 132);
        grupo.Height = 132;
        grupo.Margin = new Padding(0, 0, 0, 10);

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

        panel.Controls.Add(CrearEtiqueta("Nombre o teléfono"), 0, 0);
        txtBuscarCliente.Dock = DockStyle.Fill;
        txtBuscarCliente.CharacterCasing = CharacterCasing.Lower;
        txtBuscarCliente.PlaceholderText = "Buscar por nombre o teléfono";
        // Permite ejecutar la busqueda con Enter.
        txtBuscarCliente.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarClientes();
            }
        };

        var btnBuscar = CrearBotonPrincipal("Buscar");
        btnBuscar.Click += (_, _) => BuscarClientes();

        panel.Controls.Add(txtBuscarCliente, 0, 1);
        panel.Controls.Add(btnBuscar, 1, 1);

        var lblNota = new Label
        {
            Text = "Nota: Ingresa solo un nombre y un apellido para mayor precisión.",
            Font = new Font("Segoe UI", 8F),
            ForeColor = Color.FromArgb(100, 116, 139), // #64748B
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft
        };
        panel.Controls.Add(lblNota, 0, 2);

        grupo.Controls.Add(panel);
        return grupo;
    }

    // SECCION: resultado de busqueda de cliente.
    private Control CrearPanelResultado()
    {
        panelResultado.Dock = DockStyle.Top;
        panelResultado.MinimumSize = new Size(0, 190);
        panelResultado.Height = 190;
        panelResultado.Margin = new Padding(0, 0, 0, 10);
        panelResultado.Visible = false;

        var grupo = CrearGrupo("Resultado de búsqueda");
        var contenedorResultados = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        contenedorResultados.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenedorResultados.RowStyles.Add(filaTablaClientes);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

        // Datos del cliente encontrado o captura inicial del cliente nuevo.
        var datos = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4
        };
        datos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        datos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        datos.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        datos.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        datos.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        datos.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        PrepararSoloLectura(txtNombresEncontrado);
        PrepararSoloLectura(txtApellidosEncontrado);
        PrepararSoloLectura(txtTelefonoEncontrado);
        PrepararSoloLectura(txtEmailEncontrado);

        datos.Controls.Add(CrearEtiqueta("Nombres"), 0, 0);
        datos.Controls.Add(CrearEtiqueta("Apellidos"), 1, 0);
        datos.Controls.Add(txtNombresEncontrado, 0, 1);
        datos.Controls.Add(txtApellidosEncontrado, 1, 1);
        datos.Controls.Add(CrearEtiqueta("Teléfono"), 0, 2);
        datos.Controls.Add(CrearEtiqueta("Email"), 1, 2);
        datos.Controls.Add(txtTelefonoEncontrado, 0, 3);
        datos.Controls.Add(txtEmailEncontrado, 1, 3);

        // Botones que cambian segun exista o no el cliente.
        var acciones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(14, 0, 0, 0)
        };
        acciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        acciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        acciones.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        btnRegistrarNuevoEquipo.Text = "Registrar Nuevo Equipo";
        PrepararBotonAccion(btnRegistrarNuevoEquipo, Color.FromArgb(33, 111, 219), Color.White);
        btnRegistrarNuevoEquipo.Click += (_, _) => AbrirFormularioEquipoExistente();

        btnNuevoCliente.Text = "Nuevo Cliente";
        PrepararBotonAccion(btnNuevoCliente, Color.FromArgb(25, 35, 50), Color.White);
        btnNuevoCliente.Click += (_, _) => AbrirFormularioClienteNuevo();

        acciones.Controls.Add(btnRegistrarNuevoEquipo, 0, 0);
        acciones.Controls.Add(btnNuevoCliente, 0, 1);

        // Tabla auxiliar cuando la busqueda devuelve varios clientes.
        PrepararGrid(dgvClientes);
        dgvClientes.CellClick += (_, _) => SeleccionarClienteDesdeGrid();
        dgvClientes.CellDoubleClick += (_, _) => AbrirFormularioEquipoExistente();
        dgvClientes.Visible = false;

        layout.Controls.Add(datos, 0, 0);
        layout.Controls.Add(acciones, 1, 0);
        contenedorResultados.Controls.Add(layout, 0, 0);
        contenedorResultados.Controls.Add(dgvClientes, 0, 1);
        grupo.Controls.Add(contenedorResultados);
        panelResultado.Controls.Add(grupo);
        return panelResultado;
    }

    // SECCION: Paso 2 - formulario de cliente/equipo.
    private Control CrearPanelFormulario()
    {
        panelFormulario.Dock = DockStyle.Top;
        panelFormulario.MinimumSize = new Size(0, 600);
        panelFormulario.Height = 600;
        panelFormulario.Margin = new Padding(0, 0, 0, 12);
        panelFormulario.Visible = false;

        var grupo = CrearGrupo("Paso 2 - Registrar Equipo");
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        contenedor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

        var formulario = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));

        // Panel izquierdo: muestra datos del cliente nuevo o resumen del existente.
        var panelClienteFormulario = new Panel { Dock = DockStyle.Fill };

        grupoClienteNuevo.Text = "Datos del Cliente";
        grupoClienteNuevo.Dock = DockStyle.Fill;
        grupoClienteNuevo.Padding = new Padding(12);
        grupoClienteNuevo.Controls.Add(CrearCamposClienteNuevo());

        grupoClienteResumen.Text = "Cliente seleccionado";
        grupoClienteResumen.Dock = DockStyle.Fill;
        grupoClienteResumen.Padding = new Padding(14);
        lblClienteResumen.Dock = DockStyle.Fill;
        lblClienteResumen.ForeColor = Color.FromArgb(25, 35, 50);
        lblClienteResumen.Font = new Font("Segoe UI", 10F);
        lblClienteResumen.TextAlign = ContentAlignment.TopLeft;
        grupoClienteResumen.Controls.Add(lblClienteResumen);

        panelClienteFormulario.Controls.Add(grupoClienteNuevo);
        panelClienteFormulario.Controls.Add(grupoClienteResumen);

        formulario.Controls.Add(panelClienteFormulario, 0, 0);
        formulario.Controls.Add(CrearGrupoEquipo(), 1, 0);

        // Botones finales del formulario de registro.
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };

        var btnGuardar = CrearBotonPrincipal("Guardar Equipo");
        btnGuardar.Width = 150;
        btnGuardar.Click += (_, _) => GuardarEquipo();

        var btnCancelar = CrearBotonSecundario("Cancelar");
        btnCancelar.Width = 110;
        btnCancelar.Click += (_, _) => MostrarEstadoInicial();

        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnCancelar);

        contenedor.Controls.Add(formulario, 0, 0);
        contenedor.Controls.Add(panelBotones, 0, 1);
        grupo.Controls.Add(contenedor);
        panelFormulario.Controls.Add(grupo);
        return panelFormulario;
    }

    // Crea los campos reales que se usan para guardar un cliente nuevo.
    private Control CrearCamposClienteNuevo()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8 };
        for (var i = 0; i < 8; i++)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, i % 2 == 0 ? 26 : 38));
        }

        AgregarCampo(panel, "Nombres", txtNombresNuevo, 0);
        AgregarCampo(panel, "Apellidos", txtApellidosNuevo, 2);
        AgregarCampo(panel, "Teléfono", txtTelefonoNuevo, 4);
        AgregarCampo(panel, "Email", txtEmailNuevo, 6);
        return panel;
    }

    // Crea la seccion de datos del equipo.
    private Control CrearGrupoEquipo()
    {
        var grupo = new GroupBox
        {
            Text = "Datos del Equipo",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 14
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        for (var i = 0; i < 10; i++)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, i % 2 == 0 ? 26 : 38));
        }
        // Filas extra para el selector de repuestos multiples.
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150)); // fila 10: dgv

        cmbTipoEquipo.Dock = DockStyle.Fill;
        cmbTipoEquipo.DropDownStyle = ComboBoxStyle.DropDownList;
        dtpFechaIngreso.Dock = DockStyle.Fill;
        dtpFechaIngreso.Format = DateTimePickerFormat.Short;

        PrepararTextBox(txtMarca, "Marca");
        PrepararTextBox(txtModelo, "Modelo");
        PrepararTextBox(txtSerial, "Serial");
        PrepararTextBox(txtProblema, "Problema", true);
        // ComboBox de repuestos del inventario.
        cmbRepuestosInventario.Dock = DockStyle.Fill;
        cmbRepuestosInventario.Font = new Font("Segoe UI", 9F);
        cmbRepuestosInventario.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRepuestosInventario.FlatStyle = FlatStyle.Flat;
        CargarRepuestosInventario();

        panel.Controls.Add(CrearEtiqueta("Tipo de equipo"), 0, 0);
        panel.Controls.Add(CrearEtiqueta("Fecha de ingreso"), 1, 0);
        panel.Controls.Add(cmbTipoEquipo, 0, 1);
        panel.Controls.Add(dtpFechaIngreso, 1, 1);
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

        var lblRepuestos = CrearEtiqueta("Repuestos utilizados");
        panel.Controls.Add(lblRepuestos, 0, 8);
        panel.SetColumnSpan(lblRepuestos, 2);

        // Panel con ComboBox + NumericUpDown + Boton Agregar + Boton Quitar.
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

        nudCantidadRepuesto.Dock = DockStyle.Fill;
        nudCantidadRepuesto.Font = new Font("Segoe UI", 9F);

        var btnAgregarRepuesto = new Button
        {
            Text = "+",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAgregarRepuesto.FlatAppearance.BorderSize = 0;

        var btnQuitarRepuesto = new Button
        {
            Text = "-",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(239, 68, 68), // #EF4444
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnQuitarRepuesto.FlatAppearance.BorderSize = 0;

        panelSelector.Controls.Add(cmbRepuestosInventario, 0, 0);
        panelSelector.Controls.Add(nudCantidadRepuesto, 1, 0);
        panelSelector.Controls.Add(btnAgregarRepuesto, 2, 0);
        panelSelector.Controls.Add(btnQuitarRepuesto, 3, 0);
        panel.Controls.Add(panelSelector, 0, 9);
        panel.SetColumnSpan(panelSelector, 2);

        // DataGridView para la lista temporal de repuestos seleccionados.
        ConfigurarDataGridViewRepuestos();
        panel.Controls.Add(dgvRepuestosUtilizados, 0, 10);
        panel.SetColumnSpan(dgvRepuestosUtilizados, 2);

        // Evento: agregar repuesto al DataGridView.
        btnAgregarRepuesto.Click += (_, _) => AgregarRepuestoAlGrid();

        // Evento: quitar repuesto del DataGridView.
        btnQuitarRepuesto.Click += (_, _) =>
        {
            if (dgvRepuestosUtilizados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un repuesto de la tabla para quitarlo.",
                    "Repuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var fila = dgvRepuestosUtilizados.SelectedRows[0];
            int cantidadActual = Convert.ToInt32(fila.Cells["Cantidad"].Value);
            
            if (cantidadActual > 1)
            {
                fila.Cells["Cantidad"].Value = cantidadActual - 1;
            }
            else
            {
                dgvRepuestosUtilizados.Rows.RemoveAt(fila.Index);
            }
        };

        grupo.Controls.Add(panel);
        return grupo;
    }

    // Crea un GroupBox con estilo uniforme.
    private static GroupBox CrearGrupo(string texto)
    {
        return new GroupBox
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25, 35, 50)
        };
    }

    // Agrega etiqueta y TextBox en una fila del formulario.
    private static void AgregarCampo(TableLayoutPanel panel, string etiqueta, TextBox textBox, int fila)
    {
        PrepararTextBox(textBox, etiqueta);
        panel.Controls.Add(CrearEtiqueta(etiqueta), 0, fila);
        panel.Controls.Add(textBox, 0, fila + 1);
    }

    // Etiqueta estandar para formularios.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            ForeColor = Color.FromArgb(75, 85, 99)
        };
    }

    // Configura TextBox normales y multilinea.
    private static void PrepararTextBox(TextBox textBox, string placeholder, bool multilinea = false)
    {
        textBox.Dock = DockStyle.Fill;
        textBox.PlaceholderText = placeholder;
        textBox.MaxLength = multilinea ? 0 : 120;
        textBox.Multiline = multilinea;
        textBox.ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None;
    }

    // Configura campos de resultado que normalmente son de solo lectura.
    private static void PrepararSoloLectura(TextBox textBox)
    {
        PrepararTextBox(textBox, string.Empty);
        textBox.ReadOnly = true;
        textBox.BackColor = Color.FromArgb(248, 250, 252);
        textBox.BorderStyle = BorderStyle.FixedSingle;
    }

    // Sincroniza los campos visibles del resultado con los campos de guardado.
    private void VincularCamposClienteNuevo()
    {
        txtNombresEncontrado.TextChanged += (_, _) => SincronizarCampoClienteNuevo(txtNombresEncontrado, txtNombresNuevo);
        txtApellidosEncontrado.TextChanged += (_, _) => SincronizarCampoClienteNuevo(txtApellidosEncontrado, txtApellidosNuevo);
        txtTelefonoEncontrado.TextChanged += (_, _) => SincronizarCampoClienteNuevo(txtTelefonoEncontrado, txtTelefonoNuevo);
        txtEmailEncontrado.TextChanged += (_, _) => SincronizarCampoClienteNuevo(txtEmailEncontrado, txtEmailNuevo);
    }

    // Copia cambios mientras se captura un cliente nuevo desde el resultado.
    private void SincronizarCampoClienteNuevo(TextBox origen, TextBox destino)
    {
        if (!capturandoClienteNuevoEnResultado || destino.Text == origen.Text)
        {
            return;
        }

        destino.Text = origen.Text;
    }

    // Habilita o bloquea los campos del resultado segun el flujo actual.
    private void ConfigurarCamposResultadoClienteNuevo(bool permitirEdicion)
    {
        capturandoClienteNuevoEnResultado = permitirEdicion;

        foreach (var campo in new[] { txtNombresEncontrado, txtApellidosEncontrado, txtTelefonoEncontrado, txtEmailEncontrado })
        {
            campo.ReadOnly = !permitirEdicion;
            campo.BackColor = permitirEdicion ? Color.White : Color.FromArgb(248, 250, 252);
        }
    }

    // Boton azul para acciones principales.
    private static Button CrearBotonPrincipal(string texto)
    {
        var boton = new Button
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Height = 38,
            BackColor = Color.FromArgb(33, 111, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 10F)
        };
        boton.FlatAppearance.BorderSize = 0;
        return boton;
    }

    // Boton blanco para acciones secundarias.
    private static Button CrearBotonSecundario(string texto)
    {
        var boton = new Button
        {
            Text = texto,
            Height = 38,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25, 35, 50),
            FlatStyle = FlatStyle.Flat
        };
        boton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        return boton;
    }

    // Aplica colores y estilo a botones del panel de resultado.
    private static void PrepararBotonAccion(Button boton, Color fondo, Color texto)
    {
        boton.Dock = DockStyle.Fill;
        boton.BackColor = fondo;
        boton.ForeColor = texto;
        boton.FlatStyle = FlatStyle.Flat;
        boton.Font = new Font("Segoe UI Semibold", 10F);
        boton.FlatAppearance.BorderSize = 0;
    }

    // Configura la tabla auxiliar de clientes.
    private static void PrepararGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.RowHeadersVisible = false;
    }

    // SECCION: datos base para combos.
    private void CargarNomenclaturas()
    {
        cmbTipoEquipo.Items.Clear();
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("SELECT id, prefijo, descripcion FROM nomenclaturas ORDER BY id;", conexion);
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            cmbTipoEquipo.Items.Add(new NomenclaturaItem(
                reader.GetInt32("id"),
                reader.GetString("prefijo"),
                reader.GetString("descripcion")));
        }

        if (cmbTipoEquipo.Items.Count > 0)
        {
            cmbTipoEquipo.SelectedIndex = 0;
        }
    }

    // Algoritmo de Distancia de Levenshtein (Fuzzy Search).
    private static int CalcularDistancia(string origen, string destino)
    {
        if (string.IsNullOrEmpty(origen)) return string.IsNullOrEmpty(destino) ? 0 : destino.Length;
        if (string.IsNullOrEmpty(destino)) return origen.Length;

        int n = origen.Length;
        int m = destino.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int costo = (destino[j - 1] == origen[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + costo);
            }
        }
        return d[n, m];
    }

    // SECCION: busqueda de cliente.
    private void BuscarClientes()
    {
        var busqueda = txtBuscarCliente.Text.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(busqueda))
        {
            MessageBox.Show("Ingresa un nombre o teléfono para buscar.", "Buscar cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Busca por nombre completo o telefono.
        using var conexion = ConexionDB.ObtenerConexion();
        using var comando = new MySqlCommand("""
            SELECT id, codigo, nombres, apellidos, telefono, email
            FROM clientes
            WHERE LOWER(CONCAT(nombres, ' ', apellidos)) LIKE @busqueda
               OR telefono LIKE @busqueda
            ORDER BY nombres, apellidos
            LIMIT 20;
            """, conexion);
        comando.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");

        var tabla = new DataTable();
        using var adaptador = new MySqlDataAdapter(comando);
        adaptador.Fill(tabla);

        panelResultado.Visible = true;
        panelFormulario.Visible = false;
        AjustarPanelResultado(mostrarTablaClientes: false);
        DesplazarHacia(panelResultado);

        // Si no existe coincidencia exacta, aplicamos busqueda difusa (Fuzzy Search).
        if (tabla.Rows.Count == 0)
        {
            // Consultar todos los clientes para evaluar distancias.
            using var comandoFuzzy = new MySqlCommand("SELECT id, codigo, nombres, apellidos, telefono, email FROM clientes;", conexion);
            using var lector = comandoFuzzy.ExecuteReader();
            
            int distanciaMinima = int.MaxValue;
            object[]? filaSugerida = null;
            string nombreSugerido = "";
            
            while (lector.Read())
            {
                string n = lector["nombres"]?.ToString() ?? "";
                string a = lector["apellidos"]?.ToString() ?? "";
                string nombreCompleto = $"{n} {a}".Trim().ToLower();
                
                int dist = CalcularDistancia(busqueda, nombreCompleto);
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
                    filaSugerida = new object[lector.FieldCount];
                    lector.GetValues(filaSugerida);
                    nombreSugerido = $"{n} {a}".Trim();
                }
            }
            lector.Close();

            if (distanciaMinima <= 3 && filaSugerida != null)
            {
                DialogResult respuesta = MessageBox.Show(
                    $"No se encontró exactamente '{busqueda}'.\n\n¿Te refieres a '{nombreSugerido}'?",
                    "Sugerencia de búsqueda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                if (respuesta == DialogResult.Yes)
                {
                    tabla.Rows.Add(filaSugerida);
                }
            }

            // Si el usuario dijo No o la distancia es muy grande, preparar cliente nuevo.
            if (tabla.Rows.Count == 0)
            {
                LimpiarClienteEncontrado();
                PrepararCapturaClienteNuevoEnResultado(busqueda);
                btnRegistrarNuevoEquipo.Visible = false;
                btnNuevoCliente.Visible = true;
                dgvClientes.DataSource = null;
                AjustarPanelResultado(mostrarTablaClientes: false);
                MessageBox.Show("Cliente no encontrado. Puedes registrarlo como nuevo cliente.", "Buscar cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        // Si existe, autocompleta el primero y muestra tabla si hay varios.
        btnRegistrarNuevoEquipo.Visible = true;
        btnNuevoCliente.Visible = false;
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: false);
        MostrarCliente(tabla.Rows[0]);
        MostrarTablaClientes(tabla);
    }

    // Muestra varios clientes encontrados y oculta ids internos.
    private void MostrarTablaClientes(DataTable tabla)
    {
        dgvClientes.DataSource = tabla;
        AjustarPanelResultado(mostrarTablaClientes: tabla.Rows.Count > 1);
        if (dgvClientes.Columns["id"] is DataGridViewColumn columnaId)
        {
            columnaId.Visible = false;
        }
        if (dgvClientes.Columns["codigo"] is DataGridViewColumn columnaCodigo)
        {
            columnaCodigo.Visible = false;
        }
        if (dgvClientes.Columns["nombres"] is DataGridViewColumn columnaNombres)
        {
            columnaNombres.HeaderText = "Nombres";
        }
        if (dgvClientes.Columns["apellidos"] is DataGridViewColumn columnaApellidos)
        {
            columnaApellidos.HeaderText = "Apellidos";
        }
        if (dgvClientes.Columns["telefono"] is DataGridViewColumn columnaTelefono)
        {
            columnaTelefono.HeaderText = "Teléfono";
        }
        if (dgvClientes.Columns["email"] is DataGridViewColumn columnaEmail)
        {
            columnaEmail.HeaderText = "Email";
        }
    }

    // Prellena datos cuando la busqueda no encontro cliente.
    private void PrepararCapturaClienteNuevoEnResultado(string busqueda)
    {
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: true);

        if (busqueda.Any(char.IsDigit))
        {
            txtTelefonoEncontrado.Text = busqueda;
            return;
        }

        var partesNombre = busqueda
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (partesNombre.Length == 0)
        {
            return;
        }

        txtNombresEncontrado.Text = partesNombre[0];
        txtApellidosEncontrado.Text = partesNombre.Length > 1
            ? string.Join(' ', partesNombre.Skip(1))
            : string.Empty;
    }

    // Actualiza el cliente seleccionado desde la tabla de resultados.
    private void SeleccionarClienteDesdeGrid()
    {
        if (dgvClientes.CurrentRow?.DataBoundItem is DataRowView fila)
        {
            MostrarCliente(fila.Row);
        }
    }

    // Autocompleta los campos visibles con el cliente seleccionado.
    private void MostrarCliente(DataRow fila)
    {
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: false);
        clienteSeleccionadoId = Convert.ToInt32(fila["id"]);
        txtNombresEncontrado.Text = fila["nombres"].ToString();
        txtApellidosEncontrado.Text = fila["apellidos"].ToString();
        txtTelefonoEncontrado.Text = fila["telefono"].ToString();
        txtEmailEncontrado.Text = fila["email"].ToString();
    }

    // Abre el paso 2 para registrar un equipo a un cliente existente.
    private void AbrirFormularioEquipoExistente()
    {
        if (clienteSeleccionadoId is null)
        {
            MessageBox.Show("Selecciona un cliente antes de registrar el equipo.", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        registrandoClienteNuevo = false;
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: false);
        grupoClienteNuevo.Visible = false;
        grupoClienteResumen.Visible = true;
        lblClienteResumen.Text =
            $"Nombre: {txtNombresEncontrado.Text} {txtApellidosEncontrado.Text}\n\n" +
            $"Teléfono: {txtTelefonoEncontrado.Text}\n\n" +
            $"Email: {txtEmailEncontrado.Text}";
        LimpiarDatosEquipo();
        panelFormulario.Visible = true;
        ActualizarScroll();
        DesplazarHacia(panelFormulario);
    }

    // Abre el paso 2 para crear cliente nuevo y registrar equipo.
    private void AbrirFormularioClienteNuevo()
    {
        registrandoClienteNuevo = true;
        clienteSeleccionadoId = null;
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: true);
        grupoClienteNuevo.Visible = true;
        grupoClienteResumen.Visible = false;
        lblClienteResumen.Text = string.Empty;
        CopiarClienteResultadoAFormularioNuevo();
        LimpiarDatosEquipo();
        panelFormulario.Visible = true;
        ActualizarScroll();
        DesplazarHacia(panelFormulario);
        txtNombresNuevo.Focus();
    }

    // Copia datos capturados en resultado al formulario real de guardado.
    private void CopiarClienteResultadoAFormularioNuevo()
    {
        txtNombresNuevo.Text = txtNombresEncontrado.Text.Trim();
        txtApellidosNuevo.Text = txtApellidosEncontrado.Text.Trim();
        txtTelefonoNuevo.Text = txtTelefonoEncontrado.Text.Trim();
        txtEmailNuevo.Text = txtEmailEncontrado.Text.Trim();
    }

    // SECCION: guardado de cliente/equipo.
    private void GuardarEquipo()
    {
        if (!ValidarRegistro())
        {
            return;
        }

        var tipo = (NomenclaturaItem)cmbTipoEquipo.SelectedItem!;

        // Usa transaccion para guardar cliente nuevo y equipo juntos.
        using var conexion = ConexionDB.ObtenerConexion();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            string? codigoCliente = null;
            var clienteId = clienteSeleccionadoId;

            // Si el cliente no existia, se crea primero.
            if (registrandoClienteNuevo)
            {
                codigoCliente = GeneradorCodigo.GenerarCodigoCliente(conexion, transaccion);
                clienteId = InsertarCliente(codigoCliente, conexion, transaccion);
            }

            var estadoDiagnosticoId = ObtenerEstadoId("En diagnóstico", conexion, transaccion);
            var codigoEquipo = GeneradorCodigo.GenerarCodigoEquipo(tipo.Prefijo, conexion, transaccion);

            // Inserta el equipo con estado inicial En diagnostico.
            using var comando = new MySqlCommand("""
                INSERT INTO equipos
                    (codigo, cliente_id, nomenclatura_id, estado_id, fecha_ingreso, marca, modelo, serial, descripcion_problema, repuestos_necesarios)
                VALUES
                    (@codigo, @cliente_id, @nomenclatura_id, @estado_id, @fecha_ingreso, @marca, @modelo, @serial, @problema, @repuestos);
                """, conexion, transaccion);
            comando.Parameters.AddWithValue("@codigo", codigoEquipo);
            comando.Parameters.AddWithValue("@cliente_id", clienteId!.Value);
            comando.Parameters.AddWithValue("@nomenclatura_id", tipo.Id);
            comando.Parameters.AddWithValue("@estado_id", estadoDiagnosticoId);
            comando.Parameters.AddWithValue("@fecha_ingreso", dtpFechaIngreso.Value.Date);
            comando.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
            comando.Parameters.AddWithValue("@modelo", txtModelo.Text.Trim());
            comando.Parameters.AddWithValue("@serial", txtSerial.Text.Trim());
            comando.Parameters.AddWithValue("@problema", txtProblema.Text.Trim());
            comando.Parameters.AddWithValue("@repuestos", ObtenerTextoRepuestosDelGrid());
            comando.ExecuteNonQuery();

            transaccion.Commit();

            // Descontar stock de cada repuesto del DataGridView.
            // Si alguno falla, el equipo YA fue registrado pero el stock no fue afectado.
            if (!DescontarStockDelGrid())
            {
                MessageBox.Show(
                    "El equipo fue registrado pero hubo un error al descontar el inventario.\n" +
                    "Revisa el módulo de Inventario para ajustar el stock manualmente.",
                    "Advertencia de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MostrarEstadoInicial();
                return;
            }

            string nombreClienteFinal = registrandoClienteNuevo 
                ? $"{txtNombresNuevo.Text.Trim()} {txtApellidosNuevo.Text.Trim()}"
                : $"{txtNombresEncontrado.Text.Trim()} {txtApellidosEncontrado.Text.Trim()}";
                
            string telefonoFinal = registrandoClienteNuevo
                ? txtTelefonoNuevo.Text.Trim()
                : txtTelefonoEncontrado.Text.Trim();

            // Generar ticket PDF (2 paginas: cliente y tecnico) en la carpeta TIKETS.
            var generador = new GeneradorTickets();
            string equipoInfo = $"{txtMarca.Text.Trim()} {txtModelo.Text.Trim()}".Trim();
            generador.GenerarTicketPDF(codigoEquipo, nombreClienteFinal, telefonoFinal, txtProblema.Text.Trim(), equipoInfo);

            var mensaje = registrandoClienteNuevo && codigoCliente is not null
                ? $"Cliente registrado: {codigoCliente}\nEquipo registrado: {codigoEquipo}"
                : $"Equipo registrado correctamente con código {codigoEquipo}.";

            MensajeNanddosForm.Mostrar(mensaje, "Registrar equipo");
            MostrarEstadoInicial();
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            MessageBox.Show($"No se pudo guardar el registro.\n\n{ex.Message}", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Inserta cliente nuevo y devuelve su id interno.
    private int InsertarCliente(string codigoCliente, MySqlConnection conexion, MySqlTransaction transaccion)
    {
        using var comando = new MySqlCommand("""
            INSERT INTO clientes (codigo, nombres, apellidos, email, telefono)
            VALUES (@codigo, @nombres, @apellidos, @email, @telefono);
            """, conexion, transaccion);
        comando.Parameters.AddWithValue("@codigo", codigoCliente);
        comando.Parameters.AddWithValue("@nombres", txtNombresNuevo.Text.Trim());
        comando.Parameters.AddWithValue("@apellidos", txtApellidosNuevo.Text.Trim());
        comando.Parameters.AddWithValue("@email", txtEmailNuevo.Text.Trim());
        comando.Parameters.AddWithValue("@telefono", txtTelefonoNuevo.Text.Trim());
        comando.ExecuteNonQuery();
        return Convert.ToInt32(comando.LastInsertedId);
    }

    // Obtiene el id interno de un estado por su nombre.
    private static int ObtenerEstadoId(string estado, MySqlConnection conexion, MySqlTransaction transaccion)
    {
        using var comando = new MySqlCommand("SELECT id FROM estados WHERE nombre = @estado LIMIT 1;", conexion, transaccion);
        comando.Parameters.AddWithValue("@estado", estado);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // Valida los campos obligatorios antes de guardar.
    private bool ValidarRegistro()
    {
        if (registrandoClienteNuevo)
        {
            if (string.IsNullOrWhiteSpace(txtNombresNuevo.Text) ||
                string.IsNullOrWhiteSpace(txtApellidosNuevo.Text) ||
                string.IsNullOrWhiteSpace(txtTelefonoNuevo.Text))
            {
                MessageBox.Show("Completa nombres, apellidos y teléfono del cliente.", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        else if (clienteSeleccionadoId is null)
        {
            MessageBox.Show("Selecciona un cliente existente.", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (cmbTipoEquipo.SelectedItem is null)
        {
            MessageBox.Show("Selecciona el tipo de equipo.", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtProblema.Text))
        {
            MessageBox.Show("Describe el problema del equipo.", "Registrar equipo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    // SECCION: limpieza y estado inicial.
    private void MostrarEstadoInicial()
    {
        clienteSeleccionadoId = null;
        registrandoClienteNuevo = false;
        panelResultado.Visible = false;
        panelFormulario.Visible = false;
        grupoClienteNuevo.Visible = false;
        grupoClienteResumen.Visible = false;
        btnRegistrarNuevoEquipo.Visible = false;
        btnNuevoCliente.Visible = false;
        dgvClientes.DataSource = null;
        AjustarPanelResultado(mostrarTablaClientes: false);
        LimpiarClienteEncontrado();
        LimpiarClienteNuevo();
        LimpiarDatosEquipo();
        txtBuscarCliente.Clear();
        panelScroll.AutoScrollPosition = Point.Empty;
        txtBuscarCliente.Focus();
    }

    // Ajusta altura del resultado segun se muestre o no la tabla auxiliar.
    private void AjustarPanelResultado(bool mostrarTablaClientes)
    {
        dgvClientes.Visible = mostrarTablaClientes;
        filaTablaClientes.Height = mostrarTablaClientes ? 92 : 0;

        var altura = mostrarTablaClientes ? 300 : 190;
        panelResultado.MinimumSize = new Size(0, altura);
        panelResultado.Height = altura;

        ActualizarScroll();
    }

    // Recalcula el area minima de scroll segun el contenido visible.
    private void ActualizarScroll()
    {
        principal?.PerformLayout();
        panelScroll.PerformLayout();

        if (principal is not null)
        {
            panelScroll.AutoScrollMinSize = new Size(0, principal.Height + panelScroll.Padding.Vertical + 16);
        }
    }

    // Lleva el scroll al bloque que el usuario debe revisar.
    private void DesplazarHacia(Control control)
    {
        BeginInvoke(new Action(() => panelScroll.ScrollControlIntoView(control)));
    }

    // Limpia los campos visibles del resultado.
    private void LimpiarClienteEncontrado()
    {
        ConfigurarCamposResultadoClienteNuevo(permitirEdicion: false);
        txtNombresEncontrado.Clear();
        txtApellidosEncontrado.Clear();
        txtTelefonoEncontrado.Clear();
        txtEmailEncontrado.Clear();
    }

    // Limpia los campos usados para crear un cliente.
    private void LimpiarClienteNuevo()
    {
        txtNombresNuevo.Clear();
        txtApellidosNuevo.Clear();
        txtTelefonoNuevo.Clear();
        txtEmailNuevo.Clear();
    }

    // Limpia los campos del equipo para iniciar un nuevo registro.
    private void LimpiarDatosEquipo()
    {
        txtMarca.Clear();
        txtModelo.Clear();
        txtSerial.Clear();
        txtProblema.Clear();
        if (cmbRepuestosInventario.Items.Count > 0)
            cmbRepuestosInventario.SelectedIndex = 0;
        dgvRepuestosUtilizados.Rows.Clear();
        nudCantidadRepuesto.Value = 1;
        CargarRepuestosInventario();
        dtpFechaIngreso.Value = DateTime.Today;
        if (cmbTipoEquipo.Items.Count > 0)
        {
            cmbTipoEquipo.SelectedIndex = 0;
        }
    }

    // Objeto para mostrar tipo de equipo en ComboBox conservando id y prefijo.
    private sealed class NomenclaturaItem
    {
        public NomenclaturaItem(int id, string prefijo, string descripcion)
        {
            Id = id;
            Prefijo = prefijo;
            Descripcion = descripcion;
        }

        public int Id { get; }
        public string Prefijo { get; }
        public string Descripcion { get; }

        public override string ToString()
        {
            return $"{Prefijo} - {Descripcion}";
        }
    }

    // Carga el ComboBox de repuestos con los datos del inventario (stock > 0).
    private void CargarRepuestosInventario()
    {
        try
        {
            var lista = new List<RepuestoComboItem>
            {
                new(0, "", "No aplica", "No aplica", 0)
            };

            var repuestos = RepuestoDAO.ObtenerConStock();
            foreach (var r in repuestos)
            {
                lista.Add(new RepuestoComboItem(
                    r.IdRepuesto,
                    r.Codigo,
                    r.Nombre,
                    $"{r.Codigo} - {r.Nombre} (Stock: {r.Stock})",
                    r.Stock));
            }

            cmbRepuestosInventario.DataSource = null;
            cmbRepuestosInventario.DataSource = lista;
            cmbRepuestosInventario.DisplayMember = nameof(RepuestoComboItem.Texto);
            cmbRepuestosInventario.ValueMember = nameof(RepuestoComboItem.Id);
            cmbRepuestosInventario.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NANDDOS] Error al cargar repuestos en ComboBox: {ex.Message}");
        }
    }

    // Configura el DataGridView para la tabla temporal de repuestos seleccionados.
    private void ConfigurarDataGridViewRepuestos()
    {
        dgvRepuestosUtilizados.Dock = DockStyle.Fill;
        dgvRepuestosUtilizados.AllowUserToAddRows = false;
        dgvRepuestosUtilizados.ReadOnly = true;
        dgvRepuestosUtilizados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRepuestosUtilizados.MultiSelect = false;
        dgvRepuestosUtilizados.RowHeadersVisible = false;
        dgvRepuestosUtilizados.BackgroundColor = Color.White;
        dgvRepuestosUtilizados.Font = new Font("Segoe UI", 9F);
        dgvRepuestosUtilizados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dgvRepuestosUtilizados.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "CodigoRepuesto",
            HeaderText = "Código",
            FillWeight = 30
        });
        dgvRepuestosUtilizados.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Descripcion",
            HeaderText = "Descripción",
            FillWeight = 50
        });
        dgvRepuestosUtilizados.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Cantidad",
            HeaderText = "Cantidad",
            FillWeight = 20
        });
    }

    // Agrega un repuesto al DataGridView o suma la cantidad si ya existe.
    private void AgregarRepuestoAlGrid()
    {
        if (cmbRepuestosInventario.SelectedItem is not RepuestoComboItem seleccionado || seleccionado.Id <= 0)
        {
            return;
        }

        int cantidadAgregar = (int)nudCantidadRepuesto.Value;

        // Validar que la cantidad no supere el stock disponible.
        if (cantidadAgregar > seleccionado.StockDisponible)
        {
            MessageBox.Show($"Stock insuficiente. Solo hay {seleccionado.StockDisponible} unidades disponibles.",
                "Repuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool existe = false;
        string codigoLimpio = seleccionado.Codigo;

        // Buscar si ya existe en el grid para sumar cantidad.
        foreach (DataGridViewRow fila in dgvRepuestosUtilizados.Rows)
        {
            if (fila.Cells["CodigoRepuesto"].Value.ToString().Trim() == codigoLimpio)
            {
                int cantidadActual = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                int nuevaCantidad = cantidadActual + cantidadAgregar;

                if (nuevaCantidad > seleccionado.StockDisponible)
                {
                    MessageBox.Show($"Stock insuficiente. Ya tienes {cantidadActual} y solo hay {seleccionado.StockDisponible} disponibles.",
                        "Repuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                fila.Cells["Cantidad"].Value = nuevaCantidad;
                existe = true;
                break;
            }
        }

        if (!existe)
        {
            // Agregar nueva fila.
            dgvRepuestosUtilizados.Rows.Add(codigoLimpio, seleccionado.NombreLimpio, cantidadAgregar);
        }
        
        nudCantidadRepuesto.Value = 1;
    }

    // Concatena los repuestos del DataGridView en formato "2x RAM, 1x SSD" para la base de datos.
    private string ObtenerTextoRepuestosDelGrid()
    {
        var partes = new List<string>();
        foreach (DataGridViewRow fila in dgvRepuestosUtilizados.Rows)
        {
            var nombre = fila.Cells["Descripcion"].Value?.ToString() ?? "";
            var cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
            partes.Add($"{cantidad}x {nombre}");
        }
        return string.Join(", ", partes);
    }

    // Descuenta el stock de cada repuesto listado en el DataGridView.
    // Retorna true SOLO si todos los descuentos fueron exitosos.
    // Si alguno falla, detiene el proceso y muestra un error.
    private bool DescontarStockDelGrid()
    {
        foreach (DataGridViewRow fila in dgvRepuestosUtilizados.Rows)
        {
            string codigo = fila.Cells["CodigoRepuesto"].Value?.ToString()?.Trim() ?? "";
            int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
            string nombre = fila.Cells["Descripcion"].Value?.ToString() ?? "";
            
            if (string.IsNullOrWhiteSpace(codigo) || codigo == "N/A")
            {
                continue;
            }

            bool exito = RepuestoDAO.DescontarStock(codigo, cantidad);
            
            if (!exito)
            {
                MessageBox.Show(
                    $"Error Crítico: No se pudo descontar el stock del repuesto \"{nombre}\" (Código: {codigo}, Cantidad: {cantidad}).\n\n" +
                    "Es posible que el stock sea insuficiente o que el código no exista en la base de datos.\n" +
                    "El registro del equipo fue cancelado para proteger la integridad del inventario.",
                    "Fallo de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        return true;
    }

    // Objeto auxiliar para poblar el ComboBox de repuestos con Id, Codigo, Nombre Limpio, Texto y Stock.
    private sealed class RepuestoComboItem
    {
        public RepuestoComboItem(int id, string codigo, string nombreLimpio, string texto, int stockDisponible)
        {
            Id = id;
            Codigo = codigo;
            NombreLimpio = nombreLimpio;
            Texto = texto;
            StockDisponible = stockDisponible;
        }

        public int Id { get; }
        public string Codigo { get; }
        public string NombreLimpio { get; }
        public string Texto { get; }
        public int StockDisponible { get; }

        public override string ToString() => Texto;
    }
}
