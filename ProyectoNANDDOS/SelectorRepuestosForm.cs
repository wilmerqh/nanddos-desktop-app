using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

public class SelectorRepuestosForm : Form
{
    public sealed class RepuestoComboItem
    {
        public int Id { get; }
        public string Codigo { get; }
        public string Nombre { get; }
        public decimal PrecioVenta { get; }
        public int StockDisponible { get; }

        public RepuestoComboItem(int id, string codigo, string nombre, decimal precioVenta, int stockDisponible)
        {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            PrecioVenta = precioVenta;
            StockDisponible = stockDisponible;
        }

        public override string ToString()
        {
            if (Id <= 0) return Nombre;
            return $"{Codigo} - {Nombre} (${PrecioVenta:0.00}) [Stock: {StockDisponible}]";
        }
    }

    private readonly ComboBox cmbRepuestos = new();
    private readonly NumericUpDown nudCantidad = new();
    private readonly Button btnAceptar = new();
    private readonly Button btnCancelar = new();

    public RepuestoComboItem? RepuestoSeleccionado { get; private set; }
    public int CantidadSeleccionada { get; private set; }

    public SelectorRepuestosForm()
    {
        ConfigurarUI();
        CargarRepuestos();
    }

    private void ConfigurarUI()
    {
        Text = "Seleccionar Producto Extra";
        Size = new Size(400, 250);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(20)
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Espacio
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Botones

        // Repuesto
        cmbRepuestos.Dock = DockStyle.Fill;
        cmbRepuestos.DropDownStyle = ComboBoxStyle.DropDownList;
        tabla.Controls.Add(new Label { Text = "Producto:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        tabla.Controls.Add(cmbRepuestos, 1, 0);

        // Cantidad
        nudCantidad.Dock = DockStyle.Fill;
        nudCantidad.Minimum = 1;
        nudCantidad.Maximum = 1000;
        tabla.Controls.Add(new Label { Text = "Cantidad:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        tabla.Controls.Add(nudCantidad, 1, 1);

        // Botones
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0)
        };

        btnAceptar.Text = "Agregar";
        btnAceptar.BackColor = Color.FromArgb(33, 111, 219);
        btnAceptar.ForeColor = Color.White;
        btnAceptar.FlatStyle = FlatStyle.Flat;
        btnAceptar.FlatAppearance.BorderSize = 0;
        btnAceptar.Height = 35;
        btnAceptar.Click += BtnAceptar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.BackColor = Color.White;
        btnCancelar.ForeColor = Color.FromArgb(25, 35, 50);
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnCancelar.Height = 35;
        btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;

        panelBotones.Controls.Add(btnAceptar);
        panelBotones.Controls.Add(btnCancelar);

        tabla.Controls.Add(panelBotones, 0, 3);
        tabla.SetColumnSpan(panelBotones, 2);

        Controls.Add(tabla);
    }

    private void CargarRepuestos()
    {
        cmbRepuestos.Items.Clear();
        var inventario = RepuestoDAO.ObtenerConStock();
        
        foreach (var item in inventario)
        {
            cmbRepuestos.Items.Add(new RepuestoComboItem(item.IdRepuesto, item.Codigo, item.Nombre, item.PrecioVenta, item.Stock));
        }

        if (cmbRepuestos.Items.Count > 0)
        {
            cmbRepuestos.SelectedIndex = 0;
        }
        else
        {
            cmbRepuestos.Items.Add(new RepuestoComboItem(0, "", "No hay repuestos con stock", 0, 0));
            cmbRepuestos.SelectedIndex = 0;
            btnAceptar.Enabled = false;
        }
    }

    private void BtnAceptar_Click(object? sender, EventArgs e)
    {
        if (cmbRepuestos.SelectedItem is not RepuestoComboItem seleccionado || seleccionado.Id <= 0)
        {
            MessageBox.Show("Selecciona un producto válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (nudCantidad.Value > seleccionado.StockDisponible)
        {
            MessageBox.Show($"Stock insuficiente. Solo hay {seleccionado.StockDisponible} disponibles.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RepuestoSeleccionado = seleccionado;
        CantidadSeleccionada = (int)nudCantidad.Value;
        DialogResult = DialogResult.OK;
    }
}
