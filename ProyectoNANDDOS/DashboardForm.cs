using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

// Panel de control (Dashboard) principal del sistema con estilo corporativo.
public class DashboardForm : Form
{
    private readonly FlowLayoutPanel flpTarjetas = new();
    private readonly Label lblMensajeVacio = new();

    public DashboardForm()
    {
        InicializarComponentes();
        this.Load += DashboardForm_Load;
    }

    private void InicializarComponentes()
    {
        Text = "Dashboard";
        BackColor = Color.FromArgb(20, 25, 40);
        FormBorderStyle = FormBorderStyle.None;
        Dock = DockStyle.Fill;
        Font = new Font("Segoe UI", 10F);

        // Layout general: Logo arriba, Tarjetas en el centro
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(24)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 160)); // Logo
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Titulo sección
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Tarjetas

        // Logo centrado superior
        var picLogo = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.Zoom,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };
        
        // Cargar logo de manera segura
        try
        {
            string rutaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));
            picLogo.Image = Image.FromFile(Path.Combine(rutaIconos, "logo_transparente.png"));
        }
        catch
        {
            // Ignorar si no carga el logo
        }
        
        principal.Controls.Add(picLogo, 0, 0);

        // Título de la sección
        var lblTitulo = new Label
        {
            Text = "Estado Global de los Equipos",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.BottomCenter,
            Padding = new Padding(0, 0, 0, 10)
        };
        principal.Controls.Add(lblTitulo, 0, 1);

        // Contenedor dinámico de tarjetas
        flpTarjetas.Dock = DockStyle.Fill;
        flpTarjetas.AutoScroll = true;
        flpTarjetas.FlowDirection = FlowDirection.LeftToRight;
        flpTarjetas.WrapContents = true;
        flpTarjetas.Padding = new Padding(20);
        
        // Mensaje por si no hay datos
        lblMensajeVacio.Text = "No hay equipos registrados actualmente.";
        lblMensajeVacio.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
        lblMensajeVacio.ForeColor = Color.Gray;
        lblMensajeVacio.AutoSize = true;
        lblMensajeVacio.Visible = false;
        flpTarjetas.Controls.Add(lblMensajeVacio);

        principal.Controls.Add(flpTarjetas, 0, 2);

        Controls.Add(principal);
    }

    private void DashboardForm_Load(object? sender, EventArgs e)
    {
        CargarEstadisticas();
    }

    private void CargarEstadisticas()
    {
        try
        {
            flpTarjetas.Controls.Clear(); // Limpiar tarjetas previas
            
            var estadisticas = DashboardDAO.ObtenerEstadisticasEquipos();

            if (estadisticas.Count == 0)
            {
                lblMensajeVacio.Visible = true;
                flpTarjetas.Controls.Add(lblMensajeVacio);
                return;
            }

            // Crear tarjetas dinámicamente para cada estado
            foreach (var kvp in estadisticas)
            {
                var tarjeta = CrearTarjeta(kvp.Key, kvp.Value);
                flpTarjetas.Controls.Add(tarjeta);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudieron cargar las estadísticas del sistema.\n\n{ex.Message}", 
                "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Crea una tarjeta visual (Panel) para representar una estadística.
    private Panel CrearTarjeta(string estado, int cantidad)
    {
        var tarjeta = new Panel
        {
            Width = 175,
            Height = 110,
            Margin = new Padding(15),
            BackColor = Color.FromArgb(35, 40, 60),
            BorderStyle = BorderStyle.None
        };

        // Layout de la tarjeta
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // Titulo estado
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70)); // Numero grande

        // Obtener el color asociado al estado
        Color colorEstado = ObtenerColorEstado(estado);

        // Barra de color superior sutil
        var barraColor = new Panel
        {
            Dock = DockStyle.Top,
            Height = 6,
            BackColor = colorEstado
        };
        tarjeta.Controls.Add(barraColor);

        // Titulo del Estado
        var lblEstado = new Label
        {
            Text = estado.ToUpper(),
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.LightGray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Número Gigante
        var lblCantidad = new Label
        {
            Text = cantidad.ToString(),
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 28F, FontStyle.Bold),
            ForeColor = colorEstado,
            TextAlign = ContentAlignment.MiddleCenter
        };

        layout.Controls.Add(lblEstado, 0, 0);
        layout.Controls.Add(lblCantidad, 0, 1);
        
        tarjeta.Controls.Add(layout);
        
        // Efecto hover sutil (sombra manual cambiando el borde)
        tarjeta.MouseEnter += (s, e) => { tarjeta.BackColor = Color.FromArgb(45, 50, 70); };
        tarjeta.MouseLeave += (s, e) => { tarjeta.BackColor = Color.FromArgb(35, 40, 60); };

        return tarjeta;
    }

    // Retorna un color formal representativo para cada estado.
    private Color ObtenerColorEstado(string estado)
    {
        return estado switch
        {
            "En Diagnóstico" => Color.Magenta,
            "En Reparación" => Color.Orange,
            "En Espera de Repuestos" => Color.Crimson,
            "Terminado/Listo" => Color.SpringGreen,
            "Entregado" => Color.Cyan,
            "Inactivo" => Color.Gray,
            "Cancelado" => Color.DarkGray,
            _ => Color.DeepSkyBlue
        };
    }
}
