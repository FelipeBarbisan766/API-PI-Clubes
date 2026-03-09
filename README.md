# 🏟️ API-Clubes

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
