using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Pantalla de inicio de sesion del sistema.
// Diseno split screen: panel de branding (izquierda) y formulario de acceso (derecha).
public class LoginForm : Form
{
    // Win32 API para permitir arrastrar la ventana sin bordes nativos.
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    // Controles principales del formulario.
    private readonly TextBox txtUsuario;
    private readonly TextBox txtPassword;

    public LoginForm()
    {
        // Resuelve la ruta a la carpeta 'iconos' en la raiz de la solucion.
        // Desde bin\Debug\net8.0-windows sube 4 niveles hasta Sistema_NANDDOS\iconos.
        var carpetaIconos = Path.GetFullPath(
            Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        var campoUsuario = new CampoTextoModerno(
            "Ingresa tu usuario",
            CargarIcono(carpetaIconos, "usuario.png"));

        var campoPassword = new CampoTextoModerno(
            "Ingresa tu contraseña",
            CargarIcono(carpetaIconos, "lock_padlock_icon_152202.png"),
            esPassword: true);

        txtUsuario = campoUsuario.CajaTexto;
        txtPassword = campoPassword.CajaTexto;

        InicializarComponentes(campoUsuario, campoPassword);
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

    // Crea un contenedor con el icono corporativo centrado para la pantalla de Login.
    // Busca "icono_nanddos" en la carpeta iconos autodetectando la extension.
    private static Control CrearLogoLogin()
    {
        var contenedor = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        try
        {
            var carpetaIconos = Path.GetFullPath(
                Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

            string[] extensiones = [".png", ".jpg", ".jpeg", ".bmp"];
            string? rutaEncontrada = null;

            foreach (var ext in extensiones)
            {
                var ruta = Path.Combine(carpetaIconos, "icono_nanddos" + ext);
                if (File.Exists(ruta))
                {
                    rutaEncontrada = ruta;
                    break;
                }
            }

            if (rutaEncontrada is null)
            {
                return contenedor;
            }

            var picLogo = new PictureBox
            {
                Width = 90,
                Height = 90,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(rutaEncontrada),
                BackColor = Color.Transparent
            };

            // Mantiene el logo centrado dentro del contenedor.
            void Centrar()
            {
                picLogo.Left = Math.Max(0, (contenedor.Width - picLogo.Width) / 2);
                picLogo.Top = Math.Max(0, (contenedor.Height - picLogo.Height) / 2);
            }

            contenedor.Resize += (_, _) => Centrar();
            contenedor.Controls.Add(picLogo);
            Centrar();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] Error al cargar icono_nanddos en Login: {ex.Message}");
        }

        return contenedor;
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

    // SECCION: diseno del login con estilo Fluent Design.
    private void InicializarComponentes(CampoTextoModerno campoUsuario, CampoTextoModerno campoPassword)
    {
        Text = "NANDDOS - Inicio de Sesión";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(800, 450);
        FormBorderStyle = FormBorderStyle.None;
        DoubleBuffered = true;
        BackColor = Color.FromArgb(248, 250, 252);
        Font = new Font("Segoe UI", 10F);

        // Otorga foco al campo de usuario cuando la ventana aparece.
        Shown += (_, _) => txtUsuario.Focus();

        // Layout principal: branding (40%) + formulario (60%).
        var layoutPrincipal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layoutPrincipal.Controls.Add(CrearPanelBranding(), 0, 0);
        layoutPrincipal.Controls.Add(CrearPanelFormulario(campoUsuario, campoPassword), 1, 0);

        Controls.Add(layoutPrincipal);
    }

    // Panel izquierdo: identidad corporativa con logo y nombre.
    private Panel CrearPanelBranding()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(30, 41, 59), // #1E293B
            Margin = Padding.Empty
        };

        // Centra el contenido vertical y horizontalmente.
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = Color.Transparent,
            Padding = new Padding(24, 0, 24, 0)
        };
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Logo
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));  // Titulo
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // Subtitulo
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        contenido.Controls.Add(CrearLogoLogin(), 0, 1);

        var lblNombre = new Label
        {
            Text = "NANDDOS",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 22F),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent
        };
        contenido.Controls.Add(lblNombre, 0, 2);

        var lblSubtitulo = new Label
        {
            Text = "Sistema de Gestión Técnica",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(148, 163, 184), // #94A3B8
            Font = new Font("Segoe UI", 10F),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent
        };
        contenido.Controls.Add(lblSubtitulo, 0, 3);

        // Permite arrastrar la ventana desde cualquier parte del panel.
        HabilitarArrastre(panel);
        HabilitarArrastre(contenido);
        HabilitarArrastre(lblNombre);
        HabilitarArrastre(lblSubtitulo);

        panel.Controls.Add(contenido);
        return panel;
    }

    // Panel derecho: formulario de acceso con campos y boton.
    private Panel CrearPanelFormulario(CampoTextoModerno campoUsuario, CampoTextoModerno campoPassword)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252), // #F8FAFC
            Margin = Padding.Empty
        };
        HabilitarArrastre(panel);

        // Boton de cierre discreto en la esquina superior derecha.
        var btnCerrar = new Label
        {
            Text = "✕",
            Size = new Size(42, 34),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = new Font("Segoe UI", 11F),
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent
        };
        btnCerrar.Click += (_, _) => Application.Exit();
        btnCerrar.MouseEnter += (_, _) =>
        {
            btnCerrar.ForeColor = Color.FromArgb(220, 38, 38);
            btnCerrar.BackColor = Color.FromArgb(254, 226, 226);
        };
        btnCerrar.MouseLeave += (_, _) =>
        {
            btnCerrar.ForeColor = Color.FromArgb(148, 163, 184);
            btnCerrar.BackColor = Color.Transparent;
        };

        // Contenedor del formulario centrado con titulo, campos y boton.
        var formulario = new TableLayoutPanel
        {
            Size = new Size(320, 288),
            ColumnCount = 1,
            RowCount = 7,
            BackColor = Color.Transparent
        };
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));  // Titulo
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // Subtitulo
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));  // Espacio
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));  // Campo usuario
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));  // Campo contraseña
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // Espacio
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));  // Boton

        formulario.Controls.Add(new Label
        {
            Text = "Bienvenido de nuevo",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(15, 23, 42), // #0F172A
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomLeft
        }, 0, 0);

        formulario.Controls.Add(new Label
        {
            Text = "Por favor, ingresa tus credenciales",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(148, 163, 184), // #94A3B8
            Font = new Font("Segoe UI", 10F),
            TextAlign = ContentAlignment.TopLeft
        }, 0, 1);

        campoUsuario.Dock = DockStyle.Fill;
        campoUsuario.Margin = new Padding(0, 5, 0, 5);
        formulario.Controls.Add(campoUsuario, 0, 3);

        campoPassword.Dock = DockStyle.Fill;
        campoPassword.Margin = new Padding(0, 5, 0, 5);
        formulario.Controls.Add(campoPassword, 0, 4);

        var btnLogin = new BotonModerno
        {
            Text = "Iniciar Sesión",
            Dock = DockStyle.Fill
        };
        btnLogin.Click += (_, _) => IniciarSesion();
        formulario.Controls.Add(btnLogin, 0, 6);

        // Permite iniciar sesion presionando Enter en la contrasena.
        txtPassword.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                IniciarSesion();
                e.SuppressKeyPress = true;
            }
        };

        // Enter en el usuario mueve el foco al campo de contrasena.
        txtUsuario.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
                e.SuppressKeyPress = true;
            }
        };

        // Centra el formulario y posiciona el boton de cierre al redimensionar.
        panel.Resize += (_, _) =>
        {
            formulario.Location = new Point(
                Math.Max(0, (panel.Width - formulario.Width) / 2),
                Math.Max(0, (panel.Height - formulario.Height) / 2));
            btnCerrar.Location = new Point(panel.Width - btnCerrar.Width, 0);
        };

        panel.Controls.Add(formulario);
        panel.Controls.Add(btnCerrar);
        btnCerrar.BringToFront();
        return panel;
    }

    // Permite arrastrar la ventana haciendo clic sobre un control.
    private void HabilitarArrastre(Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        };
    }

    // SECCION: validacion de credenciales y carga de sesion.
    private void IniciarSesion()
    {
        var usuario = txtUsuario.Text.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Ingresa usuario y contraseña.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            // Busca los datos completos del usuario en la base de datos.
            using var conexion = ConexionDB.ObtenerConexion();
            using var comando = new MySqlCommand("""
                SELECT 
                    u.id_usuario,
                    u.nombre_completo,
                    u.usuario,
                    u.password_hash,
                    u.id_cargo,
                    u.es_superadministrador
                FROM usuarios u
                WHERE u.usuario = @usuario
                LIMIT 1;
                """, conexion);
            comando.Parameters.AddWithValue("@usuario", usuario);

            using var lector = comando.ExecuteReader();

            if (!lector.Read())
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Extraer el hash y verificar la contraseña con BCrypt.
            string hashAlmacenado = lector.GetString("password_hash");

            if (!BCrypt.Net.BCrypt.Verify(password, hashAlmacenado))
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Credenciales validas: poblar la sesion con los datos del usuario.
            SesionActual.IdUsuario = lector.GetInt32("id_usuario");
            SesionActual.NombreCompleto = lector.GetString("nombre_completo");
            SesionActual.Username = lector.GetString("usuario");
            SesionActual.IdCargo = lector.GetInt32("id_cargo");
            SesionActual.EsSuperAdministrador = lector.GetBoolean("es_superadministrador");

            // Cerrar el lector antes de ejecutar otra consulta en la misma conexion.
            lector.Close();

            // Cargar los permisos del cargo asignado al usuario.
            CargarPermisosDeCargo(conexion, SesionActual.IdCargo);

            // Login correcto: abre la bienvenida y luego cierra esta pantalla.
            Hide();
            using var bienvenida = new BienvenidaForm(SesionActual.NombreCompleto);
            bienvenida.ShowDialog(this);

            // Al cerrar la bienvenida, limpiar la sesion y cerrar la app.
            SesionActual.LimpiarSesion();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al iniciar sesión.\n\n{ex.Message}", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Carga los permisos asociados al cargo del usuario desde la tabla cargo_permiso.
    private static void CargarPermisosDeCargo(MySqlConnection conexion, int idCargo)
    {
        SesionActual.Permisos.Clear();

        try
        {
            using var comando = new MySqlCommand("""
                SELECT p.nombre_interno
                FROM permisos p
                INNER JOIN cargo_permiso cp ON p.id_permiso = cp.id_permiso
                WHERE cp.id_cargo = @id_cargo;
                """, conexion);
            comando.Parameters.AddWithValue("@id_cargo", idCargo);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                string nombreInterno = lector.GetString("nombre_interno");
                if (!string.IsNullOrWhiteSpace(nombreInterno))
                {
                    SesionActual.Permisos.Add(nombreInterno);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] Error al cargar permisos del cargo {idCargo}: {ex.Message}");
        }
    }

    // Campo de texto moderno con borde redondeado, icono y efecto de foco.
    private sealed class CampoTextoModerno : Panel
    {
        private bool estaEnfocado;
        private static readonly Color ColorBordeNormal = Color.FromArgb(203, 213, 225);  // #CBD5E1
        private static readonly Color ColorBordeFoco = Color.FromArgb(37, 99, 235);      // #2563EB
        private readonly PictureBox picIcono;

        public CampoTextoModerno(string placeholder, Image icono, bool esPassword = false)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Height = 46;
            BackColor = Color.Transparent;

            CajaTexto = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = placeholder,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42), // #0F172A
                UseSystemPasswordChar = esPassword,
                MaxLength = esPassword ? 100 : 50
            };

            picIcono = new PictureBox
            {
                Image = icono,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Size = new Size(20, 20)
            };

            // Centra verticalmente el icono y el TextBox cuando cambia el tamano.
            Resize += (_, _) =>
            {
                int centroY = (Height - CajaTexto.Height) / 2;
                int iconCentroY = (Height - picIcono.Height) / 2;
                
                picIcono.Location = new Point(16, iconCentroY);
                CajaTexto.Location = new Point(46, centroY);
                CajaTexto.Width = Math.Max(1, Width - 58);
            };

            // Cambia el color del borde al recibir o perder el foco.
            CajaTexto.Enter += (_, _) =>
            {
                estaEnfocado = true;
                Invalidate();
            };
            CajaTexto.Leave += (_, _) =>
            {
                estaEnfocado = false;
                Invalidate();
            };

            Controls.Add(CajaTexto);
            Controls.Add(picIcono);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            using var path = CrearRectanguloRedondeado(new Rectangle(0, 0, Width, Height), 8);
            Region = new Region(path);
        }

        // Expone el TextBox interno para conectar eventos y obtener valores.
        public TextBox CajaTexto { get; }

        // Dibuja el fondo blanco y el borde redondeado con efecto de foco.
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);
            rect.Inflate(-1, -1); // Evita que el borde sea cortado por el Region

            using var path = CrearRectanguloRedondeado(rect, 8);
            using var fondo = new SolidBrush(Color.White);
            using var borde = new Pen(
                estaEnfocado ? ColorBordeFoco : ColorBordeNormal,
                estaEnfocado ? 2F : 1.5F);
            
            e.Graphics.FillPath(fondo, path);
            e.Graphics.DrawPath(borde, path);
        }
    }

    // Boton moderno con esquinas redondeadas y efecto hover.
    private sealed class BotonModerno : Button
    {
        private bool hover;
        private static readonly Color ColorNormal = Color.FromArgb(37, 99, 235);  // #2563EB
        private static readonly Color ColorHover = Color.FromArgb(29, 78, 216);   // #1D4ED8

        public BotonModerno()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI Semibold", 12F);
            Cursor = Cursors.Hand;
            Height = 44;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using var path = CrearRectanguloRedondeado(new Rectangle(0, 0, Width, Height), 8);
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

        // Dibuja el boton con fondo redondeado y texto centrado.
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = new Rectangle(0, 0, Width, Height);
            rect.Inflate(-1, -1);

            using var path = CrearRectanguloRedondeado(rect, 8);
            using var fondo = new SolidBrush(hover ? ColorHover : ColorNormal);
            e.Graphics.FillPath(fondo, path);
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    // Crea un trazado con esquinas redondeadas para paneles y botones.
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
