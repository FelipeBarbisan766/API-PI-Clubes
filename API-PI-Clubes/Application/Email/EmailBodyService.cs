namespace API_PI_Clubes.Application.Email
{
    public class EmailBodyService
    {
         //"#FF0048"
        public string GenerateVerificationEmailHtml(string recipientName, string verificationLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='pt-BR' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<meta http-equiv='X-UA-Compatible' content='IE=edge'>
<title>Verificar email - Clubera</title>
<!--[if mso]>
<noscript>
<xml>
<o:OfficeDocumentSettings>
<o:PixelsPerInch>96</o:PixelsPerInch>
</o:OfficeDocumentSettings>
</xml>
</noscript>
<![endif]-->
<style>
  body, table, td {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
  body {{ margin:0; padding:0; width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; }}
  img {{ border:0; line-height:100%; outline:none; text-decoration:none; }}
  a {{ text-decoration:none; }}

  .fallback-link {{
    color:#9a9aa5;
    font-size:12px;
    border-bottom:1px dotted #c7c7d1;
    padding-bottom:1px;
  }}
  .fallback-link:hover,
  .fallback-link:focus {{
    color:#FF0048;
    border-bottom-color: currentColor;
  }}

  @media only screen and (max-width:480px) {{
    .container {{ width:100% !important; }}
    .px-24 {{ padding-left:20px !important; padding-right:20px !important; }}
    .heading {{ font-size:22px !important; line-height:28px !important; }}
  }}
</style>
</head>
<body style='margin:0; padding:0; background-color:#f0f0f3;'>

  <div style='display:none; max-height:0; overflow:hidden; mso-hide:all; opacity:0;'>
    Confirme seu email para começar a usar sua conta na Clubera.
  </div>

  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f0f0f3;'>
    <tr>
      <td align='center' style='padding:32px 16px;'>

        <table role='presentation' class='container' width='440' cellpadding='0' cellspacing='0' border='0' style='width:440px; max-width:100%;'>

          <!-- Cartão -->
          <tr>
            <td style='background-color:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 2px 10px rgba(0,0,0,0.06);'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0'>

                <!-- Cabeçalho colorido -->
                <tr>
                  <td class='px-24' style='background-color:#FF0048; padding:22px 24px;'>
                    <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td style='padding-right:8px; vertical-align:middle; line-height:0;'>
                          <!-- SVG suportado em Gmail, Apple Mail e Outlook novo.
                               Outlook Desktop (Windows) ignora a tag e apenas nao exibe o icone,
                               sem quebrar o layout. -->
                          <svg width='25' height='25' viewBox='0 0 24 24' xmlns='http://www.w3.org/2000/svg' fill-rule='evenodd' clip-rule='evenodd' aria-hidden='true' style='display:block;'>
                            <path fill='#ffffff' d='M22.672 15.226l-2.432.811.841 2.515c.33 1.019-.209 2.127-1.23 2.456-1.15.325-2.148-.321-2.463-1.226l-.84-2.518-5.013 1.677.84 2.517c.391 1.203-.434 2.542-1.831 2.542-.88 0-1.601-.564-1.86-1.314l-.842-2.516-2.431.809c-1.135.328-2.145-.317-2.463-1.229-.329-1.018.211-2.127 1.231-2.456l2.432-.809-1.621-4.823-2.432.808c-1.355.384-2.558-.59-2.558-1.839 0-.817.509-1.582 1.327-1.846l2.433-.809-.842-2.515c-.33-1.02.211-2.129 1.232-2.458 1.02-.329 2.13.209 2.461 1.229l.842 2.515 5.011-1.677-.839-2.517c-.403-1.238.484-2.553 1.843-2.553.819 0 1.585.509 1.85 1.326l.841 2.517 2.431-.81c1.02-.33 2.131.211 2.461 1.229.332 1.018-.21 2.126-1.23 2.456l-2.433.809 1.622 4.823 2.433-.809c1.242-.401 2.557.484 2.557 1.838 0 .819-.51 1.583-1.328 1.847m-8.992-6.428l-5.01 1.675 1.619 4.828 5.011-1.674-1.62-4.829z'></path>
                          </svg>
                        </td>
                        <td style='font-size:18px; font-weight:700; color:#ffffff; letter-spacing:0.2px; vertical-align:middle;'>
                          Clubera
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- Conteúdo -->
                <tr>
                  <td class='px-24' style='padding:32px 32px 8px 32px;'>
                    <p class='heading' style='margin:0 0 20px 0; font-size:24px; line-height:30px; font-weight:800; color:#FF0048;'>
                      Bem-vindo à Clubera, {recipientName}!
                    </p>
                    <p style='margin:0 0 28px 0; font-size:15px; line-height:23px; color:#4b4b55;'>
                      Obrigado por se registrar em nossa plataforma. Para ativar sua conta, clique no botão abaixo.
                    </p>
                  </td>
                </tr>

                <!-- Botão -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px;'>
                    <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td style='border-radius:8px; background-color:#FF0048;'>
                          <!--
                            O token de verificação fica SOMENTE no atributo href.
                            Nunca é exibido como texto na tela; o usuário pode
                            copiar o link (botão direito > ""Copiar endereço do link"")
                            sem precisar clicar e ser redirecionado.
                          -->
                          <a href='{verificationLink}'
                             target='_blank'
                             style='display:inline-block; padding:14px 28px; font-size:15px; font-weight:700; color:#ffffff; border-radius:8px;'>
                            Verificar email
                          </a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- Aviso -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px; border-top:1px solid #efeff2;'>
                    <p style='margin:20px 0 0 0; font-size:13px; line-height:20px; color:#9a9aa5;'>
                      Este link expira em 30 minutos. Se você não criou esta conta, pode ignorar e excluir este e-mail.
                    </p>
                  </td>
                </tr>

                <!-- Link de apoio oculto (fallback, sem mostrar a URL) -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px;'>
                    <p style='margin:0; font-size:12px; line-height:18px; color:#c7c7d1;'>
                      Problemas com o botão?
                      <a href='{verificationLink}' target='_blank' class='fallback-link'>Copiar link de verificação</a>
                    </p>
                  </td>
                </tr>

              </table>
            </td>
          </tr>

          <!-- Rodapé fora do cartão -->
          <tr>
            <td style='padding:24px 24px 0 24px;' align='center'>
              <p style='margin:0 0 8px 0; font-size:12px; line-height:18px; color:#9a9aa5; text-align:center;'>
                Clubera é a plataforma que ajuda você a gerenciar suas quadras e reservas em um só lugar.
              </p>
              <p style='margin:0; font-size:12px; color:#b5b5bd; text-align:center;'>
                © 2026 Clubera. Todos os direitos reservados.
              </p>
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>

</body>
</html>
            ";
        }

        public string GenerateResetPassowordHtml(string recipientName, string verificationLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='pt-BR' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<meta http-equiv='X-UA-Compatible' content='IE=edge'>
<title>Redefinir senha - Clubera</title>
<!--[if mso]>
<noscript>
<xml>
<o:OfficeDocumentSettings>
<o:PixelsPerInch>96</o:PixelsPerInch>
</o:OfficeDocumentSettings>
</xml>
</noscript>
<![endif]-->
<style>
  body, table, td {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
  body {{ margin:0; padding:0; width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; }}
  img {{ border:0; line-height:100%; outline:none; text-decoration:none; }}
  a {{ text-decoration:none; }}

  .fallback-link {{
    color:#9a9aa5;
    font-size:12px;
    border-bottom:1px dotted #c7c7d1;
    padding-bottom:1px;
  }}
  .fallback-link:hover,
  .fallback-link:focus {{
    color:#FF0048;
    border-bottom-color: currentColor;
  }}

  @media only screen and (max-width:480px) {{
    .container {{ width:100% !important; }}
    .px-24 {{ padding-left:20px !important; padding-right:20px !important; }}
    .heading {{ font-size:22px !important; line-height:28px !important; }}
  }}
</style>
</head>
<body style='margin:0; padding:0; background-color:#f0f0f3;'>

  <div style='display:none; max-height:0; overflow:hidden; mso-hide:all; opacity:0;'>
    Recebemos uma solicitação para redefinir a senha da sua conta Clubera. O link expira em 5 dias.
  </div>

  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f0f0f3;'>
    <tr>
      <td align='center' style='padding:32px 16px;'>

        <table role='presentation' class='container' width='440' cellpadding='0' cellspacing='0' border='0' style='width:440px; max-width:100%;'>

          <!-- Cartão -->
          <tr>
            <td style='background-color:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 2px 10px rgba(0,0,0,0.06);'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0'>

                <!-- Cabeçalho colorido -->
                <tr>
                  <td class='px-24' style='background-color:#FF0048; padding:22px 24px;'>
                    <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td style='padding-right:8px; vertical-align:middle; line-height:0;'>
                          <!-- SVG suportado em Gmail, Apple Mail e Outlook novo.
                               Outlook Desktop (Windows) ignora a tag e apenas nao exibe o icone,
                               sem quebrar o layout. -->
                          <svg width='25' height='25' viewBox='0 0 24 24' xmlns='http://www.w3.org/2000/svg' fill-rule='evenodd' clip-rule='evenodd' aria-hidden='true' style='display:block;'>
                            <path fill='#ffffff' d='M22.672 15.226l-2.432.811.841 2.515c.33 1.019-.209 2.127-1.23 2.456-1.15.325-2.148-.321-2.463-1.226l-.84-2.518-5.013 1.677.84 2.517c.391 1.203-.434 2.542-1.831 2.542-.88 0-1.601-.564-1.86-1.314l-.842-2.516-2.431.809c-1.135.328-2.145-.317-2.463-1.229-.329-1.018.211-2.127 1.231-2.456l2.432-.809-1.621-4.823-2.432.808c-1.355.384-2.558-.59-2.558-1.839 0-.817.509-1.582 1.327-1.846l2.433-.809-.842-2.515c-.33-1.02.211-2.129 1.232-2.458 1.02-.329 2.13.209 2.461 1.229l.842 2.515 5.011-1.677-.839-2.517c-.403-1.238.484-2.553 1.843-2.553.819 0 1.585.509 1.85 1.326l.841 2.517 2.431-.81c1.02-.33 2.131.211 2.461 1.229.332 1.018-.21 2.126-1.23 2.456l-2.433.809 1.622 4.823 2.433-.809c1.242-.401 2.557.484 2.557 1.838 0 .819-.51 1.583-1.328 1.847m-8.992-6.428l-5.01 1.675 1.619 4.828 5.011-1.674-1.62-4.829z'></path>
                          </svg>
                        </td>
                        <td style='font-size:18px; font-weight:700; color:#ffffff; letter-spacing:0.2px; vertical-align:middle;'>
                          Clubera
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- Conteúdo -->
                <tr>
                  <td class='px-24' style='padding:32px 32px 8px 32px;'>
                    <p class='heading' style='margin:0 0 20px 0; font-size:24px; line-height:30px; font-weight:800; color:#FF0048;'>
                      Esqueceu sua senha? Acontece com os melhores.
                    </p>
                    <p style='margin:0 0 28px 0; font-size:15px; line-height:23px; color:#4b4b55;'>
                      Olá, {recipientName}. Para redefinir sua senha, clique no botão abaixo. O link irá se autodestruir em 5 dias.
                    </p>
                  </td>
                </tr>

                <!-- Botão -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px;'>
                    <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td style='border-radius:8px; background-color:#FF0048;'>
                          <!--
                            O token de redefinição fica SOMENTE no atributo href.
                            Nunca é exibido como texto na tela; o usuário pode
                            copiar o link (botão direito > ""Copiar endereço do link"")
                            sem precisar clicar e ser redirecionado.
                          -->
                          <a href='{verificationLink}'
                             target='_blank'
                             style='display:inline-block; padding:14px 28px; font-size:15px; font-weight:700; color:#ffffff; border-radius:8px;'>
                            Redefinir sua senha
                          </a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- Aviso -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px; border-top:1px solid #efeff2;'>
                    <p style='margin:20px 0 0 0; font-size:13px; line-height:20px; color:#9a9aa5;'>
                      Se você não deseja alterar sua senha ou não solicitou essa redefinição, pode ignorar e excluir este e-mail.
                    </p>
                  </td>
                </tr>

                <!-- Link de apoio oculto (fallback, sem mostrar a URL) -->
                <tr>
                  <td class='px-24' style='padding:0 32px 28px 32px;'>
                    <p style='margin:0; font-size:12px; line-height:18px; color:#c7c7d1;'>
                      Problemas com o botão?
                      <a href='{verificationLink}' target='_blank' class='fallback-link'>Copiar link de redefinição</a>
                    </p>
                  </td>
                </tr>

              </table>
            </td>
          </tr>

          <!-- Rodapé fora do cartão -->
          <tr>
            <td style='padding:24px 24px 0 24px;' align='center'>
              <p style='margin:0 0 8px 0; font-size:12px; line-height:18px; color:#9a9aa5; text-align:center;'>
                Clubera é a plataforma que ajuda você a gerenciar suas quadras e reservas em um só lugar.
              </p>
              <p style='margin:0; font-size:12px; color:#b5b5bd; text-align:center;'>
                © 2026 Clubera. Todos os direitos reservados.
              </p>
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>

</body>
</html>
            ";
        }

    }
}