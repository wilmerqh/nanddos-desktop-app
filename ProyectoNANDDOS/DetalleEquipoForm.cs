using System.Data;

namespace ProyectoNANDDOS;

// Muestra la informacion completa de un equipo en modo solo lectura.
public class DetalleEquipoForm : Form
{
    // Fila de datos obtenida desde la consulta de Lista de Equipos.
    private readonly DataRow datos;

    public DetalleEquipoForm(DataRow datos)
    {
        this.datos = datos;
        InicializarComponentes();
    }

    // SECCION: construccion visual del detalle.
    private void InicializarComponentes()
    {
        Text = $"Detalles del equipo {Valor("codigo")}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 650);
        MinimumSize = new Size(720, 560);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(246, 248, 251);

        // Panel con scroll para que los textos largos siempre sean accesibles.
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(18),
            BackColor = BackColor
        };

        // Contenedor vertical de los grupos de informacion.
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 5,
            MinimumSize = new Size(680, 0)
        };
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Ajusta el ancho del contenido al tamano disponible.
        scroll.Resize += (_, _) =>
        {
            contenido.Width = Math.Max(680, scroll.ClientSize.Width - scroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        };

        contenido.Controls.Add(new Label
        {
            Text = $"Equipo {Valor("codigo")}",
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.FromArgb(25, 35, 50),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        contenido.Controls.Add(CrearGrupoCliente(), 0, 1);
        contenido.Controls.Add(CrearGrupoEquipo(), 0, 2);
        contenido.Controls.Add(CrearGrupoServicio(), 0, 3);

        var botones = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 48,
            Padding = new Padding(0, 8, 0, 0)
        };
        var btnCerrar = new Button { Text = "Cerrar", Width = 100, Height = 34, DialogResult = DialogResult.OK };
        botones.Controls.Add(btnCerrar);
        contenido.Controls.Add(botones, 0, 4);

        scroll.Controls.Add(contenido);
        Controls.Add(scroll);
        AcceptButton = btnCerrar;
    }

    // SECCION: datos del cliente.
    private Control CrearGrupoCliente()
    {
        var grupo = CrearGrupo("Cliente");
        var panel = CrearTabla(2, 3);

        AgregarCampo(panel, "Cliente", Valor("cliente"), 0, 0);
        AgregarCampo(panel, "Teléfono", Valor("telefono"), 1, 0);
        AgregarCampo(panel, "Correo", Valor("email"), 0, 2, colspan: 2);

        grupo.Controls.Add(panel);
        return grupo;
    }

    // SECCION: datos del equipo.
    private Control CrearGrupoEquipo()
    {
        var grupo = CrearGrupo("Equipo");
        var panel = CrearTabla(2, 4);

        AgregarCampo(panel, "Tipo de equipo", Valor("tipo_equipo"), 0, 0);
        AgregarCampo(panel, "Estado", Valor("estado"), 1, 0);
        AgregarCampo(panel, "Marca", Valor("marca"), 0, 2);
        AgregarCampo(panel, "Modelo", Valor("modelo"), 1, 2);
        AgregarCampo(panel, "Serial", Valor("serial"), 0, 4);
        AgregarCampo(panel, "Fecha de ingreso", Fecha("fecha_ingreso"), 1, 4);

        grupo.Controls.Add(panel);
        return grupo;
    }

    // SECCION: datos del servicio y entrega.
    private Control CrearGrupoServicio()
    {
        var grupo = CrearGrupo("Servicio");
        var panel = CrearTabla(2, 8);

        AgregarCampo(panel, "Problema reportado", Valor("descripcion_problema"), 0, 0, colspan: 2, multilinea: true);
        AgregarCampo(panel, "Repuestos", Valor("repuestos_necesarios"), 0, 2, colspan: 2, multilinea: true);
        AgregarCampo(panel, "Diagnóstico Técnico", Valor("diagnostico_tecnico"), 0, 4, colspan: 2, multilinea: true);
        AgregarCampo(panel, "Fecha de entrega", Fecha("fecha_entrega"), 0, 6);
        AgregarCampo(panel, "Costo", Costo("costo_total"), 1, 6);

        grupo.Controls.Add(panel);
        return grupo;
    }

    // Crea un grupo visual uniforme para cada bloque de informacion.
    private static GroupBox CrearGrupo(string texto)
    {
        return new GroupBox
        {
            Text = texto,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 12),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25, 35, 50)
        };
    }

    // Crea una tabla flexible con pares etiqueta/campo.
    private static TableLayoutPanel CrearTabla(int columnas, int paresDeFilas)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = columnas,
            RowCount = paresDeFilas * 2
        };

        for (var i = 0; i < columnas; i++)
        {
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columnas));
        }

        for (var i = 0; i < panel.RowCount; i++)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        return panel;
    }

    // Agrega una etiqueta y un TextBox de solo lectura a la tabla.
    private static void AgregarCampo(TableLayoutPanel panel, string etiqueta, string valor, int columna, int fila, int colspan = 1, bool multilinea = false)
    {
        var label = new Label
        {
            Text = etiqueta,
            Dock = DockStyle.Top,
            Height = 24,
            ForeColor = Color.FromArgb(75, 85, 99),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = new Padding(0, 0, 8, 0)
        };

        var textBox = new TextBox
        {
            Text = valor,
            Dock = DockStyle.Top,
            ReadOnly = true,
            BackColor = Color.White,
            Multiline = multilinea,
            Height = multilinea ? 82 : 32,
            ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None,
            Margin = new Padding(0, 0, 8, 10)
        };

        panel.Controls.Add(label, columna, fila);
        panel.Controls.Add(textBox, columna, fila + 1);
        if (colspan > 1)
        {
            panel.SetColumnSpan(label, colspan);
            panel.SetColumnSpan(textBox, colspan);
        }
    }

    // Devuelve texto seguro aunque la columna venga vacia o no exista.
    private string Valor(string columna)
    {
        if (!datos.Table.Columns.Contains(columna) || datos[columna] == DBNull.Value)
        {
            return "Sin registro";
        }

        var valor = datos[columna].ToString();
        return string.IsNullOrWhiteSpace(valor) ? "Sin registro" : valor;
    }

    // Formatea fechas para mostrarlas como dia/mes/ano.
    private string Fecha(string columna)
    {
        if (!datos.Table.Columns.Contains(columna) || datos[columna] == DBNull.Value)
        {
            return "Sin registro";
        }

        return Convert.ToDateTime(datos[columna]).ToString("dd/MM/yyyy");
    }

    // Formatea el costo en moneda local.
    private string Costo(string columna)
    {
        if (!datos.Table.Columns.Contains(columna) || datos[columna] == DBNull.Value)
        {
            return "Sin registro";
        }

        return $"Q {Convert.ToDecimal(datos[columna]):0.00}";
    }
}
