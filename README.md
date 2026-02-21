# OsService

Sistema simples para um prestador de serviços registrar **Clientes**, abrir **Ordens de Serviço**, acompanhar **Status**, registrar **Valor** e (opcionalmente) anexar **fotos antes/depois**.

> Esta solução foi construída com ASP.NET Core + MediatR (CQS) + Dapper + SQL Server e inclui testes **unitários** e **de integração** com xUnit.

---

## Quick Start

```bash
git clone https://github.com/seuusuario/ControleOrdemDeServico.git
cd ControleOrdemDeServico
cd Infra
docker compose up --build
```

Swagger:

http://localhost:8080/swagger

---

## Tecnologias

- .NET
- ASP.NET Core
- MediatR
- Dapper
- SQL Server
- Docker
- xUnit

---

## Como executar

### Via Docker Compose (recomendado)

Pré-requisitos:

- Docker Desktop instalado
- Docker Compose habilitado

### 1. Navegue até a pasta Infra

```bash
cd Infra
```

### 2. Execute o docker compose

```bash
docker compose up --build
```

### 3. Acesse

Swagger:

http://localhost:8080/swagger

### 4. Parar containers

```bash
docker compose down
```

---

## Banco de dados

Tabelas criadas automaticamente:

- Customers
- ServiceOrders
- ServiceOrderAttachments

---

## Testes

Executar:

```bash
dotnet test
```

---

## Autor

Wilson Martins da Silva
