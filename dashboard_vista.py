# ==============================================================================
# MÓDULO 4: dashboard_vista.py
# Rediseño Visual Premium 'Cyberpunk / Dark Slate' (Figma Desktop UI)
# Sistema de Gestión de Laboratorio 'ControlLabITERN' - Seguridad OWASP Top 10
# 100% Offline con CustomTkinter Nativo y ttk.Style (Sin librerías externas)
# ==============================================================================

# Importamos la librería CustomTkinter para construir la interfaz gráfica moderna
import customtkinter as ctk  # Alias estándar "ctk" para invocar componentes modernos
# Importamos submódulos esenciales de la librería estándar tkinter
from tkinter import ttk, messagebox  # ttk para tablas Treeview con estilo y messagebox para diálogos
# Importamos las funciones de operaciones seguras de base de datos desde crud_computadoras.py
from crud_computadoras import (  # Importación estructurada de funciones CRUD seguras
    inicializar_tablas_seguras,  # Verifica y crea las 4 tablas en MySQL si no existen
    registrar_equipo,  # Inserta una computadora con sentencias preparadas (%s)
    registrar_pc,  # Alias directo para registrar computadoras con codigo_pc autoincrementable
    actualizar_equipo,  # Modifica los datos de una computadora por su codigo_pc (%s)
    actualizar_pc,  # Alias directo para actualizar computadoras por codigo_pc
    eliminar_equipo,  # Elimina una computadora por su codigo_pc (%s)
    eliminar_pc,  # Alias directo para eliminar computadoras por codigo_pc
    obtener_equipos,  # Consulta todas las computadoras registradas en MySQL
    obtener_equipos_por_area,  # Filtra computadoras por id_area con sentencia preparada (%s)
    buscar_equipos,  # Busca computadoras usando sentencia preparada LIKE %s
    obtener_areas,  # Consulta las 2 áreas físicas oficiales (Laboratorio y Bodega)
    registrar_articulo_inventario,  # Inserta herramientas o periféricos con %s
    actualizar_articulo_inventario,  # Modifica un artículo del inventario por su ID (%s)
    eliminar_articulo_inventario,  # Elimina un artículo del inventario por su ID (%s)
    obtener_articulos_inventario,  # Consulta todos los artículos del inventario
    buscar_articulos_inventario,  # Busca artículos usando sentencia preparada LIKE %s
    registrar_usuario,  # Inserta un nuevo usuario cifrando su clave con bcrypt (OWASP A02)
    obtener_usuarios,  # Consulta usuarios (solo ID y Usuario, nunca el hash de la clave)
    buscar_usuarios,  # Busca usuarios por nombre con sentencia preparada LIKE %s
    eliminar_usuario,  # Elimina un usuario por su ID (%s)
    cambiar_contrasena_usuario  # Actualiza la contraseña de un usuario con bcrypt (OWASP A02)
)  # Fin de la importación de funciones CRUD seguras
# Importamos la función de generación de reportes en PDF desde generador_pdf.py
from generador_pdf import exportar_inventario_pdf  # Función para generar el reporte PDF del inventario

# ==============================================================================
# 1. PALETA DE COLORES PROFESIONAL 'CYBERPUNK / DARK SLATE' (FIGMA DESIGN)
# ==============================================================================

# Color azul oscuro pizarra profundo (Slate-900) para el fondo de la ventana principal
COLOR_FONDO = "#0F172A"  # Fondo base de la aplicación (#0F172A)
# Color gris azulado profundo (Slate-800) para paneles, tarjetas y contenedores de formularios
COLOR_PANELES = "#1E293B"  # Superficie de componentes y tarjetas (#1E293B)
# Color gris pizarra medio (Slate-700) para bordes sutiles y elegantes en tarjetas y campos
COLOR_BORDES = "#334155"  # Bordes visuales armónicos (#334155)
# Color blanco suave (Slate-50) para títulos principales y texto de máxima legibilidad
COLOR_TEXTO = "#F8FAFC"  # Tipografía principal (#F8FAFC)
# Color gris plateado suave (Slate-400) para subtítulos, etiquetas secundarias y placeholders
COLOR_MUTED = "#94A3B8"  # Tipografía secundaria (#94A3B8)
# Color azul celeste neón vibrante (Sky-500) para botón de navegación activo y acentos
COLOR_PRIMARIO = "#0EA5E9"  # Acento principal activo (#0EA5E9)
# Color azul oscuro para el efecto de desplazamiento hover sobre el botón primario
COLOR_PRIMARIO_HOVER = "#0284C7"  # Hover primario (#0284C7)
# Color verde esmeralda (Emerald-500) para acciones de éxito, registrar y métrica Activas
COLOR_EXITO = "#10B981"  # Color de éxito y guardado (#10B981)
# Color verde esmeralda oscuro para el efecto hover del botón de guardar
COLOR_EXITO_HOVER = "#059669"  # Hover éxito (#059669)
# Color azul real profesional (Blue-500) para acciones de edición y actualización
COLOR_EDITAR = "#3B82F6"  # Color de edición (#3B82F6)
# Color azul intenso para el efecto hover del botón de editar
COLOR_EDITAR_HOVER = "#2563EB"  # Hover edición (#2563EB)
# Color rojo carmesí vibrante (Red-500) para eliminación y métrica de equipos dañados
COLOR_PELIGRO = "#EF4444"  # Color de peligro y eliminación (#EF4444)
# Color rojo oscuro para el efecto hover del botón de eliminación
COLOR_PELIGRO_HOVER = "#DC2626"  # Hover peligro (#DC2626)
# Color ámbar dorado (Amber-500) para advertencias y métrica de equipos en mantenimiento
COLOR_ALERTA = "#F59E0B"  # Color de advertencia (#F59E0B)
# Color ámbar oscuro para el efecto hover en advertencias
COLOR_ALERTA_HOVER = "#D97706"  # Hover advertencia (#D97706)

# ==============================================================================
# 2. FUNCIÓN CONSTRUCTORA PRINCIPAL DEL DASHBOARD
# ==============================================================================

# Definimos la función principal que inicializa la interfaz gráfica del Dashboard
def iniciar_dashboard():  # Función que despliega la ventana moderna de ControlLabITERN
    # Verificamos y garantizamos que las 4 tablas en MySQL existan antes de dibujar la ventana
    inicializar_tablas_seguras()  # Inicialización preventiva en MySQL

    # Configuramos el modo de apariencia global en modo oscuro ('dark')
    ctk.set_appearance_mode("dark")  # Activa tema oscuro nativo de CustomTkinter

    # Instanciamos la ventana principal de CustomTkinter
    ventana_dashboard = ctk.CTk()  # Ventana raíz del Dashboard
    # Asignamos el título formal de la aplicación en la barra de ventana
    ventana_dashboard.title("ControlLabITERN - Gestión Integral de Laboratorio (Seguridad OWASP)")  # Título
    # Establecemos las dimensiones iniciales cómodas para escritorio (1140x740 píxeles)
    ventana_dashboard.geometry("1140x740")  # Dimensiones optimizadas para Figma UI
    # Fijamos el tamaño mínimo de la ventana para proteger la jerarquía visual
    ventana_dashboard.minsize(1040, 680)  # Dimensiones mínimas permitidas
    # Pintamos el fondo principal con el tono azul pizarra profundo #0F172A
    ventana_dashboard.configure(fg_color=COLOR_FONDO)  # Aplicación del fondo Slate-900

    # ==========================================================================
    # 3. ESTILO VISUAL MODO OSCURO PARA TODAS LAS TABLAS TREEVIEW (TTK)
    # ==========================================================================

    # Creamos el objeto Style de ttk para personalizar las tablas del sistema
    estilo_tablas = ttk.Style()  # Administrador de estilos TTK
    # Seleccionamos el tema 'clam' que permite control total de colores oscuros sin bordes nativos
    estilo_tablas.theme_use("clam")  # Tema 'clam' para personalización visual limpia

    # Configuramos el aspecto de las filas y celdas del Treeview
    estilo_tablas.configure(  # Configuración visual de las celdas
        "Treeview",  # Selector del componente Treeview
        background=COLOR_PANELES,  # Fondo de cada fila con tono gris azulado #1E293B
        foreground=COLOR_TEXTO,  # Texto legible en color blanco suave #F8FAFC
        fieldbackground=COLOR_PANELES,  # Fondo uniforme de las áreas vacías de la tabla
        rowheight=34,  # Altura generosa de 34 píxeles para toque moderno y cómodo
        font=("Segoe UI", 10),  # Fuente clara y legible
        borderwidth=0,  # Eliminamos bordes toscos de Windows
        relief="flat"  # Aspecto completamente plano y moderno
    )  # Fin de la configuración de celdas

    # Configuramos el aspecto de los encabezados superiores de todas las tablas
    estilo_tablas.configure(  # Configuración visual de los encabezados
        "Treeview.Heading",  # Selector de los encabezados de columna
        background=COLOR_FONDO,  # Fondo azul oscuro #0F172A para contraste con las filas
        foreground=COLOR_MUTED,  # Texto gris plateado #94A3B8 para jerarquía visual
        font=("Segoe UI", 10, "bold"),  # Letra negrita para distinguir los títulos
        borderwidth=0,  # Sin bordes de separación antiguos
        relief="flat"  # Diseño plano tipo web moderna
    )  # Fin de la configuración de encabezados

    # Mapeamos los colores dinámicos cuando el usuario selecciona una fila en la tabla
    estilo_tablas.map(  # Asignación de estados dinámicos
        "Treeview",  # Aplicado a la tabla de datos
        background=[("selected", COLOR_PRIMARIO)],  # Fondo azul neón #0EA5E9 al hacer clic
        foreground=[("selected", "#FFFFFF")]  # Texto en blanco puro de alto contraste al seleccionar
    )  # Fin del mapeo dinámico de celdas

    # Mapeamos los colores al pasar el cursor o hacer clic sobre los encabezados
    estilo_tablas.map(  # Asignación de estados dinámicos para encabezados
        "Treeview.Heading",  # Aplicado a los títulos de columna
        background=[("active", COLOR_PANELES)],  # Fondo gris azulado al pasar el mouse
        foreground=[("active", COLOR_TEXTO)]  # Texto blanco al interactuar
    )  # Fin del mapeo de encabezados

    # ==========================================================================
    # 4. GESTIÓN DE NAVEGACIÓN Y ACTUALIZACIÓN VISUAL DEL SIDEBAR
    # ==========================================================================

    # Función que actualiza el estado visual de los 4 botones del menú lateral
    def actualizar_sidebar(modulo_activo):  # Cambia colores según el módulo activo
        # Diccionario que asocia el identificador de cada módulo con su botón correspondiente
        botones_sidebar = {  # Mapeo de botones del menú lateral
            "equipos": boton_nav_equipos,  # Botón de Equipos
            "areas": boton_nav_areas,  # Botón de Áreas
            "inventario": boton_nav_inventario,  # Botón de Inventario
            "usuarios": boton_nav_usuarios  # Botón de Usuarios
        }  # Fin del diccionario
        # Recorremos cada botón del menú lateral
        for clave, boton in botones_sidebar.items():  # Itera identificador y widget
            # Si el botón coincide con el módulo activo actualmente en pantalla
            if clave == modulo_activo:  # Módulo seleccionado
                # Asignamos el fondo azul neón brillante #0EA5E9 con texto blanco puro
                boton.configure(fg_color=COLOR_PRIMARIO, text_color="#FFFFFF", hover_color=COLOR_PRIMARIO_HOVER)  # Activo
            # Si el botón pertenece a un módulo inactivo
            else:  # Módulos no seleccionados
                # Fondo transparente con texto gris plateado suave #94A3B8
                boton.configure(fg_color="transparent", text_color=COLOR_MUTED, hover_color=COLOR_PANELES)  # Inactivo

    # Función que oculta los 4 paneles de módulos de la vista central
    def ocultar_todos_los_modulos():  # Limpia el área central antes de mostrar otro módulo
        # Ocultamos el panel de Equipos si estaba visible
        panel_modulo_equipos.pack_forget()  # Oculta Equipos
        # Ocultamos el panel de Áreas si estaba visible
        panel_modulo_areas.pack_forget()  # Oculta Áreas
        # Ocultamos el panel de Inventario si estaba visible
        panel_modulo_inventario.pack_forget()  # Oculta Inventario
        # Ocultamos el panel de Usuarios si estaba visible
        panel_modulo_usuarios.pack_forget()  # Oculta Usuarios

    # Función para mostrar y activar el Módulo 1: Equipos
    def mostrar_modulo_equipos():  # Activa la vista de Equipos
        # Ocultamos cualquier otro módulo activo previamente
        ocultar_todos_los_modulos()  # Limpieza central
        # Destacamos visualmente el botón de Equipos en el menú lateral
        actualizar_sidebar("equipos")  # Botón activo: Equipos
        # Empaquetamos el panel de Equipos con márgenes amplios de 20px
        panel_modulo_equipos.pack(fill="both", expand=True, padx=20, pady=20)  # Muestra Equipos
        # Recargamos la tabla de equipos con los datos más recientes desde MySQL
        cargar_tabla_equipos()  # Llena Treeview y actualiza tarjetas de métricas

    # Función para mostrar y activar el Módulo 2: Áreas
    def mostrar_modulo_areas():  # Activa la vista de Áreas
        # Ocultamos cualquier otro módulo activo previamente
        ocultar_todos_los_modulos()  # Limpieza central
        # Destacamos visualmente el botón de Áreas en el menú lateral
        actualizar_sidebar("areas")  # Botón activo: Áreas
        # Empaquetamos el panel de Áreas con márgenes amplios
        panel_modulo_areas.pack(fill="both", expand=True, padx=20, pady=20)  # Muestra Áreas
        # Recargamos la tabla con las 2 áreas físicas oficiales (Laboratorio y Bodega)
        cargar_tabla_areas()  # Llena Treeview de Áreas

    # Función para mostrar y activar el Módulo 3: Inventario
    def mostrar_modulo_inventario():  # Activa la vista de Inventario
        # Ocultamos cualquier otro módulo activo previamente
        ocultar_todos_los_modulos()  # Limpieza central
        # Destacamos visualmente el botón de Inventario en el menú lateral
        actualizar_sidebar("inventario")  # Botón activo: Inventario
        # Empaquetamos el panel de Inventario con márgenes amplios
        panel_modulo_inventario.pack(fill="both", expand=True, padx=20, pady=20)  # Muestra Inventario
        # Recargamos los artículos del inventario desde MySQL
        cargar_tabla_inventario()  # Llena Treeview de Inventario

    # Función para mostrar y activar el Módulo 4: Usuarios
    def mostrar_modulo_usuarios():  # Activa la vista de Usuarios
        # Ocultamos cualquier otro módulo activo previamente
        ocultar_todos_los_modulos()  # Limpieza central
        # Destacamos visualmente el botón de Usuarios en el menú lateral
        actualizar_sidebar("usuarios")  # Botón activo: Usuarios
        # Empaquetamos el panel de Usuarios con márgenes amplios
        panel_modulo_usuarios.pack(fill="both", expand=True, padx=20, pady=20)  # Muestra Usuarios
        # Recargamos la lista de cuentas seguras desde MySQL
        cargar_tabla_usuarios()  # Llena Treeview de Usuarios

    # ==========================================================================
    # 5. MENÚ LATERAL IZQUIERDO (SIDEBAR PREMIUM ESTILO FIGMA)
    # ==========================================================================

    # Creamos el contenedor vertical del menú lateral con ancho fijo de 230 píxeles
    panel_menu_lateral = ctk.CTkFrame(ventana_dashboard, width=230, fg_color=COLOR_PANELES, corner_radius=0)  # Sidebar
    # Empaquetamos el menú lateral anclado a la izquierda estirándose verticalmente
    panel_menu_lateral.pack(side="left", fill="y")  # Ocupa toda la altura lateral izquierda
    # Bloqueamos el tamaño del sidebar para mantener sus 230 píxeles fijos
    panel_menu_lateral.pack_propagate(False)  # Mantiene dimensiones rígidas

    # Contenedor superior para el logo y nombre de la institución
    cabecera_sidebar = ctk.CTkFrame(panel_menu_lateral, fg_color="transparent")  # Marco para cabecera del logo
    # Empaquetamos el contenedor del logo
    cabecera_sidebar.pack(fill="x", padx=16, pady=(30, 20))  # Márgenes superiores

    # Icono emoji principal del laboratorio
    etiqueta_icono_sidebar = ctk.CTkLabel(cabecera_sidebar, text="💻", font=("Segoe UI", 36))  # Icono visual emoji
    # Empaquetamos el icono centrado
    etiqueta_icono_sidebar.pack(anchor="center")  # Posicionamiento centrado

    # Título institucional en dos líneas con tipografía gruesa
    etiqueta_logo_sidebar = ctk.CTkLabel(cabecera_sidebar, text="ControlLab\nITERN", font=("Segoe UI", 22, "bold"), text_color=COLOR_TEXTO)  # Nombre institucional
    # Empaquetamos el título con separación vertical
    etiqueta_logo_sidebar.pack(anchor="center", pady=(4, 6))  # Margen entre logo y badge

    # Badge de seguridad OWASP que certifica el cumplimiento de estándares
    etiqueta_badge_owasp = ctk.CTkLabel(cabecera_sidebar, text="🛡️ OWASP Top 10\n(A02 & A03 Blindados)", font=("Segoe UI", 10, "bold"), text_color=COLOR_PRIMARIO, fg_color=COLOR_FONDO, corner_radius=6, padx=8, pady=4)  # Badge de seguridad
    # Empaquetamos el badge
    etiqueta_badge_owasp.pack(anchor="center", pady=(2, 10))  # Margen inferior del badge

    # Línea divisoria sutil para separar la cabecera de los botones de navegación
    separador_sidebar = ctk.CTkFrame(panel_menu_lateral, height=1, fg_color=COLOR_BORDES)  # Divisor horizontal
    # Empaquetamos el divisor
    separador_sidebar.pack(fill="x", padx=16, pady=(0, 16))  # Separación visual

    # --- Botón de Navegación 1: Equipos ---
    boton_nav_equipos = ctk.CTkButton(  # Instanciamos el botón de Equipos
        panel_menu_lateral,  # Contenedor padre
        text="💻  Equipos",  # Texto con icono emoji
        font=("Segoe UI", 13, "bold"),  # Tipografía moderna
        anchor="w",  # Alineación del texto a la izquierda
        corner_radius=8,  # Esquinas redondeadas de 8 píxeles
        height=42,  # Altura cómoda para clics
        command=mostrar_modulo_equipos  # Acción vinculada
    )  # Fin del botón Equipos
    # Empaquetamos el botón en el menú
    boton_nav_equipos.pack(fill="x", padx=14, pady=4)  # Margen entre botones

    # --- Botón de Navegación 2: Áreas ---
    boton_nav_areas = ctk.CTkButton(  # Instanciamos el botón de Áreas
        panel_menu_lateral,  # Contenedor padre
        text="📍  Áreas",  # Texto con icono emoji
        font=("Segoe UI", 13, "bold"),  # Tipografía moderna
        anchor="w",  # Alineación a la izquierda
        corner_radius=8,  # Esquinas redondeadas
        height=42,  # Altura cómoda
        command=mostrar_modulo_areas  # Acción vinculada
    )  # Fin del botón Áreas
    # Empaquetamos el botón debajo de Equipos
    boton_nav_areas.pack(fill="x", padx=14, pady=4)  # Margen

    # --- Botón de Navegación 3: Inventario ---
    boton_nav_inventario = ctk.CTkButton(  # Instanciamos el botón de Inventario
        panel_menu_lateral,  # Contenedor padre
        text="📦  Inventario",  # Texto con icono emoji
        font=("Segoe UI", 13, "bold"),  # Tipografía moderna
        anchor="w",  # Alineación a la izquierda
        corner_radius=8,  # Esquinas redondeadas
        height=42,  # Altura cómoda
        command=mostrar_modulo_inventario  # Acción vinculada
    )  # Fin del botón Inventario
    # Empaquetamos el botón debajo de Áreas
    boton_nav_inventario.pack(fill="x", padx=14, pady=4)  # Margen

    # --- Botón de Navegación 4: Usuarios ---
    boton_nav_usuarios = ctk.CTkButton(  # Instanciamos el botón de Usuarios
        panel_menu_lateral,  # Contenedor padre
        text="👥  Usuarios",  # Texto con icono emoji
        font=("Segoe UI", 13, "bold"),  # Tipografía moderna
        anchor="w",  # Alineación a la izquierda
        corner_radius=8,  # Esquinas redondeadas
        height=42,  # Altura cómoda
        command=mostrar_modulo_usuarios  # Acción vinculada
    )  # Fin del botón Usuarios
    # Empaquetamos el botón debajo de Inventario
    boton_nav_usuarios.pack(fill="x", padx=14, pady=4)  # Margen

    # Etiqueta de pie de menú con versión del sistema y modo offline
    etiqueta_pie_sidebar = ctk.CTkLabel(panel_menu_lateral, text="v2.5 • 100% Offline\nControlLab ITERN", font=("Segoe UI", 9), text_color=COLOR_MUTED)  # Pie de página
    # Anclamos la etiqueta al fondo del sidebar
    etiqueta_pie_sidebar.pack(side="bottom", pady=16)  # Margen inferior

    # ==========================================================================
    # 6. CONTENEDOR CENTRAL DE LA APLICACIÓN
    # ==========================================================================

    # Creamos el contenedor central donde se montará dinámicamente cada módulo
    area_central = ctk.CTkFrame(ventana_dashboard, fg_color=COLOR_FONDO, corner_radius=0)  # Contenedor principal
    # Hacemos que ocupe todo el espacio disponible a la derecha del sidebar
    area_central.pack(side="right", fill="both", expand=True)  # Se expande en pantalla completa

    # ==========================================================================
    # 7. MÓDULO 1: EQUIPOS (TARJETAS KPI + BUSCADOR + FILTRO + CRUD + TREEVIEW)
    # ==========================================================================

    # Panel contenedor general para el Módulo 1 de Equipos
    panel_modulo_equipos = ctk.CTkFrame(area_central, fg_color="transparent")  # Marco transparente

    # Contenedor superior para el título y subtítulo del módulo de equipos
    cabecera_modulo_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color="transparent")  # Marco cabecera
    # Empaquetamos la cabecera
    cabecera_modulo_equipos.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Título de sección en tipografía grande Segoe UI 24 Bold en blanco suave
    etiqueta_tit_equipos = ctk.CTkLabel(cabecera_modulo_equipos, text="💻 Control de Equipos de Cómputo", font=("Segoe UI", 24, "bold"), text_color=COLOR_TEXTO)  # Título
    # Posicionamos el título a la izquierda
    etiqueta_tit_equipos.pack(anchor="w")  # Alineación izquierda

    # Subtítulo explicativo en gris plateado Segoe UI 12
    etiqueta_sub_equipos = ctk.CTkLabel(cabecera_modulo_equipos, text="Gestión física de PCs en Laboratorio y Bodega con Código Autoincrementable MySQL", font=("Segoe UI", 12), text_color=COLOR_MUTED)  # Subtítulo
    # Posicionamos el subtítulo
    etiqueta_sub_equipos.pack(anchor="w", pady=(2, 0))  # Margen superior sutil

    # --- CONTENEDOR HORIZONTAL PARA LAS 3 TARJETAS DE ESTADÍSTICAS (KPIS) ---
    panel_kpis_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color="transparent")  # Marco horizontal KPIs
    # Empaquetamos el panel de métricas
    panel_kpis_equipos.pack(fill="x", padx=4, pady=(0, 12))  # Margen entre KPIs y buscador
    # Configuramos 3 columnas proporcionales de igual ancho
    panel_kpis_equipos.grid_columnconfigure(0, weight=1)  # Columna métrica 1 (Activas)
    panel_kpis_equipos.grid_columnconfigure(1, weight=1)  # Columna métrica 2 (Mantenimiento)
    panel_kpis_equipos.grid_columnconfigure(2, weight=1)  # Columna métrica 3 (Dañadas)

    # --- Tarjeta KPI 1: PCs Activas (Acento Verde Esmeralda #10B981) ---
    tarjeta_kpi_activas = ctk.CTkFrame(panel_kpis_equipos, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1, height=80)  # Marco tarjeta 1
    # Ubicamos la tarjeta en la columna 0 de la cuadrícula
    tarjeta_kpi_activas.grid(row=0, column=0, sticky="ew", padx=(0, 8))  # Margen derecho
    # Evitamos que se reduzca su tamaño
    tarjeta_kpi_activas.pack_propagate(False)  # Mantiene altura fija de 80px

    # Barra lateral gruesa de acento verde esmeralda a la izquierda
    barra_acento_activas = ctk.CTkFrame(tarjeta_kpi_activas, width=6, fg_color=COLOR_EXITO, corner_radius=3)  # Barra de color verde
    # Empaquetamos la barra de acento a la izquierda de la tarjeta
    barra_acento_activas.pack(side="left", fill="y", padx=(0, 12))  # Se estira verticalmente

    # Contenedor vertical para el texto y número de la tarjeta
    cuerpo_kpi_activas = ctk.CTkFrame(tarjeta_kpi_activas, fg_color="transparent")  # Marco de textos
    # Empaquetamos el contenedor
    cuerpo_kpi_activas.pack(side="left", fill="both", expand=True, pady=6)  # Margen interno vertical

    # Título pequeño de la métrica en gris plateado
    etiqueta_tit_kpi_act = ctk.CTkLabel(cuerpo_kpi_activas, text="PCs Activas / Operativas", font=("Segoe UI", 11, "bold"), text_color=COLOR_MUTED)  # Título KPI 1
    # Empaquetamos el título alineado a la izquierda
    etiqueta_tit_kpi_act.pack(anchor="w")  # Alineación izquierda

    # Número gigante de la métrica con font=("Segoe UI", 32, "bold") en verde esmeralda
    etiqueta_val_kpi_activas = ctk.CTkLabel(cuerpo_kpi_activas, text="0", font=("Segoe UI", 32, "bold"), text_color=COLOR_EXITO)  # Número grande
    # Empaquetamos el valor numérico
    etiqueta_val_kpi_activas.pack(anchor="w")  # Número alineado a la izquierda

    # --- Tarjeta KPI 2: En Mantenimiento (Acento Ámbar Dorado #F59E0B) ---
    tarjeta_kpi_mantenimiento = ctk.CTkFrame(panel_kpis_equipos, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1, height=80)  # Marco tarjeta 2
    # Ubicamos la tarjeta en la columna 1 de la cuadrícula
    tarjeta_kpi_mantenimiento.grid(row=0, column=1, sticky="ew", padx=(0, 8))  # Margen derecho
    # Mantenemos su altura fija
    tarjeta_kpi_mantenimiento.pack_propagate(False)  # Altura fija de 80px

    # Barra lateral gruesa de acento ámbar
    barra_acento_mantenimiento = ctk.CTkFrame(tarjeta_kpi_mantenimiento, width=6, fg_color=COLOR_ALERTA, corner_radius=3)  # Barra ámbar
    # Empaquetamos la barra de acento a la izquierda
    barra_acento_mantenimiento.pack(side="left", fill="y", padx=(0, 12))  # Se estira verticalmente

    # Contenedor vertical de textos
    cuerpo_kpi_mant = ctk.CTkFrame(tarjeta_kpi_mantenimiento, fg_color="transparent")  # Marco de textos
    # Empaquetamos el contenedor
    cuerpo_kpi_mant.pack(side="left", fill="both", expand=True, pady=6)  # Margen vertical

    # Título pequeño de la métrica en ámbar/gris
    etiqueta_tit_kpi_mant = ctk.CTkLabel(cuerpo_kpi_mant, text="En Mantenimiento", font=("Segoe UI", 11, "bold"), text_color=COLOR_MUTED)  # Título KPI 2
    # Empaquetamos el título
    etiqueta_tit_kpi_mant.pack(anchor="w")  # Alineación izquierda

    # Número gigante de la métrica en ámbar dorado
    etiqueta_val_kpi_mantenimiento = ctk.CTkLabel(cuerpo_kpi_mant, text="0", font=("Segoe UI", 32, "bold"), text_color=COLOR_ALERTA)  # Número grande
    # Empaquetamos el valor numérico
    etiqueta_val_kpi_mantenimiento.pack(anchor="w")  # Número alineado a la izquierda

    # --- Tarjeta KPI 3: Dañadas (Acento Rojo Carmesí #EF4444) ---
    tarjeta_kpi_danadas = ctk.CTkFrame(panel_kpis_equipos, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1, height=80)  # Marco tarjeta 3
    # Ubicamos la tarjeta en la columna 2 de la cuadrícula
    tarjeta_kpi_danadas.grid(row=0, column=2, sticky="ew")  # Sin margen adicional a la derecha
    # Mantenemos su altura fija
    tarjeta_kpi_danadas.pack_propagate(False)  # Altura fija de 80px

    # Barra lateral gruesa de acento rojo carmesí
    barra_acento_danadas = ctk.CTkFrame(tarjeta_kpi_danadas, width=6, fg_color=COLOR_PELIGRO, corner_radius=3)  # Barra roja
    # Empaquetamos la barra de acento a la izquierda
    barra_acento_danadas.pack(side="left", fill="y", padx=(0, 12))  # Se estira verticalmente

    # Contenedor vertical de textos
    cuerpo_kpi_dan = ctk.CTkFrame(tarjeta_kpi_danadas, fg_color="transparent")  # Marco de textos
    # Empaquetamos el contenedor
    cuerpo_kpi_dan.pack(side="left", fill="both", expand=True, pady=6)  # Margen vertical

    # Título pequeño de la métrica en gris plateado
    etiqueta_tit_kpi_dan = ctk.CTkLabel(cuerpo_kpi_dan, text="PCs Dañadas / Inactivas", font=("Segoe UI", 11, "bold"), text_color=COLOR_MUTED)  # Título KPI 3
    # Empaquetamos el título
    etiqueta_tit_kpi_dan.pack(anchor="w")  # Alineación izquierda

    # Número gigante de la métrica en rojo carmesí
    etiqueta_val_kpi_danadas = ctk.CTkLabel(cuerpo_kpi_dan, text="0", font=("Segoe UI", 32, "bold"), text_color=COLOR_PELIGRO)  # Número grande
    # Empaquetamos el valor numérico
    etiqueta_val_kpi_danadas.pack(anchor="w")  # Número alineado a la izquierda

    # Función que calcula y actualiza dinámicamente los contadores de las 3 tarjetas estadísticas
    def actualizar_tarjetas_estadisticas():  # Calcula conteos de computadoras por estado
        # Consultamos todos los registros actuales de computadoras desde MySQL
        todos_equipos = obtener_equipos()  # Retorna lista de tuplas desde MySQL
        # Inicializamos los contadores de cada categoría en cero
        total_activas = 0  # Contador de PCs activas
        total_mantenimiento = 0  # Contador de PCs en mantenimiento
        total_danadas = 0  # Contador de PCs dañadas
        # Recorremos cada registro devuelto por la consulta SQL
        for pc in todos_equipos:  # Itera cada computadora
            # Leemos el estado (posición 5 de la tupla) en minúsculas y limpio de espacios
            estado_pc = str(pc[5]).strip().lower()  # Texto del estado
            # Verificamos si corresponde a una computadora activa u operativa
            if "activa" in estado_pc:  # Estado Activa
                # Incrementamos el contador de activas
                total_activas += 1  # Suma 1 a activas
            # Verificamos si corresponde a mantenimiento
            elif "mantenimiento" in estado_pc:  # Estado En Mantenimiento
                # Incrementamos el contador de mantenimiento
                total_mantenimiento += 1  # Suma 1 a mantenimiento
            # Verificamos si corresponde a dañada
            elif "dañad" in estado_pc or "danad" in estado_pc:  # Estado Dañada
                # Incrementamos el contador de dañadas
                total_danadas += 1  # Suma 1 a dañadas
        # Asignamos el total de activas a la tarjeta 1
        etiqueta_val_kpi_activas.configure(text=str(total_activas))  # Actualiza etiqueta Activas
        # Asignamos el total de mantenimiento a la tarjeta 2
        etiqueta_val_kpi_mantenimiento.configure(text=str(total_mantenimiento))  # Actualiza etiqueta Mantenimiento
        # Asignamos el total de dañadas a la tarjeta 3
        etiqueta_val_kpi_danadas.configure(text=str(total_danadas))  # Actualiza etiqueta Dañadas

    # --- BARRA DE BÚSQUEDA Y FILTRADO POR ÁREA EN EQUIPOS ---
    panel_busqueda_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color="transparent")  # Contenedor horizontal
    # Empaquetamos la barra de búsqueda y filtros
    panel_busqueda_equipos.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Caja de texto para búsqueda de equipos con placeholder profesional
    caja_buscar_equipos = ctk.CTkEntry(  # Instanciamos el CTkEntry
        panel_busqueda_equipos,  # Contenedor padre
        placeholder_text="🔍 Buscar por código, procesador, RAM o área...",  # Placeholder descriptivo
        fg_color=COLOR_FONDO,  # Fondo oscuro Slate-900
        text_color=COLOR_TEXTO,  # Texto blanco suave
        placeholder_text_color=COLOR_MUTED,  # Placeholder gris plateado
        border_color=COLOR_BORDES,  # Borde sutil Slate-700
        corner_radius=8,  # Esquinas redondeadas
        height=36  # Altura cómoda
    )  # Fin de CTkEntry
    # Empaquetamos la caja de búsqueda a la izquierda con expansión horizontal
    caja_buscar_equipos.pack(side="left", fill="x", expand=True, padx=(0, 8))  # Ocupa el ancho necesario

    # Función que ejecuta la búsqueda de equipos mediante sentencia segura con LIKE %s
    def accion_buscar_equipos():  # Evento al presionar Buscar Equipos
        # Obtenemos el texto ingresado en la caja de búsqueda
        termino = caja_buscar_equipos.get().strip()  # Texto a buscar
        # Validamos que el usuario no haya dejado la caja vacía
        if not termino:  # Si la caja está vacía
            # Mostramos un aviso informativo
            messagebox.showinfo("Búsqueda", "Ingresa un término para buscar computadoras.")  # Diálogo de aviso
            # Detenemos la acción
            return  # Fin de la función
        # Consultamos las computadoras coincidentes usando la sentencia preparada LIKE %s
        resultados = buscar_equipos(termino)  # Consulta segura
        # Limpiamos las filas actuales del Treeview de equipos
        for fila in tabla_equipos.get_children():  # Itera filas
            # Borramos cada fila previa
            tabla_equipos.delete(fila)  # Limpieza de fila
        # Insertamos las filas encontradas en la tabla
        for eq in resultados:  # Itera resultados
            # Agregamos la fila al Treeview
            tabla_equipos.insert("", "end", values=eq)  # Dibuja fila coincidente

    # Función para restaurar la vista completa y limpiar el término de búsqueda
    def accion_limpiar_equipos():  # Evento al presionar Limpiar Filtro
        # Vaciamos el texto de la caja de búsqueda
        caja_buscar_equipos.delete(0, "end")  # Limpia el campo
        # Restablecemos el ComboBox de filtrado de área a 'Todas las áreas'
        combo_filtro_area.set("Todas las áreas")  # Restablece opción general
        # Recargamos la tabla de equipos completa desde MySQL
        cargar_tabla_equipos()  # Recarga completa

    # Botón '🔍 Buscar' conectado a la búsqueda segura
    boton_buscar_equipos = ctk.CTkButton(panel_busqueda_equipos, text="🔍 Buscar", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PRIMARIO, text_color="#FFFFFF", hover_color=COLOR_PRIMARIO_HOVER, corner_radius=8, width=95, height=36, command=accion_buscar_equipos)  # Botón Buscar
    # Empaquetamos el botón Buscar
    boton_buscar_equipos.pack(side="left", padx=(0, 6))  # Margen derecho

    # Botón '✖ Limpiar' para restaurar la tabla
    boton_limpiar_equipos = ctk.CTkButton(panel_busqueda_equipos, text="✖ Limpiar", font=("Segoe UI", 12), fg_color=COLOR_BORDES, text_color=COLOR_TEXTO, hover_color="#475569", corner_radius=8, width=85, height=36, command=accion_limpiar_equipos)  # Botón Limpiar
    # Empaquetamos el botón Limpiar
    boton_limpiar_equipos.pack(side="left", padx=(0, 14))  # Margen derecho

    # Etiqueta que identifica el selector interactivo de área física
    etiqueta_filtro_area = ctk.CTkLabel(panel_busqueda_equipos, text="📍 Área:", font=("Segoe UI", 12, "bold"), text_color=COLOR_MUTED)  # Etiqueta filtro
    # Empaquetamos la etiqueta
    etiqueta_filtro_area.pack(side="left", padx=(0, 6))  # Alineación izquierda

    # Función interactiva ejecutada al cambiar la selección en el ComboBox de área
    def accion_filtrar_por_area(area_elegida):  # Filtra Treeview por área física
        # Si el usuario selecciona 'Laboratorio'
        if area_elegida == "Laboratorio":  # Área Laboratorio seleccionada
            # Ejecutamos SELECT seguro con WHERE id_area = 1 (Laboratorio)
            equipos_filtrados = obtener_equipos_por_area(1)  # id_area 1 en MySQL
        # Si el usuario selecciona 'Bodega'
        elif area_elegida == "Bodega":  # Área Bodega seleccionada
            # Ejecutamos SELECT seguro con WHERE id_area = 2 (Bodega)
            equipos_filtrados = obtener_equipos_por_area(2)  # id_area 2 en MySQL
        # Si selecciona 'Todas las áreas'
        else:  # Opción general
            # Consultamos todos los equipos sin filtrar por área
            equipos_filtrados = obtener_equipos()  # Consulta general de computadoras
        # Limpiamos las filas actuales del Treeview
        for fila in tabla_equipos.get_children():  # Itera filas
            # Borramos la fila
            tabla_equipos.delete(fila)  # Limpieza
        # Insertamos en el Treeview únicamente las computadoras filtradas
        for eq in equipos_filtrados:  # Itera registros devueltos
            # Agregamos cada equipo a la tabla
            tabla_equipos.insert("", "end", values=eq)  # Dibuja fila
        # Actualizamos las métricas de las tarjetas KPI
        actualizar_tarjetas_estadisticas()  # Refresca contadores

    # ComboBox interactivo de filtrado por área con solo las opciones autorizadas
    combo_filtro_area = ctk.CTkComboBox(  # Instanciamos el ComboBox
        panel_busqueda_equipos,  # Contenedor padre
        values=["Todas las áreas", "Laboratorio", "Bodega"],  # Opciones exactas
        command=accion_filtrar_por_area,  # Disparador interactivo
        fg_color=COLOR_FONDO,  # Fondo Slate-900
        text_color=COLOR_TEXTO,  # Texto blanco suave
        border_color=COLOR_BORDES,  # Borde Slate-700
        button_color=COLOR_PRIMARIO,  # Botón azul neón
        dropdown_fg_color=COLOR_PANELES,  # Desplegable Slate-800
        dropdown_text_color=COLOR_TEXTO,  # Texto desplegable
        state="readonly",  # Solo lectura para evitar escritura de datos inválidos
        corner_radius=8,  # Esquinas redondeadas
        width=150,  # Ancho en píxeles
        height=36  # Altura cómoda
    )  # Fin de ComboBox
    # Asignamos el valor por defecto como 'Todas las áreas'
    combo_filtro_area.set("Todas las áreas")  # Valor inicial
    # Empaquetamos el ComboBox de filtrado
    combo_filtro_area.pack(side="left")  # Alineado a la izquierda

    # --- FORMULARIO DE REGISTRO Y EDICIÓN DE EQUIPOS (LABELS ARRIBA DE CADA CAMPO) ---
    # Variable mutable para rastrear el codigo_pc en edición (None para nuevo registro)
    codigo_pc_edicion = [None]  # Lista con un elemento para almacenar el ID en edición

    # Contenedor tipo tarjeta para el formulario de computadoras con borde Slate-700
    form_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Tarjeta formulario
    # Empaquetamos el contenedor del formulario
    form_equipos.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior
    # Configuramos 3 columnas proporcionales de igual ancho
    form_equipos.grid_columnconfigure(0, weight=1)  # Columna 1 del formulario
    form_equipos.grid_columnconfigure(1, weight=1)  # Columna 2 del formulario
    form_equipos.grid_columnconfigure(2, weight=1)  # Columna 3 del formulario

    # Fila 0, Columna 0: Etiqueta de Área Física ubicada ARRIBA del selector
    etiqueta_area_eq = ctk.CTkLabel(form_equipos, text="📍 Área Física:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Área
    # Ubicamos la etiqueta
    etiqueta_area_eq.grid(row=0, column=0, sticky="w", padx=14, pady=(10, 2))  # Margen superior
    # Fila 1, Columna 0: ComboBox de Área Física (solo Laboratorio y Bodega)
    combo_area_equipo = ctk.CTkComboBox(form_equipos, values=["Laboratorio", "Bodega"], fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BORDES, button_color=COLOR_PRIMARIO, dropdown_fg_color=COLOR_PANELES, dropdown_text_color=COLOR_TEXTO, state="readonly", corner_radius=8, height=34)  # ComboBox Área
    # Asignamos la opción por defecto: Laboratorio
    combo_area_equipo.set("Laboratorio")  # Opción inicial
    # Ubicamos el ComboBox en la cuadrícula
    combo_area_equipo.grid(row=1, column=0, sticky="ew", padx=14, pady=(0, 8))  # Expande horizontalmente

    # Fila 0, Columna 1: Etiqueta de Procesador ubicada ARRIBA de la caja
    etiqueta_proc_eq = ctk.CTkLabel(form_equipos, text="💻 Procesador:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Procesador
    # Ubicamos la etiqueta
    etiqueta_proc_eq.grid(row=0, column=1, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 1: Entry de Procesador con placeholder descriptivo
    caja_procesador_equipo = ctk.CTkEntry(form_equipos, placeholder_text="Ej: Intel Core i5-10400", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Procesador
    # Ubicamos el Entry en la cuadrícula
    caja_procesador_equipo.grid(row=1, column=1, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 0, Columna 2: Etiqueta de RAM ubicada ARRIBA de la caja
    etiqueta_ram_eq = ctk.CTkLabel(form_equipos, text="⚡ Memoria RAM:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label RAM
    # Ubicamos la etiqueta
    etiqueta_ram_eq.grid(row=0, column=2, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 2: Entry de Memoria RAM con placeholder descriptivo
    caja_ram_equipo = ctk.CTkEntry(form_equipos, placeholder_text="Ej: 16 GB DDR4 3200MHz", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry RAM
    # Ubicamos el Entry en la cuadrícula
    caja_ram_equipo.grid(row=1, column=2, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 2, Columna 0: Etiqueta de Almacenamiento ubicada ARRIBA de la caja
    etiqueta_alm_eq = ctk.CTkLabel(form_equipos, text="💾 Almacenamiento:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Almacenamiento
    # Ubicamos la etiqueta
    etiqueta_alm_eq.grid(row=2, column=0, sticky="w", padx=14, pady=(0, 2))  # Margen
    # Fila 3, Columna 0: Entry de Almacenamiento con placeholder descriptivo
    caja_almacenamiento_equipo = ctk.CTkEntry(form_equipos, placeholder_text="Ej: 512 GB SSD NVMe", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Almacenamiento
    # Ubicamos el Entry en la cuadrícula
    caja_almacenamiento_equipo.grid(row=3, column=0, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 2, Columna 1: Etiqueta de Estado ubicada ARRIBA del ComboBox
    etiqueta_est_eq = ctk.CTkLabel(form_equipos, text="📊 Estado Operativo:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Estado
    # Ubicamos la etiqueta
    etiqueta_est_eq.grid(row=2, column=1, sticky="w", padx=14, pady=(0, 2))  # Margen
    # Fila 3, Columna 1: ComboBox de Estado del Equipo
    combo_estado_equipo = ctk.CTkComboBox(form_equipos, values=["Activa", "En Mantenimiento", "Dañada"], fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BORDES, button_color=COLOR_PRIMARIO, dropdown_fg_color=COLOR_PANELES, dropdown_text_color=COLOR_TEXTO, state="readonly", corner_radius=8, height=34)  # ComboBox Estado
    # Asignamos la opción por defecto: Activa
    combo_estado_equipo.set("Activa")  # Opción inicial
    # Ubicamos el ComboBox en la cuadrícula
    combo_estado_equipo.grid(row=3, column=1, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 2, Columna 2: Etiqueta de Código PC (Auto) ubicada ARRIBA
    etiqueta_cod_info = ctk.CTkLabel(form_equipos, text="⚡ Código PC:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Código PC
    # Ubicamos la etiqueta
    etiqueta_cod_info.grid(row=2, column=2, sticky="w", padx=14, pady=(0, 2))  # Margen
    # Fila 3, Columna 2: Badge visual que informa el código autoincrementable de MySQL
    etiqueta_auto_pc = ctk.CTkLabel(form_equipos, text="Autoincrementable (MySQL)", font=("Segoe UI", 11, "italic"), text_color=COLOR_PRIMARIO, fg_color=COLOR_FONDO, corner_radius=8, height=34)  # Badge Auto
    # Ubicamos el Badge en la cuadrícula
    etiqueta_auto_pc.grid(row=3, column=2, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Función que guarda un nuevo equipo o actualiza uno existente según codigo_pc_edicion
    def accion_guardar_o_actualizar_equipo():  # Procesa guardado/actualización
        # Obtenemos los valores de los campos del formulario
        area = combo_area_equipo.get()  # Área seleccionada (Laboratorio o Bodega)
        procesador = caja_procesador_equipo.get().strip()  # Procesador ingresado
        ram = caja_ram_equipo.get().strip()  # Memoria RAM ingresada
        almacenamiento = caja_almacenamiento_equipo.get().strip()  # Almacenamiento ingresado
        estado = combo_estado_equipo.get()  # Estado seleccionado
        # Validamos que los campos obligatorios no estén vacíos
        if not procesador or not ram or not almacenamiento:  # Validación
            # Notificamos al usuario
            messagebox.showwarning("Campos Incompletos", "Por favor completa todos los campos del equipo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # MODO REGISTRO: Si no estamos editando (codigo_pc_edicion es None)
        if codigo_pc_edicion[0] is None:  # Inserción de nueva PC
            # Llamamos a registrar_pc() con sentencias preparadas (%s) sin codigo manual
            if registrar_pc(area, procesador, ram, almacenamiento, estado):  # Ejecución en MySQL
                # Mostramos notificación de éxito
                messagebox.showinfo("Éxito", f"La computadora fue registrada correctamente en '{area}' con código autogenerado por MySQL.")  # Diálogo éxito
                # Limpiamos el formulario
                limpiar_formulario_equipos()  # Resetea campos
                # Recargamos la tabla de equipos y las métricas
                cargar_tabla_equipos()  # Actualiza la interfaz
        # MODO ACTUALIZACIÓN: Si estamos editando un equipo existente
        else:  # Actualización por codigo_pc
            # Obtenemos el codigo_pc en edición
            codigo_a_actualizar = codigo_pc_edicion[0]  # Código PC numérico
            # Ejecutamos UPDATE ... WHERE codigo_pc = %s
            if actualizar_pc(codigo_a_actualizar, area, procesador, ram, almacenamiento, estado):  # Actualización en MySQL
                # Mostramos notificación de éxito
                messagebox.showinfo("Éxito", f"La computadora #{codigo_a_actualizar} fue actualizada exitosamente.")  # Diálogo éxito
                # Restablecemos el formulario
                limpiar_formulario_equipos()  # Resetea campos
                # Recargamos la tabla y las métricas
                cargar_tabla_equipos()  # Actualiza interfaz

    # Función auxiliar para vaciar el formulario de equipos y regresar a modo nuevo registro
    def limpiar_formulario_equipos():  # Restablece formulario
        # Vaciamos la variable de edición
        codigo_pc_edicion[0] = None  # Regresa a modo nuevo registro
        # Limpiamos las cajas de texto
        caja_procesador_equipo.delete(0, "end")  # Limpia procesador
        caja_ram_equipo.delete(0, "end")  # Limpia RAM
        caja_almacenamiento_equipo.delete(0, "end")  # Limpia almacenamiento
        # Restablecemos los ComboBox a sus valores por defecto
        combo_area_equipo.set("Laboratorio")  # Área por defecto
        combo_estado_equipo.set("Activa")  # Estado por defecto
        # Restauramos el texto y color del botón a Verde Esmeralda #10B981
        boton_guardar_equipo.configure(text="💾 Registrar Equipo", fg_color=COLOR_EXITO, hover_color=COLOR_EXITO_HOVER)  # Botón estándar

    # Fila 4: Botón dinámico '💾 Registrar Equipo' (Verde Esmeralda #10B981)
    boton_guardar_equipo = ctk.CTkButton(form_equipos, text="💾 Registrar Equipo", font=("Segoe UI", 13, "bold"), fg_color=COLOR_EXITO, text_color="#FFFFFF", hover_color=COLOR_EXITO_HOVER, corner_radius=8, height=36, command=accion_guardar_o_actualizar_equipo)  # Botón principal
    # Ubicamos el botón ocupando las 3 columnas del formulario
    boton_guardar_equipo.grid(row=4, column=0, columnspan=3, sticky="ew", padx=14, pady=(4, 12))  # Expande horizontalmente

    # --- CONTENEDOR TARJETA PARA EL TREEVIEW DE EQUIPOS ---
    contenedor_tabla_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Marco tabla
    # Empaquetamos el contenedor ocupando el espacio vertical disponible
    contenedor_tabla_equipos.pack(fill="both", expand=True, padx=4, pady=(0, 10))  # Expande

    # Definimos las 6 columnas del Treeview de Equipos (primera columna: codigo_pc autogenerado)
    columnas_equipos = ("codigo_pc", "area", "procesador", "ram", "almacenamiento", "estado")  # Tupla de columnas
    # Instanciamos el componente Treeview
    tabla_equipos = ttk.Treeview(contenedor_tabla_equipos, columns=columnas_equipos, show="headings")  # Treeview Equipos

    # Encabezados de las 6 columnas
    tabla_equipos.heading("codigo_pc", text="Código PC (Auto)")  # Columna 1
    tabla_equipos.heading("area", text="Área Física")  # Columna 2
    tabla_equipos.heading("procesador", text="Procesador")  # Columna 3
    tabla_equipos.heading("ram", text="RAM")  # Columna 4
    tabla_equipos.heading("almacenamiento", text="Almacenamiento")  # Columna 5
    tabla_equipos.heading("estado", text="Estado")  # Columna 6

    # Dimensiones y alineación de las columnas
    tabla_equipos.column("codigo_pc", width=110, anchor="center")  # Código PC centrado
    tabla_equipos.column("area", width=120, anchor="center")  # Área centrada
    tabla_equipos.column("procesador", width=180, anchor="w")  # Procesador a la izquierda
    tabla_equipos.column("ram", width=110, anchor="center")  # RAM centrada
    tabla_equipos.column("almacenamiento", width=130, anchor="center")  # Almacenamiento centrado
    tabla_equipos.column("estado", width=130, anchor="center")  # Estado centrado

    # Barra de desplazamiento vertical para el Treeview de equipos
    scroll_equipos = ctk.CTkScrollbar(contenedor_tabla_equipos, orientation="vertical", command=tabla_equipos.yview, fg_color=COLOR_PANELES, button_color=COLOR_PRIMARIO)  # Scrollbar
    # Conectamos la tabla con la barra de desplazamiento
    tabla_equipos.configure(yscrollcommand=scroll_equipos.set)  # Sincronización
    # Empaquetamos la barra de desplazamiento a la derecha
    scroll_equipos.pack(side="right", fill="y", padx=(0, 6), pady=6)  # Se estira verticalmente
    # Empaquetamos la tabla a la izquierda expandiéndose completamente
    tabla_equipos.pack(side="left", fill="both", expand=True, padx=(6, 0), pady=6)  # Se expande

    # Función que recarga los equipos en el Treeview según el filtro de área seleccionado
    def cargar_tabla_equipos():  # Actualiza la tabla de equipos
        # Leemos el filtro actual del ComboBox de área
        area_actual = combo_filtro_area.get()  # 'Todas las áreas', 'Laboratorio' o 'Bodega'
        # Ejecutamos la función de filtrado con el valor actual
        accion_filtrar_por_area(area_actual)  # Recarga la tabla de datos

    # --- BARRA INFERIOR DE BOTONES DE ACCIÓN (EDITAR, ELIMINAR, CANCELAR Y EXPORTAR PDF) ---
    panel_acciones_equipos = ctk.CTkFrame(panel_modulo_equipos, fg_color="transparent")  # Marco inferior de acciones
    # Empaquetamos el panel de acciones
    panel_acciones_equipos.pack(fill="x", padx=4, pady=(0, 6))  # Margen inferior

    # Función que carga el equipo seleccionado en el formulario para editarlo
    def accion_editar_equipo():  # Evento del botón Editar
        # Obtenemos la fila seleccionada en el Treeview
        seleccion = tabla_equipos.selection()  # Fila seleccionada
        # Validamos que el usuario haya seleccionado una fila
        if not seleccion:  # Si no hay selección
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un equipo de la tabla para editarlo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los valores de la fila seleccionada (6 columnas)
        valores_fila = tabla_equipos.item(seleccion[0], "values")  # Tupla de 6 valores
        # Guardamos el codigo_pc de la fila en la variable de edición
        codigo_pc_edicion[0] = valores_fila[0]  # Guarda el codigo_pc numérico
        # Leemos el área física asegurando que sea Laboratorio o Bodega
        area_leida = valores_fila[1]  # Área de la fila
        # Asignamos el área al ComboBox
        combo_area_equipo.set(area_leida if area_leida in ["Laboratorio", "Bodega"] else "Laboratorio")  # Carga área
        # Limpiamos las cajas de texto antes de llenarlas
        caja_procesador_equipo.delete(0, "end")  # Limpia procesador
        # Cargamos el procesador
        caja_procesador_equipo.insert(0, valores_fila[2])  # Carga procesador
        # Limpiamos la caja de RAM
        caja_ram_equipo.delete(0, "end")  # Limpia RAM
        # Cargamos la RAM
        caja_ram_equipo.insert(0, valores_fila[3])  # Carga RAM
        # Limpiamos la caja de almacenamiento
        caja_almacenamiento_equipo.delete(0, "end")  # Limpia almacenamiento
        # Cargamos el almacenamiento
        caja_almacenamiento_equipo.insert(0, valores_fila[4])  # Carga almacenamiento
        # Asignamos el estado en el ComboBox
        combo_estado_equipo.set(valores_fila[5])  # Carga estado
        # Cambiamos el botón principal a 'Actualizar Equipo' en color Azul #3B82F6
        boton_guardar_equipo.configure(text=f"💾 Actualizar Equipo (Código PC: #{valores_fila[0]})", fg_color=COLOR_EDITAR, hover_color=COLOR_EDITAR_HOVER)  # Modo edición

    # Función que elimina permanentemente el equipo seleccionado previa confirmación
    def accion_eliminar_equipo():  # Evento del botón Eliminar
        # Obtenemos la fila seleccionada en el Treeview
        seleccion = tabla_equipos.selection()  # Fila seleccionada
        # Validamos que exista una fila seleccionada
        if not seleccion:  # Si no hay selección
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un equipo de la tabla para eliminarlo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los datos de la fila
        valores_fila = tabla_equipos.item(seleccion[0], "values")  # Valores
        # Obtenemos el codigo_pc del equipo
        codigo_a_eliminar = valores_fila[0]  # Código PC numérico
        # Solicitamos confirmación explícita al usuario antes de borrar
        confirmado = messagebox.askyesno("Confirmar Eliminación", f"¿Estás seguro de que deseas eliminar permanentemente la computadora con Código PC #{codigo_a_eliminar}?")  # Confirmación
        # Si el usuario confirma
        if confirmado:  # Usuario dijo SÍ
            # Ejecutamos DELETE FROM computadoras WHERE codigo_pc = %s
            if eliminar_pc(codigo_a_eliminar):  # Borrado en MySQL
                # Mostramos notificación de éxito
                messagebox.showinfo("Éxito", f"La computadora #{codigo_a_eliminar} fue eliminada correctamente.")  # Mensaje
                # Si estábamos editando la computadora eliminada, reseteamos el formulario
                if codigo_pc_edicion[0] == codigo_a_eliminar:  # Si coincide
                    # Limpiamos el formulario
                    limpiar_formulario_equipos()  # Resetea a modo nuevo registro
                # Recargamos la tabla y las métricas
                cargar_tabla_equipos()  # Actualiza la vista

    # Función que genera y abre el reporte PDF con los equipos visibles actualmente
    def accion_exportar_pdf():  # Evento al presionar Exportar a PDF
        # Obtenemos las filas visibles en el Treeview
        filas_visibles = [tabla_equipos.item(item, "values") for item in tabla_equipos.get_children()]  # Filas visibles
        # Validamos que existan datos para exportar
        if not filas_visibles:  # Si la lista está vacía
            # Mostramos aviso
            messagebox.showwarning("Sin Datos", "No hay equipos en la tabla para exportar al reporte PDF.")  # Aviso
            # Salimos de la función
            return  # Detiene la exportación
        # Leemos el filtro actualmente seleccionado
        filtro_actual = combo_filtro_area.get()  # 'Todas las áreas', 'Laboratorio' o 'Bodega'
        # Llamamos a exportar_inventario_pdf() con las filas y el filtro aplicado
        exportar_inventario_pdf(filas_visibles, filtro_actual)  # Genera el PDF profesional

    # Botón '✏️ Editar Seleccionada' (Azul Profesional #3B82F6)
    boton_editar_equipo = ctk.CTkButton(panel_acciones_equipos, text="✏️ Editar Seleccionada", font=("Segoe UI", 12, "bold"), fg_color=COLOR_EDITAR, text_color="#FFFFFF", hover_color=COLOR_EDITAR_HOVER, corner_radius=8, height=36, command=accion_editar_equipo)  # Botón Editar
    # Empaquetamos el botón Editar a la izquierda
    boton_editar_equipo.pack(side="left", padx=(0, 8))  # Margen derecho

    # Botón '🗑️ Eliminar Seleccionada' (Rojo Carmesí #EF4444)
    boton_eliminar_equipo = ctk.CTkButton(panel_acciones_equipos, text="🗑️ Eliminar Seleccionada", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PELIGRO, text_color="#FFFFFF", hover_color=COLOR_PELIGRO_HOVER, corner_radius=8, height=36, command=accion_eliminar_equipo)  # Botón Eliminar
    # Empaquetamos el botón Eliminar al lado de Editar
    boton_eliminar_equipo.pack(side="left", padx=(0, 8))  # Margen derecho

    # Botón '✖ Cancelar Edición'
    boton_cancelar_eq = ctk.CTkButton(panel_acciones_equipos, text="✖ Cancelar Edición", font=("Segoe UI", 12), fg_color=COLOR_BORDES, text_color=COLOR_TEXTO, hover_color="#475569", corner_radius=8, height=36, command=limpiar_formulario_equipos)  # Botón Cancelar
    # Empaquetamos el botón Cancelar Edición
    boton_cancelar_eq.pack(side="left")  # Al lado de Eliminar

    # Botón '📄 Exportar a PDF' (Verde Esmeralda #10B981) ubicado a la derecha
    boton_exportar_pdf_acciones = ctk.CTkButton(panel_acciones_equipos, text="📄 Exportar a PDF", font=("Segoe UI", 12, "bold"), fg_color=COLOR_EXITO, text_color="#FFFFFF", hover_color=COLOR_EXITO_HOVER, corner_radius=8, height=36, command=accion_exportar_pdf)  # Botón PDF
    # Empaquetamos el botón de exportación a la derecha
    boton_exportar_pdf_acciones.pack(side="right")  # Alineación a la derecha

    # ==========================================================================
    # 8. MÓDULO 2: ÁREAS (TARJETAS DESCRIPTIVAS + TREEVIEW OFICIAL)
    # ==========================================================================

    # Panel contenedor general para el Módulo 2 de Áreas Físicas
    panel_modulo_areas = ctk.CTkFrame(area_central, fg_color="transparent")  # Marco transparente

    # Contenedor superior para el título y subtítulo del módulo de áreas
    cabecera_modulo_areas = ctk.CTkFrame(panel_modulo_areas, fg_color="transparent")  # Marco cabecera
    # Empaquetamos la cabecera
    cabecera_modulo_areas.pack(fill="x", padx=4, pady=(0, 14))  # Margen inferior

    # Título de sección en tipografía grande Segoe UI 24 Bold
    etiqueta_tit_areas = ctk.CTkLabel(cabecera_modulo_areas, text="📍 Áreas Físicas Oficiales del Instituto", font=("Segoe UI", 24, "bold"), text_color=COLOR_TEXTO)  # Título
    # Posicionamos el título
    etiqueta_tit_areas.pack(anchor="w")  # Alineación izquierda

    # Subtítulo explicativo destacando que solo operan Laboratorio y Bodega
    etiqueta_sub_areas = ctk.CTkLabel(cabecera_modulo_areas, text="El sistema opera exclusivamente con 2 áreas físicas autorizadas para la gestión de equipos y resguardo.", font=("Segoe UI", 12), text_color=COLOR_MUTED)  # Subtítulo
    # Posicionamos el subtítulo
    etiqueta_sub_areas.pack(anchor="w", pady=(2, 0))  # Margen

    # --- TARJETAS DESCRIPTIVAS VISUALES PARA LAS 2 ÁREAS OFICIALES ---
    panel_tarjetas_areas = ctk.CTkFrame(panel_modulo_areas, fg_color="transparent")  # Marco horizontal de tarjetas de área
    # Empaquetamos el panel de tarjetas
    panel_tarjetas_areas.pack(fill="x", padx=4, pady=(0, 14))  # Margen inferior
    # Configuramos 2 columnas iguales
    panel_tarjetas_areas.grid_columnconfigure(0, weight=1)  # Columna Área 1 (Laboratorio)
    panel_tarjetas_areas.grid_columnconfigure(1, weight=1)  # Columna Área 2 (Bodega)

    # Tarjeta Informativa 1: Laboratorio
    card_area_lab = ctk.CTkFrame(panel_tarjetas_areas, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1, height=95)  # Tarjeta Laboratorio
    # Ubicamos la tarjeta en la columna 0
    card_area_lab.grid(row=0, column=0, sticky="ew", padx=(0, 8))  # Margen derecho
    # Mantenemos su altura fija
    card_area_lab.pack_propagate(False)  # Altura fija

    # Barra lateral de acento azul neón
    barra_lab = ctk.CTkFrame(card_area_lab, width=6, fg_color=COLOR_PRIMARIO, corner_radius=3)  # Barra azul
    # Empaquetamos la barra a la izquierda
    barra_lab.pack(side="left", fill="y", padx=(0, 12))  # Se estira verticalmente

    # Contenedor de texto de Laboratorio
    cuerpo_card_lab = ctk.CTkFrame(card_area_lab, fg_color="transparent")  # Marco de textos
    # Empaquetamos el contenedor
    cuerpo_card_lab.pack(side="left", fill="both", expand=True, pady=10)  # Margen vertical

    # Título de la tarjeta Laboratorio
    etiqueta_tit_card_lab = ctk.CTkLabel(cuerpo_card_lab, text="💻 Área 1: Laboratorio de Cómputo", font=("Segoe UI", 13, "bold"), text_color=COLOR_TEXTO)  # Título
    # Empaquetamos el título
    etiqueta_tit_card_lab.pack(anchor="w")  # Alineación izquierda

    # Descripción de uso operativo del Laboratorio
    etiqueta_desc_card_lab = ctk.CTkLabel(cuerpo_card_lab, text="Espacio activo destinado a computadoras en uso directo por docentes y alumnos.", font=("Segoe UI", 11), text_color=COLOR_MUTED)  # Descripción
    # Empaquetamos la descripción
    etiqueta_desc_card_lab.pack(anchor="w", pady=(2, 0))  # Margen superior

    # Tarjeta Informativa 2: Bodega
    card_area_bod = ctk.CTkFrame(panel_tarjetas_areas, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1, height=95)  # Tarjeta Bodega
    # Ubicamos la tarjeta en la columna 1
    card_area_bod.grid(row=0, column=1, sticky="ew")  # Sin margen adicional derecho
    # Mantenemos su altura fija
    card_area_bod.pack_propagate(False)  # Altura fija

    # Barra lateral de acento ámbar dorado
    barra_bod = ctk.CTkFrame(card_area_bod, width=6, fg_color=COLOR_ALERTA, corner_radius=3)  # Barra ámbar
    # Empaquetamos la barra a la izquierda
    barra_bod.pack(side="left", fill="y", padx=(0, 12))  # Se estira verticalmente

    # Contenedor de texto de Bodega
    cuerpo_card_bod = ctk.CTkFrame(card_area_bod, fg_color="transparent")  # Marco de textos
    # Empaquetamos el contenedor
    cuerpo_card_bod.pack(side="left", fill="both", expand=True, pady=10)  # Margen vertical

    # Título de la tarjeta Bodega
    etiqueta_tit_card_bod = ctk.CTkLabel(cuerpo_card_bod, text="📦 Área 2: Bodega de Resguardo", font=("Segoe UI", 13, "bold"), text_color=COLOR_TEXTO)  # Título
    # Empaquetamos el título
    etiqueta_tit_card_bod.pack(anchor="w")  # Alineación izquierda

    # Descripción de uso operativo de la Bodega
    etiqueta_desc_card_bod = ctk.CTkLabel(cuerpo_card_bod, text="Área de resguardo seguro para repuestos, herramientas y equipos en mantenimiento.", font=("Segoe UI", 11), text_color=COLOR_MUTED)  # Descripción
    # Empaquetamos la descripción
    etiqueta_desc_card_bod.pack(anchor="w", pady=(2, 0))  # Margen superior

    # --- CONTENEDOR TARJETA PARA EL TREEVIEW DE ÁREAS ---
    contenedor_tabla_areas = ctk.CTkFrame(panel_modulo_areas, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Marco tabla Áreas
    # Empaquetamos el contenedor
    contenedor_tabla_areas.pack(fill="both", expand=True, padx=4, pady=(0, 10))  # Expande

    # Columnas del Treeview de Áreas
    columnas_areas = ("id", "nombre_area", "descripcion")  # Columnas
    # Creamos el Treeview de Áreas
    tabla_areas = ttk.Treeview(contenedor_tabla_areas, columns=columnas_areas, show="headings")  # Treeview Áreas

    # Encabezados de Áreas
    tabla_areas.heading("id", text="ID")  # Título ID
    tabla_areas.heading("nombre_area", text="Área Física Oficial")  # Título Nombre
    tabla_areas.heading("descripcion", text="Propósito y Uso Operativo")  # Título Descripción

    # Ancho de columnas de Áreas
    tabla_areas.column("id", width=70, anchor="center")  # ID centrado
    tabla_areas.column("nombre_area", width=180, anchor="center")  # Nombre centrado
    tabla_areas.column("descripcion", width=500, anchor="w")  # Descripción a la izquierda

    # Barra de desplazamiento vertical para la tabla de áreas
    scroll_areas = ctk.CTkScrollbar(contenedor_tabla_areas, orientation="vertical", command=tabla_areas.yview, fg_color=COLOR_PANELES, button_color=COLOR_PRIMARIO)  # Scrollbar
    # Conectamos la tabla con la barra
    tabla_areas.configure(yscrollcommand=scroll_areas.set)  # Sincronización
    # Empaquetamos la barra y la tabla
    scroll_areas.pack(side="right", fill="y", padx=(0, 6), pady=6)  # Barra a la derecha
    tabla_areas.pack(side="left", fill="both", expand=True, padx=(6, 0), pady=6)  # Tabla a la izquierda

    # Función que carga las 2 áreas físicas en el Treeview
    def cargar_tabla_areas():  # Llena tabla_areas
        # Borramos las filas actuales
        for fila in tabla_areas.get_children():  # Itera filas
            # Borra fila
            tabla_areas.delete(fila)  # Limpieza
        # Consultamos las áreas desde MySQL
        lista_areas = obtener_areas()  # Lee áreas en MySQL
        # Si la base de datos no devuelve registros, usamos los datos de respaldo oficiales
        if not lista_areas:  # Lista vacía
            # Datos oficiales de respaldo
            lista_areas = [  # Tuplas oficiales
                (1, "Laboratorio", "Área principal para computadoras y prácticas de estudiantes en uso activo"),  # Laboratorio
                (2, "Bodega", "Área de resguardo para repuestos, herramientas y equipos guardados o en espera")  # Bodega
            ]  # Fin respaldo
        # Insertamos las 2 áreas oficiales en la tabla
        for area in lista_areas:  # Itera áreas
            # Agrega fila al Treeview
            tabla_areas.insert("", "end", values=area)  # Dibuja fila

    # ==========================================================================
    # 9. MÓDULO 3: INVENTARIO (BUSCADOR + FORMULARIO + TREEVIEW + ACCIONES)
    # ==========================================================================

    # Panel contenedor general para el Módulo 3 de Inventario
    panel_modulo_inventario = ctk.CTkFrame(area_central, fg_color="transparent")  # Marco transparente

    # Contenedor superior para el título y subtítulo de inventario
    cabecera_modulo_inv = ctk.CTkFrame(panel_modulo_inventario, fg_color="transparent")  # Marco cabecera
    # Empaquetamos la cabecera
    cabecera_modulo_inv.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Título de sección en tipografía grande Segoe UI 24 Bold
    etiqueta_tit_inv = ctk.CTkLabel(cabecera_modulo_inv, text="📦 Inventario General de Herramientas y Periféricos", font=("Segoe UI", 24, "bold"), text_color=COLOR_TEXTO)  # Título
    # Posicionamos el título
    etiqueta_tit_inv.pack(anchor="w")  # Alineación izquierda

    # Subtítulo explicativo
    etiqueta_sub_inv = ctk.CTkLabel(cabecera_modulo_inv, text="Control físico de pinzas ponchadoras, cables de red, teclados, testers y periféricos", font=("Segoe UI", 12), text_color=COLOR_MUTED)  # Subtítulo
    # Posicionamos el subtítulo
    etiqueta_sub_inv.pack(anchor="w", pady=(2, 0))  # Margen

    # --- BARRA DE BÚSQUEDA DE INVENTARIO ---
    panel_busqueda_inv = ctk.CTkFrame(panel_modulo_inventario, fg_color="transparent")  # Marco del buscador
    # Empaquetamos la barra de búsqueda
    panel_busqueda_inv.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Caja de texto para buscar artículos con placeholder descriptivo
    caja_buscar_inv = ctk.CTkEntry(panel_busqueda_inv, placeholder_text="🔍 Buscar artículo por nombre, categoría o estado...", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=36)  # Entry buscador
    # Empaquetamos la caja de texto
    caja_buscar_inv.pack(side="left", fill="x", expand=True, padx=(0, 8))  # Ocupa el ancho disponible

    # Función que ejecuta la búsqueda de artículos con SELECT ... LIKE %s
    def accion_buscar_inventario():  # Evento del botón Buscar en inventario
        # Obtenemos el término escrito
        termino_inv = caja_buscar_inv.get().strip()  # Texto buscado
        # Validamos que no esté vacío
        if not termino_inv:  # Caja vacía
            # Mostramos advertencia
            messagebox.showinfo("Búsqueda", "Ingresa un término para buscar en inventario.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Consultamos los artículos usando la sentencia preparada LIKE %s
        resultados_inv = buscar_articulos_inventario(termino_inv)  # Búsqueda segura
        # Limpiamos las filas actuales del Treeview
        for fila in tabla_inventario.get_children():  # Itera filas
            # Borra cada fila
            tabla_inventario.delete(fila)  # Limpieza
        # Insertamos los artículos coincidentes en la tabla
        for item in resultados_inv:  # Itera resultados
            # Dibuja fila
            tabla_inventario.insert("", "end", values=item)  # Inserta fila coincidente

    # Función para limpiar el filtro del inventario y mostrar todos los registros
    def accion_limpiar_inventario():  # Evento del botón Limpiar Filtro
        # Limpiamos la caja de búsqueda
        caja_buscar_inv.delete(0, "end")  # Limpia caja
        # Recargamos todos los artículos de inventario desde MySQL
        cargar_tabla_inventario()  # Muestra todos los artículos

    # Botón '🔍 Buscar' de inventario
    boton_buscar_inv = ctk.CTkButton(panel_busqueda_inv, text="🔍 Buscar", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PRIMARIO, text_color="#FFFFFF", hover_color=COLOR_PRIMARIO_HOVER, corner_radius=8, width=95, height=36, command=accion_buscar_inventario)  # Botón Buscar
    # Empaquetamos el botón Buscar
    boton_buscar_inv.pack(side="left", padx=(0, 6))  # Margen derecho

    # Botón '✖ Limpiar' de inventario
    boton_limpiar_inv = ctk.CTkButton(panel_busqueda_inv, text="✖ Limpiar", font=("Segoe UI", 12), fg_color=COLOR_BORDES, text_color=COLOR_TEXTO, hover_color="#475569", corner_radius=8, width=85, height=36, command=accion_limpiar_inventario)  # Botón Limpiar
    # Empaquetamos el botón Limpiar
    boton_limpiar_inv.pack(side="left")  # Al lado del botón Buscar

    # --- FORMULARIO DE INVENTARIO (LABELS ARRIBA DE CADA CAMPO) ---
    # Variable mutable para rastrear el ID del artículo en edición
    id_item_edicion = [None]  # Rastreador del ID en edición

    # Contenedor tipo tarjeta para el formulario de inventario con borde Slate-700
    form_inventario = ctk.CTkFrame(panel_modulo_inventario, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Tarjeta formulario
    # Empaquetamos el formulario
    form_inventario.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior
    # Configuramos 2 columnas de igual ancho
    form_inventario.grid_columnconfigure(0, weight=1)  # Columna 1
    form_inventario.grid_columnconfigure(1, weight=1)  # Columna 2

    # Fila 0, Columna 0: Etiqueta de Nombre ubicada ARRIBA del campo
    etiqueta_nom_inv = ctk.CTkLabel(form_inventario, text="📦 Nombre del Artículo:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Nombre
    # Ubicamos la etiqueta
    etiqueta_nom_inv.grid(row=0, column=0, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 0: Entry de Nombre con placeholder descriptivo
    caja_nombre_inventario = ctk.CTkEntry(form_inventario, placeholder_text="Ej: Pinza Ponchadora RJ45 / Teclado USB", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Nombre
    # Ubicamos el Entry en la cuadrícula
    caja_nombre_inventario.grid(row=1, column=0, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 0, Columna 1: Etiqueta de Categoría ubicada ARRIBA del ComboBox
    etiqueta_cat_inv = ctk.CTkLabel(form_inventario, text="🏷️ Categoría:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Categoría
    # Ubicamos la etiqueta
    etiqueta_cat_inv.grid(row=0, column=1, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 1: ComboBox de Categoría
    combo_categoria_inventario = ctk.CTkComboBox(form_inventario, values=["Herramienta", "Periférico"], fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BORDES, button_color=COLOR_PRIMARIO, dropdown_fg_color=COLOR_PANELES, dropdown_text_color=COLOR_TEXTO, state="readonly", corner_radius=8, height=34)  # ComboBox Categoría
    # Asignamos la opción por defecto: Herramienta
    combo_categoria_inventario.set("Herramienta")  # Opción inicial
    # Ubicamos el ComboBox en la cuadrícula
    combo_categoria_inventario.grid(row=1, column=1, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 2, Columna 0: Etiqueta de Cantidad ubicada ARRIBA del campo
    etiqueta_cant_inv = ctk.CTkLabel(form_inventario, text="🔢 Cantidad:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Cantidad
    # Ubicamos la etiqueta
    etiqueta_cant_inv.grid(row=2, column=0, sticky="w", padx=14, pady=(0, 2))  # Margen
    # Fila 3, Columna 0: Entry de Cantidad con placeholder numérico
    caja_cantidad_inventario = ctk.CTkEntry(form_inventario, placeholder_text="Ej: 15", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Cantidad
    # Ubicamos el Entry en la cuadrícula
    caja_cantidad_inventario.grid(row=3, column=0, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 2, Columna 1: Etiqueta de Estado ubicada ARRIBA del ComboBox
    etiqueta_est_inv = ctk.CTkLabel(form_inventario, text="📊 Estado Físico:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Estado
    # Ubicamos la etiqueta
    etiqueta_est_inv.grid(row=2, column=1, sticky="w", padx=14, pady=(0, 2))  # Margen
    # Fila 3, Columna 1: ComboBox de Estado de Inventario
    combo_estado_inventario = ctk.CTkComboBox(form_inventario, values=["Disponible", "En Uso", "Dañado"], fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, border_color=COLOR_BORDES, button_color=COLOR_PRIMARIO, dropdown_fg_color=COLOR_PANELES, dropdown_text_color=COLOR_TEXTO, state="readonly", corner_radius=8, height=34)  # ComboBox Estado
    # Asignamos la opción por defecto: Disponible
    combo_estado_inventario.set("Disponible")  # Opción inicial
    # Ubicamos el ComboBox en la cuadrícula
    combo_estado_inventario.grid(row=3, column=1, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Función que guarda o actualiza un artículo según id_item_edicion
    def accion_guardar_o_actualizar_inventario():  # Evento de guardado de inventario
        # Obtenemos los valores ingresados en el formulario
        nombre = caja_nombre_inventario.get().strip()  # Nombre del artículo
        categoria = combo_categoria_inventario.get()  # Categoría seleccionada
        cantidad_texto = caja_cantidad_inventario.get().strip()  # Cantidad como texto
        estado = combo_estado_inventario.get()  # Estado seleccionado
        # Validamos que los campos obligatorios no estén vacíos
        if not nombre or not cantidad_texto:  # Validación
            # Mostramos advertencia
            messagebox.showwarning("Campos Incompletos", "Por favor ingresa el Nombre y la Cantidad del artículo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Validamos que la cantidad sea un número entero positivo
        if not cantidad_texto.isdigit():  # Si no es dígito
            # Mostramos advertencia
            messagebox.showwarning("Cantidad Inválida", "La cantidad debe ser un número entero (ej: 10).")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # MODO NUEVO ARTÍCULO: Si no estamos editando
        if id_item_edicion[0] is None:  # Inserción
            # Llamamos a registrar_articulo_inventario() con sentencias preparadas (%s)
            if registrar_articulo_inventario(nombre, categoria, int(cantidad_texto), estado):  # Ejecución en MySQL
                # Mostramos notificación de éxito
                messagebox.showinfo("Éxito", f"El artículo '{nombre}' fue registrado en el inventario.")  # Mensaje
                # Limpiamos el formulario
                limpiar_formulario_inventario()  # Vuelve a modo nuevo registro
                # Recargamos la tabla de inventario
                cargar_tabla_inventario()  # Actualiza la vista
        # MODO ACTUALIZACIÓN: Si estamos editando un artículo existente por su ID
        else:  # Actualización
            # Obtenemos el ID del artículo en edición
            id_a_actualizar = id_item_edicion[0]  # ID numérico
            # Llamamos a actualizar_articulo_inventario() que ejecuta UPDATE ... WHERE id = %s
            if actualizar_articulo_inventario(id_a_actualizar, nombre, categoria, int(cantidad_texto), estado):  # Actualización en MySQL
                # Mostramos notificación de éxito
                messagebox.showinfo("Éxito", f"El artículo '{nombre}' (ID: {id_a_actualizar}) fue actualizado exitosamente.")  # Mensaje
                # Limpiamos el formulario
                limpiar_formulario_inventario()  # Vuelve a modo registro normal
                # Recargamos la tabla de inventario
                cargar_tabla_inventario()  # Actualiza la vista

    # Función auxiliar para vaciar el formulario de inventario y restablecer el botón
    def limpiar_formulario_inventario():  # Restablece formulario
        # Vaciamos la variable de edición
        id_item_edicion[0] = None  # Regresa a modo nuevo registro
        # Limpiamos las cajas de texto
        caja_nombre_inventario.delete(0, "end")  # Limpia nombre
        caja_cantidad_inventario.delete(0, "end")  # Limpia cantidad
        # Restablecemos los ComboBox a sus valores por defecto
        combo_categoria_inventario.set("Herramienta")  # Categoría por defecto
        combo_estado_inventario.set("Disponible")  # Estado por defecto
        # Restauramos el texto y color del botón principal
        boton_guardar_inv.configure(text="💾 Registrar en Inventario", fg_color=COLOR_EXITO, hover_color=COLOR_EXITO_HOVER)  # Botón estándar

    # Fila 4: Botón dinámico '💾 Registrar en Inventario' (Verde Esmeralda #10B981)
    boton_guardar_inv = ctk.CTkButton(form_inventario, text="💾 Registrar en Inventario", font=("Segoe UI", 13, "bold"), fg_color=COLOR_EXITO, text_color="#FFFFFF", hover_color=COLOR_EXITO_HOVER, corner_radius=8, height=36, command=accion_guardar_o_actualizar_inventario)  # Botón principal
    # Ubicamos el botón ocupando las 2 columnas del formulario
    boton_guardar_inv.grid(row=4, column=0, columnspan=2, sticky="ew", padx=14, pady=(4, 12))  # Expande horizontalmente

    # --- CONTENEDOR TARJETA PARA EL TREEVIEW DE INVENTARIO ---
    contenedor_tabla_inv = ctk.CTkFrame(panel_modulo_inventario, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Marco tabla
    # Empaquetamos el contenedor
    contenedor_tabla_inv.pack(fill="both", expand=True, padx=4, pady=(0, 10))  # Expande

    # Columnas del Treeview de Inventario
    columnas_inv = ("id", "nombre", "categoria", "cantidad", "estado")  # 5 columnas
    # Creamos el Treeview de inventario
    tabla_inventario = ttk.Treeview(contenedor_tabla_inv, columns=columnas_inv, show="headings")  # Treeview Inventario

    # Encabezados de las 5 columnas
    tabla_inventario.heading("id", text="ID")  # Título ID
    tabla_inventario.heading("nombre", text="Nombre del Artículo")  # Título Nombre
    tabla_inventario.heading("categoria", text="Categoría")  # Título Categoría
    tabla_inventario.heading("cantidad", text="Cantidad")  # Título Cantidad
    tabla_inventario.heading("estado", text="Estado")  # Título Estado

    # Ancho de las 5 columnas
    tabla_inventario.column("id", width=60, anchor="center")  # ID centrado
    tabla_inventario.column("nombre", width=280, anchor="w")  # Nombre a la izquierda
    tabla_inventario.column("categoria", width=140, anchor="center")  # Categoría centrada
    tabla_inventario.column("cantidad", width=100, anchor="center")  # Cantidad centrada
    tabla_inventario.column("estado", width=130, anchor="center")  # Estado centrado

    # Barra de desplazamiento vertical para la tabla de inventario
    scroll_inv = ctk.CTkScrollbar(contenedor_tabla_inv, orientation="vertical", command=tabla_inventario.yview, fg_color=COLOR_PANELES, button_color=COLOR_PRIMARIO)  # Scrollbar
    # Conectamos la tabla con la barra
    tabla_inventario.configure(yscrollcommand=scroll_inv.set)  # Sincronización
    # Empaquetamos la barra y la tabla
    scroll_inv.pack(side="right", fill="y", padx=(0, 6), pady=6)  # Barra a la derecha
    tabla_inventario.pack(side="left", fill="both", expand=True, padx=(6, 0), pady=6)  # Tabla a la izquierda

    # Función que recarga los artículos en la tabla de inventario desde MySQL
    def cargar_tabla_inventario():  # Actualiza Treeview de inventario
        # Borramos las filas actuales
        for fila in tabla_inventario.get_children():  # Itera filas
            # Borra fila
            tabla_inventario.delete(fila)  # Limpieza
        # Obtenemos los registros desde la base de datos
        for articulo in obtener_articulos_inventario():  # Recorre artículos
            # Inserta cada artículo en la tabla
            tabla_inventario.insert("", "end", values=articulo)  # Dibuja fila

    # --- BARRA INFERIOR DE ACCIONES DE INVENTARIO ---
    panel_acciones_inv = ctk.CTkFrame(panel_modulo_inventario, fg_color="transparent")  # Marco de acciones
    # Empaquetamos el panel de acciones
    panel_acciones_inv.pack(fill="x", padx=4, pady=(0, 6))  # Margen inferior

    # Función que carga el artículo seleccionado en el formulario para editarlo
    def accion_editar_inventario():  # Evento del botón Editar en inventario
        # Obtenemos la selección del Treeview
        seleccion = tabla_inventario.selection()  # Fila seleccionada
        # Validamos que exista una fila seleccionada
        if not seleccion:  # Ninguna fila seleccionada
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un artículo de la tabla para editarlo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los valores de la fila
        valores_fila = tabla_inventario.item(seleccion[0], "values")  # Tupla de 5 valores
        # Guardamos el ID del artículo en la variable de edición
        id_item_edicion[0] = valores_fila[0]  # Guarda el ID
        # Limpiamos las cajas de texto antes de llenarlas
        caja_nombre_inventario.delete(0, "end")  # Limpia nombre
        caja_cantidad_inventario.delete(0, "end")  # Limpia cantidad
        # Cargamos los datos del artículo en el formulario
        caja_nombre_inventario.insert(0, valores_fila[1])  # Carga nombre
        combo_categoria_inventario.set(valores_fila[2])  # Carga categoría
        caja_cantidad_inventario.insert(0, valores_fila[3])  # Carga cantidad
        combo_estado_inventario.set(valores_fila[4])  # Carga estado
        # Cambiamos el texto del botón a 'Actualizar' y le damos color Azul #3B82F6
        boton_guardar_inv.configure(text=f"💾 Actualizar Artículo (ID: {valores_fila[0]})", fg_color=COLOR_EDITAR, hover_color=COLOR_EDITAR_HOVER)  # Modo edición

    # Función que elimina el artículo seleccionado de MySQL previa confirmación
    def accion_eliminar_inventario():  # Evento del botón Eliminar en inventario
        # Obtenemos la selección del Treeview
        seleccion = tabla_inventario.selection()  # Fila seleccionada
        # Validamos que exista una fila seleccionada
        if not seleccion:  # Ninguna fila seleccionada
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un artículo de la tabla para eliminarlo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los valores de la fila seleccionada
        valores_fila = tabla_inventario.item(seleccion[0], "values")  # Valores
        # Obtenemos el ID del artículo (posición 0)
        id_a_eliminar = valores_fila[0]  # ID del artículo
        # Obtenemos el nombre del artículo (posición 1)
        nombre_a_eliminar = valores_fila[1]  # Nombre del artículo
        # Pedimos confirmación al usuario antes de borrar
        confirmado = messagebox.askyesno("Confirmar Eliminación", f"¿Estás seguro de que deseas eliminar permanentemente el artículo '{nombre_a_eliminar}' (ID: {id_a_eliminar})?")  # Confirmación
        # Si el usuario confirma la acción
        if confirmado:  # Usuario dijo SÍ
            # Ejecutamos la eliminación segura mediante DELETE FROM inventario_general WHERE id = %s
            if eliminar_articulo_inventario(id_a_eliminar):  # Borrado en MySQL
                # Notificamos éxito
                messagebox.showinfo("Éxito", f"El artículo '{nombre_a_eliminar}' fue eliminado correctamente.")  # Mensaje
                # Si estábamos editando este artículo, reseteamos el formulario
                if id_item_edicion[0] == id_a_eliminar:  # Si coincide
                    # Limpiamos el formulario
                    limpiar_formulario_inventario()  # Vuelve a modo nuevo registro
                # Recargamos la tabla de inventario
                cargar_tabla_inventario()  # Actualiza la vista

    # Botón '✏️ Editar Seleccionada' (Azul #3B82F6)
    boton_editar_inv = ctk.CTkButton(panel_acciones_inv, text="✏️ Editar Seleccionada", font=("Segoe UI", 12, "bold"), fg_color=COLOR_EDITAR, text_color="#FFFFFF", hover_color=COLOR_EDITAR_HOVER, corner_radius=8, height=36, command=accion_editar_inventario)  # Botón Editar
    # Empaquetamos el botón Editar a la izquierda
    boton_editar_inv.pack(side="left", padx=(0, 8))  # Margen derecho

    # Botón '🗑️ Eliminar Seleccionada' (Rojo Carmesí #EF4444)
    boton_eliminar_inv = ctk.CTkButton(panel_acciones_inv, text="🗑️ Eliminar Seleccionada", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PELIGRO, text_color="#FFFFFF", hover_color=COLOR_PELIGRO_HOVER, corner_radius=8, height=36, command=accion_eliminar_inventario)  # Botón Eliminar
    # Empaquetamos el botón Eliminar al lado de Editar
    boton_eliminar_inv.pack(side="left", padx=(0, 8))  # Margen derecho

    # Botón '✖ Cancelar Edición' de inventario
    boton_cancelar_inv = ctk.CTkButton(panel_acciones_inv, text="✖ Cancelar Edición", font=("Segoe UI", 12), fg_color=COLOR_BORDES, text_color=COLOR_TEXTO, hover_color="#475569", corner_radius=8, height=36, command=limpiar_formulario_inventario)  # Botón Cancelar
    # Empaquetamos el botón Cancelar Edición
    boton_cancelar_inv.pack(side="left")  # Al lado de Eliminar

    # ==========================================================================
    # 10. MÓDULO 4: CONTROL DE USUARIOS (BCRYPT OWASP A02 & A03)
    # ==========================================================================

    # Panel contenedor general para el Módulo 4 de Usuarios
    panel_modulo_usuarios = ctk.CTkFrame(area_central, fg_color="transparent")  # Marco transparente

    # Contenedor superior para el título y subtítulo del módulo de usuarios
    cabecera_modulo_usr = ctk.CTkFrame(panel_modulo_usuarios, fg_color="transparent")  # Marco cabecera
    # Empaquetamos la cabecera
    cabecera_modulo_usr.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Título de sección en tipografía grande Segoe UI 24 Bold
    etiqueta_tit_usuarios = ctk.CTkLabel(cabecera_modulo_usr, text="👥 Control de Usuarios y Seguridad Criptográfica", font=("Segoe UI", 24, "bold"), text_color=COLOR_TEXTO)  # Título
    # Posicionamos el título
    etiqueta_tit_usuarios.pack(anchor="w")  # Alineación izquierda

    # Subtítulo explicativo sobre la seguridad de contraseñas Bcrypt
    etiqueta_sub_usuarios = ctk.CTkLabel(cabecera_modulo_usr, text="Autenticación segura con salt criptográfico irreversible. Cumplimiento estricto de OWASP Top 10 (A02 & A03).", font=("Segoe UI", 12), text_color=COLOR_MUTED)  # Subtítulo
    # Posicionamos el subtítulo
    etiqueta_sub_usuarios.pack(anchor="w", pady=(2, 0))  # Margen

    # --- BARRA DE BÚSQUEDA DE USUARIOS ---
    panel_busqueda_usr = ctk.CTkFrame(panel_modulo_usuarios, fg_color="transparent")  # Contenedor del buscador
    # Empaquetamos el panel del buscador
    panel_busqueda_usr.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior

    # Caja de texto para buscar usuarios por su nombre de cuenta
    caja_buscar_usr = ctk.CTkEntry(panel_busqueda_usr, placeholder_text="🔍 Buscar usuario por nombre de cuenta...", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=36)  # Entry buscador usuario
    # Empaquetamos la caja de texto
    caja_buscar_usr.pack(side="left", fill="x", expand=True, padx=(0, 8))  # Ocupa el espacio disponible

    # Función que busca usuarios por nombre usando sentencia preparada LIKE %s
    def accion_buscar_usuario():  # Evento del botón Buscar Usuario
        # Obtenemos el nombre buscado
        termino_usr = caja_buscar_usr.get().strip()  # Texto ingresado
        # Validamos que no esté vacío
        if not termino_usr:  # Si está vacía
            # Aviso informativo
            messagebox.showinfo("Búsqueda", "Ingresa un nombre de usuario para buscar.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Consultamos los usuarios coincidentes
        resultados_usr = buscar_usuarios(termino_usr)  # Consulta segura con %s
        # Limpiamos el Treeview de usuarios
        for fila in tabla_usuarios.get_children():  # Itera filas
            # Borra fila
            tabla_usuarios.delete(fila)  # Limpieza
        # Insertamos solo los usuarios encontrados (ID y Nombre, nunca el hash)
        for u in resultados_usr:  # Itera filas
            # Agrega la fila al Treeview
            tabla_usuarios.insert("", "end", values=u)  # Muestra usuario

    # Función para limpiar el filtro de búsqueda y ver todos los usuarios
    def accion_limpiar_usuario():  # Evento del botón Limpiar Filtro en usuarios
        # Vaciamos la caja de texto
        caja_buscar_usr.delete(0, "end")  # Limpia la caja
        # Recargamos todos los usuarios de la base de datos
        cargar_tabla_usuarios()  # Muestra todos los usuarios

    # Botón '🔍 Buscar' de usuarios
    boton_buscar_usr = ctk.CTkButton(panel_busqueda_usr, text="🔍 Buscar", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PRIMARIO, text_color="#FFFFFF", hover_color=COLOR_PRIMARIO_HOVER, corner_radius=8, width=95, height=36, command=accion_buscar_usuario)  # Botón Buscar
    # Empaquetamos el botón Buscar
    boton_buscar_usr.pack(side="left", padx=(0, 6))  # Margen derecho

    # Botón '✖ Limpiar' de usuarios
    boton_limpiar_usr = ctk.CTkButton(panel_busqueda_usr, text="✖ Limpiar", font=("Segoe UI", 12), fg_color=COLOR_BORDES, text_color=COLOR_TEXTO, hover_color="#475569", corner_radius=8, width=85, height=36, command=accion_limpiar_usuario)  # Botón Limpiar
    # Empaquetamos el botón Limpiar
    boton_limpiar_usr.pack(side="left")  # Al lado del botón Buscar

    # --- FORMULARIO PARA REGISTRAR NUEVO USUARIO (BCRYPT OWASP A02) ---
    form_usuarios = ctk.CTkFrame(panel_modulo_usuarios, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Tarjeta formulario usuarios
    # Empaquetamos el contenedor del formulario
    form_usuarios.pack(fill="x", padx=4, pady=(0, 10))  # Margen inferior
    # Configuramos 2 columnas de igual ancho
    form_usuarios.grid_columnconfigure(0, weight=1)  # Columna 1
    form_usuarios.grid_columnconfigure(1, weight=1)  # Columna 2

    # Fila 0, Columna 0: Etiqueta de Nombre de Usuario ubicada ARRIBA del campo
    etiqueta_nom_usr = ctk.CTkLabel(form_usuarios, text="👤 Nombre de Usuario:", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Usuario
    # Ubicamos la etiqueta
    etiqueta_nom_usr.grid(row=0, column=0, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 0: Entry de Nombre de Usuario con placeholder
    caja_usuario_nuevo = ctk.CTkEntry(form_usuarios, placeholder_text="Ej: docente_lab / tecnico1", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Usuario
    # Ubicamos el Entry en la cuadrícula
    caja_usuario_nuevo.grid(row=1, column=0, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Fila 0, Columna 1: Etiqueta de Contraseña ubicada ARRIBA del campo
    etiqueta_pass_usr = ctk.CTkLabel(form_usuarios, text="🔒 Contraseña Segura (Cifrado Bcrypt):", font=("Segoe UI", 13, "bold"), text_color=COLOR_MUTED)  # Label Contraseña
    # Ubicamos la etiqueta
    etiqueta_pass_usr.grid(row=0, column=1, sticky="w", padx=14, pady=(10, 2))  # Margen
    # Fila 1, Columna 1: Entry de Contraseña con asteriscos protectores
    caja_contrasena_nueva = ctk.CTkEntry(form_usuarios, placeholder_text="Ingresa la contraseña segura", show="*", fg_color=COLOR_FONDO, text_color=COLOR_TEXTO, placeholder_text_color=COLOR_MUTED, border_color=COLOR_BORDES, corner_radius=8, height=34)  # Entry Contraseña
    # Ubicamos el Entry en la cuadrícula
    caja_contrasena_nueva.grid(row=1, column=1, sticky="ew", padx=14, pady=(0, 8))  # Expande

    # Función que guarda un nuevo usuario cifrando su contraseña con bcrypt
    def accion_guardar_usuario():  # Evento del botón Registrar Usuario
        # Obtenemos el nombre de usuario escrito
        nombre_usr = caja_usuario_nuevo.get().strip()  # Usuario ingresado
        # Obtenemos la contraseña en texto plano ingresada
        clave_plana = caja_contrasena_nueva.get().strip()  # Contraseña ingresada
        # Validamos que no se dejen campos vacíos
        if not nombre_usr or not clave_plana:  # Validación
            # Alerta al usuario
            messagebox.showwarning("Campos Incompletos", "Por favor ingresa tanto el Nombre de Usuario como la Contraseña.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Validamos una longitud mínima de seguridad para la contraseña
        if len(clave_plana) < 4:  # Mínimo 4 caracteres
            # Advertencia de seguridad
            messagebox.showwarning("Contraseña Débil", "La contraseña debe tener al menos 4 caracteres.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Llamamos a registrar_usuario() que cifra con bcrypt y ejecuta INSERT con sentencias preparadas (%s)
        if registrar_usuario(nombre_usr, clave_plana):  # Ejecución en MySQL
            # Notificamos éxito indicando la protección bcrypt
            messagebox.showinfo("Éxito", f"El usuario '{nombre_usr}' fue registrado exitosamente con cifrado seguro Bcrypt.")  # Mensaje
            # Limpiamos las cajas de texto del formulario
            caja_usuario_nuevo.delete(0, "end")  # Limpia usuario
            caja_contrasena_nueva.delete(0, "end")  # Limpia clave
            # Recargamos la tabla de usuarios
            cargar_tabla_usuarios()  # Actualiza la vista

    # Fila 2: Botón '💾 Registrar Usuario Seguro (Bcrypt)' (Verde Esmeralda #10B981)
    boton_guardar_usr = ctk.CTkButton(form_usuarios, text="💾 Registrar Usuario Seguro (Bcrypt)", font=("Segoe UI", 13, "bold"), fg_color=COLOR_EXITO, text_color="#FFFFFF", hover_color=COLOR_EXITO_HOVER, corner_radius=8, height=36, command=accion_guardar_usuario)  # Botón Registrar
    # Ubicamos el botón ocupando las 2 columnas del formulario
    boton_guardar_usr.grid(row=2, column=0, columnspan=2, sticky="ew", padx=14, pady=(4, 12))  # Expande horizontalmente

    # --- CONTENEDOR TARJETA PARA EL TREEVIEW DE USUARIOS (SOLO ID Y USUARIO, NUNCA EL HASH) ---
    contenedor_tabla_usr = ctk.CTkFrame(panel_modulo_usuarios, fg_color=COLOR_PANELES, corner_radius=12, border_color=COLOR_BORDES, border_width=1)  # Marco tabla usuarios
    # Empaquetamos el contenedor
    contenedor_tabla_usr.pack(fill="both", expand=True, padx=4, pady=(0, 10))  # Expande

    # Columnas del Treeview de Usuarios (estrictamente solo ID y Usuario por seguridad OWASP)
    columnas_usr = ("id", "usuario")  # 2 columnas seguras
    # Creamos el Treeview
    tabla_usuarios = ttk.Treeview(contenedor_tabla_usr, columns=columnas_usr, show="headings")  # Treeview Usuarios

    # Encabezados de las 2 columnas
    tabla_usuarios.heading("id", text="ID")  # Título ID
    tabla_usuarios.heading("usuario", text="Nombre de Usuario")  # Título Nombre de Usuario

    # Ancho de las columnas
    tabla_usuarios.column("id", width=80, anchor="center")  # ID centrado
    tabla_usuarios.column("usuario", width=480, anchor="w")  # Nombre de usuario a la izquierda

    # Barra de desplazamiento vertical para la tabla de usuarios
    scroll_usr = ctk.CTkScrollbar(contenedor_tabla_usr, orientation="vertical", command=tabla_usuarios.yview, fg_color=COLOR_PANELES, button_color=COLOR_PRIMARIO)  # Scrollbar
    # Conectamos la tabla con la barra
    tabla_usuarios.configure(yscrollcommand=scroll_usr.set)  # Sincronización
    # Empaquetamos la barra y la tabla
    scroll_usr.pack(side="right", fill="y", padx=(0, 6), pady=6)  # Barra a la derecha
    tabla_usuarios.pack(side="left", fill="both", expand=True, padx=(6, 0), pady=6)  # Tabla a la izquierda

    # Función que recarga los usuarios en la tabla desde MySQL
    def cargar_tabla_usuarios():  # Actualiza tabla_usuarios
        # Borramos las filas actuales
        for fila in tabla_usuarios.get_children():  # Itera filas
            # Borra fila
            tabla_usuarios.delete(fila)  # Limpieza
        # Obtenemos la lista de usuarios (solo ID y Usuario)
        for u in obtener_usuarios():  # Recorre usuarios devueltos
            # Insertamos la fila en la tabla
            tabla_usuarios.insert("", "end", values=u)  # Agrega fila

    # --- BARRA INFERIOR DE ACCIONES DE USUARIOS (CAMBIAR CLAVE Y ELIMINAR) ---
    panel_acciones_usr = ctk.CTkFrame(panel_modulo_usuarios, fg_color="transparent")  # Marco inferior de acciones
    # Empaquetamos el panel de acciones
    panel_acciones_usr.pack(fill="x", padx=4, pady=(0, 6))  # Margen inferior

    # Función que permite cambiar la contraseña de un usuario cifrándola con bcrypt
    def accion_cambiar_contrasena():  # Evento del botón Cambiar Contraseña
        # Obtenemos la selección del Treeview
        seleccion = tabla_usuarios.selection()  # Fila seleccionada
        # Validamos que exista una fila seleccionada
        if not seleccion:  # Ningún usuario elegido
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un usuario de la tabla para cambiar su contraseña.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los datos de la fila seleccionada
        valores_fila = tabla_usuarios.item(seleccion[0], "values")  # Valores de usuario
        # Obtenemos el ID del usuario
        id_usr_cambio = valores_fila[0]  # ID del usuario
        # Obtenemos el nombre del usuario
        nombre_usr_cambio = valores_fila[1]  # Nombre del usuario
        # Abrimos un cuadro de diálogo moderno para pedir la nueva contraseña
        dialogo = ctk.CTkInputDialog(text=f"Ingresa la nueva contraseña para el usuario '{nombre_usr_cambio}':", title="Cambiar Contraseña (Bcrypt)")  # Diálogo de entrada
        # Capturamos la nueva contraseña escrita por el administrador
        nueva_clave = dialogo.get_input()  # Entrada del usuario
        # Si el usuario presionó Cancelar o no escribió nada
        if nueva_clave is None or not nueva_clave.strip():  # Entrada vacía o cancelada
            # Salimos sin realizar cambios
            return  # Detiene la acción
        # Validamos que la nueva contraseña tenga longitud adecuada
        if len(nueva_clave.strip()) < 4:  # Mínimo 4 caracteres
            # Advertencia de seguridad
            messagebox.showwarning("Contraseña Inválida", "La nueva contraseña debe tener al menos 4 caracteres.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Llamamos a cambiar_contrasena_usuario() que cifra con bcrypt y ejecuta UPDATE ... WHERE id_usuario = %s
        if cambiar_contrasena_usuario(id_usr_cambio, nueva_clave.strip()):  # Actualización en MySQL
            # Notificamos éxito
            messagebox.showinfo("Éxito", f"La contraseña de '{nombre_usr_cambio}' fue actualizada y cifrada con Bcrypt exitosamente.")  # Mensaje

    # Función que elimina un usuario previa confirmación (protegiendo al admin principal)
    def accion_eliminar_usuario():  # Evento del botón Eliminar Usuario
        # Obtenemos la selección del Treeview
        seleccion = tabla_usuarios.selection()  # Fila seleccionada
        # Validamos que exista una fila seleccionada
        if not seleccion:  # Ningún usuario elegido
            # Mostramos advertencia
            messagebox.showwarning("Selección Requerida", "Por favor selecciona un usuario de la tabla para eliminarlo.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Extraemos los datos de la fila
        valores_fila = tabla_usuarios.item(seleccion[0], "values")  # Valores
        # Obtenemos el ID del usuario
        id_usr_eliminar = valores_fila[0]  # ID del usuario
        # Obtenemos el nombre del usuario
        nombre_usr_eliminar = valores_fila[1]  # Nombre del usuario
        # Si el usuario es el 'admin' principal, evitamos su eliminación para no bloquear el sistema
        if str(nombre_usr_eliminar).strip().lower() == "admin":  # Protección de cuenta admin
            # Advertencia de seguridad
            messagebox.showwarning("Acción Bloqueada", "Por motivos de seguridad del laboratorio, la cuenta 'admin' principal no puede ser eliminada.")  # Aviso
            # Salimos de la función
            return  # Detiene la acción
        # Pedimos confirmación al usuario antes de borrar la cuenta
        confirmado = messagebox.askyesno("Confirmar Eliminación", f"¿Estás seguro de que deseas eliminar permanentemente al usuario '{nombre_usr_eliminar}' (ID: {id_usr_eliminar})?")  # Confirmación
        # Si el usuario confirma
        if confirmado:  # Usuario dijo SÍ
            # Ejecutamos DELETE FROM usuarios WHERE id_usuario = %s
            if eliminar_usuario(id_usr_eliminar):  # Borrado en MySQL
                # Notificamos éxito
                messagebox.showinfo("Éxito", f"El usuario '{nombre_usr_eliminar}' fue eliminado correctamente.")  # Mensaje
                # Recargamos la tabla de usuarios
                cargar_tabla_usuarios()  # Actualiza la vista

    # Botón '🔑 Cambiar Contraseña' (Azul Profesional #3B82F6)
    boton_cambiar_pass = ctk.CTkButton(panel_acciones_usr, text="🔑 Cambiar Contraseña", font=("Segoe UI", 12, "bold"), fg_color=COLOR_EDITAR, text_color="#FFFFFF", hover_color=COLOR_EDITAR_HOVER, corner_radius=8, height=36, command=accion_cambiar_contrasena)  # Botón Cambiar Contraseña
    # Empaquetamos el botón a la izquierda
    boton_cambiar_pass.pack(side="left", padx=(0, 8))  # Margen derecho

    # Botón '🗑️ Eliminar Usuario' (Rojo Carmesí #EF4444)
    boton_eliminar_usr = ctk.CTkButton(panel_acciones_usr, text="🗑️ Eliminar Usuario", font=("Segoe UI", 12, "bold"), fg_color=COLOR_PELIGRO, text_color="#FFFFFF", hover_color=COLOR_PELIGRO_HOVER, corner_radius=8, height=36, command=accion_eliminar_usuario)  # Botón Eliminar Usuario
    # Empaquetamos el botón al lado de Cambiar Contraseña
    boton_eliminar_usr.pack(side="left")  # Al lado de Cambiar Contraseña

    # ==========================================================================
    # 11. INICIALIZACIÓN POR DEFECTO DEL DASHBOARD Y BUCLE PRINCIPAL
    # ==========================================================================

    # Mostramos inicialmente el Módulo 1: Equipos al abrir la aplicación
    mostrar_modulo_equipos()  # Despliega Equipos como vista inicial

    # Iniciamos el bucle principal de eventos de CustomTkinter para mantener la ventana activa
    ventana_dashboard.mainloop()  # Bucle principal de la interfaz gráfica
