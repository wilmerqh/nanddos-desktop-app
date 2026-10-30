# ==============================================================================
# MÓDULO 3: generador_pdf.py (Generación de Reportes Profesionales en PDF)
# ==============================================================================

# Importamos la clase FPDF de la librería fpdf para diseñar y crear documentos PDF
from fpdf import FPDF  # Herramienta base para la creación de documentos PDF
# Importamos messagebox desde tkinter para mostrar alertas informativas de éxito o error
from tkinter import messagebox  # Herramienta para cuadros de diálogo emergentes
# Importamos datetime para registrar la fecha y hora exacta de emisión del reporte
from datetime import datetime  # Módulo para el manejo de fecha y hora del sistema

# Definimos la clase personalizada ReporteInventarioPDF que hereda de FPDF
class ReporteInventarioPDF(FPDF):  # Clase para personalizar encabezado y pie de página
    # Definimos el método footer para colocar el pie de página automático en cada hoja
    def footer(self):  # Función que se ejecuta automáticamente al pie de cada página
        # Nos posicionamos a 15 milímetros del borde inferior de la hoja
        self.set_y(-15)  # Coordenada Y negativa medida desde el fondo
        # Configuramos la fuente en Arial cursiva de tamaño 9
        self.set_font("Arial", "I", 9)  # Letra estilo itálica y tamaño pequeño
        # Configuramos un color gris tenue para el texto del pie de página
        self.set_text_color(148, 163, 184)  # Color gris pizarra suave (#94A3B8)
        # Dibujamos el texto del pie de página con el nombre del sistema y el número de página
        texto_pie = f"ControlLab ITERN - Sistema de Gestion de Laboratorio  |  Pagina {self.page_no()}"  # Texto del pie
        # Imprimimos la celda centrada en todo el ancho de la hoja
        self.cell(0, 10, texto_pie, border=0, ln=0, align="C")  # Celda de pie de página centrada

# Definimos la función principal exportar_inventario_pdf (con alias exportar_pdf)
def exportar_inventario_pdf(lista_computadoras, area_filtro="Todas las áreas"):  # Recibe los equipos visibles y el filtro activo
    # Usamos try para capturar cualquier fallo (por ejemplo, si el PDF está abierto en otro programa)
    try:  # Inicio del bloque de generación segura
        # Creamos la instancia del documento PDF en orientación horizontal ("L" Landscape, A4)
        documento_pdf = ReporteInventarioPDF(orientation="L", unit="mm", format="A4")  # Hoja A4 horizontal
        # Habilitamos el salto de página automático dejando 15 mm de margen inferior
        documento_pdf.set_auto_page_break(auto=True, margin=15)  # Salto de página automático
        # Añadimos la primera página del documento
        documento_pdf.add_page()  # Inserta la página inicial

        # ======================================================================
        # 1. ENCABEZADO INSTITUCIONAL Y TÍTULO DEL REPORTE
        # ======================================================================

        # Configuramos la fuente para el título principal: Arial negrita tamaño 18
        documento_pdf.set_font("Arial", "B", 18)  # Fuente destacada para el título
        # Configuramos el color azul cian oscuro institucional (#0284C7)
        documento_pdf.set_text_color(2, 132, 199)  # Tono azul característico del sistema
        # Dibujamos el título exacto solicitado: 'Reporte de Inventario - ControlLab ITERN'
        documento_pdf.cell(0, 10, "Reporte de Inventario - ControlLab ITERN", border=0, ln=1, align="C")  # Título centrado

        # Configuramos la fuente para la fecha de emisión y el filtro aplicado
        documento_pdf.set_font("Arial", "", 10)  # Letra tamaño regular para metadatos
        # Configuramos color gris medio para el subtítulo informativo
        documento_pdf.set_text_color(100, 116, 139)  # Color gris pizarra (#64748B)
        # Obtenemos la fecha y hora actual formateada en DD/MM/AAAA HH:MM:SS
        fecha_emision = datetime.now().strftime("%d/%m/%Y %H:%M:%S")  # Fecha y hora actual
        # Construimos el texto del subtítulo con la fecha y el filtro aplicado
        subtitulo_info = f"Fecha de emision: {fecha_emision}   |   Filtro aplicado: {area_filtro}"  # Subtítulo con filtro
        # Imprimimos el subtítulo informativo centrado
        documento_pdf.cell(0, 6, subtitulo_info, border=0, ln=1, align="C")  # Subtítulo centrado

        # Dibujamos una línea decorativa horizontal que separa el encabezado de la tabla
        documento_pdf.set_draw_color(2, 132, 199)  # Color azul de la línea
        # Establecemos el grosor de la línea decorativa
        documento_pdf.set_line_width(0.6)  # Grosor de 0.6 mm
        # Trazamos la línea horizontal a lo ancho de los márgenes (de X=11 a X=286)
        documento_pdf.line(11, documento_pdf.get_y() + 2, 286, documento_pdf.get_y() + 2)  # Línea divisoria
        # Agregamos un espacio vertical de 6 milímetros
        documento_pdf.ln(6)  # Espaciado antes de la tabla

        # ======================================================================
        # 2. ENCABEZADOS DE LAS 6 COLUMNAS DE LA TABLA
        # ======================================================================

        # Configuramos la fuente de los encabezados: Arial negrita tamaño 11
        documento_pdf.set_font("Arial", "B", 11)  # Negrita para encabezados
        # Configuramos el color de fondo de las celdas de encabezado en azul (#0284C7)
        documento_pdf.set_fill_color(2, 132, 199)  # Fondo azul institucional
        # Configuramos el color del texto de los encabezados en blanco puro
        documento_pdf.set_text_color(255, 255, 255)  # Texto blanco
        # Restablecemos el color de los bordes de celda en azul oscuro
        documento_pdf.set_draw_color(14, 116, 144)  # Borde elegante
        # Restablecemos el grosor normal de borde
        documento_pdf.set_line_width(0.2)  # Grosor estándar de tabla

        # Definimos los anchos milimétricos exactos para las 6 columnas (suma total: 275 mm)
        ancho_id = 25  # Ancho Columna 1: ID/Código (número autoincrementado)
        ancho_area = 35  # Ancho Columna 2: Área (Laboratorio o Bodega)
        ancho_proc = 75  # Ancho Columna 3: Procesador
        ancho_ram = 40  # Ancho Columna 4: Memoria RAM
        ancho_alm = 55  # Ancho Columna 5: Almacenamiento
        ancho_est = 45  # Ancho Columna 6: Estado actual

        # Columna 1: ID/Código (solicitado exactamente con ese nombre)
        documento_pdf.cell(ancho_id, 9, "ID/Codigo", border=1, ln=0, align="C", fill=True)  # Celda ID/Código
        # Columna 2: Área
        documento_pdf.cell(ancho_area, 9, "Area", border=1, ln=0, align="C", fill=True)  # Celda Área
        # Columna 3: Procesador
        documento_pdf.cell(ancho_proc, 9, "Procesador", border=1, ln=0, align="C", fill=True)  # Celda Procesador
        # Columna 4: RAM
        documento_pdf.cell(ancho_ram, 9, "RAM", border=1, ln=0, align="C", fill=True)  # Celda RAM
        # Columna 5: Almacenamiento
        documento_pdf.cell(ancho_alm, 9, "Almacenamiento", border=1, ln=0, align="C", fill=True)  # Celda Almacenamiento
        # Columna 6: Estado (con ln=1 para bajar a la primera fila de datos)
        documento_pdf.cell(ancho_est, 9, "Estado", border=1, ln=1, align="C", fill=True)  # Celda Estado

        # ======================================================================
        # 3. FILAS DE DATOS (EQUIPOS VISIBLES EN EL TREEVIEW)
        # ======================================================================

        # Configuramos la fuente para las filas de datos: Arial regular tamaño 10
        documento_pdf.set_font("Arial", "", 10)  # Texto regular y legible
        # Configuramos el color del texto en gris oscuro casi negro (#0F172A)
        documento_pdf.set_text_color(15, 23, 42)  # Letras legibles
        # Color de borde gris claro entre las celdas
        documento_pdf.set_draw_color(203, 213, 225)  # Bordes suaves (#CBD5E1)

        # Contador de filas para alternar colores de fondo y sumar total
        numero_fila = 0  # Inicializamos el contador de filas

        # Recorremos cada una de las computadoras recibidas (filas visibles en el Treeview)
        for equipo in lista_computadoras:  # Iteramos por cada equipo visible
            # Si el equipo tiene 6 o más columnas (proveniente del Treeview)
            texto_id = str(equipo[0])  # Columna 0: ID/Código autoincrementado de MySQL
            # Limpiamos el texto del área para asegurar que sea estrictamente 'Laboratorio' o 'Bodega'
            area_cruda = str(equipo[1]).strip()  # Leemos el valor del área
            # Asignamos 'Bodega' si contiene 'bodega', o 'Laboratorio' en cualquier otro caso
            texto_area = "Bodega" if "bodega" in area_cruda.lower() else "Laboratorio"  # Área limpia
            texto_proc = str(equipo[2])  # Columna 2: Procesador
            texto_ram = str(equipo[3])  # Columna 3: Memoria RAM
            texto_alm = str(equipo[4])  # Columna 4: Almacenamiento
            texto_est = str(equipo[5])  # Columna 5: Estado actual

            # Alternamos el color de fondo: filas pares en blanco, impares en gris muy claro (#F8FAFC)
            if numero_fila % 2 == 0:  # Fila par
                documento_pdf.set_fill_color(255, 255, 255)  # Fondo blanco puro
            else:  # Fila impar
                documento_pdf.set_fill_color(248, 250, 252)  # Fondo grisáceo suave (#F8FAFC)

            # Dibujamos la celda Columna 1: ID/Código centrado
            documento_pdf.cell(ancho_id, 8, texto_id, border=1, ln=0, align="C", fill=True)  # ID/Código
            # Dibujamos la celda Columna 2: Área limpia centrada ('Laboratorio' o 'Bodega')
            documento_pdf.cell(ancho_area, 8, texto_area, border=1, ln=0, align="C", fill=True)  # Área limpia
            # Dibujamos la celda Columna 3: Procesador alineado a la izquierda con margen visual
            documento_pdf.cell(ancho_proc, 8, f" {texto_proc}", border=1, ln=0, align="L", fill=True)  # Procesador
            # Dibujamos la celda Columna 4: RAM centrada
            documento_pdf.cell(ancho_ram, 8, texto_ram, border=1, ln=0, align="C", fill=True)  # RAM
            # Dibujamos la celda Columna 5: Almacenamiento centrado
            documento_pdf.cell(ancho_alm, 8, texto_alm, border=1, ln=0, align="C", fill=True)  # Almacenamiento
            # Dibujamos la celda Columna 6: Estado centrado y bajamos al siguiente renglón (ln=1)
            documento_pdf.cell(ancho_est, 8, texto_est, border=1, ln=1, align="C", fill=True)  # Estado
            # Incrementamos el contador de filas
            numero_fila += 1  # Siguiente número de fila

        # ======================================================================
        # 4. RESUMEN FINAL AL PIE DE LA TABLA
        # ======================================================================

        # Dejamos un espacio vertical de 4 milímetros
        documento_pdf.ln(4)  # Espacio después de las filas
        # Configuramos fuente Arial negrita tamaño 10 para el resumen
        documento_pdf.set_font("Arial", "B", 10)  # Negrita para el resumen
        # Color azul institucional para el total
        documento_pdf.set_text_color(2, 132, 199)  # Color azul (#0284C7)
        # Construimos el texto con el total de equipos listados en el reporte
        resumen_texto = f"Total de equipos listados en este reporte: {numero_fila}"  # Texto resumen
        # Imprimimos el resumen alineado a la derecha de la hoja
        documento_pdf.cell(0, 7, resumen_texto, border=0, ln=1, align="R")  # Resumen a la derecha

        # ======================================================================
        # 5. GUARDADO DEL DOCUMENTO PDF EN DISCO
        # ======================================================================

        # Nombre oficial del archivo PDF de inventario
        nombre_archivo_pdf = "Reporte_Inventario.pdf"  # Nombre del archivo de salida
        # Guardamos el archivo PDF físicamente en el directorio del proyecto
        documento_pdf.output(nombre_archivo_pdf)  # Escritura física del PDF

        # Mostramos una alerta de éxito al usuario
        messagebox.showinfo(  # Ventana emergente de éxito
            "Reporte PDF Generado",  # Título de la alerta
            f"El reporte de inventario se guardo exitosamente como '{nombre_archivo_pdf}'.\n\nEquipos incluidos: {numero_fila}\nFiltro: {area_filtro}"  # Mensaje explicativo
        )  # Fin de la alerta de éxito
        # Retornamos True indicando que la exportación fue exitosa
        return True  # Éxito
    # Si ocurre una excepción (archivo bloqueado por lector PDF u otro fallo)
    except Exception as error_pdf:  # Capturamos el error
        # Imprimimos el detalle técnico en consola (OWASP A05)
        print(f"[LOG SEGURIDAD - OWASP A05] Error al exportar PDF de inventario: {error_pdf}")  # Log interno
        # Mostramos una alerta segura al usuario
        messagebox.showerror(  # Ventana de error
            "Error al Generar PDF",  # Título de la alerta
            f"No se pudo crear el archivo PDF.\nVerifica que no este abierto en otro programa.\n\nDetalle: {error_pdf}"  # Mensaje claro
        )  # Fin de la alerta de error
        # Retornamos False indicando que falló
        return False  # Fallo

# Definimos el alias exportar_pdf solicitado por el usuario para acceso directo
exportar_pdf = exportar_inventario_pdf  # Alias para compatibilidad de nomenclatura
