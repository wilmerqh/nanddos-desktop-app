# ==============================================================================
# MÓDULO 2: crud_computadoras.py (Operaciones de Base de Datos Seguras bajo OWASP)
# Sentencias Preparadas (OWASP A03), Bcrypt (OWASP A02) y Gestión de Errores (OWASP A05)
# CRUD Completo: Equipos, Áreas (Laboratorio y Bodega), Inventario y Control de Usuarios
# ==============================================================================

# Importamos la función de conexión segura desde nuestro módulo conexion_db.py
from conexion_db import obtener_conexion  # Importamos la función que conecta con MySQL
# Importamos mysql.connector para capturar los errores propios de la base de datos
import mysql.connector  # Herramienta para detectar fallos en las consultas SQL
# Importamos bcrypt para la validación y cifrado criptográfico de contraseñas (OWASP A02)
import bcrypt  # Librería de seguridad criptográfica con sal aleatoria
# Importamos messagebox desde tkinter para mostrar alertas genéricas y seguras al usuario
from tkinter import messagebox  # Herramienta para mostrar ventanas de aviso

# ==============================================================================
# 1. FUNCIÓN DE INICIALIZACIÓN AUTOMÁTICA DE TABLAS SEGURAS
# ==============================================================================

# Definimos la función inicializar_tablas_seguras que prepara las tablas en MySQL si aún no existen
def inicializar_tablas_seguras():  # Inicio de la función que verifica y prepara la base de datos
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló (es None), salimos de la función
    if conexion is None:  # Verificamos si no hay conexión
        # Salimos de la función de forma segura
        return  # Retorno seguro

    # Usamos try para ejecutar las sentencias de inicialización de forma segura
    try:  # Inicio del bloque seguro de inicialización
        # Creamos el cursor para comunicarnos con la base de datos MySQL
        cursor = conexion.cursor()  # Inicializamos el mensajero de la base de datos

        # Creamos la tabla 'areas' si aún no existe en la base de datos
        cursor.execute(  # Ejecutamos la orden de creación preventiva de areas
            "CREATE TABLE IF NOT EXISTS areas ("  # Tabla para las 2 áreas físicas
            "id_area INT AUTO_INCREMENT PRIMARY KEY, "  # Identificador numérico único
            "nombre_area VARCHAR(50) NOT NULL UNIQUE)"  # Nombre único del área operativa
        )  # Cerramos la creación de la tabla areas

        # Creamos la tabla 'computadoras' si aún no existe en la base de datos
        cursor.execute(  # Ejecutamos la orden de creación preventiva de computadoras
            "CREATE TABLE IF NOT EXISTS computadoras ("  # Tabla para los equipos de cómputo
            "codigo_pc INT AUTO_INCREMENT PRIMARY KEY, "  # Código numérico autoincrementable y clave primaria
            "id_area INT DEFAULT NULL, "  # Llave foránea hacia la tabla areas
            "procesador VARCHAR(50) NOT NULL, "  # Modelo del procesador
            "memoria_ram VARCHAR(20) NOT NULL, "  # Capacidad de la memoria RAM
            "almacenamiento VARCHAR(50) NOT NULL, "  # Capacidad del disco duro o SSD
            "estado_actual VARCHAR(30) NOT NULL, "  # Estado del equipo (Activa, etc.)
            "detalles_falla TEXT)"  # Descripción de fallas u observaciones
        )  # Cerramos la creación de la tabla computadoras

        # Creamos la tabla 'inventario_general' si aún no existe en la base de datos
        cursor.execute(  # Ejecutamos la orden de creación preventiva de inventario_general
            "CREATE TABLE IF NOT EXISTS inventario_general ("  # Tabla para herramientas y periféricos
            "id_item INT AUTO_INCREMENT PRIMARY KEY, "  # Identificador único del artículo
            "nombre_item VARCHAR(50) NOT NULL, "  # Nombre del artículo (ej: Pinzas, Teclados)
            "categoria VARCHAR(30) NOT NULL, "  # Categoría (Herramienta o Periférico)
            "cantidad INT NOT NULL, "  # Cantidad de unidades
            "estado VARCHAR(30) NOT NULL)"  # Estado del artículo
        )  # Cerramos la creación de la tabla inventario_general

        # Creamos la tabla 'usuarios' si aún no existe en la base de datos
        cursor.execute(  # Ejecutamos la orden de creación preventiva de usuarios
            "CREATE TABLE IF NOT EXISTS usuarios ("  # Tabla para credenciales de acceso
            "id_usuario INT AUTO_INCREMENT PRIMARY KEY, "  # Identificador único del usuario
            "nombre_usuario VARCHAR(50) NOT NULL UNIQUE, "  # Nombre del usuario de acceso
            "contrasena_hash VARCHAR(255) NOT NULL)"  # Hash bcrypt de la contraseña protegida
        )  # Cerramos la creación de la tabla usuarios

        # Consultamos las áreas registradas actualmente en la base de datos
        cursor.execute("SELECT nombre_area FROM areas ORDER BY id_area ASC")  # Consulta de áreas
        # Guardamos la lista con los nombres de las áreas existentes
        areas_existentes = [fila[0] for fila in cursor.fetchall()]  # Nombres actuales de áreas
        # Si las áreas no son exactamente las 2 áreas oficiales: Laboratorio y Bodega
        if areas_existentes != ["Laboratorio", "Bodega"]:  # Verificamos si no son las 2 áreas requeridas
            # Limpiamos las áreas anteriores que pudieran existir
            cursor.execute("DELETE FROM areas")  # Limpieza de áreas no requeridas
            # Escribimos la sentencia preparada con marcadores %s (OWASP A03: Cero concatenación)
            sql_insertar_area = "INSERT INTO areas (nombre_area) VALUES (%s)"  # Sentencia preparada
            # Insertamos el Área 1 oficial: 'Laboratorio' (para equipos en uso)
            cursor.execute(sql_insertar_area, ("Laboratorio",))  # Inserción de Laboratorio
            # Insertamos el Área 2 oficial: 'Bodega' (para repuestos y herramientas guardadas)
            cursor.execute(sql_insertar_area, ("Bodega",))  # Inserción de Bodega

        # Comprobamos cuántos usuarios están registrados en la tabla 'usuarios'
        cursor.execute("SELECT COUNT(*) FROM usuarios")  # Contamos los usuarios existentes
        # Obtenemos la cantidad de usuarios
        total_usuarios = cursor.fetchone()[0]  # Guardamos el total de usuarios
        # Si no existe ningún usuario registrado, creamos el usuario administrador seguro
        if total_usuarios == 0:  # Si la tabla usuarios está vacía
            # Convertimos la contraseña 'itern2026' a formato de bytes para cifrarla
            contrasena_bytes = "itern2026".encode("utf-8")  # Bytes de la contraseña 'itern2026'
            # Generamos una sal aleatoria con bcrypt para que el cifrado sea seguro e irreversible (OWASP A02)
            sal_segura = bcrypt.gensalt()  # Semilla aleatoria criptográfica
            # Generamos el hash criptográfico combinando la contraseña con la sal
            hash_bcrypt = bcrypt.hashpw(contrasena_bytes, sal_segura).decode("utf-8")  # Hash bcrypt generado
            # Escribimos la sentencia preparada para insertar el usuario admin con su hash
            sql_crear_admin = "INSERT INTO usuarios (nombre_usuario, contrasena_hash) VALUES (%s, %s)"  # Sentencia segura
            # Ejecutamos la inserción usando la tupla con 'admin' y el hash de 'itern2026'
            cursor.execute(sql_crear_admin, ("admin", hash_bcrypt))  # Inserción segura del usuario admin

        # Confirmamos los cambios de forma permanente en la base de datos
        conexion.commit()  # Guardado permanente en MySQL
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
    # Si ocurre algún error técnico, aplicamos OWASP A05 (Gestión Segura de Errores)
    except mysql.connector.Error as error_tecnico:  # Capturamos el error de MySQL
        # Imprimimos el detalle real únicamente en la consola del programador
        print(f"[LOG SEGURIDAD - OWASP A05] Error al inicializar tablas: {error_tecnico}")  # Log interno en consola
    # Cerramos la conexión con la base de datos al finalizar
    finally:  # Bloque final obligatorio
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos si está conectada
            # Cerramos la conexión de forma segura
            conexion.close()  # Desconectamos de MySQL

# ==============================================================================
# 2. VALIDACIÓN DE LOGIN CON BCRYPT Y SENTENCIAS PREPARADAS (OWASP A02 Y A03)
# ==============================================================================

# Definimos la función validar_login que autentica al usuario comparando el hash con bcrypt.checkpw()
def validar_login(usuario_ingresado, contrasena_ingresada):  # Recibe usuario y contraseña en texto
    # Nos aseguramos de que las tablas de la base de datos estén listas antes de validar
    inicializar_tablas_seguras()  # Inicializa tablas de forma preventiva
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si falló la conexión, devolvemos False para no permitir el acceso
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # Acceso denegado

    # Usamos try para realizar la autenticación de forma segura
    try:  # Inicio del bloque de verificación de credenciales
        # Creamos el cursor para ejecutar la consulta SQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la consulta preparada usando exclusivamente marcadores %s (OWASP A03: Cero concatenación)
        consulta_sql_usuario = "SELECT contrasena_hash FROM usuarios WHERE nombre_usuario = %s"  # Sentencia preparada
        # Creamos la tupla inmutable con el nombre de usuario ingresado
        parametro_usuario = (usuario_ingresado,)  # Tupla segura de un solo elemento
        # Ejecutamos la consulta preparada enviando por separado la consulta y la tupla de parámetros
        cursor.execute(consulta_sql_usuario, parametro_usuario)  # Búsqueda segura contra Inyección SQL
        # Obtenemos la fila encontrada (o None si el usuario no existe)
        registro_encontrado = cursor.fetchone()  # Obtenemos el registro del usuario
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor

        # Si no encontramos ningún registro con ese nombre de usuario
        if registro_encontrado is None:  # Si el usuario no existe
            # Devolvemos False negando el acceso
            return False  # Usuario no encontrado

        # Extraemos el hash guardado en la base de datos (columna contrasena_hash)
        hash_almacenado = registro_encontrado[0]  # Obtenemos el hash bcrypt de la base de datos
        # Convertimos la contraseña escrita por el usuario a formato de bytes UTF-8
        bytes_contrasena = contrasena_ingresada.encode("utf-8")  # Bytes de la contraseña ingresada
        # Convertimos el hash guardado en MySQL a formato de bytes UTF-8
        bytes_hash = hash_almacenado.encode("utf-8")  # Bytes del hash bcrypt

        # Usamos bcrypt.checkpw() para verificar criptográficamente si la contraseña coincide (OWASP A02)
        if bcrypt.checkpw(bytes_contrasena, bytes_hash):  # Comparación criptográfica segura
            # Si bcrypt confirma que coincide, devolvemos True permitiendo el acceso
            return True  # Contraseña correcta y validada con bcrypt
        # Si bcrypt indica que no coincide la contraseña
        else:  # Si la contraseña no coincide
            # Devolvemos False negando el acceso
            return False  # Contraseña incorrecta
    # Si ocurre un error técnico durante la autenticación, aplicamos OWASP A05
    except Exception as error_login:  # Capturamos cualquier error técnico
        # Imprimimos el detalle real únicamente en la consola interna
        print(f"[LOG SEGURIDAD - OWASP A05] Error al validar login: {error_login}")  # Log solo en consola
        # Mostramos una alerta genérica sin dar pistas a atacantes
        messagebox.showerror("Error", "Error interno al procesar el inicio de sesión.")  # Mensaje genérico seguro
        # Devolvemos False por seguridad
        return False  # Acceso denegado
    # Cerramos siempre la conexión en el bloque finally
    finally:  # Bloque de cierre seguro
        # Verificamos si la conexión sigue activa
        if conexion.is_connected():  # Comprobamos conexión
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# ==============================================================================
# 3. FUNCIONES DEL MÓDULO EQUIPOS (CRUD COMPLETO Y BÚSQUEDA PARAMETRIZADA)
# ==============================================================================

# Definimos la función registrar_equipo (y su alias registrar_pc) que guarda una computadora en MySQL
def registrar_equipo(area, procesador, ram, almacenamiento, estado):  # Recibe los 5 datos del equipo (sin codigo_pc manual)
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False
        return False  # No se pudo guardar

    # Usamos try para insertar el equipo de forma segura
    try:  # Inicio del bloque de inserción de equipo
        # Creamos el cursor para ejecutar las consultas
        cursor = conexion.cursor()  # Inicializamos el cursor

        # Buscamos el id_area correspondiente al área seleccionada (Laboratorio o Bodega)
        cursor.execute("SELECT id_area FROM areas WHERE nombre_area = %s OR nombre_area LIKE %s", (area, f"%{area}%"))  # Búsqueda preparada
        # Obtenemos el resultado de la búsqueda del área
        area_encontrada = cursor.fetchone()  # Obtenemos la fila del área
        # Si encontramos el área guardamos su id_area; si no encontramos asignamos None
        id_area_asignada = area_encontrada[0] if area_encontrada is not None else None  # ID de área asignado

        # Escribimos el INSERT INTO sin codigo_pc porque MySQL lo genera automáticamente con AUTO_INCREMENT
        sql_insertar_equipo = (  # Consulta parametrizada sin codigo_pc (generado por MySQL)
            "INSERT INTO computadoras "  # Indicamos la tabla computadoras
            "(id_area, procesador, memoria_ram, almacenamiento, estado_actual, detalles_falla) "  # Columnas a insertar
            "VALUES (%s, %s, %s, %s, %s, %s)"  # 6 marcadores %s para parámetros seguros
        )  # Fin de la consulta preparada
        # Creamos la tupla con los 6 valores que reemplazarán a los marcadores %s
        valores_equipo = (  # Empaquetamos los valores en la tupla
            id_area_asignada,  # Llave foránea del área correspondiente (Laboratorio o Bodega)
            procesador,  # Modelo del procesador
            ram,  # Capacidad de la memoria RAM
            almacenamiento,  # Capacidad de disco duro o SSD
            estado,  # Estado actual del equipo (Activa, etc.)
            "Sin observaciones"  # Detalles iniciales de falla
        )  # Fin de la tupla de valores
        # Ejecutamos la sentencia preparada pasando por separado la consulta y la tupla de valores
        cursor.execute(sql_insertar_equipo, valores_equipo)  # Inserción 100% libre de inyección SQL
        # Confirmamos los cambios permanentemente en la base de datos
        conexion.commit()  # Guardado permanente en MySQL
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que el equipo se registró con éxito
        return True  # Registro exitoso
    # Si ocurre un error técnico al guardar, aplicamos OWASP A05
    except mysql.connector.Error as error_equipo:  # Capturamos el error de MySQL
        # Imprimimos el error técnico únicamente en la consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al registrar equipo: {error_equipo}")  # Log interno
        # Mostramos un mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al guardar el equipo en la base de datos.")  # Mensaje genérico
        # Devolvemos False
        return False  # Registro fallido
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión de forma segura
            conexion.close()  # Desconectamos de MySQL

# Definimos el alias registrar_pc para compatibilidad didáctica directa
registrar_pc = registrar_equipo  # Alias directo solicitado para registrar computadoras

# Definimos la función actualizar_equipo (y su alias actualizar_pc) que modifica los datos de una PC por su codigo_pc
def actualizar_equipo(codigo_pc, area, procesador, ram, almacenamiento, estado):  # Recibe el codigo_pc y nuevos datos
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo actualizar

    # Usamos try para actualizar los datos de la PC de forma segura
    try:  # Inicio del bloque de actualización segura
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor

        # Buscamos el id_area correspondiente al área recibida (Laboratorio o Bodega)
        cursor.execute("SELECT id_area FROM areas WHERE nombre_area = %s OR nombre_area LIKE %s", (area, f"%{area}%"))  # Búsqueda preparada
        # Obtenemos la fila con el id de área
        area_encontrada = cursor.fetchone()  # Obtenemos el registro
        # Guardamos el id del área o None si no se halló
        id_area_asignada = area_encontrada[0] if area_encontrada is not None else None  # ID de área

        # Escribimos la sentencia preparada UPDATE usando marcadores %s (OWASP A03)
        sql_actualizar_equipo = (  # Sentencia preparada de actualización
            "UPDATE computadoras SET "  # Indicamos la tabla computadoras
            "id_area = %s, procesador = %s, "  # Parámetros 1 y 2
            "memoria_ram = %s, almacenamiento = %s, estado_actual = %s "  # Parámetros 3, 4 y 5
            "WHERE codigo_pc = %s"  # Condición WHERE por codigo_pc parametrizada (%s)
        )  # Fin de la sentencia UPDATE
        # Creamos la tupla con los 6 valores seguros en orden
        valores_actualizar = (  # Empaquetamos los valores
            id_area_asignada,  # Nuevo ID de área asignada (Laboratorio o Bodega)
            procesador,  # Nuevo procesador
            ram,  # Nueva memoria RAM
            almacenamiento,  # Nuevo almacenamiento
            estado,  # Nuevo estado actual
            int(codigo_pc)  # Código autoincrementable de la PC a actualizar
        )  # Fin de la tupla
        # Ejecutamos la actualización segura contra Inyección SQL
        cursor.execute(sql_actualizar_equipo, valores_actualizar)  # Ejecución de UPDATE con tupla
        # Confirmamos los cambios permanentemente en MySQL
        conexion.commit()  # Guardado permanente
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando éxito en la actualización
        return True  # Actualización exitosa
    # Si ocurre un error técnico al actualizar, aplicamos OWASP A05
    except mysql.connector.Error as error_actualizar_eq:  # Capturamos el error técnico
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al actualizar equipo: {error_actualizar_eq}")  # Log interno
        # Mostramos una alerta genérica y segura al usuario
        messagebox.showerror("Error", "Error interno al actualizar los datos del equipo.")  # Mensaje genérico
        # Devolvemos False
        return False  # Actualización fallida
    # Cerramos siempre la conexión en el bloque finally
    finally:  # Bloque final
        # Verificamos si la conexión continúa activa
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión de forma segura
            conexion.close()  # Desconectamos de MySQL

# Definimos el alias actualizar_pc para compatibilidad didáctica directa
actualizar_pc = actualizar_equipo  # Alias directo solicitado para actualizar computadoras

# Definimos la función eliminar_equipo (y su alias eliminar_pc) que borra una PC por su codigo_pc usando %s
def eliminar_equipo(codigo_pc):  # Recibe el codigo_pc numérico del equipo
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo eliminar

    # Usamos try para eliminar de forma segura
    try:  # Inicio del bloque de borrado seguro
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada DELETE usando el marcador seguro %s (OWASP A03)
        sql_eliminar_equipo = "DELETE FROM computadoras WHERE codigo_pc = %s"  # Sentencia DELETE parametrizada
        # Empaquetamos el codigo_pc en una tupla de un elemento
        parametro_codigo = (int(codigo_pc),)  # Tupla inmutable con el codigo_pc
        # Ejecutamos el borrado seguro pasando la consulta y la tupla de parámetros
        cursor.execute(sql_eliminar_equipo, parametro_codigo)  # Ejecución segura de eliminación
        # Confirmamos los cambios de forma permanente en la base de datos
        conexion.commit()  # Borrado definitivo en MySQL
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que la PC fue eliminada
        return True  # Eliminación exitosa
    # Si ocurre un error técnico al borrar, aplicamos OWASP A05
    except mysql.connector.Error as error_borrar_eq:  # Capturamos el error de MySQL
        # Imprimimos el detalle únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al eliminar equipo: {error_borrar_eq}")  # Log interno
        # Mostramos un mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al eliminar el equipo de la base de datos.")  # Mensaje genérico
        # Devolvemos False
        return False  # Eliminación fallida
    # Cerramos la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos el alias eliminar_pc para compatibilidad didáctica directa
eliminar_pc = eliminar_equipo  # Alias directo solicitado para eliminar computadoras

# Definimos la función obtener_equipos que consulta todas las computadoras para mostrarlas en el Treeview
def obtener_equipos():  # Inicio de la función de lectura de equipos
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía para no romper la interfaz
        return []  # Retorno seguro

    # Usamos try para consultar la tabla computadoras
    try:  # Inicio del bloque de consulta de equipos
        # Creamos el cursor para ejecutar la consulta
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Consulta SQL: la primera columna es el codigo_pc autoincrementable de MySQL (1, 2, 3...)
        sql_consultar_equipos = (  # Consulta de lectura con unión de tablas
            "SELECT c.codigo_pc, COALESCE(a.nombre_area, 'Laboratorio'), "  # Columna 1: codigo_pc autoincrementable
            "c.procesador, c.memoria_ram, c.almacenamiento, c.estado_actual "  # Procesador, RAM, Disco y Estado
            "FROM computadoras c "  # Desde la tabla computadoras
            "LEFT JOIN areas a ON c.id_area = a.id_area "  # Unimos con la tabla areas
            "ORDER BY c.codigo_pc DESC"  # Ordenamos por los códigos más recientes primero
        )  # Fin de la consulta de lectura
        # Ejecutamos la consulta de lectura
        cursor.execute(sql_consultar_equipos)  # Leemos los equipos en MySQL
        # Guardamos todos los registros obtenidos en una lista
        lista_equipos = cursor.fetchall()  # Filas de equipos
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de equipos lista para el Treeview
        return lista_equipos  # Retornamos las computadoras
    # Si ocurre un error técnico en la consulta, aplicamos OWASP A05
    except mysql.connector.Error as error_consulta_equipos:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al obtener equipos: {error_consulta_equipos}")  # Log interno
        # Mostramos una alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al cargar la lista de equipos.")  # Mensaje seguro
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión al finalizar
    finally:  # Bloque final
        # Verificamos si la conexión continúa activa
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función obtener_equipos_por_area que filtra PCs con sentencias preparadas WHERE id_area = %s
def obtener_equipos_por_area(id_area):  # Recibe el ID numérico del área (1 para Laboratorio, 2 para Bodega)
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía para no romper la interfaz
        return []  # Retorno seguro

    # Usamos try para consultar la tabla computadoras por área
    try:  # Inicio del bloque de consulta por área
        # Creamos el cursor para ejecutar la consulta
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada con marcador %s (OWASP A03: Cero concatenación)
        sql_por_area = (  # Consulta filtrada por área
            "SELECT c.codigo_pc, COALESCE(a.nombre_area, 'Laboratorio'), "  # Columna 1: codigo_pc autoincrementable
            "c.procesador, c.memoria_ram, c.almacenamiento, c.estado_actual "  # Datos del equipo
            "FROM computadoras c "  # Desde la tabla computadoras
            "LEFT JOIN areas a ON c.id_area = a.id_area "  # Unimos con la tabla areas
            "WHERE c.id_area = %s "  # Filtro por id_area parametrizado (%s)
            "ORDER BY c.codigo_pc DESC"  # Orden descendente por codigo_pc
        )  # Fin de la consulta preparada
        # Ejecutamos la consulta preparada con la tupla parametrizada
        cursor.execute(sql_por_area, (int(id_area),))  # Ejecución segura contra Inyección SQL
        # Obtenemos todos los registros de computadoras que pertenecen a esa área
        equipos_area = cursor.fetchall()  # Filas filtradas por área
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de equipos del área seleccionada
        return equipos_area  # Retornamos los equipos
    # Si ocurre un error técnico en la consulta, aplicamos OWASP A05
    except mysql.connector.Error as error_area_eq:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al filtrar equipos por área: {error_area_eq}")  # Log interno
        # Mostramos una alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al filtrar los equipos por área.")  # Mensaje seguro
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión al finalizar
    finally:  # Bloque final
        # Verificamos si la conexión continúa activa
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función buscar_equipos que filtra computadoras con sentencias preparadas LIKE %s (OWASP A03)
def buscar_equipos(filtro_busqueda):  # Recibe el texto que el usuario escribió en la caja de búsqueda
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía
        return []  # Retorno seguro

    # Usamos try para ejecutar la búsqueda segura
    try:  # Inicio del bloque de búsqueda
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la consulta preparada usando marcadores %s (Cero concatenación - OWASP A03)
        sql_buscar_equipos = (  # Sentencia de búsqueda con LIKE parametrizado
            "SELECT c.codigo_pc, COALESCE(a.nombre_area, 'Laboratorio'), "  # Columna 1: codigo_pc autoincrementable
            "c.procesador, c.memoria_ram, c.almacenamiento, c.estado_actual "  # Procesador, RAM, Disco y Estado
            "FROM computadoras c "  # Desde la tabla computadoras
            "LEFT JOIN areas a ON c.id_area = a.id_area "  # Unión con la tabla areas
            "WHERE c.codigo_pc LIKE %s OR c.procesador LIKE %s OR a.nombre_area LIKE %s OR c.memoria_ram LIKE %s OR c.almacenamiento LIKE %s OR c.estado_actual LIKE %s "  # Búsqueda segura
            "ORDER BY c.codigo_pc DESC"  # Ordenamos por los más recientes
        )  # Fin de la sentencia de búsqueda
        # Formateamos el patrón de búsqueda con los signos de porcentaje
        patron_parametro = f"%{filtro_busqueda}%"  # Patrón seguro para el comodín SQL
        # Creamos la tupla de 6 parámetros para los 6 marcadores %s
        tupla_parametros = (patron_parametro, patron_parametro, patron_parametro, patron_parametro, patron_parametro, patron_parametro)  # Parámetros seguros
        # Ejecutamos la consulta preparada pasando por separado la sentencia y los parámetros
        cursor.execute(sql_buscar_equipos, tupla_parametros)  # Búsqueda 100% blindada contra Inyección SQL
        # Obtenemos los registros que coincidieron con el filtro
        resultados_busqueda = cursor.fetchall()  # Filas filtradas
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de equipos encontrados
        return resultados_busqueda  # Retornamos resultados
    # Si ocurre un error técnico en la búsqueda, aplicamos OWASP A05
    except mysql.connector.Error as error_busqueda_eq:  # Capturamos el error
        # Imprimimos el detalle únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al buscar equipos: {error_busqueda_eq}")  # Log interno
        # Mostramos mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al realizar la búsqueda de equipos.")  # Mensaje genérico
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# ==============================================================================
# 4. FUNCIONES DEL MÓDULO ÁREAS (ÚNICAS 2 ÁREAS FÍSICAS: LABORATORIO Y BODEGA)
# ==============================================================================

# Definimos la función obtener_areas que consulta las únicas 2 áreas físicas para el Treeview
def obtener_areas():  # Inicio de la función de lectura de áreas oficiales
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no se pudo conectar, devolvemos una lista de respaldo
    if conexion is None:  # Verificamos si falló la conexión
        # Retornamos lista vacía
        return []  # Retorno seguro

    # Usamos try para consultar la tabla areas de forma segura
    try:  # Inicio del bloque de lectura de áreas
        # Creamos el cursor para ejecutar la consulta
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Consultamos únicamente las 2 áreas físicas oficiales (Laboratorio y Bodega)
        cursor.execute("SELECT id_area, nombre_area FROM areas WHERE nombre_area IN ('Laboratorio', 'Bodega') ORDER BY id_area ASC")  # Consulta de áreas
        # Guardamos las filas devueltas por MySQL
        filas_areas = cursor.fetchall()  # Obtenemos las áreas oficiales
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor

        # Diccionario con descripciones funcionales exactas para las 2 áreas físicas del instituto
        descripciones_areas = {  # Descripciones de las 2 únicas áreas físicas
            "Laboratorio": "Área principal para computadoras y prácticas de estudiantes en uso",  # Laboratorio (en uso)
            "Bodega": "Área de resguardo para repuestos, herramientas y equipos guardados"  # Bodega (guardados)
        }  # Fin del diccionario de descripciones

        # Creamos la lista que tendrá los 3 datos por área (id, nombre, descripción) para el Treeview
        lista_areas_resultado = []  # Lista preparada para la tabla
        # Recorremos cada fila de área que nos entregó MySQL
        for fila in filas_areas:  # Pasamos por cada área
            # Obtenemos el ID del área (posición 0)
            id_area = fila[0]  # Guardamos el ID
            # Obtenemos el nombre del área (posición 1)
            nombre_area = fila[1]  # Guardamos el nombre del área
            # Obtenemos la descripción específica para esa área
            descripcion_area = descripciones_areas.get(nombre_area, "Área física del centro de cómputo")  # Descripción
            # Agregamos la tupla con los 3 valores a la lista resultante
            lista_areas_resultado.append((id_area, nombre_area, descripcion_area))  # Agregamos el registro

        # Devolvemos la lista de áreas lista para la tabla Treeview
        return lista_areas_resultado  # Retornamos las 2 áreas oficiales
    # Si ocurre un error técnico al leer las áreas, aplicamos OWASP A05
    except mysql.connector.Error as error_areas:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al obtener áreas: {error_areas}")  # Log interno
        # Mostramos mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al cargar las áreas del laboratorio.")  # Mensaje genérico
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# ==============================================================================
# 5. FUNCIONES DEL MÓDULO INVENTARIO (CRUD COMPLETO Y BÚSQUEDA PARAMETRIZADA)
# ==============================================================================

# Definimos la función registrar_articulo_inventario que guarda una herramienta o periférico con %s
def registrar_articulo_inventario(nombre, categoria, cantidad, estado):  # Recibe los 4 datos del artículo
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False
        return False  # No se pudo guardar

    # Usamos try para insertar el artículo de forma segura
    try:  # Inicio del bloque de inserción de inventario
        # Creamos el cursor para ejecutar la orden SQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada INSERT INTO usando marcadores %s (OWASP A03: Cero concatenación)
        sql_insertar_inventario = (  # Sentencia preparada segura
            "INSERT INTO inventario_general (nombre_item, categoria, cantidad, estado) "  # Columnas de inventario_general
            "VALUES (%s, %s, %s, %s)"  # 4 marcadores seguros %s
        )  # Fin de la sentencia preparada
        # Creamos la tupla inmutable con los 4 valores del artículo
        valores_inventario = (nombre, categoria, int(cantidad), estado)  # Parámetros seguros en tupla
        # Ejecutamos la sentencia preparada pasando por separado la consulta y la tupla de valores
        cursor.execute(sql_insertar_inventario, valores_inventario)  # Inserción segura contra Inyección SQL
        # Confirmamos los cambios de forma permanente en la base de datos
        conexion.commit()  # Guardado permanente en MySQL
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que el artículo se registró con éxito
        return True  # Registro exitoso
    # Si ocurre un error técnico al registrar en inventario, aplicamos OWASP A05
    except Exception as error_inv:  # Capturamos cualquier error técnico
        # Imprimimos el detalle real únicamente en la consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al registrar artículo en inventario: {error_inv}")  # Log interno
        # Mostramos una alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al guardar el artículo en el inventario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Registro fallido
    # Cerramos la conexión con MySQL al finalizar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función actualizar_articulo_inventario que modifica un artículo de inventario por su ID
def actualizar_articulo_inventario(id_item, nombre, categoria, cantidad, estado):  # Recibe el ID y nuevos datos
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo actualizar

    # Usamos try para actualizar de forma segura
    try:  # Inicio del bloque de actualización segura
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada UPDATE usando marcadores seguros %s (OWASP A03)
        sql_actualizar_inv = (  # Sentencia preparada de actualización
            "UPDATE inventario_general SET "  # Indicamos la tabla inventario_general
            "nombre_item = %s, categoria = %s, cantidad = %s, estado = %s "  # Columnas a modificar
            "WHERE id_item = %s"  # Condición WHERE por ID parametrizada (%s)
        )  # Fin de la sentencia UPDATE
        # Creamos la tupla con los 5 valores seguros
        valores_actualizar = (nombre, categoria, int(cantidad), estado, int(id_item))  # Parámetros en tupla
        # Ejecutamos la actualización segura contra Inyección SQL
        cursor.execute(sql_actualizar_inv, valores_actualizar)  # UPDATE seguro con tupla
        # Confirmamos los cambios permanentemente en MySQL
        conexion.commit()  # Guardado permanente
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando éxito en la actualización
        return True  # Actualización exitosa
    # Si ocurre un error técnico al actualizar, aplicamos OWASP A05
    except Exception as error_act_inv:  # Capturamos el error técnico
        # Imprimimos el detalle real únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al actualizar artículo en inventario: {error_act_inv}")  # Log interno
        # Mostramos alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al actualizar el artículo en inventario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Actualización fallida
    # Cerramos la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión continúa abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función eliminar_articulo_inventario que borra un artículo por su ID usando %s
def eliminar_articulo_inventario(id_item):  # Recibe el ID numérico del artículo
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo eliminar

    # Usamos try para eliminar de forma segura
    try:  # Inicio del bloque de borrado seguro
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada DELETE usando el marcador seguro %s (OWASP A03)
        sql_eliminar_inv = "DELETE FROM inventario_general WHERE id_item = %s"  # Sentencia DELETE parametrizada
        # Empaquetamos el ID en una tupla de un solo elemento
        parametro_id = (int(id_item),)  # Tupla inmutable con el ID
        # Ejecutamos el borrado seguro pasando la sentencia y la tupla
        cursor.execute(sql_eliminar_inv, parametro_id)  # Ejecución segura de borrado
        # Confirmamos los cambios en MySQL
        conexion.commit()  # Borrado definitivo
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que el artículo fue eliminado
        return True  # Eliminación exitosa
    # Si ocurre un error técnico al borrar, aplicamos OWASP A05
    except Exception as error_eli_inv:  # Capturamos el error
        # Imprimimos el detalle real únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al eliminar artículo de inventario: {error_eli_inv}")  # Log interno
        # Mostramos mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al eliminar el artículo de inventario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Eliminación fallida
    # Cerramos la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función obtener_articulos_inventario que consulta todos los artículos de inventario_general
def obtener_articulos_inventario():  # Inicio de la función de lectura de inventario
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía para no romper la tabla
        return []  # Retorno seguro

    # Usamos try para consultar la tabla inventario_general
    try:  # Inicio del bloque de consulta de inventario
        # Creamos el cursor para ejecutar la consulta
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Consultamos id_item, nombre_item, categoria, cantidad y estado desde inventario_general
        cursor.execute("SELECT id_item, nombre_item, categoria, cantidad, estado FROM inventario_general ORDER BY id_item DESC")  # Lectura segura con ID
        # Guardamos todos los registros obtenidos en una lista
        lista_articulos = cursor.fetchall()  # Obtenemos los artículos
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de artículos lista para el Treeview
        return lista_articulos  # Retornamos los artículos
    # Si ocurre un error técnico en la consulta, aplicamos OWASP A05
    except mysql.connector.Error as error_consulta_inv:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al obtener artículos de inventario: {error_consulta_inv}")  # Log interno
        # Mostramos un mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al cargar los datos del inventario.")  # Mensaje genérico
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos siempre la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión continúa abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función buscar_articulos_inventario que filtra artículos con sentencias preparadas LIKE %s (OWASP A03)
def buscar_articulos_inventario(filtro_busqueda):  # Recibe el texto que el usuario escribió en la caja de búsqueda
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía
        return []  # Retorno seguro

    # Usamos try para ejecutar la búsqueda segura
    try:  # Inicio del bloque de búsqueda
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la consulta preparada usando marcadores %s (Cero concatenación - OWASP A03)
        sql_buscar_inv = (  # Sentencia preparada de búsqueda
            "SELECT id_item, nombre_item, categoria, cantidad, estado "  # Columnas a consultar
            "FROM inventario_general "  # Desde la tabla inventario_general
            "WHERE nombre_item LIKE %s OR categoria LIKE %s OR estado LIKE %s "  # Filtro seguro con LIKE
            "ORDER BY id_item DESC"  # Orden descendente por ID
        )  # Fin de la consulta preparada
        # Formateamos el patrón de búsqueda con los signos de porcentaje
        patron_parametro = f"%{filtro_busqueda}%"  # Patrón seguro para el comodín SQL
        # Creamos la tupla de 3 parámetros para los 3 marcadores %s
        tupla_parametros = (patron_parametro, patron_parametro, patron_parametro)  # Parámetros seguros
        # Ejecutamos la consulta preparada pasando por separado la sentencia y los parámetros
        cursor.execute(sql_buscar_inv, tupla_parametros)  # Búsqueda protegida contra Inyección SQL
        # Obtenemos los registros que coincidieron con el filtro
        resultados_inv = cursor.fetchall()  # Filas filtradas
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de artículos encontrados
        return resultados_inv  # Retornamos resultados
    # Si ocurre un error técnico en la búsqueda, aplicamos OWASP A05
    except mysql.connector.Error as error_busqueda_inv:  # Capturamos el error
        # Imprimimos el detalle únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al buscar en inventario: {error_busqueda_inv}")  # Log interno
        # Mostramos mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al realizar la búsqueda en inventario.")  # Mensaje genérico
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# ==============================================================================
# 6. FUNCIONES DEL MÓDULO CONTROL DE USUARIOS (BCRYPT Y OWASP A02 / A03)
# ==============================================================================

# Definimos la función registrar_usuario que guarda un nuevo usuario cifrando su contraseña con bcrypt
def registrar_usuario(nombre_usuario, contrasena_plana):  # Recibe el nombre de usuario y su clave en texto plano
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo registrar

    # Usamos try para registrar el usuario de forma segura
    try:  # Inicio del bloque de registro de usuario
        # Convertimos la contraseña en texto plano a formato de bytes UTF-8 para bcrypt (OWASP A02)
        contrasena_bytes = contrasena_plana.encode("utf-8")  # Bytes de la clave
        # Generamos una sal criptográfica aleatoria con bcrypt
        sal_segura = bcrypt.gensalt()  # Semilla aleatoria única
        # Ciframos la contraseña combinándola con la sal aleatoria
        hash_cifrado = bcrypt.hashpw(contrasena_bytes, sal_segura).decode("utf-8")  # Hash bcrypt generado

        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada INSERT INTO con marcadores %s (OWASP A03: Cero concatenación)
        sql_insertar_usuario = "INSERT INTO usuarios (nombre_usuario, contrasena_hash) VALUES (%s, %s)"  # Sentencia preparada
        # Creamos la tupla inmutable con el nombre y el hash seguro
        tupla_usuario = (nombre_usuario, hash_cifrado)  # Parámetros seguros
        # Ejecutamos la inserción segura en la base de datos
        cursor.execute(sql_insertar_usuario, tupla_usuario)  # Inserción con tupla contra Inyección SQL
        # Confirmamos los cambios de forma permanente en MySQL
        conexion.commit()  # Guardado permanente
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que el usuario se registró con éxito
        return True  # Registro exitoso
    # Si ocurre un error técnico al registrar el usuario, aplicamos OWASP A05
    except mysql.connector.Error as error_usuario:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al registrar usuario: {error_usuario}")  # Log interno
        # Mostramos un mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al registrar el nuevo usuario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Registro fallido
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función obtener_usuarios que consulta la lista de usuarios (solo ID y Usuario, nunca el hash)
def obtener_usuarios():  # Inicio de la función de lectura de usuarios
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía
        return []  # Retorno seguro

    # Usamos try para consultar la tabla usuarios
    try:  # Inicio del bloque de consulta de usuarios
        # Creamos el cursor para ejecutar la consulta
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Consultamos id_usuario y nombre_usuario (NUNCA seleccionamos la columna contrasena_hash por seguridad)
        cursor.execute("SELECT id_usuario, nombre_usuario FROM usuarios ORDER BY id_usuario ASC")  # Consulta segura
        # Guardamos todos los registros de usuarios obtenidos
        lista_usuarios = cursor.fetchall()  # Filas de usuarios
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de usuarios
        return lista_usuarios  # Retornamos usuarios
    # Si ocurre un error técnico en la consulta, aplicamos OWASP A05
    except mysql.connector.Error as error_usuarios:  # Capturamos el error
        # Imprimimos el error técnico únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al obtener usuarios: {error_usuarios}")  # Log interno
        # Mostramos una alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al cargar la lista de usuarios.")  # Mensaje seguro
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos siempre la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión continúa activa
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función buscar_usuarios que filtra usuarios por nombre con sentencias preparadas LIKE %s
def buscar_usuarios(filtro_busqueda):  # Recibe el texto ingresado en la caja de búsqueda
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si no hay conexión disponible, devolvemos una lista vacía
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos lista vacía
        return []  # Retorno seguro

    # Usamos try para ejecutar la búsqueda segura
    try:  # Inicio del bloque de búsqueda
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada usando marcadores %s (OWASP A03)
        sql_buscar_usuarios = "SELECT id_usuario, nombre_usuario FROM usuarios WHERE nombre_usuario LIKE %s ORDER BY id_usuario ASC"  # Sentencia segura
        # Formateamos el parámetro de búsqueda con los signos de porcentaje
        patron_parametro = f"%{filtro_busqueda}%"  # Patrón seguro
        # Ejecutamos la consulta preparada con la tupla parametrizada
        cursor.execute(sql_buscar_usuarios, (patron_parametro,))  # Búsqueda protegida
        # Obtenemos los registros encontrados
        resultados_usuarios = cursor.fetchall()  # Filas filtradas
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos la lista de usuarios encontrados
        return resultados_usuarios  # Retornamos resultados
    # Si ocurre un error técnico en la búsqueda, aplicamos OWASP A05
    except mysql.connector.Error as error_buscar_user:  # Capturamos el error
        # Imprimimos el detalle únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al buscar usuarios: {error_buscar_user}")  # Log interno
        # Mostramos mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al buscar usuarios.")  # Mensaje genérico
        # Devolvemos lista vacía
        return []  # Retorno seguro
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función eliminar_usuario que borra una cuenta de usuario por su ID usando %s
def eliminar_usuario(id_usuario):  # Recibe el ID numérico del usuario
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo eliminar

    # Usamos try para eliminar el usuario de forma segura
    try:  # Inicio del bloque de eliminación de usuario
        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada DELETE usando el marcador %s (OWASP A03)
        sql_eliminar_usuario = "DELETE FROM usuarios WHERE id_usuario = %s"  # Sentencia DELETE parametrizada
        # Empaquetamos el ID en una tupla de un solo elemento
        parametro_id = (int(id_usuario),)  # Tupla inmutable con el ID
        # Ejecutamos el borrado seguro pasando la sentencia y la tupla
        cursor.execute(sql_eliminar_usuario, parametro_id)  # Ejecución de eliminación
        # Confirmamos los cambios de forma permanente en MySQL
        conexion.commit()  # Borrado definitivo en la base de datos
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que el usuario fue eliminado
        return True  # Eliminación exitosa
    # Si ocurre un error técnico al borrar el usuario, aplicamos OWASP A05
    except mysql.connector.Error as error_del_user:  # Capturamos el error de MySQL
        # Imprimimos el detalle real únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al eliminar usuario: {error_del_user}")  # Log interno
        # Mostramos un mensaje genérico al usuario
        messagebox.showerror("Error", "Error interno al eliminar el usuario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Eliminación fallida
    # Cerramos la conexión al terminar
    finally:  # Bloque final
        # Verificamos si la conexión sigue abierta
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL

# Definimos la función cambiar_contrasena_usuario que actualiza la clave cifrando la nueva contraseña con bcrypt
def cambiar_contrasena_usuario(id_usuario, nueva_contrasena_plana):  # Recibe el ID del usuario y la nueva clave
    # Abrimos la conexión con la base de datos MySQL
    conexion = obtener_conexion()  # Conectamos con MySQL
    # Si la conexión falló, salimos devolviendo False
    if conexion is None:  # Verificamos si no hay conexión
        # Retornamos False por seguridad
        return False  # No se pudo actualizar

    # Usamos try para cifrar y actualizar la contraseña de forma segura
    try:  # Inicio del bloque de actualización de contraseña
        # Convertimos la nueva contraseña en texto plano a formato de bytes UTF-8 para bcrypt (OWASP A02)
        nueva_bytes = nueva_contrasena_plana.encode("utf-8")  # Bytes de la nueva clave
        # Generamos una nueva sal criptográfica aleatoria con bcrypt
        sal_segura = bcrypt.gensalt()  # Semilla aleatoria única
        # Generamos el nuevo hash criptográfico con bcrypt
        nuevo_hash = bcrypt.hashpw(nueva_bytes, sal_segura).decode("utf-8")  # Nuevo hash bcrypt generado

        # Creamos el cursor para comunicarse con MySQL
        cursor = conexion.cursor()  # Inicializamos el cursor
        # Escribimos la sentencia preparada UPDATE usando marcadores %s (OWASP A03: Cero concatenación)
        sql_cambiar_clave = "UPDATE usuarios SET contrasena_hash = %s WHERE id_usuario = %s"  # Sentencia UPDATE parametrizada
        # Empaquetamos el nuevo hash y el id del usuario en la tupla
        valores_clave = (nuevo_hash, int(id_usuario))  # Parámetros seguros
        # Ejecutamos la actualización segura en la base de datos
        cursor.execute(sql_cambiar_clave, valores_clave)  # UPDATE seguro con tupla
        # Confirmamos los cambios de forma permanente en MySQL
        conexion.commit()  # Guardado permanente
        # Cerramos el cursor
        cursor.close()  # Liberamos el cursor
        # Devolvemos True indicando que la contraseña se actualizó con éxito
        return True  # Cambio exitoso
    # Si ocurre un error técnico al cambiar la contraseña, aplicamos OWASP A05
    except mysql.connector.Error as error_cambiar_clave:  # Capturamos el error
        # Imprimimos el detalle real únicamente en consola
        print(f"[LOG SEGURIDAD - OWASP A05] Error al cambiar contraseña: {error_cambiar_clave}")  # Log interno
        # Mostramos una alerta genérica al usuario
        messagebox.showerror("Error", "Error interno al actualizar la contraseña del usuario.")  # Mensaje genérico
        # Devolvemos False
        return False  # Cambio fallido
    # Cerramos la conexión con MySQL al terminar
    finally:  # Bloque final
        # Verificamos si la conexión continúa activa
        if conexion.is_connected():  # Comprobamos estado
            # Cerramos la conexión
            conexion.close()  # Desconectamos de MySQL
