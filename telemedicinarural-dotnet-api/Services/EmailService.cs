using System.Collections.Generic;
using System.Threading.Tasks;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace Medicina.Services
{
    public class EmailService
    {
        
        private readonly TransactionalEmailsApi _emailApi;
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;

            var apiKey = configuration["Brevo:api-key"] ?? "";

            Configuration.Default.ApiKey.Add("api-key", apiKey);
            _emailApi = new TransactionalEmailsApi();
        }

        public async System.Threading.Tasks.Task EnviarCorreoConfirmacionCita(string emailDestino, string asunto, string contenido)
        {
            if (string.IsNullOrEmpty(emailDestino))
            {
                throw new ArgumentException("El email de destino no puede ser nulo o vacío.", nameof(emailDestino));
            }

            var emailApi = new TransactionalEmailsApi();
            var email = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender { Email = "jose.molina.castro18@gmail.com", Name = "Telemedicina" },
                To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(emailDestino, null) }, // null en el nombre si no es necesario
                Subject = asunto,
                HtmlContent = contenido
            };

            try
            {
                await emailApi.SendTransacEmailAsync(email);
                Console.WriteLine("Correo enviado a " + emailDestino);
            }
            catch (ApiException ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
            }
        }
    }
}
