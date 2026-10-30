# Clubera — API

> Sistema para reserva de quadras esportivas online.

API REST do **Clubera**, uma plataforma web que centraliza o gerenciamento de clubes, quadras, horários e reservas esportivas. Desenvolvida em **.NET 10** com **Entity Framework Core** e **SQL Server**, e consumida por um frontend em **Angular**.

---

## Sobre o projeto

### História

O Clubera nasceu como projeto acadêmico da **Fatec Taquaritinga**. A motivação veio de um problema comum: o gerenciamento de reservas de quadras esportivas, quando feito de forma manual (planilhas, mensagens, ligações), gera conflitos de horário, perda de informação e muito retrabalho para quem administra o espaço.

A proposta é um sistema web que centraliza o controle de quadras, horários e reservas, tornando o processo mais organizado, eficiente e prático tanto para quem administra quanto para quem joga.

### Objetivo

Desenvolver uma aplicação que facilite o gerenciamento e a reserva de quadras esportivas, reduzindo conflitos de agenda e facilitando o controle das informações.

### Como o projeto é construído

O desenvolvimento envolve levantamento de requisitos, modelagem, implementação e testes das funcionalidades. Ferramentas de inteligência artificial (Claude e GitHub Copilot) são usadas como apoio na implementação e na resolução de dúvidas técnicas.

### Equipe

|Papel|Nome|
|---|---|
|Desenvolvedor|Felipe Adrian Lourenço Barbisan|
|Desenvolvedor|Felipe Gabriel Galati de Oliveira|
|Orientador|Gustavo Henrique del Vechio|

Fatec Taquaritinga — 2026

> ⚠️ O sistema está **em desenvolvimento**. Algumas funcionalidades podem mudar ou ainda não estar completas.

---

## Funcionalidades

- **Clubes e quadras:** cadastro e busca de clubes, com quadras vinculadas a um ou mais esportes.
- **Horários e reservas:** seleção de horários disponíveis, criação e cancelamento de reservas, com regras de antecedência.
- **Painel administrativo:** consulta das reservas realizadas nos clubes do administrador.
- **Planos e assinaturas:** acesso de administradores por assinatura, com pagamento via **Mercado Pago (Checkout Pro)** e validação de assinatura do webhook.
- **Disponibilidade em tempo real:** atualização de horários via **SignalR**, agrupada por clube.
- **Imagens:** upload com geração de três variantes (Thumb, Medium e Full) usando SkiaSharp. Armazenamento local ou em Azure Blob Storage.
- **Contas e segurança:**
    - Cadastro em duas etapas, em conformidade com a **LGPD**.
    - CPF armazenado criptografado (AES-256-GCM) com hash HMAC-SHA256 para detecção de duplicidade.
    - Login com Google, verificação de e-mail e redefinição de senha.
- **Limpeza automática:** serviços em segundo plano para rotinas de manutenção (ex.: limpeza de reservas antigas).

---

##  Arquitetura

Arquitetura em camadas, no estilo "Clean-ish":

```
Controller → Service → Repository → Mapper → DTO → Validator → EF Configuration
```

- Controllers sem `try/catch`: um `ExceptionMiddleware` global converte exceções de domínio em respostas padronizadas (`statusCode`, `errorCode`, `message`, `errors`, `traceId`, `timestamp`).
- Validações com **FluentValidation**, incluindo validadores para CPF, CEP, telefone e UF.
- Configurações de entidades via `IEntityTypeConfiguration<T>`.
- Rotinas em background com `BackgroundService` + `PeriodicTimer`.

## Stack

|Camada|Tecnologia|
|---|---|
|API|.NET 10, ASP.NET Core|
|Persistência|Entity Framework Core, SQL Server|
|Frontend|Angular|
|Pagamentos|Mercado Pago|
|Tempo real|SignalR|
|Imagens|SkiaSharp|
|Infra|Docker, Docker Compose, GitHub Actions, GHCR|

---

## Rodando com Docker

A API é publicada como imagem no **GitHub Container Registry (GHCR)**.

### Pré-requisitos

- Docker e Docker Compose instalados
- Um **SQL Server** rodando na rede Docker externa `clubera-network`, com o container chamado `sqlserver-clubes` (é o host usado na connection string)
- Um arquivo `.env` configurado (veja a próxima seção)

### Passo a passo

**1. Crie a rede externa (apenas na primeira vez):**

```bash
docker network create clubera-network
```

**2. Suba o banco de dados** (compose do SQL Server do projeto, na mesma rede):

```bash
docker compose -f docker-compose.db.yml --env-file .env up -d
```

**3. Crie a pasta de uploads no servidor (apenas na primeira vez):**

```bash
mkdir -p /DATA/AppData/clubera-api-uploads
```

> Ela fica **fora** da pasta de trabalho do runner do GitHub Actions de propósito: arquivos enviados por usuários não podem ser apagados por um `git clean` a cada deploy.

**4. Baixe a imagem e suba a API:**

```bash
docker compose -f docker-compose.api.yml --env-file .env pull
docker compose -f docker-compose.api.yml --env-file .env up -d
```

A API ficará disponível em `http://localhost:5000` (porta `5000` do host → `8080` do container).

### Comandos úteis

```bash
# Ver logs
docker logs -f api-clubes

# Parar a API
docker compose -f docker-compose.api.yml --env-file .env down

# Atualizar para a versão mais recente
docker compose -f docker-compose.api.yml --env-file .env pull
docker compose -f docker-compose.api.yml --env-file .env up -d
```

### Usando outra imagem

Por padrão o compose usa `ghcr.io/felipebarbisan766/api-clubes:latest`. Para usar outra tag ou imagem, defina `API_IMAGE` no `.env`:

```env
API_IMAGE=ghcr.io/felipebarbisan766/api-clubes:1.0.0
```

### Deploy automático

O compose `docker-compose.api.yml` é o que o **GitHub Actions** recria a cada deploy.

---

## Configurando o `.env`

Crie um arquivo `.env` na mesma pasta do `docker-compose.api.yml`. **Nunca commite esse arquivo** — adicione `.env` ao `.gitignore`.

### Modelo

```env
# --- Geral ---
ASPNETCORE_ENVIRONMENT=Production
API_IMAGE=ghcr.io/felipebarbisan766/api-clubes:latest
API_BASE_URL=https://api.seudominio.com
PUBLIC_BASE_URL=https://app.seudominio.com

# --- Banco de dados ---
MSSQL_SA_PASSWORD=TroqueEstaSenha@123

# --- JWT ---
JWT_KEY=
JWT_EMAIL_VERIFICATION_KEY=
JWT_RESET_PASSWORD_KEY=

# --- Segurança (CPF) ---
CPF_ENCRYPTION_KEY=
CPF_HMAC_KEY=

# --- E-mail (SMTP) ---
EMAIL_SENDER=seu-email@gmail.com
EMAIL_SMTP_USERNAME=seu-email@gmail.com
EMAIL_SMTP_PASSWORD=

# --- Azure Storage (deixe vazio se usar armazenamento local) ---
AZURE_STORAGE_CONNECTION_STRING=

# --- Google ---
GOOGLE_CLIENT_ID=

# --- Mercado Pago ---
MERCADOPAGO_ACCESS_TOKEN=
MERCADOPAGO_WEBHOOK_SECRET=
```

### Descrição das variáveis

|Variável|Descrição|
|---|---|
|`ASPNETCORE_ENVIRONMENT`|Ambiente da aplicação. Padrão: `Production`.|
|`API_IMAGE`|Imagem Docker da API (opcional).|
|`API_BASE_URL`|URL pública da **API**. Usada no webhook do Mercado Pago (`/api/payment/webhook`) e na URL dos arquivos enviados.|
|`PUBLIC_BASE_URL`|URL pública do **frontend**. Usada nos links dos e-mails e nos redirecionamentos de pagamento (`/payment/success`, `/failure`, `/pending`).|
|`MSSQL_SA_PASSWORD`|Senha do usuário `sa` do SQL Server. Precisa atender à política de complexidade do SQL Server.|
|`JWT_KEY`|Chave de assinatura dos tokens JWT.|
|`JWT_EMAIL_VERIFICATION_KEY`|Chave dos tokens de verificação de e-mail.|
|`JWT_RESET_PASSWORD_KEY`|Chave dos tokens de redefinição de senha.|
|`CPF_ENCRYPTION_KEY`|Chave de criptografia do CPF (AES-256-GCM).|
|`CPF_HMAC_KEY`|Chave do HMAC-SHA256 usado para detectar CPFs duplicados.|
|`EMAIL_SENDER`|E-mail remetente.|
|`EMAIL_SMTP_USERNAME`|Usuário do SMTP (o servidor configurado é `smtp.gmail.com:587`).|
|`EMAIL_SMTP_PASSWORD`|Senha do SMTP. No Gmail, use uma **senha de app**, não a senha da conta.|
|`AZURE_STORAGE_CONNECTION_STRING`|Connection string do Azure Blob Storage. Só é necessária se `Storage__Provider` for Azure.|
|`GOOGLE_CLIENT_ID`|Client ID do Google OAuth, para login com Google.|
|`MERCADOPAGO_ACCESS_TOKEN`|Access token da aplicação no Mercado Pago.|
|`MERCADOPAGO_WEBHOOK_SECRET`|Segredo para validar a assinatura (`x-signature`) dos webhooks.|

### Gerando chaves seguras

Use valores longos e aleatórios, diferentes para cada chave:

```bash
openssl rand -base64 48   # chaves JWT
openssl rand -base64 32   # chaves de CPF (256 bits)
```

> Confirme no código o formato esperado por `CPF_ENCRYPTION_KEY` (tamanho e codificação) antes de gerar a sua.
> 
> **Atenção:** se você trocar `CPF_ENCRYPTION_KEY` ou `CPF_HMAC_KEY` depois que já existirem CPFs salvos, os dados antigos deixam de ser legíveis e a detecção de duplicidade quebra. Guarde essas chaves com cuidado.

### Configurações fixas no compose

Estas já vêm definidas no `docker-compose.api.yml` e podem ser editadas lá, se necessário:

|Configuração|Valor padrão|
|---|---|
|`Jwt__Expiration`|`3`|
|`Jwt__ExpirationEmailVerification`|`30`|
|`Jwt__ExpirationResetPassword`|`15`|
|`Jwt__Issuer` / `Jwt__Audience`|`api-pi-clubes`|
|`Storage__Provider`|`Local`|
|`ReserveCleanup__RetentionMonths`|`6`|
|`Email__SenderName`|`Clube PI`|

### Armazenamento de imagens

Por padrão (`Storage__Provider: "Local"`), os arquivos são gravados em `/app/wwwroot/uploads` dentro do container, mapeado para `/DATA/AppData/clubera-api-uploads` no host. Para usar Azure Blob Storage, preencha `AZURE_STORAGE_CONNECTION_STRING` e altere `Storage__Provider` no compose.

---

## Boas práticas de segurança

- Nunca versione o `.env` nem exponha as chaves em logs ou issues.
- Use senhas e chaves diferentes por ambiente (desenvolvimento, homologação, produção).
- Em produção, mantenha `ASPNETCORE_ENVIRONMENT=Production` e restrinja `AllowedHosts` ao seu domínio.
- Faça backup periódico do banco e da pasta de uploads.

---

## Referências

- [Documentação do Angular](https://angular.dev/docs)
- [Documentação do ASP.NET Core](https://learn.microsoft.com/pt-br/aspnet/core/)
- [Documentação técnica do SQL Server](https://learn.microsoft.com/pt-br/sql/sql-server/)
- [Claude](https://www.anthropic.com/claude)

---

## Licença

Projeto acadêmico desenvolvido na Fatec Taquaritinga. 
[Apache 2.0 License](LICENSE), Version 2.0, January 2004, [www.apache.org/licenses/](http://www.apache.org/licenses/)
