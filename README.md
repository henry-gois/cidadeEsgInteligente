# Projeto - Cidades ESG Inteligentes

**API de monitoramento ESG de eficiência energética | C# / .NET 8**

> Este repositório contém o backend recebido no projeto original, acrescido de containerização, orquestração e pipeline GitHub Actions. Por padrão, staging e produção são implantados em runners Linux efêmeros do GitHub, com API + SQL Server, health check real e evidências como artefatos. Um modo opcional de deploy persistente por SSH também está preparado para quando houver servidor.

## Integrantes

- Guilherme Luz Martins - RM 564344
- Henrique Miranda Paulino - RM 565689
- Nicolas Granatelli Sanches - RM 565755
- Henry Gois Ribeiro - RM 564329
- Matheus Pedrosa Mendes da Costa - RM 564328

## Visão geral

A API REST oferece funcionalidades para consumo de energia, indicadores e dashboards, alertas, automação IoT e autenticação HMAC Bearer com papéis `ADMIN` e `OPERATOR`.

### Estrutura principal

```text
.
├── .github/workflows/ci-cd.yml
├── deploy/docker-compose.deploy.yml
├── src/Esg.Api/                 # API ASP.NET Core .NET 8
├── tests/Esg.Api.Tests/         # Testes xUnit existentes
├── Dockerfile
├── docker-compose.yml           # API + SQL Server para desenvolvimento local
├── .env.example
├── EsgEnergy.sln
└── docs/
    ├── documentacao-tecnica.pdf
    └── evidencias/              # Prints reais a adicionar após execução
```

## Como executar localmente com Docker

### Pré-requisitos

- Docker Desktop atualizado (ou Docker Engine + Docker Compose v2.20 ou superior).
- Máquina/ambiente compatível com a imagem Linux do SQL Server, preferencialmente x86-64.
- Aproximadamente 4 GB ou mais de memória disponível para os containers; SQL Server pode consumir bastante memória.

### 1. Preparar variáveis de ambiente

Na raiz do repositório, copie `.env.example` para `.env` e altere os valores:

**PowerShell:**

```powershell
Copy-Item .env.example .env
```

**Linux/macOS:**

```bash
cp .env.example .env
```

O valor `MSSQL_SA_PASSWORD` deve obedecer à política do SQL Server (mínimo de 8 caracteres e pelo menos três destas categorias: maiúsculas, minúsculas, números e símbolos). Para evitar problemas com arquivos `.env`, use um valor forte sem espaços, `$`, `#` ou aspas. `AUTH_SIGNING_KEY` deve ser uma chave aleatória com pelo menos 32 bytes; para gerar uma string hexadecimal no Linux/macOS, use `openssl rand -hex 32`. Os valores de exemplo servem apenas para desenvolvimento.

**Nunca envie o arquivo `.env` ao Git, nem use essas credenciais em um ambiente real.**

### 2. Subir a aplicação

```bash
docker compose up --build -d --wait --wait-timeout 180
docker compose ps
```

A configuração inicia dois serviços: `api` (ASP.NET Core) e `db` (SQL Server 2022 Developer). O banco usa o volume Docker `esg_sqlserver_data`. A API aguarda o banco ficar saudável, aplica as migrations do Entity Framework Core no início e expõe um endpoint de prontidão que verifica a conexão com o banco.

- **Health check:** http://localhost:8080/health
- **Swagger (ambiente local):** http://localhost:8080/swagger
- **API:** http://localhost:8080

O endpoint `/health` deve responder HTTP 200 com `{"status":"healthy"}` quando o banco estiver acessível. A primeira inicialização pode demorar enquanto o SQL Server inicia e as migrations são aplicadas.

### 3. Ver logs e encerrar

```bash
docker compose logs -f api db
docker compose down
```

`docker compose down` mantém os dados no volume. Para apagar também o banco local e seus dados de teste, use `docker compose down -v` — **essa operação é destrutiva**.

### 4. Executar testes fora dos containers

Com o SDK .NET 8 instalado:

```bash
dotnet restore EsgEnergy.sln
dotnet build EsgEnergy.sln --configuration Release
dotnet test EsgEnergy.sln --configuration Release
```

O projeto de testes original usa xUnit e stubs para validar respostas HTTP 200 dos controllers. Os testes são de unidade e não substituem testes de integração com SQL Server.

**Validação local realizada em 13/10/2026:** restore concluído, build Release com 0 erros e 0 avisos e 5 de 5 testes xUnit aprovados com o SDK .NET 8.0.426. A execução no GitHub Actions continua necessária para produzir a evidência do pipeline.

Para uma validação estática do Compose sem criar `.env` nem iniciar containers:

```bash
docker compose --env-file .env.example config --quiet
```

O script `scripts/verificar-local.ps1` (Windows) ou `scripts/verificar-local.sh` (Linux/macOS) reúne restore, build, testes e essa validação do Compose.

## Pipeline CI/CD

Ferramenta: **GitHub Actions**. O workflow está em `.github/workflows/ci-cd.yml`.

1. **Build e testes:** em pull requests e em pushes para `main`, faz checkout, configura o SDK .NET 8, restaura dependências, compila em Release, executa os testes xUnit e publica o arquivo TRX como artefato.
   Antes do restore, os dois arquivos Compose também são normalizados por `docker compose config --quiet`; qualquer erro interrompe o job.
2. **Build de imagem:** após sucesso do build/testes em `main`, constrói a imagem com Docker Buildx e publica no GitHub Container Registry (GHCR), com tag pelo SHA do commit e tag `latest`.
3. **Deploy em staging:** em um runner Linux isolado, baixa a imagem exata do commit, sobe API + SQL Server com o projeto Compose `esg-staging`, aguarda os health checks e publica logs/estado dos containers como artefato.
4. **Deploy em produção:** só começa após staging terminar com sucesso. Repete a validação com o projeto Compose `esg-production`, usa a mesma imagem imutável e publica suas próprias evidências.
5. **Deploy remoto opcional:** se a repository variable `REMOTE_DEPLOY_ENABLED` for `true`, o pipeline troca os jobs efêmeros por deploy SSH persistente, com host key verificada e configurações isoladas por GitHub Environment.

Pull requests executam build/testes, mas não publicam imagem nem fazem deploy. Push para `main` publica a imagem e inicia staging e, depois de staging saudável, produção. Os ambientes padrão existem durante os respectivos jobs; servem como deploy automatizado e prova de integração, não como hospedagem pública permanente.

### Execução padrão sem servidor próprio

Nenhum secret manual é necessário. O workflow usa o `GITHUB_TOKEN` para publicar e baixar a imagem. Senha do SQL Server e chave HMAC são geradas aleatoriamente em cada job, mascaradas nos logs e descartadas junto com o runner.

Os GitHub Environments `staging` e `production` são referenciados diretamente pelo workflow. É possível adicionar aprovação manual em **Settings → Environments → production**, embora isso não seja obrigatório para a demonstração automática.

### Configurar deploy remoto persistente (opcional)

1. Defina a repository variable `REMOTE_DEPLOY_ENABLED` como `true` somente quando o servidor estiver preparado.
2. Em **Settings → Environments**, configure os valores abaixo em `staging` e `production`. É possível usar servidores separados; se usar o mesmo host, configure portas de API diferentes e mantenha os projetos Compose isolados.
3. Instale Docker Engine e o plugin Docker Compose v2 no host remoto. O usuário SSH precisa poder executar `docker compose`.
4. Crie um token pessoal do GitHub com escopo `read:packages` para o pull da imagem privada do GHCR; use uma conta que tenha acesso ao pacote.
5. Para produção, configure opcionalmente **Required reviewers** em `Settings → Environments → production` antes de entregar.

**Variables (não secretas) de cada Environment:**

| Nome | Exemplo para staging | Exemplo para produção |
|---|---|---|
| `SSH_HOST` | IP/DNS do servidor de staging | IP/DNS do servidor de produção |
| `SSH_USER` | `deploy` | `deploy` |
| `SSH_PORT` | `22` | `22` |
| `DEPLOY_PATH` | `/opt/esg-staging` | `/opt/esg-production` |
| `APP_PORT` | `8081` | `8080` |

**Secrets de cada Environment:**

| Nome | Conteúdo |
|---|---|
| `SSH_PRIVATE_KEY` | Chave privada dedicada à automação, cuja chave pública foi instalada em `~/.ssh/authorized_keys` do servidor. |
| `SSH_KNOWN_HOSTS` | Linha de host key verificada do servidor (não desabilite a verificação de host). |
| `MSSQL_SA_PASSWORD` | Senha forte do SQL Server daquele ambiente. |
| `AUTH_SIGNING_KEY` | Chave HMAC aleatória de pelo menos 32 bytes, diferente por ambiente. Para facilitar o `.env`, prefira hexadecimal. |
| `GHCR_USERNAME` | Usuário GitHub que tem acesso ao pacote. |
| `GHCR_READ_TOKEN` | Token com permissão `read:packages`. |

O workflow usa o `GITHUB_TOKEN` da própria execução para publicar a imagem. Em servidores remotos, ele usa `GHCR_USERNAME`/`GHCR_READ_TOKEN` para baixá-la. Nunca coloque senhas, chaves privadas ou tokens diretamente no YAML nem no repositório.

As conexões `ssh`/`scp` exigem correspondência com `SSH_KNOWN_HOSTS` (`StrictHostKeyChecking=yes`). O login temporário no GHCR é removido pelo job remoto mesmo se o pull ou a subida dos containers falhar.

Para gerar `SSH_KNOWN_HOSTS`, o comando `ssh-keyscan -p PORTA -H IP_OU_DNS` pode ajudar, mas compare a fingerprint obtida com a fornecida pelo provedor do servidor antes de confiar nela. Gere chaves separadas para staging e produção quando possível.

**Importante:** estas variáveis são exigidas somente no modo remoto. Sem `REMOTE_DEPLOY_ENABLED=true`, o pipeline usa automaticamente os ambientes efêmeros e ignora a configuração SSH. No modo remoto, staging e produção devem usar portas, caminhos e credenciais distintos.

## Containerização

### Dockerfile

O `Dockerfile` usa build multi-stage:

- `mcr.microsoft.com/dotnet/sdk:8.0` para restaurar e publicar a API em Release.
- `mcr.microsoft.com/dotnet/aspnet:8.0` como runtime final, instala `curl` para health checks e executa a aplicação com o usuário não-root fornecido pela imagem .NET.
- Expõe a porta `8080`, definida por `ASPNETCORE_HTTP_PORTS`.

### Orquestração

O `docker-compose.yml` local define:

- **api:** serviço ASP.NET Core, variáveis de ambiente, health check e dependência de banco saudável.
- **db:** SQL Server 2022 Developer, health check por `sqlcmd` e volume persistente.
- **rede `esg-network`:** comunicação interna via nome de serviço `db`.
- **volume `esg_sqlserver_data`:** preserva os dados quando os containers são recriados.

O arquivo `deploy/docker-compose.deploy.yml` usa a imagem publicada no GHCR. Ele é compartilhado pelos jobs efêmeros e pelo modo SSH opcional, sempre separando dados/containers pelos projetos Compose `esg-staging` e `esg-production`.

### Comandos comuns

```bash
# Construir e subir localmente
docker compose up --build -d --wait --wait-timeout 180

# Listar serviços e saúde
docker compose ps

# Acompanhar logs
docker compose logs -f api

# Conferir imagem publicada (substitua OWNER/REPO e TAG)
docker pull ghcr.io/OWNER/REPO:TAG
```

**Limites da demonstração:** os deployments padrão são efêmeros e deixam de existir ao fim de cada job; a comprovação permanece no histórico dos GitHub Environments e nos artefatos. SQL Server Developer é apropriado para desenvolvimento/teste, não para produção comercial. Hospedagem persistente real deve usar o modo SSH opcional ou uma plataforma de nuvem, licença adequada do banco, backup/restore, TLS, monitoramento e rotação de segredos.

## Prints do funcionamento

Ainda não há prints de execução real no pacote inicial, porque a API não foi executada neste ambiente e não há credenciais/servidores associados ao aluno. Para concluir esta seção, capture evidências reais após executar o pipeline e o deploy. Guarde os arquivos em `docs/evidencias/` e use-os aqui:

- `pipeline-build-test.png`: execução do GitHub Actions mostrando build e testes aprovados.
- `ghcr-image.png`: pacote/imagem com a tag do commit publicada no GHCR.
- `staging-health.png`: ambiente staging respondendo HTTP 200 em `/health`.
- `production-health.png`: ambiente produção respondendo HTTP 200 em `/health`.

Não substitua essas evidências por telas simuladas. Instruções detalhadas estão em [`docs/evidencias/README.md`](docs/evidencias/README.md).

## Tecnologias utilizadas

- C# / ASP.NET Core .NET 8
- Entity Framework Core 8 e migrations
- SQL Server 2022
- xUnit / Microsoft.NET.Test.Sdk
- Docker / Docker Compose
- GitHub Actions
- GitHub Container Registry (GHCR)
- OpenAPI / Swagger
- HMAC Bearer authentication

## Checklist de entrega

O status distingue **arquivos preparados** de **execução comprovada**. Atualize o checklist depois de validar o ambiente e coletar os prints.

| Item | Status atual |
|---|---|
| Projeto compactado em ZIP com estrutura organizada | [x] Arquivo gerado para revisão/entrega |
| Dockerfile funcional | [ ] Arquivo criado; build real ainda precisa ser validado |
| `docker-compose.yml` / orquestração | [ ] Arquivo criado; executar `docker compose config` e subir |
| Pipeline com build, testes e deploy | [ ] Workflow configurado; precisa ser executado no GitHub |
| README com instruções e espaço para evidências | [x] Instruções incluídas; prints reais pendentes |
| Documentação técnica PDF | [x] PDF incluído; prints reais pendentes |
| Restore, build Release e testes locais | [x] Executados: 0 erros, 0 avisos e 5/5 testes aprovados |
| Testes aprovados no pipeline | [ ] A comprovar no GitHub Actions |
| Deploy realizado em staging | [ ] A comprovar no job efêmero do GitHub Actions |
| Deploy realizado em produção | [ ] A comprovar após sucesso de staging |
| Prints reais de pipeline, staging e produção anexados | [ ] Pendentes após execução |

**Antes de enviar ao professor:** confirme que os jobs estão verdes, abra os dois ambientes, adicione os prints reais e gere novamente o ZIP final. Se o professor exigir deploy público, inclua URLs acessíveis e não exponha portas/credenciais desnecessariamente.
