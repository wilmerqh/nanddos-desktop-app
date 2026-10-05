# Manual de Usuario — System NANDDOS

**Sistema de Soporte Técnico y Punto de Venta**
**Versión del sistema:** 1.1
**Dirigido a:** Administradores y Técnicos del taller

---

## Contenido

1. [Introducción](#1-introducción)
2. [Inicio de Sesión y Seguridad](#2-inicio-de-sesión-y-seguridad)
3. [Gestión de Inventario](#3-gestión-de-inventario)
4. [Registro y Edición de Equipos](#4-registro-y-edición-de-equipos)
5. [Módulo de Entregas (Punto de Venta)](#5-módulo-de-entregas-punto-de-venta)
6. [Preguntas Frecuentes](#6-preguntas-frecuentes)

---

## 1. Introducción

### 1.1 ¿Qué es System NANDDOS?

**System NANDDOS** es el sistema oficial de gestión del taller técnico. Reúne en una sola aplicación todo el ciclo de vida de un equipo, desde que el cliente lo deja en el mostrador hasta que se le entrega reparado y se le cobra.

### 1.2 Propósito del sistema

El sistema permite:

- **Registrar clientes y equipos** que ingresan al taller, asignándoles un código único (por ejemplo, `LP-0001`).
- **Dar seguimiento** al estado de cada equipo (en diagnóstico, en reparación, entregado, etc.).
- **Documentar** por separado el problema que reporta el cliente y el diagnóstico real que determina el técnico.
- **Controlar el inventario** de repuestos y productos, con descuento automático de existencias.
- **Cobrar y facturar** desde el módulo de Entregas, generando un comprobante en **PDF** para el cliente.
- **Proteger la información sensible** mediante roles y permisos.

### 1.3 Pantalla principal

Al ingresar verá la ventana principal, dividida en dos zonas:

- **Barra lateral izquierda (menú):** contiene los accesos a cada módulo. Solo aparecen los módulos para los que su usuario tiene permiso.
- **Área central:** aquí se abre el módulo que seleccione.

Las opciones del menú son:

| Opción del menú | Descripción |
|---|---|
| **Inicio** | Panel con el resumen general del taller. |
| **Usuarios** | Administración de las cuentas de los empleados *(solo Administrador Global)*. |
| **Cargos** | Administración de cargos y permisos *(solo Administrador Global)*. |
| **Registrar Equipo** | Ingreso de clientes y equipos nuevos. |
| **Lista de Equipos** | Consulta, edición y seguimiento de equipos. |
| **Clientes** | Consulta de clientes registrados. |
| **Entrega** | Punto de venta: cobro y entrega de equipos. |
| **Inventario** | Control de repuestos y productos. |
| **Cerrar Sesión** | Botón rojo en la parte inferior del menú para salir del sistema. |

En la parte inferior del menú se muestra el nombre del **usuario que tiene la sesión abierta**.

---

## 2. Inicio de Sesión y Seguridad

### 2.1 Cómo iniciar sesión

1. Abra la aplicación **System NANDDOS**.
2. En la pantalla de inicio de sesión, escriba su **usuario** y su **contraseña**.
3. Presione el botón para ingresar.
4. Si los datos son correctos, se abrirá la pantalla principal en el módulo **Inicio**.

> **Importante:** Su usuario y contraseña son personales e intransferibles. Todo lo que se haga en el sistema queda asociado a la cuenta con la que se inició sesión.

### 2.2 Cómo cerrar sesión

1. Haga clic en el botón rojo **Cerrar Sesión**, ubicado en la parte inferior del menú lateral.
2. El sistema le preguntará: **"¿Estás seguro que deseas cerrar sesión?"**
3. Seleccione **Sí**. La aplicación se reiniciará y volverá a la pantalla de inicio de sesión.

> **Recomendación:** Cierre siempre su sesión al terminar su turno o al alejarse del equipo, para que nadie pueda operar con su usuario.

### 2.3 Roles del sistema

System NANDDOS trabaja con dos niveles de acceso:

#### Administrador Global

Es el nivel más alto del sistema. El Administrador Global:

- Tiene **acceso a todos los módulos**, sin restricciones.
- Es el **único** que ve los módulos **Usuarios** y **Cargos**, desde donde crea cuentas y define qué puede hacer cada cargo.
- Es el **único** que puede ver el **Precio de Costo** de los repuestos.
- Es el **único** que puede **modificar el Problema reportado por el cliente** una vez registrado el equipo.

#### Técnico (y demás cargos del personal)

Son los usuarios operativos del taller. Su acceso depende de los **permisos asignados a su cargo** por el Administrador Global. Según esos permisos, un técnico puede:

- Registrar equipos nuevos.
- Ver la lista de equipos.
- Editar equipos y cambiar su estado.
- Eliminar equipos.
- Consultar clientes.
- Generar entregas (cobrar).
- Consultar el inventario.

Si un botón u opción del menú **no aparece** en su pantalla, significa que su cargo no tiene ese permiso. En ese caso, solicítelo al Administrador Global.

### 2.4 Resumen de diferencias

| Acción | Administrador Global | Técnico |
|---|:---:|:---:|
| Acceder a Usuarios y Cargos | Permitido | Denegado |
| Ver el Precio de Costo de repuestos | Permitido | Denegado |
| Modificar el Problema reportado por el cliente | Permitido | Denegado |
| Escribir y editar el Diagnóstico Técnico | Permitido | Permitido *(si tiene permiso)* |
| Registrar equipos, cobrar entregas, ver inventario | Permitido | Permitido *(según su cargo)* |

---

## 3. Gestión de Inventario

### 3.1 Acceder al inventario

Haga clic en **Inventario** en el menú lateral. Se abrirá la pantalla **Inventario de Repuestos**, que muestra en una tabla todos los repuestos y productos registrados, con datos como su código, nombre, categoría, existencias (stock) y precio de venta.

### 3.2 Buscar un repuesto

1. Escriba el **código** o el **nombre** del repuesto en el cuadro de búsqueda (*"Buscar por código o nombre..."*).
2. Presione **Enter** o haga clic en **Buscar**.
3. La tabla mostrará únicamente los resultados que coincidan.

Para volver a ver todo el inventario, borre el texto de búsqueda y presione **Buscar** nuevamente.

### 3.3 Agregar un repuesto nuevo

1. Haga clic en **Agregar Nuevo**.
2. Complete los datos en la ventana que aparece (código, nombre, categoría, existencias, precio de venta, etc.).
3. Guarde los cambios. El repuesto aparecerá de inmediato en la tabla.

### 3.4 Editar un repuesto

1. Seleccione el repuesto en la tabla haciendo clic sobre su fila.
2. Haga clic en **Editar**.
3. Modifique los datos necesarios y guarde.

> Si hace clic en **Editar** sin haber seleccionado una fila, el sistema le mostrará el aviso: *"Seleccione un repuesto de la tabla para editarlo."*

### 3.5 Eliminar un repuesto

1. Seleccione el repuesto en la tabla.
2. Haga clic en **Eliminar** y confirme la acción.

### 3.6 Confidencialidad del Precio de Costo

> **Importante: Información confidencial**
>
> El **Precio de Costo** (lo que el taller paga por cada repuesto) es información **estrictamente confidencial** y **solo es visible para el Administrador Global**.

Esto significa que:

- Si usted **no** es Administrador Global, la columna **Precio de Costo** **no aparecerá** en la tabla de inventario.
- En la ventana de **Agregar Nuevo** o **Editar**, el campo **Precio de Costo** también estará **oculto**.
- Los técnicos sí pueden ver el **Precio de Venta**, que es el precio que se cobra al cliente.

Esta restricción es intencional y no se trata de un error del sistema.

---

## 4. Registro y Edición de Equipos

### 4.1 Registrar un equipo nuevo

Haga clic en **Registrar Equipo** en el menú lateral. El registro se realiza en un flujo guiado por pasos.

#### Paso 1 — Buscar Cliente

1. En el cuadro **"Nombre o teléfono"**, escriba el nombre o el número de teléfono del cliente.
   - **Consejo:** escriba solo **un nombre y un apellido** para obtener resultados más precisos.
2. Presione **Enter** o haga clic en **Buscar**.

#### Resultado de búsqueda

Pueden ocurrir dos situaciones:

- **El cliente ya existe:** se mostrarán sus datos (nombres, apellidos, teléfono y email). Si hay varias coincidencias, aparecerá una tabla para que elija al cliente correcto. Luego haga clic en **Registrar Nuevo Equipo** (o haga doble clic sobre el cliente en la tabla).
- **El cliente no existe:** haga clic en **Nuevo Cliente** para registrarlo.

#### Paso 2 — Registrar Equipo

Se mostrará el formulario de registro, dividido en dos secciones:

**Sección izquierda — Datos del Cliente**
- Si el cliente es nuevo, complete: **Nombres**, **Apellidos**, **Teléfono** y **Email**.
- Si el cliente ya existía, se mostrará un resumen del **Cliente seleccionado**.

**Sección derecha — Datos del Equipo**

| Campo | Qué debe escribir |
|---|---|
| **Tipo de equipo** | Seleccione de la lista el tipo (laptop, celular, etc.). Esto determina el prefijo del código del equipo. |
| **Fecha de ingreso** | Fecha en que el cliente deja el equipo. |
| **Marca** | Marca del equipo. |
| **Modelo** | Modelo del equipo. |
| **Serial** | Número de serie del equipo. |
| **Problema** | La falla **tal como la describe el cliente**. Escríbala con sus palabras, sin interpretarla. |
| **Repuestos utilizados** | *(Opcional)* Repuestos que ya se sabe que se usarán. |

**Para agregar repuestos al registro:**
1. Seleccione el repuesto en la lista desplegable.
2. Indique la cantidad.
3. Haga clic en el botón azul **(+)** para agregarlo a la tabla.
4. Para quitar uno, selecciónelo en la tabla y haga clic en el botón rojo **(-)** (cada clic resta una unidad).

**Para finalizar:**
- Haga clic en **Guardar Equipo**. El sistema asignará automáticamente un **código único** al equipo (por ejemplo, `LP-0007`) y lo dejará en estado **"En diagnóstico"**.
- Si desea abandonar el registro, haga clic en **Cancelar**.

> **Importante:** Anote o entregue al cliente el **código del equipo**. Será necesario para localizarlo y entregarlo más adelante.

### 4.2 Problema vs. Diagnóstico Técnico

> **Concepto Crítico — Lea esta sección con atención**

System NANDDOS separa estrictamente dos descripciones distintas de la falla de un equipo:

#### Problema (reportado por el cliente)

- Es **lo que el cliente dice** que le pasa a su equipo al momento de dejarlo.
  *Ejemplo: "La computadora no enciende."*
- Se escribe **una sola vez**, al registrar el equipo.
- Funciona como **constancia de lo que el cliente reportó**, por lo que debe conservarse tal como fue registrado.
- **Los técnicos NO pueden modificarlo.** En la ventana de edición aparece como campo de **solo lectura**.
- **Solo el Administrador Global** puede corregirlo (por ejemplo, si hubo un error de escritura al registrarlo).

#### Diagnóstico Técnico (determinado por el taller)

- Es **la falla real** que encuentra el técnico después de revisar el equipo.
  *Ejemplo: "Fuente de poder dañada; se reemplazó el conector de carga."*
- **Es editable por los técnicos** con permiso de edición, tantas veces como sea necesario durante la reparación.
- Si nunca se llena, en el comprobante de entrega aparecerá el texto **"Sin diagnóstico registrado"**.

#### ¿Por qué se separan?

- Protege al taller y al cliente: queda registrado **qué se reportó** y **qué se encontró realmente**.
- Ambos textos se imprimen en el **comprobante PDF de entrega**, de modo que el cliente puede ver con claridad la diferencia entre lo que reportó y lo que se reparó.

| | Problema | Diagnóstico Técnico |
|---|---|---|
| **¿Quién lo origina?** | El cliente | El técnico |
| **¿Cuándo se escribe?** | Al registrar el equipo | Durante la revisión o reparación |
| **¿Lo puede editar un técnico?** | No (solo lectura) | Sí |
| **¿Lo puede editar el Administrador Global?** | Sí | Sí |
| **¿Aparece en el PDF de entrega?** | Sí | Sí |

### 4.3 Consultar la Lista de Equipos

Haga clic en **Lista de Equipos** en el menú lateral. Verá una tabla con todos los equipos del taller y las columnas **Código**, **Cliente**, **Equipo**, **Problema**, **Estado** y **Fecha**.

**Para filtrar la lista:**
1. Escriba en el cuadro de búsqueda un código, nombre de cliente, teléfono, marca, modelo o parte del problema.
2. Si lo desea, elija un **estado** en la lista desplegable para ver solo los equipos en ese estado.
3. Haga clic en **Buscar**.

**Botones disponibles** *(según los permisos de su cargo)*:

| Botón | Función |
|---|---|
| **Buscar** | Aplica los filtros de búsqueda. |
| **Cambiar Estado** | Cambia el estado del equipo seleccionado (por ejemplo, de "En diagnóstico" a "En reparación"). |
| **Ver Detalles** | Muestra toda la información del equipo: cliente, datos del equipo, problema reportado, repuestos, diagnóstico técnico, fecha de entrega y costo. |
| **Editar** | Abre la ventana de edición del equipo. |
| **Eliminar** | Elimina el equipo seleccionado. |

### 4.4 Editar un equipo y escribir el Diagnóstico Técnico

1. En la **Lista de Equipos**, seleccione el equipo haciendo clic en su fila.
2. Haga clic en **Editar**. Se abrirá la ventana **"Editar equipo [código]"**.
3. Podrá modificar:
   - **Marca**, **Modelo** y **Serial**.
   - **Diagnóstico Técnico:** escriba aquí la falla real encontrada y el trabajo realizado.
   - **Repuestos utilizados:** agregue repuestos con el botón azul **(+)** o quítelos con el botón rojo **(-)**.
4. El campo **Problema Reportado** aparecerá **bloqueado** (solo lectura), salvo que usted sea Administrador Global.
5. Haga clic en **Guardar** para confirmar, o en **Cancelar** para descartar los cambios.

> **Atención con los repuestos en la edición:** al agregar un repuesto con el botón **(+)**, la cantidad **se descuenta del inventario en ese mismo momento**. Al quitarlo con el botón **(-)**, la unidad **se devuelve al inventario**. Si el sistema indica *"Stock insuficiente"*, no hay existencias suficientes del repuesto.

### 4.5 Botón de copiado rápido

En la **Lista de Equipos**, la **primera columna** de cada fila contiene un pequeño botón con el ícono de copiar.

**¿Para qué sirve?**
Permite copiar el **código del equipo** de esa fila con un solo clic, sin necesidad de escribirlo a mano. Esto evita errores de digitación.

**Cómo usarlo:**
1. Localice el equipo en la tabla.
2. Haga clic en el botón de copiar de su fila.
3. El sistema mostrará el mensaje: *"Código [código] copiado al portapapeles."*
4. Vaya al lugar donde necesita el código (por ejemplo, el buscador del módulo **Entrega**, un mensaje al cliente o un correo) y péguelo con **Ctrl + V**.

> **Uso más común:** copiar el código en la Lista de Equipos y pegarlo directamente en el buscador del módulo **Entrega** al momento de cobrar.

---

## 5. Módulo de Entregas (Punto de Venta)

El módulo **Entrega** es el punto de venta del taller. Desde aquí se cobra al cliente, se marca el equipo como entregado y se genera el comprobante en PDF.

Haga clic en **Entrega** en el menú lateral para abrirlo. La pantalla tiene cuatro zonas:

1. **Buscador de equipo** (parte superior).
2. **Equipo encontrado** (datos del cliente y del equipo).
3. **Datos de entrega y facturación** (repuestos, extras, costos y total).
4. **Resumen** (vista previa de la entrega) y el botón **Generar entrega**.

### 5.1 Buscar un equipo

1. En el cuadro de búsqueda, escriba el código del equipo (por ejemplo, `LP-0001`) o péguelo con **Ctrl + V** si lo copió con el botón de copiado rápido.
   - No se preocupe por las mayúsculas: el sistema las convierte automáticamente.
2. Presione **Enter** o haga clic en **Buscar**.

Si el código existe, el sistema completará **automáticamente** la sección **Equipo encontrado** con:

- **Cliente**, **Teléfono** y **Email**.
- **Equipo** (tipo, marca y modelo).
- **Problema** reportado por el cliente (solo lectura).

Si el código no existe, aparecerá el aviso: *"No se encontró un equipo con ese código."*

> **Equipo ya entregado:** si el equipo buscado ya fue entregado anteriormente, el sistema **no permitirá cobrarlo de nuevo** (para evitar entregas duplicadas) y le ofrecerá **regenerar el comprobante PDF** existente.

### 5.2 Precio de repuestos (automático)

Al encontrar el equipo, el sistema también completa automáticamente:

- **Repuestos usados:** la lista de repuestos que se registraron para ese equipo (por ejemplo, *"2x Memoria RAM, 1x Disco SSD"*).
- **Precio Repuestos:** el **total calculado automáticamente** según el precio de venta de cada repuesto en el inventario y la cantidad utilizada.

Este campo es de **solo lectura**: no necesita escribir nada. Si el precio no es correcto, revise los repuestos registrados en el equipo (módulo **Lista de Equipos → Editar**) o el precio de venta en **Inventario**.

### 5.3 Agregar el costo de mano de obra (Servicio)

1. Haga clic en el campo **Costo Servicio**.
2. Escriba el monto de la mano de obra (por ejemplo, `150.00`).
   - El campo solo acepta **números** y **un punto decimal**.
3. El **TOTAL A COBRAR** se actualizará **automáticamente** mientras escribe.

### 5.4 Agregar productos extra (tabla de Extras)

La tabla **Extras Agregados** sirve para cobrar productos adicionales que el cliente se lleva en el momento de la entrega y que no formaban parte de la reparación (por ejemplo, un cargador, un protector de pantalla, un mouse o una memoria USB).

**Para agregar un extra:**
1. Haga clic en el botón azul **+ Agregar Extra** (arriba a la derecha de la tabla).
2. En la ventana que aparece, **seleccione el producto** del inventario y la **cantidad**.
3. Confirme. El producto aparecerá en la tabla con su **Nombre**, **Cant.** (cantidad), **Precio** y **Subtotal**.
4. El campo **Costo Extras** y el **TOTAL A COBRAR** se actualizarán automáticamente.

**Para quitar un extra:**
- Haga clic en el botón rojo **(-)** al final de la fila del producto que desea eliminar. La fila desaparecerá y los totales se recalcularán al instante.

**¿Cómo afecta al inventario?**

> **Descuento de inventario**
>
> Los productos de la tabla de Extras **se descuentan del inventario únicamente cuando se confirma la entrega** con el botón **Generar entrega**.
>
> - Mientras no confirme la entrega, puede agregar y quitar extras libremente **sin afectar el inventario**.
> - Si cancela la entrega, **no se descuenta nada**.
> - Al confirmar la entrega, el sistema resta del stock la cantidad exacta de cada producto extra.

### 5.5 Fecha de entrega y total

- **Fecha Entrega:** por defecto muestra la fecha de hoy. Puede cambiarla si es necesario.
- **TOTAL A COBRAR:** se calcula automáticamente con la fórmula:

```
TOTAL A COBRAR = Precio Repuestos + Costo Servicio + Costo Extras
```

Este campo no se puede editar manualmente; siempre refleja la suma de los tres conceptos.

### 5.6 Generar la entrega y el comprobante PDF

1. Revise que todos los datos y el **TOTAL A COBRAR** sean correctos.
2. Haga clic en el botón **Generar entrega** (esquina inferior derecha).
3. El sistema mostrará un **resumen** de la entrega y preguntará **"¿Confirmar entrega?"**.
   - Seleccione **Sí** para continuar.
   - Seleccione **No** para regresar y corregir; en ese caso no se guarda nada.
4. Al confirmar, el sistema realiza **automáticamente** las siguientes acciones:
   - Asigna un **código de entrega** único.
   - Genera el **comprobante en PDF**.
   - Guarda la entrega en el historial.
   - Cambia el estado del equipo a **"Entregado"**.
   - **Descuenta del inventario** los productos extra.

> **Seguridad de la operación:** todas estas acciones se realizan como una sola operación. Si ocurre algún error a mitad del proceso, el sistema **revierte todo** automáticamente, de modo que nunca quedan entregas a medias ni descuentos de inventario incorrectos.

### 5.7 Contenido del comprobante PDF

El comprobante de entrega incluye:

- Logotipo del taller.
- **Código de entrega** y **fecha**.
- **Equipo** (código del equipo).
- **Cliente**, **Teléfono** y **Email**.
- **Descripción del equipo**.
- **Problema Reportado por el Cliente**.
- **Estado** (Entregado).
- **Diagnóstico Técnico** *(si no se registró, aparece "Sin diagnóstico registrado")*.
- **Repuestos usados**, incluido el detalle de los **extras** con cantidad, precio unitario y subtotal.
- **Costo total** (en quetzales, `Q`).
- Espacios para la **firma del cliente** y la **firma de NANDDOS**.

**Recomendaciones:**
- Imprima el comprobante y solicite la **firma del cliente** como constancia de que recibió su equipo conforme.
- Si necesita una copia posterior, busque el equipo en el módulo **Entrega**: el sistema detectará que ya fue entregado y le ofrecerá **regenerar el comprobante**.

---

## 6. Preguntas Frecuentes

**No veo algunas opciones del menú o algunos botones. ¿Es un error?**
No. El sistema muestra únicamente lo que su cargo tiene permitido. Si necesita acceso adicional, solicítelo al Administrador Global.

**No puedo escribir en el campo "Problema Reportado". ¿Por qué?**
Porque el problema reportado por el cliente solo puede ser modificado por el Administrador Global. Escriba sus hallazgos en el campo **Diagnóstico Técnico**.

**No veo el Precio de Costo en el inventario.**
Es correcto. El Precio de Costo es confidencial y solo lo ve el Administrador Global.

**El sistema dice "Stock insuficiente" al agregar un repuesto.**
No hay existencias suficientes de ese repuesto en el inventario. Verifique el stock en el módulo **Inventario** o solicite una reposición.

**Agregué un extra por error en la entrega. ¿Se descontó del inventario?**
No. Los extras solo se descuentan al confirmar la entrega. Quítelo con el botón rojo **(-)** de su fila antes de presionar **Generar entrega**.

**El cliente perdió su comprobante. ¿Cómo se lo vuelvo a dar?**
Abra el módulo **Entrega**, busque el código del equipo y acepte la opción de **regenerar el comprobante**.

**El comprobante dice "Sin diagnóstico registrado".**
Significa que nadie escribió el Diagnóstico Técnico antes de la entrega. Para futuras reparaciones, recuerde llenar este campo desde **Lista de Equipos → Editar** antes de entregar el equipo.

---

*Documento oficial de capacitación — System NANDDOS.*
*Para soporte o solicitudes de acceso, comuníquese con el Administrador Global del sistema.*

---

<br><a href="#" style="padding: 10px 15px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;">Descargar en PDF</a>
*(Nota: Si está visualizando este documento en un editor, puede presionar Ctrl + P y seleccionar "Guardar como PDF")*
