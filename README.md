# OsService

Sistema simples para um prestador de serviços registrar **Clientes**, abrir **Ordens de Serviço**, acompanhar **Status**, registrar **Valor** e (opcionalmente) anexar **fotos antes/depois**.

> Esta solução foi construída com **ASP.NET Core + MediatR (CQS) + Dapper + SQL Server + RabbitMQ** e inclui testes **unitários** e **de integração** com xUnit.

---

# 🚀 Quick Start

```bash
git clone https://github.com/seuusuario/ControleOrdemDeServico.git
cd ControleOrdemDeServico
cd Infra
docker compose up -d --build
```

---

# 🌐 Acessos

Após subir os containers:

## API / Swagger

http://localhost:8080/swagger

---

## RabbitMQ Management

http://localhost:15672

**Login:**

```
user: guest
password: guest
```

---

## SQL Server

```
Server: localhost,1433
User: sa
Password: SqlServer2024!Strong#
Database: OsServiceDb
```

---

# 🧰 Tecnologias

* .NET
* ASP.NET Core
* MediatR
* Dapper
* SQL Server
* RabbitMQ
* Docker
* xUnit

---

# ▶️ Como executar

## Via Docker Compose (RECOMENDADO)

### Pré-requisitos

* Docker Desktop instalado
* Docker Compose habilitado

---

## 1. Navegue até a pasta Infra

```bash
cd Infra
```

---

## 2. Execute o ambiente

```bash
docker compose up -d --build
```

---

## 3. Verificar containers

```bash
docker compose ps
```

---

## 4. Ver logs

```bash
docker compose logs -f
```

---

## 5. Parar containers

```bash
docker compose down
```

---

## 6. Reset completo do ambiente (apaga banco e filas)

```bash
docker compose down -v
```

---

# 🗄 Banco de dados

O banco é criado automaticamente na primeira execução.

**Tabelas criadas:**

* Customers
* ServiceOrders
* ServiceOrderAttachments

---

# 📨 Mensageria

RabbitMQ é iniciado automaticamente.

**Portas:**

| Serviço       | Porta |
| ------------- | ----- |
| AMQP          | 5672  |
| Management UI | 15672 |

---

# 🧪 Testes

Executar:

```bash
dotnet test
```

---

# 🧱 Arquitetura

Projeto baseado em:

* CQS (Command Query Separation)
* Repository Pattern
* Dapper
* SQL Server
* RabbitMQ
* Docker

---

# 👨‍💻 Autor

**Wilson Martins da Silva**

---

# 📦 Estrutura do Ambiente Docker

Serviços iniciados automaticamente:

* ✅ SQL Server
* ✅ RabbitMQ
* ✅ API ASP.NET Core

---

# 📍 Observações

A API estará disponível em:

```
http://localhost:8080/swagger
```

RabbitMQ Management:

```
http://localhost:15672
```

---

# 📄 Licença

Este projeto é de uso educacional e para portfólio.
