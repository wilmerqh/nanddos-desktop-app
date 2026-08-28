using System.Drawing.Drawing2D;

namespace ProyectoNANDDOS;

// Pantalla intermedia estilo Windows 11 OOBE que aparece despues del login.
// Muestra la identidad corporativa y un resumen de las capacidades del sistema.
public class BienvenidaForm : Form
{
    // Usuario autenticado que se muestra en el saludo principal.
    private readonly string usuario;

    public BienvenidaForm(string usuario)
    {
        this.usuario = usuario;
        InicializarComponentes();
    }

    // SECCION: construccion visual de la pantalla.
    private void InicializarComponentes()
    {
        Text = "Bienvenido - Proyecto NANDDOS";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 636);
        ClientSize = new Size(940, 676);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(239, 243, 248); // #EFF3F8
        FormBorderStyle = FormBorderStyle.None;
        DoubleBuffered = true;

        // Contenedor externo: centra la tarjeta blanca dentro de la ventana.
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1,
            Padding = new Padding(54)
        };

        // Tarjeta principal con bordes redondeados.
        var tarjeta = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0)
        };

        // Layout para centrar horizontalmente el contenido dentro de la tarjeta.
        var layoutCentrado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1
        };
        layoutCentrado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layoutCentrado.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 520));
        layoutCentrado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layoutCentrado.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layoutCentrado.Controls.Add(CrearContenidoPrincipal(), 1, 0);

        tarjeta.Controls.Add(layoutCentrado);
        contenedor.Controls.Add(tarjeta, 0, 0);
        Controls.Add(contenedor);

        // Inyecta la barra de titulo personalizada estilo Windows 11.
        BarraTitulo.Inyectar(this);
    }

    // SECCION: contenido central con estructura OOBE.
    private Control CrearContenidoPrincipal()
    {
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            Padding = new Padding(0, 24, 0, 20)
        };

        // Fila 0: Logo (120x80)
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        // Fila 1: Lema corporativo
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        // Fila 2: Espacio entre lema y titulo
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        // Fila 3: Titulo principal
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        // Fila 4: Subtitulo
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        // Fila 5: Espacio antes de las caracteristicas
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        // Fila 6: Item de caracteristica 1
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        // Fila 7: Item de caracteristica 2
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        // Fila 8: Boton + texto auxiliar (espacio restante)
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // --- SECCION: Logo y marca ---
        contenido.Controls.Add(ImagenEmpresa.CrearLogoCentrado(120, 80), 0, 0);

        contenido.Controls.Add(new Label
        {
            Text = "INNOVACIÓN EN SOPORTE TÉCNICO",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(156, 163, 175), // #9CA3AF
            Font = new Font("Segoe UI", 7F, FontStyle.Bold),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent
        }, 0, 1);

        // --- SECCION: Titulos principales ---
        contenido.Controls.Add(new Label
        {
            Text = $"Bienvenido al Sistema NANDDOS",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(15, 47, 91), // #0F2F5B
            Font = new Font("Segoe UI Semibold", 19F),
            TextAlign = ContentAlignment.BottomCenter,
            BackColor = Color.Transparent
        }, 0, 3);

        contenido.Controls.Add(new Label
        {
            Text = "Gestión Integral de Soporte Técnico",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(51, 65, 85), // #334155
            Font = new Font("Segoe UI", 12F),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent
        }, 0, 4);

        // --- SECCION: Lista de caracteristicas ---
        var carpetaIconos = Path.GetFullPath(
            Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        contenido.Controls.Add(CrearItemCaracteristica(
            CargarIcono(carpetaIconos, "feature1.png"),
            "Gestione Clientes, Equipos y Flujos de Trabajo Eficientes",
            "Centralice operaciones y asigne tareas en un entorno unificado y claro."
        ), 0, 6);

        contenido.Controls.Add(CrearItemCaracteristica(
            CargarIcono(carpetaIconos, "feature2.png"),
            "Seguimiento y Registro de Servicios Eficaz",
            "Controle informes de tickets, genere comprobantes y realice un seguimiento detallado."
        ), 0, 7);

        // --- SECCION: Boton y texto auxiliar ---
        contenido.Controls.Add(CrearZonaAccion(carpetaIconos), 0, 8);

        return contenido;
    }

    // Construye una fila de caracteristica: icono (32x32) + titulo + descripcion.
    private static Panel CrearItemCaracteristica(Image? icono, string titulo, string descripcion)
    {
        var fila = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(12, 4, 12, 4)
        };

        // Icono a la izquierda.
        var pic = new PictureBox
        {
            Image = icono,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Size = new Size(32, 32),
            Location = new Point(12, 10)
        };

        // Titulo de la caracteristica.
        var lblTitulo = new Label
        {
            Text = titulo,
            AutoSize = true,
            MaximumSize = new Size(420, 0),
            ForeColor = Color.FromArgb(15, 23, 42), // #0F172A
            Font = new Font("Segoe UI Semibold", 10F),
            BackColor = Color.Transparent,
            Location = new Point(56, 6)
        };

        // Descripcion con salto de linea automatico.
        var lblDescripcion = new Label
        {
            Text = descripcion,
            AutoSize = true,
            MaximumSize = new Size(420, 0),
            ForeColor = Color.FromArgb(100, 116, 139), // #64748B
            Font = new Font("Segoe UI", 9F),
            BackColor = Color.Transparent,
            Location = new Point(56, 28)
        };

        fila.Controls.Add(pic);
        fila.Controls.Add(lblTitulo);
        fila.Controls.Add(lblDescripcion);
        return fila;
    }

    // Construye la zona inferior con el boton Siguiente y un texto auxiliar.
    private Control CrearZonaAccion(string carpetaIconos)
    {
        var zona = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };
        zona.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        zona.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Panel dedicado para centrar manualmente el boton Siguiente.
        var panelBoton = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };
        var btnSiguiente = new RoundedButton
        {
            Text = "Continuar",
            Icono = CargarIcono(carpetaIconos, "flecha_derecha.png"),
            Width = 180,
            Height = 42
        };

        // Recalcula la posicion del boton cuando cambia el tamano del panel.
        void CentrarBoton()
        {
            btnSiguiente.Left = Math.Max(0, (panelBoton.Width - btnSiguiente.Width) / 2);
            btnSiguiente.Top = 8;
        }

        panelBoton.Resize += (_, _) => CentrarBoton();
        btnSiguiente.Click += (_, _) => AbrirMenuPrincipal();
        panelBoton.Controls.Add(btnSiguiente);
        CentrarBoton();
        zona.Controls.Add(panelBoton, 0, 0);

        zona.Controls.Add(new Label
        {
            Text = "Puedes continuar al menú principal cuando estés listo.",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(148, 163, 184), // #94A3B8
            Font = new Font("Segoe UI", 9F),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent
        }, 0, 1);

        return zona;
    }

    // Carga un icono desde la carpeta local de recursos del proyecto.
    // Si el archivo no existe, devuelve null y el PictureBox queda vacio.
    private static Image? CargarIcono(string carpeta, string archivo)
    {
        try
        {
            var ruta = Path.Combine(carpeta, archivo);
            return Image.FromFile(ruta);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] No se pudo cargar el icono '{archivo}': {ex.Message}");
            return null;
        }
    }

    // Abre el menu y cierra la bienvenida cuando el usuario continua.
    private void AbrirMenuPrincipal()
    {
        Hide();
        using var menu = new MenuPrincipalForm(usuario);
        menu.ShowDialog(this);
        Close();
    }

    // Panel con borde redondeado para dar aspecto de tarjeta moderna.
    private sealed class RoundedPanel : Panel
    {
        public RoundedPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            using var path = CrearRectanguloRedondeado(new Rectangle(0, 0, Width, Height), 18);
            Region = new Region(path);
        }

        // Dibuja manualmente el fondo y borde redondeado.
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);
            rect.Inflate(-1, -1);

            using var path = CrearRectanguloRedondeado(rect, 18);
            using var fondo = new SolidBrush(BackColor);
            using var borde = new Pen(Color.FromArgb(225, 232, 240), 1.5F);
            e.Graphics.FillPath(fondo, path);
            e.Graphics.DrawPath(borde, path);
        }
    }

    // Boton redondeado con soporte para icono alineado a la derecha del texto.
    private sealed class RoundedButton : Button
    {
        private bool hover;

        // Icono opcional que se dibuja a la derecha del texto.
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Image? Icono { get; set; }

        public RoundedButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI Semibold", 10.5F);
            Cursor = Cursors.Hand;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using var path = CrearRectanguloRedondeado(new Rectangle(0, 0, Width, Height), 10);
            Region = new Region(path);
        }

        // Cambia estado visual cuando el cursor entra.
        protected override void OnMouseEnter(EventArgs e)
        {
            hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        // Restaura estado visual cuando el cursor sale.
        protected override void OnMouseLeave(EventArgs e)
        {
            hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        // Dibuja el boton con color corporativo, texto e icono.
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);
            rect.Inflate(-1, -1);

            using var path = CrearRectanguloRedondeado(rect, 10);
            using var fondo = new SolidBrush(hover ? Color.FromArgb(29, 78, 216) : Color.FromArgb(37, 99, 235));
            e.Graphics.FillPath(fondo, path);

            // Calcula el ancho total del contenido (texto + espacio + icono).
            var tamanoTexto = TextRenderer.MeasureText(Text, Font);
            var anchoIcono = Icono != null ? 18 : 0;
            var espacio = Icono != null ? 6 : 0;
            var anchoTotal = tamanoTexto.Width + espacio + anchoIcono;

            // Centra el conjunto texto+icono dentro del boton.
            var xInicio = (Width - anchoTotal) / 2;
            var yTexto = (Height - tamanoTexto.Height) / 2;

            TextRenderer.DrawText(e.Graphics, Text, Font,
                new Point(xInicio, yTexto), Color.White);

            // Dibuja el icono a la derecha del texto si esta disponible.
            if (Icono != null)
            {
                var xIcono = xInicio + tamanoTexto.Width + espacio;
                var yIcono = (Height - 16) / 2;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(Icono, xIcono, yIcono, 16, 16);
            }
        }
    }

    // Crea la figura usada para paneles y botones redondeados.
    private static GraphicsPath CrearRectanguloRedondeado(Rectangle rect, int radio)
    {
        var path = new GraphicsPath();
        var diametro = radio * 2;
        path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);
        path.AddArc(rect.Right - diametro, rect.Y, diametro, diametro, 270, 90);
        path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diametro, diametro, diametro, 90, 90);
        path.CloseFigure();
        return path;
    }
}
