using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

// Modulo de administracion de Usuarios y asignacion de Cargos.
// Layout: DataGridView a la izquierda, editor de campos a la derecha.
public class GestionUsuariosForm : Form
{
    // Panel izquierdo: tabla de usuarios.
    private readonly DataGridView dgvUsuarios = new();

    // Panel derecho: editor del usuario.
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly ComboBox cmbCargos = new();
    
    // Botones de accion.
    private readonly Button btnNuevo = new();
    private readonly Button btnGuardar = new();
    private readonly Button btnEliminar = new();
    private readonly Button btnEstado = new(); // Cambiara entre Activar/Desactivar

    // Datos en memoria.
    private List<Usuario> usuarios = new();
    private List<Cargo> cargos = new();
    
    // Usuario actualmente seleccionado.
    private Usuario? usuarioSeleccionado = null;

    public GestionUsuariosForm()
    {
        ConfigurarUI();
        ConfigurarEstiloBotones();
        CargarCargos();
        CargarUsuarios();
    }

    // Construye el layout principal del formulario.
    private void ConfigurarUI()
    {
        Text = "Gestión de Usuarios Técnicos";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout principal: 1 fila, 2 columnas (lista 60% | editor 40%).
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(12)
        };
        principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(CrearPanelListaUsuarios(), 0, 0);
        principal.Controls.Add(CrearPanelEditor(), 1, 0);

        Controls.Add(principal);
    }

    // Crea el panel izquierdo con el titulo y el DataGridView.
    private Panel CrearPanelListaUsuarios()
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
            Text = "Usuarios Registrados",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lblTitulo, 0, 0);

        ConfigurarDataGridView();
        layout.Controls.Add(dgvUsuarios, 0, 1);

        panel.Controls.Add(layout);
        return panel;
    }

    private void ConfigurarDataGridView()
    {
        dgvUsuarios.Dock = DockStyle.Fill;
        dgvUsuarios.AllowUserToAddRows = false;
        dgvUsuarios.ReadOnly = true;
        dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsuarios.MultiSelect = false;
        dgvUsuarios.RowHeadersVisible = false;
        dgvUsuarios.BackgroundColor = Color.White;
        dgvUsuarios.Font = new Font("Segoe UI", 10F);
        dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsuarios.BorderStyle = BorderStyle.FixedSingle;

        dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdUsuario", Visible = false });
        dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "NombreCompleto", HeaderText = "Nombre Completo", FillWeight = 40 });
        dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Usuario", FillWeight = 25 });
        dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cargo", HeaderText = "Cargo", FillWeight = 25 });
        dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 10 });

        dgvUsuarios.SelectionChanged += (_, _) => SeleccionarUsuarioDeGrilla();
        
        // Efecto visual para filas inactivas
        dgvUsuarios.CellFormatting += (s, e) =>
        {
            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Estado" && e.Value != null)
            {
                if (e.Value.ToString() == "Inactivo")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font ?? dgvUsuarios.Font, FontStyle.Strikeout);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Green;
                }
            }
        };
    }

    // Crea el panel derecho con los campos de edicion y botones.
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Titulo
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Nombre
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Username
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Password
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Cargo
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Botones alineados arriba

        // Titulo del editor.
        var lblTitulo = new Label
        {
            Text = "Detalles del Usuario",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lblTitulo, 0, 0);
        layout.SetColumnSpan(lblTitulo, 2);

        // Nombre Completo.
        layout.Controls.Add(CrearEtiqueta("Nombre Completo:"), 0, 1);
        txtNombre.Dock = DockStyle.Fill;
        txtNombre.BorderStyle = BorderStyle.FixedSingle;
        layout.Controls.Add(txtNombre, 1, 1);

        // Username.
        layout.Controls.Add(CrearEtiqueta("Usuario (Login):"), 0, 2);
        txtUsername.Dock = DockStyle.Fill;
        txtUsername.BorderStyle = BorderStyle.FixedSingle;
        layout.Controls.Add(txtUsername, 1, 2);

        // Password.
        layout.Controls.Add(CrearEtiqueta("Contraseña:"), 0, 3);
        txtPassword.Dock = DockStyle.Fill;
        txtPassword.BorderStyle = BorderStyle.FixedSingle;
        txtPassword.UseSystemPasswordChar = true;
        layout.Controls.Add(txtPassword, 1, 3);

        // Cargo.
        layout.Controls.Add(CrearEtiqueta("Cargo Asignado:"), 0, 4);
        cmbCargos.Dock = DockStyle.Fill;
        cmbCargos.DropDownStyle = ComboBoxStyle.DropDownList;
        layout.Controls.Add(cmbCargos, 1, 4);

        // Panel de botones.
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 16, 0, 0)
        };

        btnNuevo.Text = "Nuevo";
        btnNuevo.Width = 100;
        btnNuevo.Height = 36;
        btnNuevo.Click += (_, _) => NuevoUsuario();

        btnGuardar.Text = "Guardar";
        btnGuardar.Width = 100;
        btnGuardar.Height = 36;
        btnGuardar.Click += (_, _) => GuardarUsuario();

        btnEliminar.Text = "Eliminar";
        btnEliminar.Width = 100;
        btnEliminar.Height = 36;
        btnEliminar.Click += (_, _) => EliminarUsuario();

        btnEstado.Text = "Desactivar";
        btnEstado.Width = 100;
        btnEstado.Height = 36;
        btnEstado.Click += (_, _) => CambiarEstado();

        panelBotones.Controls.Add(btnNuevo);
        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnEliminar);
        panelBotones.Controls.Add(btnEstado);

        layout.Controls.Add(panelBotones, 0, 5);
        layout.SetColumnSpan(panelBotones, 2);

        panel.Controls.Add(layout);
        return panel;
    }

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
        Estilizar(btnEstado, Color.FromArgb(245, 158, 11), Color.FromArgb(217, 119, 6));        // Naranja
    }

    // SECCION: Logica de datos y validaciones.

    private void CargarCargos()
    {
        try
        {
            cargos = CargoDAO.ObtenerCargos();
            cmbCargos.DataSource = null;
            cmbCargos.DataSource = cargos;
            cmbCargos.DisplayMember = nameof(Cargo.Nombre);
            cmbCargos.ValueMember = nameof(Cargo.IdCargo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar la lista de cargos.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CargarUsuarios()
    {
        try
        {
            usuarios = UsuarioDAO.ObtenerUsuarios();
            dgvUsuarios.Rows.Clear();

            foreach (var u in usuarios)
            {
                string textoEstado = u.Activo ? "Activo" : "Inactivo";
                string nombreCargo = u.EsSuperAdmin ? "Super Administrador" : u.NombreCargo;
                
                dgvUsuarios.Rows.Add(u.IdUsuario, u.NombreCompleto, u.Username, nombreCargo, textoEstado);
            }
            
            LimpiarEditor();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar los usuarios.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SeleccionarUsuarioDeGrilla()
    {
        if (dgvUsuarios.SelectedRows.Count == 0) return;

        int idSeleccionado = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["IdUsuario"].Value);
        usuarioSeleccionado = usuarios.Find(u => u.IdUsuario == idSeleccionado);

        if (usuarioSeleccionado == null) return;

        txtNombre.Text = usuarioSeleccionado.NombreCompleto;
        txtUsername.Text = usuarioSeleccionado.Username;
        txtPassword.Clear(); // Nunca mostrar la contrasena

        // Seleccionar el cargo en el ComboBox
        cmbCargos.SelectedValue = usuarioSeleccionado.IdCargo;

        // Configurar boton de estado
        if (usuarioSeleccionado.Activo)
        {
            btnEstado.Text = "Desactivar";
            btnEstado.BackColor = Color.FromArgb(245, 158, 11); // Naranja
            btnEstado.FlatAppearance.MouseOverBackColor = Color.FromArgb(217, 119, 6);
        }
        else
        {
            btnEstado.Text = "Activar";
            btnEstado.BackColor = Color.FromArgb(16, 185, 129); // Verde
            btnEstado.FlatAppearance.MouseOverBackColor = Color.FromArgb(5, 150, 105);
        }

        // SEGURIDAD VISUAL CRITICA:
        // Si el usuario es Super Administrador, bloquear controles sensibles.
        bool esSuper = usuarioSeleccionado.EsSuperAdmin;
        
        cmbCargos.Enabled = !esSuper;
        btnEstado.Visible = !esSuper;
        btnEstado.Enabled = !esSuper;
        btnEliminar.Visible = !esSuper;
        btnEliminar.Enabled = !esSuper;

        if (esSuper)
        {
            cmbCargos.SelectedIndex = -1; // Deseleccionar cargo porque es SuperAdmin y no depende de cargo
        }
    }

    private void LimpiarEditor()
    {
        dgvUsuarios.ClearSelection();
        usuarioSeleccionado = null;

        txtNombre.Clear();
        txtUsername.Clear();
        txtPassword.Clear();
        if (cmbCargos.Items.Count > 0) cmbCargos.SelectedIndex = 0;

        cmbCargos.Enabled = true;
        btnEstado.Visible = false; // Solo visible si hay usuario seleccionado
        btnEstado.Enabled = true;
        btnEliminar.Visible = false; // Solo visible si hay usuario seleccionado
        btnEliminar.Enabled = true;
        
        txtNombre.Focus();
    }

    private void NuevoUsuario()
    {
        LimpiarEditor();
    }

    private void GuardarUsuario()
    {
        string nombre = txtNombre.Text.Trim();
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(username))
        {
            MessageBox.Show("El nombre y el usuario (username) son campos obligatorios.", 
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (usuarioSeleccionado == null && string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Debe asignar una contraseña al registrar un usuario nuevo.", 
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbCargos.SelectedValue == null)
        {
            MessageBox.Show("Debe seleccionar un cargo válido.", 
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int idCargo = Convert.ToInt32(cmbCargos.SelectedValue);

        try
        {
            var userParaGuardar = new Usuario
            {
                IdUsuario = usuarioSeleccionado?.IdUsuario ?? 0,
                NombreCompleto = nombre,
                Username = username,
                Password = password,
                IdCargo = idCargo,
                EsSuperAdmin = usuarioSeleccionado?.EsSuperAdmin ?? false,
                Activo = usuarioSeleccionado?.Activo ?? true
            };

            UsuarioDAO.GuardarUsuario(userParaGuardar);
            
            MensajeNanddosForm.Mostrar(
                userParaGuardar.IdUsuario == 0 
                    ? $"Usuario '{username}' registrado exitosamente." 
                    : $"Usuario '{username}' actualizado correctamente.", 
                "Gestión de Usuarios");

            CargarUsuarios();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Elimina fisicamente un usuario de la base de datos (Hard Delete).
    private void EliminarUsuario()
    {
        if (usuarioSeleccionado == null) return;

        if (usuarioSeleccionado.EsSuperAdmin)
        {
            MessageBox.Show("No puedes eliminar la cuenta maestra del sistema.", 
                "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (usuarioSeleccionado.IdUsuario == SesionActual.IdUsuario)
        {
            MessageBox.Show("No puedes eliminar tu propia cuenta desde esta sesión.", 
                "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Está seguro de ELIMINAR PERMANENTEMENTE al usuario '{usuarioSeleccionado.Username}'?\n\n" +
            "Esta acción no se puede deshacer. Si el usuario tiene registros vinculados (equipos, entregas, etc.), " +
            "la eliminación podría ser bloqueada por la base de datos.",
            "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes) return;

        try
        {
            bool eliminado = UsuarioDAO.EliminarUsuario(usuarioSeleccionado.IdUsuario);
            if (eliminado)
            {
                MensajeNanddosForm.Mostrar($"Usuario '{usuarioSeleccionado.Username}' eliminado permanentemente.", "Gestión de Usuarios");
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show("No se pudo eliminar el usuario. Es posible que ya haya sido eliminado.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo eliminar el usuario.\n\n" +
                "Si tiene registros vinculados (equipos reparados, entregas, etc.), " +
                "MySQL bloquea la eliminación por integridad referencial.\n\n" +
                "Considere usar 'Desactivar' en su lugar.\n\n" +
                $"Error técnico: {ex.Message}",
                "Error de Eliminación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CambiarEstado()
    {
        if (usuarioSeleccionado == null) return;
        
        if (usuarioSeleccionado.EsSuperAdmin)
        {
            MessageBox.Show("No se puede cambiar el estado de la cuenta maestra del sistema.", 
                "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (usuarioSeleccionado.IdUsuario == SesionActual.IdUsuario)
        {
            MessageBox.Show("No puedes desactivar tu propia cuenta desde esta sesión.", 
                "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool nuevoEstado = !usuarioSeleccionado.Activo;
        string accionStr = nuevoEstado ? "Activar" : "Desactivar";
        
        var confirmacion = MessageBox.Show(
            $"¿Está seguro de {accionStr.ToLower()} al usuario '{usuarioSeleccionado.Username}'?", 
            "Confirmar Acción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes) return;

        try
        {
            UsuarioDAO.CambiarEstadoUsuario(usuarioSeleccionado.IdUsuario, nuevoEstado);
            MensajeNanddosForm.Mostrar($"Usuario {accionStr.ToLower()}do con éxito.", "Estado Actualizado");
            CargarUsuarios();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al {accionStr.ToLower()} el usuario.\n\n{ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
