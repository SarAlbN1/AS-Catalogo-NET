# 📧 Configuración de Email Local con MailDev

## ¿Qué es MailDev?

MailDev es un servidor SMTP local para desarrollo que **captura todos los emails** sin enviarlos realmente. Tiene una interfaz web donde puedes ver todos los emails HTML que se "enviarían".

**✅ Ventajas:**
- No necesitas configurar Gmail ni App Passwords
- Funciona completamente offline
- Interfaz web para ver los emails
- Emails HTML se visualizan perfectamente
- Gratis y fácil de usar

---

## 🚀 Opción 1: Usar con Docker Compose (Más fácil)

### 1. Iniciar todos los servicios

```bash
docker-compose up -d
```

Esto iniciará:
- MySQL
- Zookeeper
- Kafka
- DataTier
- BusinessTier
- **MailDev** ← Servidor de email local
- **KafkaConsumer** ← Ya configurado para usar MailDev

### 2. Abrir la interfaz web de MailDev

Abre tu navegador en: **http://localhost:1080**

Verás una interfaz como esta:

```
┌─────────────────────────────────────────┐
│  MailDev - Captured Emails              │
├─────────────────────────────────────────┤
│  From          │ Subject     │ Date     │
├─────────────────────────────────────────┤
│  (vacío - esperando emails)              │
└─────────────────────────────────────────┘
```

### 3. Probar el sistema

```bash
# En otra terminal
cd ClientApp
dotnet run

# Selecciona opción 3 - Crear producto
# Ingresa los datos
```

### 4. Ver el email

1. Vuelve a http://localhost:1080
2. Deberías ver el email recibido
3. Haz clic para verlo con todo el HTML renderizado

---

## 🖥️ Opción 2: Usar MailDev Standalone (Sin Docker)

### 1. Instalar MailDev

```bash
# Necesitas Node.js instalado
npm install -g maildev
```

### 2. Ejecutar MailDev

```bash
maildev
```

Verás:
```
MailDev webapp running at http://0.0.0.0:1080
MailDev SMTP Server running at 0.0.0.0:1025
```

### 3. Configurar KafkaConsumer

El archivo `appsettings.Development.json` ya está configurado:

```json
{
  "Email": {
    "SmtpServer": "localhost",
    "SmtpPort": "1025",
    "Username": "",
    "Password": "",
    "FromAddress": "catalogo@localhost.com",
    "ToAddress": "admin@localhost.com"
  }
}
```

### 4. Ejecutar KafkaConsumer

```bash
cd KafkaConsumer
dotnet run
```

### 5. Abrir interfaz web

http://localhost:1080

---

## 🎨 Alternativa: Papercut-SMTP (Solo Windows)

Si prefieres una aplicación de escritorio:

### 1. Descargar Papercut-SMTP

https://github.com/ChangemakerStudios/Papercut-SMTP/releases

### 2. Ejecutar Papercut

- Doble clic en `Papercut-SMTP.exe`
- No requiere instalación
- Se ejecuta en el puerto 25 por defecto

### 3. Configurar KafkaConsumer

Edita `appsettings.Development.json`:

```json
{
  "Email": {
    "SmtpServer": "localhost",
    "SmtpPort": "25",
    "Username": "",
    "Password": "",
    "FromAddress": "catalogo@localhost.com",
    "ToAddress": "admin@localhost.com"
  }
}
```

---

## ✅ Verificación

### Logs del KafkaConsumer (exitoso)

```
🚀 Kafka Consumer Worker iniciado...
📡 Conectado a: localhost:9092
📬 Suscrito al topic: product-events
📨 Mensaje recibido - Key: product-1
⚙️ Procesando evento: ProductCreated para producto: Laptop Dell
Conectando a servidor SMTP local: localhost:1025
Sin autenticación (servidor local)
✅ Email enviado correctamente
✉️ Email de creación enviado para: Laptop Dell
✅ Mensaje procesado y commiteado
```

### Interfaz Web de MailDev

Cuando abras http://localhost:1080 verás:

```
From: catalogo@localhost.com
To: admin@localhost.com
Subject: ✅ Nuevo Producto Creado: Laptop Dell

[Haz clic para ver el email HTML renderizado]
```

---

## 🔧 Troubleshooting

### MailDev no inicia

**Problema**: Error al iniciar MailDev

**Solución**:
```bash
# Verificar si el puerto 1025 está en uso
netstat -ano | findstr :1025

# Si está en uso, matar el proceso o usar otro puerto
maildev --smtp 2525 --web 2580
```

Luego actualiza la configuración:
```json
{
  "Email": {
    "SmtpPort": "2525"
  }
}
```

### No veo emails en MailDev

**Checklist**:
- ✅ MailDev está corriendo en http://localhost:1080
- ✅ KafkaConsumer está configurado con `localhost:1025`
- ✅ KafkaConsumer está ejecutándose
- ✅ Se creó un producto (evento)
- ✅ Revisa los logs del KafkaConsumer

### Email no se envía

**Verifica los logs**:
```bash
docker-compose logs -f kafkaconsumer

# O si es local:
cd KafkaConsumer
dotnet run
```

Busca:
```
✅ Email enviado correctamente
```

O errores:
```
❌ Error al enviar email: ...
```

---

## 📊 Comparación de Opciones

| Opción | Ventajas | Desventajas |
|--------|----------|-------------|
| **MailDev (Docker)** | ✅ Más fácil<br>✅ Ya incluido en docker-compose<br>✅ Interfaz web moderna | ⚠️ Requiere Docker |
| **MailDev (NPM)** | ✅ Funciona en cualquier OS<br>✅ Liviano | ⚠️ Requiere Node.js |
| **Papercut-SMTP** | ✅ App de escritorio<br>✅ No requiere instalación | ⚠️ Solo Windows |
| **Gmail** | ✅ Emails reales | ❌ Requiere App Password<br>❌ Requiere internet |

---

## 🎯 Recomendación

**Para desarrollo local**: Usa **MailDev con Docker Compose**

```bash
# Un solo comando y todo funciona:
docker-compose up -d

# Abrir interfaz web:
start http://localhost:1080
```

**Sin Docker**: Usa **MailDev standalone**

```bash
npm install -g maildev
maildev
```

---

## 📚 Recursos

- [MailDev GitHub](https://github.com/maildev/maildev)
- [Papercut-SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP)
- [MailHog](https://github.com/mailhog/MailHog) (Alternativa en Go)

---

**¡Ahora puedes probar el sistema sin necesidad de configurar Gmail!** 🎉

Simplemente ejecuta `docker-compose up -d` y abre http://localhost:1080 para ver los emails.
