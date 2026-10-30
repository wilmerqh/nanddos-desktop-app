# ==============================================================================
# SCRIPT AUXILIAR DE SEGURIDAD (OWASP A02): generar_hash.py
# Genera el hash criptográfico con bcrypt para la contraseña 'itern2026'
# ==============================================================================

# Importamos la librería bcrypt para cifrar contraseñas de forma segura
import bcrypt  # Librería estándar en ciberseguridad para proteger contraseñas

# Guardamos en una variable la contraseña original que queremos proteger
contrasena_plana = "itern2026"  # Contraseña en texto plano solicitada

# Convertimos el texto de la contraseña a formato de bytes (UTF-8) porque bcrypt trabaja con bytes
contrasena_en_bytes = contrasena_plana.encode("utf-8")  # Transformamos el texto a secuencia de bytes

# Generamos una "sal" (salt) aleatoria para que el hash sea único y resistente a ataques de diccionario
sal_aleatoria = bcrypt.gensalt()  # Crea una semilla aleatoria de seguridad

# Calculamos el hash criptográfico combinando los bytes de la contraseña con la sal aleatoria
hash_generado_bytes = bcrypt.hashpw(contrasena_en_bytes, sal_aleatoria)  # Ciframos la contraseña con bcrypt

# Convertimos el hash resultante de bytes a texto normal (UTF-8) para poder guardarlo en MySQL
hash_para_mysql = hash_generado_bytes.decode("utf-8")  # Texto del hash listo para copiarse a la base de datos

# Imprimimos en la consola una línea separadora para que se lea con claridad
print("=" * 75)  # Línea decorativa en consola
# Mostramos la contraseña original que se cifró
print(f"Contraseña original : {contrasena_plana}")  # Muestra: itern2026
# Mostramos el hash bcrypt generado que empieza con $2b$12$...
print(f"Hash bcrypt generado: {hash_para_mysql}")  # Muestra el hash seguro para insertar en MySQL
# Imprimimos otra línea separadora
print("=" * 75)  # Línea decorativa en consola
# Mostramos también la consulta SQL lista para pegar en phpMyAdmin o consola MySQL
print("Comando SQL para insertar el usuario administrador en MySQL:")  # Guía para el estudiante
# Imprimimos la sentencia INSERT con el hash generado
print(f"INSERT INTO usuarios (usuario, contrasena) VALUES ('admin', '{hash_para_mysql}');")  # Consulta SQL lista
# Imprimimos línea final
print("=" * 75)  # Cierre visual en consola
