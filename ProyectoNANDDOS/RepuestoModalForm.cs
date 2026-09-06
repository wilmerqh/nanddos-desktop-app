using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

public class RepuestoModalForm : Form
{
    private readonly ComboBox cmbPrefijo = new();
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtCategoria = new();
    private readonly NumericUpDown nudStock = new();
    private readonly NumericUpDown nudPrecioCosto = new();
    private readonly NumericUpDown nudPrecioVenta = new();

    private readonly Button btnGuardar = new();
    private readonly Button btnCancelar = new();

    private readonly Repuesto? repuestoEdicion;

    // Modo Agregar
    public RepuestoModalForm()
    {
        repuestoEdicion = null;
        ConfigurarUI();
        CargarPrefijos();
    }

    // Modo Edición
    public RepuestoModalForm(Repuesto repuesto)
    {
        repuestoEdicion = repuesto;
        ConfigurarUI();
        CargarPrefijos();
        
        Text = "Editar Repuesto";
        btnGuardar.Text = "Actualizar";

        cmbPrefijo.Text = repuesto.Codigo.Split('-')[0]; // Extrae el prefijo para mostrar, aunque este bloqueado
        cmbPrefijo.Enabled = false; // No se puede cambiar el prefijo/código en edición
        txtNombre.Text = repuesto.Nombre;
        txtCategoria.Text = repuesto.Categoria;
        nudStock.Value = repuesto.Stock;
        nudPrecioCosto.Value = repuesto.PrecioCosto;
        nudPrecioVenta.Value = repuesto.PrecioVenta;
    }

    private void ConfigurarUI()
    {
        Text = "Nuevo Repuesto";
        Size = new Size(500, 380);
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
            RowCount = 7,
            Padding = new Padding(20)
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        
        for (int i = 0; i < 6; i++)
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Botones

        // Prefijo (Categoría/Tipo)
        cmbPrefijo.Dock = DockStyle.Fill;
        tabla.Controls.Add(new Label { Text = "Prefijo:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        tabla.Controls.Add(cmbPrefijo, 1, 0);

        // Nombre
        txtNombre.Dock = DockStyle.Fill;
        tabla.Controls.Add(new Label { Text = "Nombre:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        tabla.Controls.Add(txtNombre, 1, 1);

        // Categoría
        txtCategoria.Dock = DockStyle.Fill;
        tabla.Controls.Add(new Label { Text = "Categoría:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        tabla.Controls.Add(txtCategoria, 1, 2);

        // Stock
        nudStock.Dock = DockStyle.Fill;
        nudStock.Maximum = 10000;
        tabla.Controls.Add(new Label { Text = "Stock Inicial:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        tabla.Controls.Add(nudStock, 1, 3);

        // Precio Costo (CONFIDENCIAL - Solo visible para Super Administrador)
        nudPrecioCosto.Dock = DockStyle.Fill;
        nudPrecioCosto.DecimalPlaces = 2;
        nudPrecioCosto.Maximum = 1000000;
        var lblPrecioCosto = new Label { Text = "Precio Costo ($):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        tabla.Controls.Add(lblPrecioCosto, 0, 4);
        tabla.Controls.Add(nudPrecioCosto, 1, 4);

        // Blindaje de Seguridad: Solo el Super Administrador puede ver y editar el precio de costo.
        if (!SesionActual.EsSuperAdministrador)
        {
            lblPrecioCosto.Visible = false;
            nudPrecioCosto.Visible = false;
        }

        // Precio Venta
        nudPrecioVenta.Dock = DockStyle.Fill;
        nudPrecioVenta.DecimalPlaces = 2;
        nudPrecioVenta.Maximum = 1000000;
        tabla.Controls.Add(new Label { Text = "Precio Venta ($):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        tabla.Controls.Add(nudPrecioVenta, 1, 5);

        // Panel de Botones
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 10, 0, 0)
        };

        btnGuardar.Text = "Guardar";
        btnGuardar.Width = 100;
        btnGuardar.Height = 35;
        btnGuardar.BackColor = Color.FromArgb(37, 99, 235);
        btnGuardar.ForeColor = Color.White;
        btnGuardar.FlatStyle = FlatStyle.Flat;
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Cursor = Cursors.Hand;
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.Width = 100;
        btnCancelar.Height = 35;
        btnCancelar.BackColor = Color.FromArgb(100, 116, 139);
        btnCancelar.ForeColor = Color.White;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;

        panelBotones.Controls.Add(btnCancelar);
        panelBotones.Controls.Add(btnGuardar);

        tabla.Controls.Add(panelBotones, 0, 6);
        tabla.SetColumnSpan(panelBotones, 2);

        Controls.Add(tabla);
        AcceptButton = btnGuardar;
        CancelButton = btnCancelar;
    }

    private void CargarPrefijos()
    {
        var prefijos = RepuestoDAO.ObtenerPrefijosExistentes();
        cmbPrefijo.Items.Clear();
        foreach (var p in prefijos)
        {
            cmbPrefijo.Items.Add(p);
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(cmbPrefijo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("El prefijo y el nombre son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (repuestoEdicion == null) // Nuevo
            {
                // Alerta de doble verificación de nombre similar
                if (RepuestoDAO.ExisteNombreSimilar(txtNombre.Text.Trim()))
                {
                    DialogResult res1 = MessageBox.Show(
                        "Se ha detectado un repuesto similar en el sistema. ¿Seguro que deseas registrar este nuevo repuesto?", 
                        "Advertencia de Similitud", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning);

                    if (res1 == DialogResult.Yes)
                    {
                        DialogResult res2 = MessageBox.Show(
                            "Esta acción creará un registro que podría ser un duplicado. ¿Estás completamente seguro de proceder?", 
                            "Doble Verificación", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Stop);

                        if (res2 == DialogResult.No) return;
                    }
                    else
                    {
                        return; // Aborta
                    }
                }

                // Genera el código secuencial
                string prefijoLimpio = cmbPrefijo.Text.Trim();
                string nuevoCodigo = RepuestoDAO.GenerarSiguienteCodigo(prefijoLimpio);

                var repuestoNuevo = new Repuesto
                {
                    Codigo = nuevoCodigo,
                    Nombre = txtNombre.Text.Trim(),
                    Categoria = txtCategoria.Text.Trim(),
                    Stock = (int)nudStock.Value,
                    PrecioCosto = nudPrecioCosto.Value,
                    PrecioVenta = nudPrecioVenta.Value,
                    FechaIngreso = DateTime.Now
                };

                RepuestoDAO.Insertar(repuestoNuevo);
                MensajeNanddosForm.Mostrar($"Repuesto guardado exitosamente con código: {nuevoCodigo}", "Éxito");
            }
            else // Edición
            {
                var repuestoEditado = new Repuesto
                {
                    IdRepuesto = repuestoEdicion.IdRepuesto,
                    Codigo = repuestoEdicion.Codigo, // Mantiene su código original
                    Nombre = txtNombre.Text.Trim(),
                    Categoria = txtCategoria.Text.Trim(),
                    Stock = (int)nudStock.Value,
                    PrecioCosto = nudPrecioCosto.Value,
                    PrecioVenta = nudPrecioVenta.Value,
                    FechaIngreso = repuestoEdicion.FechaIngreso
                };

                RepuestoDAO.Actualizar(repuestoEditado);
                MensajeNanddosForm.Mostrar("Repuesto actualizado exitosamente.", "Éxito");
            }

            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
