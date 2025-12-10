using EmailSenderAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace EmailSenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailController> _logger;

        public GmailController(IConfiguration configuration, ILogger<GmailController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                _logger.LogInformation("Gmail: отправка на {Email}", request.ToEmail);

                // Получаем настройки из конфига
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var host = smtpSettings["Host"] ?? "smtp.gmail.com";
                var port = int.Parse(smtpSettings["Port"] ?? "587");
                var username = smtpSettings["Username"];
                var password = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"] ?? username;
                var fromName = smtpSettings["FromName"] ?? "Служба погоды";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Настройки Gmail не заданы"
                    });
                }

                // Создаем сообщение
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", request.ToEmail));
                message.Subject = request.Subject;

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = request.Body
                };

                // Отправляем через Gmail
                using var client = new SmtpClient();

                // Подключаемся с STARTTLS (порт 587)
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);

                // Аутентификация
                await client.AuthenticateAsync(username, password);

                // Отправка
                await client.SendAsync(message);

                // Отключение
                await client.DisconnectAsync(true);

                _logger.LogInformation("✅ Gmail: письмо отправлено на {Email}", request.ToEmail);

                return Ok(new
                {
                    success = true,
                    message = $"Письмо отправлено на {request.ToEmail}",
                    provider = "Gmail"
                });
            }
            catch (MailKit.Security.AuthenticationException authEx)
            {
                _logger.LogError(authEx, "Gmail: ошибка аутентификации");
                return BadRequest(new
                {
                    success = false,
                    message = "Ошибка аутентификации Gmail",
                    error = "Проверьте пароль приложения и двухфакторную аутентификацию",
                    tip = "Убедитесь что создали пароль приложения, а не используете обычный пароль"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gmail: ошибка отправки");
                return BadRequest(new
                {
                    success = false,
                    message = "Ошибка отправки через Gmail",
                    error = ex.Message
                });
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestGmail()
        {
            try
            {
                _logger.LogInformation("Тестирование Gmail SMTP");

                // Можно хардкодить для теста
                var host = "smtp.gmail.com";
                var port = 587;
                var username = "ваш_email@gmail.com";  // ЗАМЕНИТЕ на ваш
                var password = "ваш_пароль_приложения"; // ЗАМЕНИТЕ на пароль приложения
                var testEmail = username; // Отправляем себе

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Тест Gmail", username));
                message.To.Add(new MailboxAddress("", testEmail));
                message.Subject = $"✅ Тест Gmail SMTP {DateTime.Now:HH:mm:ss}";
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = @"
                    <h1 style='color: green;'>✅ ТЕСТ GMAIL УСПЕШЕН!</h1>
                    <p>Поздравляем! Ваш Gmail аккаунт настроен для отправки писем через SMTP.</p>
                    <p><strong>Время отправки:</strong> " + DateTime.Now.ToString("F") + @"</p>
                    <p><strong>SMTP сервер:</strong> smtp.gmail.com:587</p>
                    <p><strong>Метод:</strong> STARTTLS</p>
                    <hr>
                    <p>Теперь вы можете отправлять прогнозы погоды на любые email адреса!</p>
                    "
                };

                using var client = new SmtpClient();

                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return Ok(new
                {
                    success = true,
                    message = "✅ Тест Gmail пройден! Проверьте вашу почту Gmail.",
                    details = new
                    {
                        host,
                        port,
                        from = username,
                        to = testEmail,
                        authentication = "OAuth2 via App Password"
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ Тест Gmail не пройден",
                    error = ex.Message,
                    troubleshooting = new[]
                    {
                        "1. Убедитесь, что включена двухфакторная аутентификация в Google",
                        "2. Создайте пароль приложения (не используйте обычный пароль)",
                        "3. Разрешите 'менее безопасные приложения' (если отключено)",
                        "4. Попробуйте порт 465 с SSL"
                    }
                });
            }
        }

        [HttpGet("test-simple")]
        public IActionResult TestSimple()
        {
            // Простой тест без отправки - только проверка настроек
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            return Ok(new
            {
                configured = !string.IsNullOrEmpty(smtpSettings["Username"]),
                settings = new
                {
                    host = smtpSettings["Host"],
                    port = smtpSettings["Port"],
                    username = smtpSettings["Username"],
                    passwordSet = !string.IsNullOrEmpty(smtpSettings["Password"]),
                    from = smtpSettings["FromEmail"]
                },
                instructions = new[]
                {
                    "1. Включите 2FA в Google Аккаунте",
                    "2. Создайте пароль приложения для 'Почты'",
                    "3. Используйте этот пароль в настройках",
                    "4. Порт: 587, Метод: STARTTLS"
                }
            });
        }

        [HttpPost("send-with-fallback")]
        public async Task<IActionResult> SendWithFallback([FromBody] EmailRequest request)
        {
            // Пытаемся отправить через Gmail, если не получается - демо режим
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var username = smtpSettings["Username"];
                var password = smtpSettings["Password"];

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    // Пробуем реальную отправку
                    var result = await SendEmail(request);
                    return result;
                }
                else
                {
                    // Демо режим
                    _logger.LogInformation("📧 ДЕМО: Письмо для {Email}", request.ToEmail);

                    return Ok(new
                    {
                        success = true,
                        message = $"Демо: письмо было бы отправлено на {request.ToEmail}",
                        demo = true,
                        preview = new
                        {
                            to = request.ToEmail,
                            subject = request.Subject,
                            bodyLength = request.Body.Length
                        }
                    });
                }
            }
            catch
            {
                // Если ошибка - тоже демо режим
                return Ok(new
                {
                    success = true,
                    message = $"Демо режим: письмо для {request.ToEmail}",
                    demo = true
                });
            }
        }
    }
}