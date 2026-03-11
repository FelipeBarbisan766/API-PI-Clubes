# API-Clubes

Este projeto utiliza **Docker** para padronizar o ambiente de desenvolvimento e facilitar a execução de todos os serviços necessários.

---

## 🚀 Comandos para o Docker

Siga as instruções abaixo para gerenciar os containers do projeto.

### 🛠️ Instalação e Primeira Execução

Para construir as imagens e subir os containers pela primeira vez (ou após alterações no `Dockerfile`):

```bash
docker compose up --build
```
Para encerrar os containers e remover as redes criadas pelo Compose:

```Bash
docker compose down
```
Para rodar o ambiente novamente (após a primeira build ter sido concluída):

```Bash
docker compose up
```
Rodar em Segundo Plano: Se não quiser que o terminal fique preso aos logs, use a flag -d:

```Bash
docker compose up -d
```
Verificar Status: Para ver quais containers estão rodando:

```Bash
docker ps
```
Nota: Certifique-se de que o Docker Desktop ou o Daemon do Docker esteja ativo antes de executar os comandos.

---

## 🚀 Comandos para o Entity Framework Core

Siga as instruções abaixo para gerenciar as migrações e a estrutura do banco de dados do projeto.

### 🛠️ Gestão de Migrations

Para criar uma nova migração após alterar suas classes de modelo (substitua `NomeDaMigration` pelo nome desejado):
```Bash
dotnet ef migrations add NomeDaMigration
```

Para remover a última migração criada (desde que ainda não tenha sido aplicada ao banco):
```Bash
dotnet ef migrations remove
```

Para listar todas as migrações que já foram criadas no projeto:
```Bash
dotnet ef migrations list
```

### 🗄️ Atualização do Banco de Dados

Para aplicar todas as migrações pendentes e atualizar o esquema do banco de dados:
```Bash
dotnet ef database update`
```

Para reverter o banco de dados para uma migração específica (útil para "rollback"):
```Bash
dotnet ef database update NomeDeUmaMigrationAntiga`
```

Para apagar completamente o banco de dados (Cuidado! Isso remove todos os dados):
```Bash
dotnet ef database drop`
```

Nota: Certifique-se de que a Connection String no seu `appsettings.json` ou `Program.cs` esteja configurada corretamente antes de rodar os comandos de update.

