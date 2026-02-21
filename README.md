# Instru��es

- Caso o tempo n�o seja suficiente, priorize a **qualidade, o padr�o e a estrutura do c�digo**, definindo claramente quais funcionalidades n�o ser�o implementadas.
- Caso alguma funcionalidade n�o seja implementada, isso **deve ser documentado neste README**, explicando o motivo.
- O c�digo fornecido cont�m **problemas que devem ser identificados e corrigidos**.
- Fique a vontade para criar, renomear e remover pastas,bibliotecas e at� a solu��o n�o utilizadas.
- O sistema deve **compilar corretamente e executar todas as a��es previstas**.
- O c�digo final **n�o deve apresentar erros nem warnings** durante a compila��o.
- Deve ser enviado via e-mail para consultoria com o link do projeto no Github. 
- Utilize a extensão do SonarLint para verificar os problemas.
- Teste de unidade e de integração devem ser feitos utilizando xUnit.
---

## 1. Introdu��o

Sistema para um prestador de servi�os (ou pequena equipe) registrar clientes, abrir ordens de servi�o, acompanhar status, registrar valores e anexar fotos de antes/depois do servi�o.

---

## 2. Funcionalidades Detalhadas

### 2.1 Cadastro de Cliente

#### Objetivo
Permitir registrar e consultar dados do cliente para vincula��o em Ordens de Servi�o (OS).

#### Campos (m�nimo)
- Nome (obrigat�rio, 2�150 caracteres)
- Id (gerado pelo sistema)
- Telefone (opcional, at� 30 caracteres)
- E-mail (opcional, at� 120 caracteres, formato v�lido)
- Documento (CPF/CNPJ) (opcional, at� 30 caracteres, sem valida��o pesada)
- Data de cria��o (gerado pelo sistema)

#### Regras de Neg�cio
1. Nome � obrigat�rio e n�o pode conter apenas whitespace.
2. Telefone e e-mail podem ser nulos; se informados, devem ser trimados.
3. Opcionalmente, bloquear ou alertar duplicidade por:
   - Documento (CPF/CNPJ), quando informado
   - Telefone, quando informado

#### Opera��es
- Criar cliente
- Consultar cliente por Id
- Buscar cliente por telefone ou documento

#### Casos de Teste
- Criar cliente com nome v�lido retorna 201 Created + id
- Criar cliente sem nome retorna 400 Validation Error
- Criar cliente com e-mail inv�lido retorna 400 Validation Error
- Criar cliente com telefone e buscar retorna dados consistentes
- Criar cliente com documento duplicado (se regra ativa) retorna 409 Conflict ou 400

---

### 2.2 Abertura de Ordem de Servi�o

#### Objetivo
Criar uma OS vinculada a um cliente, com descri��o e dados iniciais.

#### Campos (m�nimo)
- ClienteId (obrigat�rio)
- Descri��o do servi�o (obrigat�rio, 1�500 caracteres)
- N�mero da OS (gerado automaticamente, sequencial/identity)
- Status (inicial = Aberta)
- Data de abertura (gerado pelo sistema)
- Valor do servi�o (decimal(18,2)) (opcional no momento da abertura)
- Moeda (BRL)
- Data de atualiza��o valor (opcional)

#### Regras de Neg�cio
1. S� � poss�vel abrir OS para cliente existente.
2. Descri��o � obrigat�ria.
3. Status inicial deve ser sempre Aberta.
4. N�mero da OS deve ser �nico e sequencial.
5. Regra de neg�cio item 2.4 

#### Opera��es
- Abrir OS
- Consultar OS por Id
- Listar OS por cliente, status ou per�odo

#### Casos de Teste
- Abrir OS para cliente existente retorna 201 Created
- Abrir OS para cliente inexistente retorna 404 Not Found
- Abrir OS com descri��o vazia retorna 400 Bad Request
- Consultar OS rec�m-criada retorna status Aberta

---

### 2.3 Status da Ordem de Servi�o

#### Objetivo
Permitir acompanhar o ciclo do servi�o.

#### Estados
- Aberta
- Em Execu��o
- Finalizada

#### Regras de Transi��o
- Aberta -> Em Execu��o (permitido)
- Em Execu��o -> Finalizada (permitido)
- Aberta -> Finalizada (bloqueado)
- Finalizada -> qualquer outro (bloqueado)

#### Opera��es
- Alterar status
- Registrar datas opcionais:
  - StartedAt ao entrar em Em Execu��o
  - FinishedAt ao entrar em Finalizada

#### Casos de Teste
- Alterar Aberta para Em Execu��o retorna 200 OK
- Alterar Em Execu��o para Finalizada retorna 200 OK
- Alterar Finalizada para outro status retorna 409 Conflict

---

### 2.4 Valor do Servi�o

#### Objetivo
Permitir definir ou ajustar o valor do servi�o.

#### Campos
- Valor (decimal(18,2))
- Moeda (BRL)
- Data de atualiza��o (opcional)

#### Regras de Neg�cio
1. Valor pode ser nulo enquanto Aberta ou Em Execu��o.
2. Valor pode ser obrigat�rio para finalizar a OS.
3. Valor n�o pode ser negativo.
4. Ap�s Finalizada, n�o permitir altera��o.

#### Opera��es
- Definir ou alterar valor
- Validar valor ao finalizar OS

---

### 2.5 Fotos Antes / Depois (Opcional)

#### Objetivo
Permitir anexar evid�ncias do servi�o.

#### Campos do Anexo
- Id
- ServiceOrderId
- Type (Before | After)
- FileName
- ContentType (image/jpeg, image/png)
- SizeBytes
- StoragePath
- UploadedAt

#### Regras de Neg�cio
1. Aceitar apenas JPG e PNG.
2. Tamanho m�ximo sugerido: 5MB.
3. Permitir m�ltiplos anexos.
4. Upload local em /data/uploads (container ou volume).

---

## 3. API Sugerida

### Clientes
- POST /v1/customers
- GET /v1/customers/{id}

### Ordens de Servi�o
- POST /v1/service-orders
- GET /v1/service-orders/{id}
- PATCH /v1/service-orders/{id}/status
- PUT /v1/service-orders/{id}/price
- POST /v1/service-orders/{id}/attachments/before
- POST /v1/service-orders/{id}/attachments/after
- GET /v1/service-orders/{id}/attachments

---

## 4. Requisitos N�o Funcionais (Opcional)

### Performance
- Upload deve ser feito via streaming, evitando carregar todo o arquivo em mem�ria.

### Seguran�a
- Validar content-type e extens�o real do arquivo.
- Sanitizar nome do arquivo.

### Observabilidade
- Registrar logs para cria��o de cliente, abertura de OS e mudan�a de status.
