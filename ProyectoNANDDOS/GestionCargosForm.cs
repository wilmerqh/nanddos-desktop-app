using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

// Clase wrapper para vincular un permiso al CheckedListBox con ID accesible.
public class PermisoUI
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;

    public override string ToString() => Display;
}

// Módulo de administración de Cargos (Roles) y asignación visual de Permisos.
// Layout: lista de cargos a la izquierda, editor + checklist de permisos a la derecha.
public class GestionCargosForm : Form
{
    // Panel izquierdo: lista de cargos.
    private readonly ListBox lstCargos = new();

    // Panel derecho: datos del cargo.
    private readonly TextBox txtNombreCargo = new();
    private readonly TextBox txtDescripcionCargo = new();

    // Contenedor dinámico de permisos separados por módulo.
    private readonly FlowLayoutPanel flpPermisos = new();

    // Botones de accion.
    private readonly Button btnNuevo = new();
    private readonly Button btnGuardar = new();
    private readonly Button btnEliminar = new();

    // Datos en memoria.
    private List<Cargo> cargos = new();
    private List<Permiso> todosLosPermisos = new();

    public GestionCargosForm()
    {
        ConfigurarUI();
        ConfigurarEstiloBotones();
        CargarDatos();
    }

    // Construye el layout principal del formulario.
    private void ConfigurarUI()
    {
        Text = "Gestión de Cargos y Permisos";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout principal: 1 fila, 2 columnas (lista 30% | editor 70%).
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(12)
        };
        principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Panel izquierdo: lista de cargos.
        principal.Controls.Add(CrearPanelListaCargos(), 0, 0);

        // Panel derecho: editor de cargo y permisos.
        principal.Controls.Add(CrearPanelEditor(), 1, 0);

        Controls.Add(principal);
    }

    // Crea el panel izquierdo con el titulo y la lista de cargos.
    private Panel CrearPanelListaCargos()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 0, 8, 0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblTitulo = new Label
        {
            Text = "Cargos del Sistema",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lblTitulo, 0, 0);

        lstCargos.Dock = DockStyle.Fill;
        lstCargos.Font = new Font("Segoe UI", 11F);
        lstCargos.BorderStyle = BorderStyle.FixedSingle;
        lstCargos.BackColor = Color.White;
        lstCargos.SelectedIndexChanged += (_, _) => CargarCargoSeleccionado();
        layout.Controls.Add(lstCargos, 0, 1);

        panel.Controls.Add(layout);
        return panel;
    }

    // Crea el panel derecho con los campos de edicion, checklist y botones.
    private Panel CrearPanelEditor()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 0, 0, 0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Titulo
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Nombre
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // Descripcion
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));  // Subtitulo permisos
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // FlowLayoutPanel de permisos
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Botones

        // Titulo del editor.
        var lblTitulo = new Label
        {
            Text = "Detalles del Cargo",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lblTitulo, 0, 0);
        layout.SetColumnSpan(lblTitulo, 2);

        // Nombre del cargo.
        layout.Controls.Add(CrearEtiqueta("Nombre:"), 0, 1);
        txtNombreCargo.Dock = DockStyle.Fill;
        txtNombreCargo.Font = new Font("Segoe UI", 10F);
        txtNombreCargo.BorderStyle = BorderStyle.FixedSingle;
        layout.Controls.Add(txtNombreCargo, 1, 1);

        // Descripcion del cargo.
        layout.Controls.Add(CrearEtiqueta("Descripción:"), 0, 2);
        txtDescripcionCargo.Dock = DockStyle.Fill;
        txtDescripcionCargo.Font = new Font("Segoe UI", 10F);
        txtDescripcionCargo.BorderStyle = BorderStyle.FixedSingle;
        txtDescripcionCargo.Multiline = true;
        txtDescripcionCargo.ScrollBars = ScrollBars.Vertical;
        layout.Controls.Add(txtDescripcionCargo, 1, 2);

        // Subtitulo de permisos.
        var lblPermisos = new Label
        {
            Text = "Permisos Asignados (Organizados por Módulo):",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(0, 0, 0, 4)
        };
        layout.Controls.Add(lblPermisos, 0, 3);
        layout.SetColumnSpan(lblPermisos, 2);

        // FlowLayoutPanel dinámico para módulos de permisos.
        flpPermisos.Dock = DockStyle.Fill;
        flpPermisos.AutoScroll = true;
        flpPermisos.FlowDirection = FlowDirection.TopDown;
        flpPermisos.WrapContents = false;
        flpPermisos.BackColor = Color.White;
        flpPermisos.BorderStyle = BorderStyle.FixedSingle;
        
        // Ajustar dinamicamente el ancho de los GroupBox hijos al cambiar el tamaño del panel
        flpPermisos.Resize += (s, e) => {
            foreach (Control c in flpPermisos.Controls)
            {
                c.Width = flpPermisos.ClientSize.Width - 10;
            }
        };

        layout.Controls.Add(flpPermisos, 0, 4);
        layout.SetColumnSpan(flpPermisos, 2);

        // Panel de botones.
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0)
        };

        btnNuevo.Text = "Nuevo";
        btnNuevo.Width = 110;
        btnNuevo.Height = 36;
        btnNuevo.Click += (_, _) => NuevoCargo();

        btnGuardar.Text = "Guardar";
        btnGuardar.Width = 110;
        btnGuardar.Height = 36;
        btnGuardar.Click += (_, _) => GuardarCargo();

        btnEliminar.Text = "Eliminar";
        btnEliminar.Width = 110;
        btnEliminar.Height = 36;
        btnEliminar.Click += (_, _) => EliminarCargo();

        panelBotones.Controls.Add(btnNuevo);
        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnEliminar);

        layout.Controls.Add(panelBotones, 0, 5);
        layout.SetColumnSpan(panelBotones, 2);

        panel.Controls.Add(layout);
        return panel;
    }

    // Crea una etiqueta con el estilo uniforme del formulario.
    private static Label CrearEtiqueta(string texto)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(51, 65, 85),
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    // Aplica estilo Fluent Design a los botones de accion.
    private void ConfigurarEstiloBotones()
    {
        void Estilizar(Button btn, Color fondo, Color hover)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = fondo;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = hover;
        }

        Estilizar(btnNuevo, Color.FromArgb(100, 116, 139), Color.FromArgb(71, 85, 105));       // Gris
        Estilizar(btnGuardar, Color.FromArgb(37, 99, 235), Color.FromArgb(29, 78, 216));        // Azul
        Estilizar(btnEliminar, Color.FromArgb(239, 68, 68), Color.FromArgb(220, 38, 38));       // Rojo
    }

    // SECCION: logica de datos.

    // Carga los cargos y estructura los contenedores de permisos por modulo desde la base de datos.
    private void CargarDatos()
    {
        try
        {
            cargos = CargoDAO.ObtenerCargos();
            todosLosPermisos = CargoDAO.ObtenerTodosLosPermisos();

            // Poblar la lista de cargos.
            lstCargos.Items.Clear();
            foreach (var c in cargos)
            {
                lstCargos.Items.Add(c.Nombre);
            }

            // Construir la nueva arquitectura de contenedores (GroupBox) para cada Modulo
            flpPermisos.Controls.Clear();

            var permisosAgrupados = todosLosPermisos
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Modulo) ? "General" : p.Modulo)
                .OrderBy(g => g.Key);

            foreach (var grupo in permisosAgrupados)
            {
                var gb = new GroupBox
                {
                    Text = grupo.Key.ToUpper(),
                    // El ancho se ajustara dinamicamente por el evento Resize de flpPermisos
                    Width = flpPermisos.ClientSize.Width > 0 ? flpPermisos.ClientSize.Width - 10 : 300,
                    AutoSize = true,
                    Padding = new Padding(5, 5, 5, 10),
                    Margin = new Padding(3, 3, 3, 10),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42)
                };

                var clb = new CheckedListBox
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    CheckOnClick = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                    BackColor = gb.BackColor
                };

                // Llenar el CheckedListBox interno usando la clase wrapper PermisoUI
                foreach (var p in grupo)
                {
                    clb.Items.Add(new PermisoUI
                    {
                        Id = p.IdPermiso,
                        Display = p.Descripcion
                    });
                }

                // Altura automatica para que no tenga scroll interno y muestre todos los items
                int itemHeight = Math.Max(clb.ItemHeight, 18);
                clb.Height = (clb.Items.Count * itemHeight) + 15;

                gb.Controls.Add(clb);
                flpPermisos.Controls.Add(gb);
            }

            LimpiarEditor();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar los datos.\n\n{ex.Message}",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Muestra los datos del cargo seleccionado y marca sus permisos.
    private void CargarCargoSeleccionado()
    {
        int indice = lstCargos.SelectedIndex;
        if (indice < 0 || indice >= cargos.Count)
        {
            return;
        }

        var cargo = cargos[indice];
        txtNombreCargo.Text = cargo.Nombre;
        txtDescripcionCargo.Text = cargo.Descripcion;

        // 1. Limpiar todos los CheckListBox de todos los contenedores.
        foreach (Control ctrl in flpPermisos.Controls)
        {
            if (ctrl is GroupBox gb && gb.Controls.Count > 0 && gb.Controls[0] is CheckedListBox clb)
            {
                for (int i = 0; i < clb.Items.Count; i++)
                {
                    clb.SetItemChecked(i, false);
                }
            }
        }

        // 2. Obtener los IDs de permisos asignados a este cargo.
        try
        {
            var idsAsignados = CargoDAO.ObtenerPermisosPorCargo(cargo.IdCargo);

            // 3. Iterar por cada modulo y marcar los que coincidan.
            foreach (Control ctrl in flpPermisos.Controls)
            {
                if (ctrl is GroupBox gb && gb.Controls.Count > 0 && gb.Controls[0] is CheckedListBox clb)
                {
                    for (int i = 0; i < clb.Items.Count; i++)
                    {
                        if (clb.Items[i] is PermisoUI pui && idsAsignados.Contains(pui.Id))
                        {
                            clb.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar permisos del cargo.\n\n{ex.Message}",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Seguridad visual: bloquear cargos protegidos.
        AplicarBloqueoProtegido(cargo.Protegido);
    }

    // Bloquea o desbloquea los controles segun si el cargo es protegido.
    private void AplicarBloqueoProtegido(bool protegido)
    {
        txtNombreCargo.ReadOnly = protegido;
        txtDescripcionCargo.ReadOnly = protegido;
        flpPermisos.Enabled = !protegido;
        btnGuardar.Enabled = !protegido;
        btnEliminar.Enabled = !protegido;

        // Cambiar color visual para indicar el bloqueo.
        var colorFondo = protegido ? Color.FromArgb(241, 245, 249) : Color.White;
        txtNombreCargo.BackColor = colorFondo;
        txtDescripcionCargo.BackColor = colorFondo;
        flpPermisos.BackColor = colorFondo;

        foreach (Control ctrl in flpPermisos.Controls)
        {
            if (ctrl is GroupBox gb && gb.Controls.Count > 0 && gb.Controls[0] is CheckedListBox clb)
            {
                gb.BackColor = colorFondo;
                clb.BackColor = colorFondo;
            }
        }
    }

    // Limpia el editor para preparar un nuevo cargo.
    private void LimpiarEditor()
    {
        txtNombreCargo.Clear();
        txtDescripcionCargo.Clear();
        
        foreach (Control ctrl in flpPermisos.Controls)
        {
            if (ctrl is GroupBox gb && gb.Controls.Count > 0 && gb.Controls[0] is CheckedListBox clb)
            {
                for (int i = 0; i < clb.Items.Count; i++)
                {
                    clb.SetItemChecked(i, false);
                }
            }
        }
        
        AplicarBloqueoProtegido(false);
    }

    // Prepara el formulario para registrar un cargo nuevo.
    private void NuevoCargo()
    {
        lstCargos.ClearSelected();
        LimpiarEditor();
        txtNombreCargo.Focus();
    }

    // Guarda el cargo actual (nuevo o existente) junto con sus permisos.
    private void GuardarCargo()
    {
        string nombre = txtNombreCargo.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("El nombre del cargo es obligatorio.",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            // Determinar si es edicion o creacion.
            int indice = lstCargos.SelectedIndex;
            var cargo = new Cargo
            {
                IdCargo = (indice >= 0 && indice < cargos.Count) ? cargos[indice].IdCargo : 0,
                Nombre = nombre,
                Descripcion = txtDescripcionCargo.Text.Trim()
            };

            // Extraccion critica: recolectar los IDs navegando por la nueva arquitectura visual.
            var idsPermisosMarcados = new List<int>();
            
            foreach (Control ctrl in flpPermisos.Controls)
            {
                if (ctrl is GroupBox gb && gb.Controls.Count > 0 && gb.Controls[0] is CheckedListBox clb)
                {
                    foreach (var item in clb.CheckedItems)
                    {
                        // Aseguramos el casteo correcto a la clase wrapper PermisoUI.
                        if (item is PermisoUI pui)
                        {
                            idsPermisosMarcados.Add(pui.Id);
                        }
                    }
                }
            }

            // Guardar con transaccion.
            CargoDAO.GuardarCargoYPermisos(cargo, idsPermisosMarcados);

            MensajeNanddosForm.Mostrar(
                cargo.IdCargo == 0
                    ? $"Cargo \"{nombre}\" creado exitosamente con {idsPermisosMarcados.Count} permiso(s)."
                    : $"Cargo \"{nombre}\" actualizado exitosamente con {idsPermisosMarcados.Count} permiso(s).",
                "Gestión de Cargos");

            // Recargar datos y reseleccionar.
            CargarDatos();

            // Intentar reseleccionar el cargo guardado.
            int nuevoIndice = lstCargos.Items.IndexOf(nombre);
            if (nuevoIndice >= 0)
            {
                lstCargos.SelectedIndex = nuevoIndice;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error SQL al guardar el cargo y permisos.\n\n{ex.Message}",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Elimina el cargo seleccionado previa confirmacion.
    private void EliminarCargo()
    {
        int indice = lstCargos.SelectedIndex;
        if (indice < 0 || indice >= cargos.Count)
        {
            MessageBox.Show("Selecciona un cargo de la lista para eliminarlo.",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var cargo = cargos[indice];

        if (cargo.Protegido)
        {
            MessageBox.Show("Este cargo está protegido y no puede ser eliminado.",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Está seguro de eliminar el cargo \"{cargo.Nombre}\"?\n\n" +
            "Se eliminarán también todos sus permisos asociados. Esta acción no se puede deshacer.",
            "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes) return;

        try
        {
            CargoDAO.EliminarCargo(cargo.IdCargo);
            MensajeNanddosForm.Mostrar($"Cargo \"{cargo.Nombre}\" eliminado exitosamente.", "Gestión de Cargos");
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar el cargo.\n\n{ex.Message}",
                "Gestión de Cargos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
