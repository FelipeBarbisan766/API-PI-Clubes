namespace API_PI_Clubes.Application.Email
{
    public class EmailBodyService
    {
        public string GenerateVerificationEmailHtml(string recipientName, string verificationLink)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
                        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Bem-vindo, {recipientName}!</h2>
                        </div>
                        <div class='content'>
                            <p>Obrigado por se registrar em nossa plataforma. Para ativar sua conta, clique no link abaixo:</p>
                            <a href='{verificationLink}' class='button'>Verificar Email</a>
                            <p>Ou copie e cole este link no seu navegador:</p>
                            <p><small>{verificationLink}</small></p>
                            <p>Este link expira em 30 minutos.</p>
                        </div>
                        <div class='footer'>
                            <p>Se você não criou esta conta, ignore este email.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
        public string GenerateResetPassowordHtml(string recipientName, string verificationLink)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
                        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Atenção !, {recipientName}!</h2>
                        </div>
                        <div class='content'>
                            <p>Foi requisitado a recuperação da senha !. Para recuperar a senha, clique no link abaixo:</p>
                            <a href='{verificationLink}' class='button'>Recuperar senha</a>
                            <p>Ou copie e cole este link no seu navegador:</p>
                            <p><small>{verificationLink}</small></p>
                            <p>Este link expira em 15 minutos.</p>
                        </div>
                        <div class='footer'>
                            <p>Se você não requisitou a recuperação da senha, ignore este email.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

    }
}
