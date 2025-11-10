# 📧 Guía de Configuración de Email con Gmail

Esta guía te ayudará a configurar el servicio de email para el KafkaConsumer usando Gmail.

## 🔐 Configuración de Gmail App Password

### Paso 1: Verificación en Dos Pasos

1. Ve a tu cuenta de Google: https://myaccount.google.com/security
2. En la sección "Cómo inicias sesión en Google", busca **"Verificación en dos pasos"**
3. Si no está activada, haz clic en **"Verificación en dos pasos"** y sigue los pasos para activarla
4. Necesitarás tu número de teléfono para recibir códigos de verificación

### Paso 2: Crear App Password

1. Una vez activada la verificación en dos pasos, regresa a: https://myaccount.google.com/security
2. Busca **"Contraseñas de aplicación"** (App passwords)
   - Si no ves esta opción, asegúrate de tener la verificación en dos pasos activada
3. Haz clic en **"Contraseñas de aplicación"**
4. En el selector, elige:
   - **Seleccionar aplicación**: Correo
   - **Seleccionar dispositivo**: Otro (nombre personalizado)
   - Escribe: "KafkaConsumer Catalogo"
5. Haz clic en **"Generar"**
6. Gmail mostrará una contraseña de 16 caracteres como: `xxxx xxxx xxxx xxxx`
7. **⚠️ IMPORTANTE**: Copia esta contraseña inmediatamente, no podrás verla de nuevo

### Paso 3: Configurar en el Proyecto

#### Opción A: Variables de Entorno (Recomendado para Docker)

1. Crea un archivo `.env` en la raíz del proyecto (copiar de `.env.example`):

```bash
# .env
EMAIL_USERNAME=tu-email@gmail.com
EMAIL_PASSWORD=xxxxxxxxxxxxxxxx
EMAIL_FROM=tu-email@gmail.com
EMAIL_TO=destinatario@gmail.com
```

2. Reemplaza:
   - `tu-email@gmail.com`: Tu dirección de Gmail
   - `xxxxxxxxxxxxxxxx`: La contraseña de aplicación generada (sin espacios)
   - `destinatario@gmail.com`: Email donde quieres recibir las notificaciones

#### Opción B: appsettings.json (Solo para desarrollo local)

Edita `KafkaConsumer/appsettings.Development.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "tu-email@gmail.com",
    "Password": "xxxx-xxxx-xxxx-xxxx",
    "FromAddress": "tu-email@gmail.com",
    "ToAddress": "destinatario@gmail.com"
  }
}
```

**⚠️ IMPORTANTE**: 
- NO subas este archivo a Git si contiene credenciales reales
- Usa la App Password, NO tu contraseña normal de Gmail
- Quita los espacios de la contraseña

## 🚀 Verificar Configuración

### Prueba Local (sin Docker)

```bash
cd KafkaConsumer
dotnet run
```

### Prueba con Docker Compose

```bash
# Asegúrate de tener el archivo .env configurado
docker-compose up kafkaconsumer
```

## 📨 Tipos de Email que se Envían

El sistema envía tres tipos de emails HTML:

### 1. Producto Creado ✅
- **Color**: Verde (#4CAF50)
- **Emoji**: 🎉
- **Asunto**: "✅ Nuevo Producto Creado: [Nombre]"

### 2. Producto Actualizado 🔄
- **Color**: Azul (#2196F3)
- **Emoji**: 🔄
- **Asunto**: "🔄 Producto Actualizado: [Nombre]"

### 3. Producto Eliminado 🗑️
- **Color**: Rojo (#f44336)
- **Emoji**: 🗑️
- **Asunto**: "🗑️ Producto Eliminado: [Nombre]"

## 🔍 Troubleshooting

### Error: "Authentication failed"

**Problema**: Credenciales incorrectas o App Password no válida

**Solución**:
1. Verifica que estás usando la App Password, no tu contraseña normal
2. Asegúrate de que no hay espacios en la contraseña
3. Regenera la App Password en Google

### Error: "SMTP connection failed"

**Problema**: No se puede conectar al servidor SMTP

**Solución**:
1. Verifica tu conexión a internet
2. Verifica que el puerto 587 no esté bloqueado por un firewall
3. Si estás en una red corporativa, puede haber restricciones SMTP

### Error: "The remote certificate is invalid"

**Problema**: Problemas con certificados SSL

**Solución**:
1. Asegúrate de que tu sistema tiene los certificados CA actualizados
2. En Docker, el Dockerfile ya incluye la instalación de `ca-certificates`

### No recibo emails

**Checklist**:
- ✅ Verificación en dos pasos activada en Gmail
- ✅ App Password generada correctamente
- ✅ Variables de entorno configuradas en `.env`
- ✅ Email "ToAddress" es correcto
- ✅ Revisa la carpeta de SPAM
- ✅ Revisa los logs del KafkaConsumer

## 📝 Logs del Sistema

El KafkaConsumer genera logs detallados:

```bash
# Ver logs del consumer
docker-compose logs -f kafkaconsumer
```

Ejemplos de logs exitosos:

```
✅ Email enviado exitosamente para producto creado: Laptop Dell
🚀 Kafka Consumer Worker iniciado...
📡 Conectado a: kafka:29092
📬 Suscrito al topic: product-events
📨 Mensaje recibido - Key: product-1
✉️ Email de creación enviado para: Laptop Dell
```

## 🔒 Seguridad

### Buenas Prácticas

1. **Nunca subas credenciales a Git**
   - Agrega `.env` al `.gitignore`
   - Usa `.env.example` como template

2. **Usa App Passwords**
   - NO uses tu contraseña principal de Gmail
   - Las App Passwords se pueden revocar individualmente

3. **Rotación de Credenciales**
   - Cambia la App Password periódicamente
   - Revoca App Passwords que ya no uses

4. **Variables de Entorno en Producción**
   - Usa secretos de Docker Swarm
   - O servicios como Azure Key Vault, AWS Secrets Manager

## 📚 Recursos Adicionales

- [Contraseñas de aplicación de Google](https://support.google.com/accounts/answer/185833)
- [Verificación en dos pasos](https://www.google.com/landing/2step/)
- [MailKit Documentation](https://github.com/jstedfast/MailKit)
- [SMTP con Gmail](https://support.google.com/mail/answer/7126229)

## ✉️ Soporte

Si tienes problemas con la configuración:

1. Revisa los logs del KafkaConsumer
2. Verifica que Kafka esté funcionando
3. Prueba enviar un email de prueba manualmente
4. Consulta la sección de Troubleshooting

---

**Última actualización**: Noviembre 2025
