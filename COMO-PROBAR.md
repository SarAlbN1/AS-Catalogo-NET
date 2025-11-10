# 🧪 Guía de Prueba - KafkaConsumer y ClientApp

## ✅ Tareas Completadas

- ✅ KafkaConsumer creado y configurado
- ✅ EmailService con templates HTML
- ✅ ClientApp (aplicación de consola)
- ✅ Documentación completa
- ✅ Proyectos agregados a la solución
- ✅ **MailDev integrado** - No necesitas Gmail!

## 🚀 Opciones para Probar

### ⭐ Opción 1: Prueba Completa con Docker + MailDev (RECOMENDADO)

**✨ La forma más fácil - Sin necesidad de configurar Gmail!**

**Requisitos**:
- Docker y Docker Compose instalados
- ¡Eso es todo!

**Pasos**:

1. **Iniciar todos los servicios**:
   ```bash
   docker-compose up -d
   ```

   Esto iniciará:
   - ✅ MySQL (puerto 3306)
   - ✅ Zookeeper (puerto 2181)
   - ✅ Kafka (puerto 9092)
   - ✅ DataTier (puerto 5001)
   - ✅ BusinessTier (puerto 8080)
   - ✅ **MailDev** (puerto 1080 web, 1025 SMTP)
   - ✅ KafkaConsumer (conectado a MailDev)

2. **Abrir la interfaz web de MailDev**:
   ```bash
   start http://localhost:1080
   ```
   
   O abre manualmente: http://localhost:1080
   
   Verás una interfaz web donde aparecerán todos los emails capturados

3. **Verificar que todo esté corriendo**:
   ```bash
   docker-compose ps
   ```

4. **Ver logs del consumer**:
   ```bash
   docker-compose logs -f kafkaconsumer
   ```

5. **Probar con ClientApp**:
   ```bash
   cd ClientApp
   dotnet run
   # Selecciona opción 3 - Crear producto
   ```

6. **Ver el email en MailDev**:
   - Vuelve a http://localhost:1080
   - Verás el email con todo el HTML renderizado
   - ¡No necesitas configurar nada de Gmail!

---

### Opción 2: Prueba Completa con Docker (Con Gmail Real)

**Requisitos**:
- Docker y Docker Compose instalados
- Credenciales de Gmail configuradas

**Pasos**:

1. **Configurar credenciales de email**:
   ```bash
   # Editar archivo con tus credenciales
   notepad KafkaConsumer\appsettings.json
   
   # O usar variables de entorno en .env
   notepad .env
   ```

2. **Iniciar servicios**:
   ```bash
   docker-compose up -d
   ```

3. **Verificar que todo esté corriendo**:
   ```bash
   docker-compose ps
   ```
   
   Deberías ver:
   - ✅ mysql
   - ✅ zookeeper
   - ✅ kafka
   - ✅ datatier
   - ✅ businesstier
   - ✅ kafkaconsumer

4. **Ver logs del consumer**:
   ```bash
   docker-compose logs -f kafkaconsumer
   ```

5. **Probar con ClientApp**:
   ```bash
   cd ClientApp
   dotnet run
   # Selecciona opción 3 - Crear producto
   ```

6. **Verificar email**:
   - Revisa tu bandeja de entrada
   - Verifica también SPAM

---

### Opción 2: Prueba Local (Sin Docker)

**Requisitos**:
- Kafka corriendo localmente en localhost:9092
- .NET 9.0 SDK

**Pasos**:

1. **Configurar credenciales**:
   ```bash
   notepad KafkaConsumer\appsettings.Development.json
   ```
   
   Reemplazar:
   - `"Username": "tu-email@gmail.com"` → Tu email
   - `"Password": "xxxx-xxxx-xxxx-xxxx"` → Tu App Password
   - `"ToAddress": "destinatario@gmail.com"` → Email destino

2. **Ejecutar KafkaConsumer**:
   ```bash
   cd KafkaConsumer
   dotnet run
   ```
   
   Deberías ver:
   ```
   🚀 Kafka Consumer Worker iniciado...
   📡 Conectado a: localhost:9092
   📬 Suscrito al topic: product-events
   ```

3. **En otra terminal, simular un evento** (opcional):
   ```bash
   # Necesitas kafka-console-producer instalado
   ```

---

### Opción 3: Prueba Solo de ClientApp

**Requisitos**:
- BusinessTier corriendo en http://localhost:8080

**Pasos**:

1. **Ejecutar ClientApp**:
   ```bash
   cd ClientApp
   dotnet run
   ```

2. **Menú interactivo**:
   ```
   ╔════════════════════════════════════════════════╗
   ║   📦 CLIENTE CATÁLOGO DE PRODUCTOS 📦         ║
   ╚════════════════════════════════════════════════╝
   
   📋 CONSULTAS:
     1️⃣  - Listar todos los productos
     2️⃣  - Buscar producto por ID
     6️⃣  - Buscar producto por nombre
   
   ✏️  OPERACIONES:
     3️⃣  - Crear nuevo producto
     4️⃣  - Actualizar producto
     5️⃣  - Eliminar producto
   
     0️⃣  - Salir
   ```

3. **Crear un producto de prueba**:
   - Selecciona opción 3
   - Ingresa datos del producto
   - Verifica que se crea exitosamente

---

## 📧 Configurar Gmail App Password

### Guía Rápida

1. **Ve a Google Security**: https://myaccount.google.com/security

2. **Activa verificación en 2 pasos**:
   - Si no está activada, sigue los pasos
   - Necesitarás tu número de teléfono

3. **Crea App Password**:
   - Busca "Contraseñas de aplicación" (App passwords)
   - Selecciona "Correo" → "Otro (nombre personalizado)"
   - Escribe "KafkaConsumer"
   - Haz clic en "Generar"

4. **Copia la contraseña**:
   - Gmail mostrará algo como: `abcd efgh ijkl mnop`
   - Cópiala SIN ESPACIOS: `abcdefghijklmnop`

5. **Pega en la configuración**:
   ```json
   "Email": {
     "Username": "tu-email@gmail.com",
     "Password": "abcdefghijklmnop",
     "FromAddress": "tu-email@gmail.com",
     "ToAddress": "destinatario@gmail.com"
   }
   ```

### Documentación Completa

Ver: `docs/email-setup-guide.md` para instrucciones detalladas y troubleshooting.

---

## 🔍 Verificación del Sistema

### Verificar Compilación

```bash
# KafkaConsumer
cd KafkaConsumer
dotnet build
# Debería mostrar: ✅ Compilación realizado correctamente

# ClientApp
cd ..\ClientApp
dotnet build
# Debería mostrar: ✅ Compilación realizado correctamente
```

### Verificar Proyectos en Solución

```bash
cd ..
dotnet sln AS.Catalogo.sln list
```

Debería mostrar:
```
Proyectos
---------
BusinessTier\BusinessTier.csproj
ClientApp\ClientApp.csproj
DataTier\DataTier.csproj
KafkaConsumer\KafkaConsumer.csproj
```

---

## 📊 Flujo de Prueba Completo

```
1. Usuario ejecuta ClientApp
        ↓
2. Crea un producto (opción 3)
        ↓
3. ClientApp → HTTP POST → BusinessTier:8080
        ↓
4. BusinessTier → gRPC → DataTier:5001
        ↓
5. DataTier → Guarda en MySQL + Publica a Kafka
        ↓
6. KafkaConsumer → Lee evento del topic
        ↓
7. EmailService → Envía email vía Gmail
        ↓
8. ✅ Email recibido con notificación
```

---

## 🐛 Troubleshooting

### Error: "No se puede conectar a Kafka"

**Síntoma**: Consumer no inicia o lanza error de conexión

**Solución**:
- Verifica que Kafka esté corriendo: `docker-compose ps kafka`
- Si es local, verifica `localhost:9092`
- Revisa los logs: `docker-compose logs kafka`

### Error: "Authentication failed" (Email)

**Síntoma**: Consumer procesa evento pero falla al enviar email

**Solución**:
- Verifica que usas App Password, NO tu contraseña normal
- Asegúrate de NO tener espacios en la contraseña
- Verifica que tienes verificación en 2 pasos activada
- Regenera el App Password si es necesario

### Error: "No se puede conectar a BusinessTier"

**Síntoma**: ClientApp no puede hacer requests

**Solución**:
- Verifica que BusinessTier esté corriendo en puerto 8080
- Prueba abrir: http://localhost:8080/api/productos
- Revisa los logs de BusinessTier

---

## 📝 Logs Importantes

### KafkaConsumer - Funcionamiento Correcto

```
🚀 Kafka Consumer Worker iniciado...
📡 Conectado a: kafka:29092
📬 Suscrito al topic: product-events
👥 Group ID: product-consumer-group
📨 Mensaje recibido - Key: product-1
⚙️ Procesando evento: ProductCreated para producto: Laptop Dell
✉️ Email de creación enviado para: Laptop Dell
✅ Email enviado exitosamente
✅ Mensaje procesado y commiteado
```

### ClientApp - Operación Exitosa

```
✅ PRODUCTO CREADO EXITOSAMENTE!

┌─────────────────────────────────────────────┐
│ ID:          1                              │
│ Nombre:      Laptop Dell                    │
│ Descripción: Laptop gaming de alta gama    │
│ Precio:      $999.99                        │
│ Stock:       10                             │
└─────────────────────────────────────────────┘

📧 Se enviará un email de notificación...
```

---

## ✅ Checklist de Prueba

- [ ] Credenciales de Gmail configuradas
- [ ] KafkaConsumer compila sin errores
- [ ] ClientApp compila sin errores
- [ ] Servicios de Docker iniciados (si aplica)
- [ ] KafkaConsumer se conecta a Kafka
- [ ] ClientApp puede crear productos
- [ ] Evento llega a Kafka
- [ ] KafkaConsumer procesa el evento
- [ ] Email se envía correctamente
- [ ] Email se recibe en la bandeja

---

## 📚 Documentación Adicional

- **KafkaConsumer**: `KafkaConsumer/README.md`
- **ClientApp**: `ClientApp/README.md`
- **Email Setup**: `docs/email-setup-guide.md`
- **Resumen Completo**: `TAREA-4-COMPLETADA.md`

---

## 🎯 Próximos Pasos

1. ✅ Configurar credenciales de Gmail
2. ✅ Probar el sistema completo
3. ✅ Verificar que los emails llegan
4. ✅ Documentar cualquier ajuste necesario
5. ✅ Commit y push a la rama `feature/kafka-consumer-alejandroc`

---

**¡Todo está listo para probar!** 🚀

Si encuentras algún problema, consulta las secciones de Troubleshooting o los README individuales de cada proyecto.
