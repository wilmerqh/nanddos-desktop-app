# ==============================================================================
# MÓDULO 3: login_vista.py (Ventana de Inicio de Sesión validando con Bcrypt - OWASP A02)
# ==============================================================================

# Importamos customtkinter para construir la ventana moderna en modo oscuro
import customtkinter as ctk  # Librería gráfica con el nombre corto "ctk"
# Importamos messagebox desde tkinter para mostrar mensajes de alerta al usuario
from tkinter import messagebox  # Herramienta para mostrar ventanas emergentes
# Importamos la función validar_login que verifica el hash con bcrypt.checkpw()
from crud_computadoras import validar_login  # Función de autenticación segura con bcrypt
# Importamos la función iniciar_dashboard que abre la ventana principal de los 3 módulos
from dashboard_vista import iniciar_dashboard  # Función que abre el panel principal

# ==============================================================================
# 1. CONFIGURACIÓN DE LA PALETA MODO OSCURO LAB
# ==============================================================================

# Color azul muy oscuro para el fondo principal
COLOR_FONDO = "#0F172A"  # Fondo principal solicitado (#0F172A)
# Color gris azulado oscuro para el contenedor del formulario
COLOR_PANELES = "#1E293B"  # Color de paneles (#1E293B)
# Color azul celeste brillante para los botones y bordes
COLOR_BOTONES = "#0EA5E9"  # Color de botones (#0EA5E9)
# Color blanco suave para todos los textos
COLOR_TEXTO = "#F8FAFC"  # Color de texto (#F8FAFC)

# ==============================================================================
# 2. FUNCIÓN PARA CONSTRUIR Y MOSTRAR EL LOGIN
# ==============================================================================

# Definimos la función iniciar_login() que muestra la ventana de acceso seguro
def iniciar_login():  # Inicio de la función del Login
    # Activamos el modo oscuro general en CustomTkinter
    ctk.set_appearance_mode("dark")  # Tema oscuro activado

    # Creamos la ventana de inicio de sesión
    ventana_login = ctk.CTk()  # Instanciamos la ventana de CustomTkinter
    # Asignamos el título en la barra superior
    ventana_login.title("ControlLabITERN - Acceso Seguro (OWASP + Bcrypt)")  # Título de la ventana
    # Aplicamos el color de fondo oscuro (#0F172A)
    ventana_login.configure(fg_color=COLOR_FONDO)  # Pintamos el fondo de la ventana
    # Bloqueamos el cambio de tamaño para mantener el diseño centrado
    ventana_login.resizable(False, False)  # Tamaño fijo

    # Calculamos las coordenadas para centrar la ventana de 400x420 en el monitor
    ancho_ventana = 400  # Ancho de 400 píxeles
    alto_ventana = 420  # Alto de 420 píxeles
    ancho_pantalla = ventana_login.winfo_screenwidth()  # Ancho del monitor en píxeles
    alto_pantalla = ventana_login.winfo_screenheight()  # Alto del monitor en píxeles
    pos_x = int((ancho_pantalla / 2) - (ancho_ventana / 2))  # Posición horizontal centrada
    pos_y = int((alto_pantalla / 2) - (alto_ventana / 2))  # Posición vertical centrada
    # Aplicamos la geometría centrada a la ventana
    ventana_login.geometry(f"{ancho_ventana}x{alto_ventana}+{pos_x}+{pos_y}")  # Ventana centrada en pantalla

    # Función que se ejecuta al hacer clic en el botón 'Ingresar'
    def evento_ingresar():  # Valida las credenciales con bcrypt
        # Obtenemos el usuario escrito quitando espacios en blanco a los lados
        usuario_escrito = caja_usuario.get().strip()  # Leemos el usuario
        # Obtenemos la contraseña escrita quitando espacios a los lados
        contrasena_escrita = caja_contrasena.get().strip()  # Leemos la contraseña

        # Verificamos que ningún campo esté vacío antes de consultar la base de datos
        if not usuario_escrito or not contrasena_escrita:  # Si falta usuario o contraseña
            # Mostramos advertencia pidiendo llenar ambos campos
            messagebox.showwarning("Campos Requeridos", "Por favor, ingresa tu usuario y contraseña.")  # Aviso
            # Salimos de la función
            return  # Detenemos el proceso

        # Llamamos a validar_login() que verifica el hash con bcrypt.checkpw() (OWASP A02)
        if validar_login(usuario_escrito, contrasena_escrita):  # Si bcrypt valida correctamente la contraseña
            # Cerramos la ventana de login
            ventana_login.destroy()  # Destruimos la ventana de inicio de sesión
            # Abrimos el Dashboard Principal con los 3 módulos (Equipos, Áreas, Inventario)
            iniciar_dashboard()  # Ejecutamos la ventana principal
        else:  # Si las credenciales no coinciden
            # Mostramos un mensaje genérico de credenciales inválidas (sin decir cuál de los dos falló por seguridad)
            messagebox.showerror("Acceso Denegado", "Credenciales incorrectas. Verifique su usuario y contraseña.")  # Mensaje seguro

    # Creamos el panel central donde van las cajas de texto y el botón
    panel_login = ctk.CTkFrame(ventana_login, fg_color=COLOR_PANELES, corner_radius=15)  # Tarjeta central oscura
    # Empaquetamos el panel con margen de 30 píxeles alrededor
    panel_login.pack(fill="both", expand=True, padx=30, pady=30)  # Ocupa el centro de la ventana

    # Etiqueta del título principal
    etiqueta_titulo = ctk.CTkLabel(panel_login, text="Inicio de Sesión", font=("Segoe UI", 24, "bold"), text_color=COLOR_TEXTO)  # Título
    # Colocamos el título en la parte superior
    etiqueta_titulo.pack(pady=(22, 4))  # Margen superior

    # Etiqueta de subtítulo indicando protección Bcrypt
    etiqueta_subtitulo = ctk.CTkLabel(panel_login, text="Seguridad Criptográfica Bcrypt (OWASP)", font=("Segoe UI", 11), text_color=COLOR_BOTONES)  # Subtítulo
    # Colocamos el subtítulo debajo del título
    etiqueta_subtitulo.pack(pady=(0, 18))  # Margen inferior

    # Etiqueta para el campo Usuario
    etiqueta_usuario = ctk.CTkLabel(panel_login, text="Usuario:", font=("Segoe UI", 14, "bold"), text_color=COLOR_TEXTO)  # Etiqueta Usuario
    # Alineamos la etiqueta a la izquierda
    etiqueta_usuario.pack(anchor="w", padx=35, pady=(0, 4))  # Arriba de la caja de usuario

    # Caja de texto (Entry) para el Usuario
    caja_usuario = ctk.CTkEntry(panel_login, placeholder_text="Ej: admin", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BOTONES, height=38)  # Entry Usuario
    # Colocamos la caja de usuario en el panel
    caja_usuario.pack(fill="x", padx=35, pady=(0, 15))  # Se estira horizontalmente

    # Etiqueta para el campo Contraseña
    etiqueta_contrasena = ctk.CTkLabel(panel_login, text="Contraseña:", font=("Segoe UI", 14, "bold"), text_color=COLOR_TEXTO)  # Etiqueta Contraseña
    # Alineamos la etiqueta a la izquierda
    etiqueta_contrasena.pack(anchor="w", padx=35, pady=(0, 4))  # Arriba de la caja de contraseña

    # Caja de texto (Entry) para la Contraseña ocultando los caracteres con '*'
    caja_contrasena = ctk.CTkEntry(panel_login, placeholder_text="Ej: itern2026", show="*", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BOTONES, height=38)  # Entry Contraseña
    # Colocamos la caja de contraseña en el panel
    caja_contrasena.pack(fill="x", padx=35, pady=(0, 22))  # Se estira horizontalmente

    # Botón 'Ingresar' que dispara la validación con bcrypt
    boton_ingresar = ctk.CTkButton(panel_login, text="Ingresar", font=("Segoe UI", 16, "bold"), fg_color=COLOR_BOTONES, text_color=COLOR_TEXTO, hover_color="#0284C7", height=42, command=evento_ingresar)  # Botón de acceso
    # Colocamos el botón en la parte inferior del panel
    boton_ingresar.pack(fill="x", padx=35, pady=(0, 15))  # Mismo ancho que los Entry

    # Iniciamos el bucle principal de la ventana de login
    ventana_login.mainloop()  # Mantiene abierta la ventana

# Si se ejecuta directamente este archivo, abrimos la ventana de login
if __name__ == "__main__":  # Punto de arranque directo
    # Llamamos a la función iniciar_login
    iniciar_login()  # Inicia la ventana
