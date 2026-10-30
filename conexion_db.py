# ==============================================================================
# MÓDULO 1: conexion_db.py (Conexión Segura a MySQL bajo normativa OWASP A05)
# ==============================================================================

# Importamos la librería mysql.connector para poder comunicarnos con la base de datos MySQL
import mysql.connector  # Herramienta oficial para conectar Python con el servidor MySQL
# Importamos messagebox desde tkinter para mostrar avisos al usuario en caso de error
from tkinter import messagebox  # Herramienta para mostrar ventanas emergentes de alerta

# Definimos la función obtener_conexion() que abrirá el puente entre nuestro programa y MySQL
def obtener_conexion():  # Inicio de la función que devuelve la conexión lista para usarse
    # Definimos la lista con las posibles contraseñas del entorno local (incluyendo 'W@qh1' y vacía '')
    lista_claves = ["W@qh1", ""]  # Permite conectar con la contraseña actual o sin contraseña

    # Probamos cada contraseña de la lista para establecer la conexión
    for clave_actual in lista_claves:  # Recorremos cada contraseña posible
        # Usamos try para intentar conectar con la contraseña actual
        try:  # Intentamos abrir la conexión con MySQL
            # Creamos la conexión indicando host, usuario, contraseña y la base de datos controllabitern
            conexion_mysql = mysql.connector.connect(  # Llamamos a connect de mysql.connector
                host="localhost",  # Servidor local en nuestra propia computadora
                user="root",  # Usuario administrador por defecto de MySQL
                password=clave_actual,  # Contraseña configurada en MySQL (W@qh1)
                database="controllabitern"  # Nombre de la base de datos del laboratorio
            )  # Cerramos los parámetros de conexión
            # Si la conexión fue exitosa, la devolvemos inmediatamente
            return conexion_mysql  # Retornamos la conexión activa y lista para usarse
        # Si la contraseña no coincide, intentamos con la siguiente de la lista
        except mysql.connector.Error:  # Si falla con esta contraseña
            # Continuamos al siguiente intento
            continue  # Pasa a la siguiente clave

    # Si ninguna contraseña funcionó o MySQL está apagado, aplicamos OWASP A05 (Manejo Seguro de Errores)
    print("[LOG SEGURIDAD - OWASP A05] No se pudo conectar a MySQL con ninguna contraseña.")  # Detalle solo en consola
    # Mostramos al usuario una alerta con mensaje genérico sin revelar datos internos de la base de datos
    messagebox.showerror("Error de Conexión", "Error interno al conectar con el servidor de la base de datos.")  # Mensaje seguro
    # Devolvemos None indicando que la conexión no se pudo realizar
    return None  # Retornamos vacío
