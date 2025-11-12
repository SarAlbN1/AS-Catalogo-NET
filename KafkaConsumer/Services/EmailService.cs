using KafkaConsumer.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace KafkaConsumer.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendProductCreatedEmailAsync(ProductEvent productEvent)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "Catalogo System",
                _config["Email:FromAddress"]));
            message.To.Add(new MailboxAddress(
                "Admin",
                _config["Email:ToAddress"]));
            message.Subject = $"✅ Nuevo Producto Creado: {productEvent.ProductName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                            .container {{ padding: 20px; }}
                            .card {{ 
                                background: white; 
                                padding: 30px; 
                                border-radius: 8px; 
                                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                                max-width: 600px;
                                margin: 0 auto;
                            }}
                            .header {{ 
                                color: #4CAF50; 
                                border-bottom: 3px solid #4CAF50;
                                padding-bottom: 15px;
                                margin-bottom: 20px;
                            }}
                            .detail {{ 
                                margin: 15px 0; 
                                padding: 10px;
                                background-color: #f9f9f9;
                                border-left: 4px solid #4CAF50;
                            }}
                            .label {{ 
                                font-weight: bold; 
                                color: #333;
                                display: inline-block;
                                width: 120px;
                            }}
                            .value {{
                                color: #666;
                            }}
                            .footer {{
                                margin-top: 30px;
                                padding-top: 20px;
                                border-top: 1px solid #ddd;
                                color: #999;
                                font-size: 12px;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='card'>
                                <h2 class='header'>🎉 Nuevo Producto en el Catálogo</h2>
                                <div class='detail'>
                                    <span class='label'>ID:</span>
                                    <span class='value'>{productEvent.ProductId}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Nombre:</span>
                                    <span class='value'>{productEvent.ProductName}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Precio:</span>
                                    <span class='value'>${productEvent.Price:F2}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Fecha:</span>
                                    <span class='value'>{productEvent.Timestamp:dd/MM/yyyy HH:mm:ss}</span>
                                </div>
                                <div class='footer'>
                                    Sistema de Catálogo - Notificación Automática
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);

            _logger.LogInformation($"✅ Email enviado exitosamente para producto creado: {productEvent.ProductName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error enviando email de producto creado: {ex.Message}");
            throw;
        }
    }

    public async Task SendProductUpdatedEmailAsync(ProductEvent productEvent)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "Catalogo System",
                _config["Email:FromAddress"]));
            message.To.Add(new MailboxAddress(
                "Admin",
                _config["Email:ToAddress"]));
            message.Subject = $"🔄 Producto Actualizado: {productEvent.ProductName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                            .container {{ padding: 20px; }}
                            .card {{ 
                                background: white; 
                                padding: 30px; 
                                border-radius: 8px; 
                                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                                max-width: 600px;
                                margin: 0 auto;
                            }}
                            .header {{ 
                                color: #2196F3; 
                                border-bottom: 3px solid #2196F3;
                                padding-bottom: 15px;
                                margin-bottom: 20px;
                            }}
                            .detail {{ 
                                margin: 15px 0; 
                                padding: 10px;
                                background-color: #f9f9f9;
                                border-left: 4px solid #2196F3;
                            }}
                            .label {{ 
                                font-weight: bold; 
                                color: #333;
                                display: inline-block;
                                width: 120px;
                            }}
                            .value {{
                                color: #666;
                            }}
                            .footer {{
                                margin-top: 30px;
                                padding-top: 20px;
                                border-top: 1px solid #ddd;
                                color: #999;
                                font-size: 12px;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='card'>
                                <h2 class='header'>🔄 Producto Actualizado en el Catálogo</h2>
                                <div class='detail'>
                                    <span class='label'>ID:</span>
                                    <span class='value'>{productEvent.ProductId}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Nombre:</span>
                                    <span class='value'>{productEvent.ProductName}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Precio:</span>
                                    <span class='value'>${productEvent.Price:F2}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Fecha:</span>
                                    <span class='value'>{productEvent.Timestamp:dd/MM/yyyy HH:mm:ss}</span>
                                </div>
                                <div class='footer'>
                                    Sistema de Catálogo - Notificación Automática
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);

            _logger.LogInformation($"✅ Email enviado exitosamente para producto actualizado: {productEvent.ProductName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error enviando email de producto actualizado: {ex.Message}");
            throw;
        }
    }

    public async Task SendProductDeletedEmailAsync(ProductEvent productEvent)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "Catalogo System",
                _config["Email:FromAddress"]));
            message.To.Add(new MailboxAddress(
                "Admin",
                _config["Email:ToAddress"]));
            message.Subject = $"🗑️ Producto Eliminado: {productEvent.ProductName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                            .container {{ padding: 20px; }}
                            .card {{ 
                                background: white; 
                                padding: 30px; 
                                border-radius: 8px; 
                                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                                max-width: 600px;
                                margin: 0 auto;
                            }}
                            .header {{ 
                                color: #f44336; 
                                border-bottom: 3px solid #f44336;
                                padding-bottom: 15px;
                                margin-bottom: 20px;
                            }}
                            .detail {{ 
                                margin: 15px 0; 
                                padding: 10px;
                                background-color: #f9f9f9;
                                border-left: 4px solid #f44336;
                            }}
                            .label {{ 
                                font-weight: bold; 
                                color: #333;
                                display: inline-block;
                                width: 120px;
                            }}
                            .value {{
                                color: #666;
                            }}
                            .footer {{
                                margin-top: 30px;
                                padding-top: 20px;
                                border-top: 1px solid #ddd;
                                color: #999;
                                font-size: 12px;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='card'>
                                <h2 class='header'>🗑️ Producto Eliminado del Catálogo</h2>
                                <div class='detail'>
                                    <span class='label'>ID:</span>
                                    <span class='value'>{productEvent.ProductId}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Nombre:</span>
                                    <span class='value'>{productEvent.ProductName}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Precio:</span>
                                    <span class='value'>${productEvent.Price:F2}</span>
                                </div>
                                <div class='detail'>
                                    <span class='label'>Fecha:</span>
                                    <span class='value'>{productEvent.Timestamp:dd/MM/yyyy HH:mm:ss}</span>
                                </div>
                                <div class='footer'>
                                    Sistema de Catálogo - Notificación Automática
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);

            _logger.LogInformation($"✅ Email enviado exitosamente para producto eliminado: {productEvent.ProductName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error enviando email de producto eliminado: {ex.Message}");
            throw;
        }
    }

    private async Task SendEmailAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        
        try
        {
            var smtpServer = _config["Email:SmtpServer"] ?? Environment.GetEnvironmentVariable("Email__SmtpServer");
            var portSetting = _config["Email:SmtpPort"] ?? Environment.GetEnvironmentVariable("Email__SmtpPort");
            var username = _config["Email:Username"] ?? Environment.GetEnvironmentVariable("Email__Username");
            var password = _config["Email:Password"] ?? Environment.GetEnvironmentVariable("Email__Password");

            _logger.LogInformation($"SMTP config -> server: {smtpServer ?? "null"}, portSetting: {portSetting ?? "null"}");

            // Para servidores locales como MailDev, no usar SSL/TLS
            var isLocalServer = smtpServer?.Contains("maildev") == true || 
                               smtpServer?.Contains("localhost") == true ||
                               smtpServer?.Contains("127.0.0.1") == true;

            var smtpPort = isLocalServer
                ? int.Parse(string.IsNullOrWhiteSpace(portSetting) ? "1025" : portSetting)
                : int.Parse(string.IsNullOrWhiteSpace(portSetting) ? "587" : portSetting);

            if (isLocalServer)
            {
                // Conexión sin seguridad para servidores locales
                _logger.LogInformation($"Conectando a servidor SMTP local: {smtpServer}:{smtpPort}");
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.None);
            }
            else
            {
                // Conexión con StartTls para servidores externos (Gmail, etc.)
                _logger.LogInformation($"Conectando a servidor SMTP externo: {smtpServer}:{smtpPort}");
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            }

            // Solo autenticar si hay credenciales configuradas
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                _logger.LogInformation("Autenticando con credenciales...");
                await client.AuthenticateAsync(username, password);
            }
            else
            {
                _logger.LogInformation("Sin autenticación (servidor local)");
            }

            await client.SendAsync(message);
            
            _logger.LogInformation("✅ Email enviado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error al enviar email: {ex.Message}");
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
