namespace ProyectoNANDDOS;

// Ventana principal que contiene la barra de titulo, barra lateral y los modulos del sistema.
public class MenuPrincipalForm : Form
{
    // Panel donde se muestran los formularios internos.
    private readonly Panel panelContenido = new();
    // Usuario autenticado que aparece en la barra lateral.
    private readonly string usuario;
    // Guarda el boton seleccionado para resaltar la opcion activa.
    private Button? botonActivo;

    public MenuPrincipalForm(string usuario)
    {
        this.usuario = usuario;
        InicializarComponentes();
    }

    // Agrega sombra sutil a la ventana sin bordes.
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
            return cp;
        }
    }

    // SECCION: construccion del menu principal.
    private void InicializarComponentes()
    {
        Text = "NANDDOS - Sistema de Soporte Técnico v1.1";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        ClientSize = new Size(1180, 740);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.None;
        DoubleBuffered = true;

        // Barra izquierda estilo Dashboard / Fluent Design.
        var barraLateral = new Panel
        {
            Dock = DockStyle.Left,
            Width = 240,
            BackColor = Color.FromArgb(15, 23, 42), // #0F172A (Azul Oscuro Corporativo)
            Padding = new Padding(12, 24, 12, 12)
        };

        // Encabezado superior con logo transparente.
        var picLogo = new PictureBox
        {
            Image = CargarIconoLocal("logo_transparente.png"),
            SizeMode = PictureBoxSizeMode.Zoom,
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.Transparent
        };

        var panelEspacio = new Panel
        {
            Dock = DockStyle.Top,
            Height = 32
        };

        // Area de usuario inferior.
        var panelUsuario = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.FromArgb(15, 23, 42)
        };

        var picUsuario = new PictureBox
        {
            Image = CargarIconoLocal("nav_usuario.png"),
            SizeMode = PictureBoxSizeMode.Zoom,
            Location = new Point(8, 14),
            Size = new Size(32, 32),
            Anchor = AnchorStyles.Left | AnchorStyles.Top
        };

        var lblUsuario = new Label
        {
            Text = $"Usuario: {usuario}",
            Location = new Point(48, 14),
            Size = new Size(160, 32),
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
            ForeColor = Color.FromArgb(248, 250, 252), // #F8FAFC
            Font = new Font("Segoe UI Semibold", 9.5F),
            TextAlign = ContentAlignment.MiddleLeft
        };

        panelUsuario.Controls.Add(picUsuario);
        panelUsuario.Controls.Add(lblUsuario);

        // Botones principales de navegacion con iconos.
        var btnEntrega = CrearBotonMenu("nav_entrega.png", "Entrega", () => new EntregaForm());
        var btnClientes = CrearBotonMenu("nav_clientes.png", "Clientes", () => new ClientesForm());
        var btnListaEquipos = CrearBotonMenu("nav_lista.png", "Lista de Equipos", () => new ListaEquiposForm());
        var btnRegistrarEquipo = CrearBotonMenu("nav_registrar.png", "Registrar Equipo", () => new RegistrarEquipoForm());

        // Boton de acceso al modulo de Inventario de Repuestos.
        var btnInventario = CrearBotonMenu("nav_inventario.png", "Inventario", () => new InventarioForm());

        // Boton de acceso al modulo de Gestion de Cargos y Permisos.
        var btnCargos = CrearBotonMenu("nav_clientes.png", "Cargos", () => new GestionCargosForm());

        // Botón de acceso al módulo de Usuarios Técnicos.
        var btnUsuarios = CrearBotonMenu("nav_clientes.png", "Usuarios", () => new GestionUsuariosForm());

        // Botón de acceso al módulo de Servicios.
        var btnServicios = CrearBotonMenu("nav_clientes.png", "Servicios", () => new ServiciosForm());

        // Botón público del Dashboard de Inicio.
        var btnDashboard = CrearBotonMenu("nav_clientes.png", "Inicio", () => new DashboardForm());

        // Blindaje de Seguridad RBAC
        btnRegistrarEquipo.Visible = GestorSeguridad.TienePermiso("equipos_registrar");
        btnListaEquipos.Visible = GestorSeguridad.TienePermiso("equipos_ver");
        btnClientes.Visible = GestorSeguridad.TienePermiso("clientes_ver");
        btnEntrega.Visible = GestorSeguridad.TienePermiso("entregas_generar");
        btnInventario.Visible = GestorSeguridad.TienePermiso("inventario_ver");
        btnServicios.Visible = GestorSeguridad.TienePermiso("servicios_ver");
        
        btnUsuarios.Visible = SesionActual.EsSuperAdministrador;
        btnCargos.Visible = SesionActual.EsSuperAdministrador;
        // Botón de Cerrar Sesión (Dock = Bottom, siempre abajo del menú)
        var btnCerrarSesion = new Button
        {
            Text = "Cerrar Sesión",
            Dock = DockStyle.Bottom,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(127, 29, 29), // Rojo oscuro formal
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        btnCerrarSesion.FlatAppearance.BorderSize = 0;
        btnCerrarSesion.FlatAppearance.MouseOverBackColor = Color.FromArgb(153, 27, 27); // Hover rojo más claro
        btnCerrarSesion.Click += (_, _) =>
        {
            var resultado = MessageBox.Show(
                "¿Estás seguro que deseas cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                SesionActual.LimpiarSesion();
                Application.Restart();
            }
        };

        barraLateral.Controls.Add(btnCerrarSesion); // Dock = Bottom: se pega abajo
        barraLateral.Controls.Add(panelUsuario);
        barraLateral.Controls.Add(btnServicios);
        barraLateral.Controls.Add(btnInventario);
        barraLateral.Controls.Add(btnEntrega);
        barraLateral.Controls.Add(btnClientes);
        barraLateral.Controls.Add(btnListaEquipos);
        barraLateral.Controls.Add(btnRegistrarEquipo);
        barraLateral.Controls.Add(btnCargos);
        barraLateral.Controls.Add(btnUsuarios);
        barraLateral.Controls.Add(btnDashboard); // Agregado antes del espacio y logo (se visualiza arriba)
        barraLateral.Controls.Add(panelEspacio);
        barraLateral.Controls.Add(picLogo);

        // Area central donde se incrusta el formulario seleccionado.
        panelContenido.Dock = DockStyle.Fill;
        panelContenido.BackColor = Color.FromArgb(246, 248, 251);
        panelContenido.Padding = new Padding(24);

        // Orden de insercion: contenido primero, luego lateral (Dock apila en reversa).
        Controls.Add(panelContenido);
        Controls.Add(barraLateral);

        // Inyecta la barra de titulo personalizada estilo Windows 11.
        BarraTitulo.Inyectar(this);

        // Pantalla inicial al entrar al sistema.
        AbrirFormulario(new DashboardForm(), btnDashboard);
    }

    // Crea un boton de la barra lateral y asocia el formulario que debe abrir.
    private Button CrearBotonMenu(string archivoIcono, string texto, Func<Form> crearFormulario)
    {
        var boton = new Button
        {
            Text = texto,
            Image = CargarIconoLocal(archivoIcono),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Dock = DockStyle.Top,
            Height = 48,
            Margin = new Padding(0, 0, 0, 8),
            Padding = new Padding(12, 0, 0, 0), // Empuja el texto e icono hacia la derecha
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(248, 250, 252), // #F8FAFC
            BackColor = Color.FromArgb(15, 23, 42),    // #0F172A
            Font = new Font("Segoe UI", 10F),
            Cursor = Cursors.Hand
        };
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85); // Hover #334155
        boton.Click += (_, _) => AbrirFormulario(crearFormulario(), boton);
        return boton;
    }

    // Carga un icono local desde la carpeta de recursos del proyecto.
    private static Image? CargarIconoLocal(string archivo)
    {
        try
        {
            var ruta = Path.GetFullPath(
                Path.Combine(Application.StartupPath, @"..\..\..\..\iconos", archivo));
            return Image.FromFile(ruta);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] No se pudo cargar el icono '{archivo}': {ex.Message}");
            return null;
        }
    }

    // Limpia el contenido actual y muestra un formulario dentro del panel central.
    private void AbrirFormulario(Form formulario, Button? boton)
    {
        if (botonActivo is not null)
        {
            // Restaura el color inactivo.
            botonActivo.BackColor = Color.FromArgb(15, 23, 42); // #0F172A
        }

        botonActivo = boton;
        if (botonActivo is not null)
        {
            // Aplica el color azul acento para el boton seleccionado.
            botonActivo.BackColor = Color.FromArgb(37, 99, 235); // #2563EB
        }

        // Eliminación del Borde Blanco (Fusión Total)
        if (formulario is DashboardForm)
        {
            panelContenido.Padding = new Padding(0);
        }
        else
        {
            panelContenido.Padding = new Padding(24);
        }

        // Libera el formulario anterior para evitar ventanas acumuladas.
        foreach (Control control in panelContenido.Controls)
        {
            control.Dispose();
        }

        panelContenido.Controls.Clear();
        formulario.TopLevel = false;
        formulario.FormBorderStyle = FormBorderStyle.None;
        formulario.Dock = DockStyle.Fill;
        panelContenido.Controls.Add(formulario);
        formulario.Show();
    }
}
