# Relatório de validação

Data da auditoria: 13 de outubro de 2026.

## Resultado neste ambiente

| Verificação | Resultado |
|---|---|
| Inventário de código, migrations, testes, Docker, CI/CD e documentação | Concluído |
| Sintaxe e execução de `.github/workflows/ci-cd.yml` | Aprovadas; execução `#2` terminou com Success em 3m42s |
| `docker-compose.yml` e `deploy/docker-compose.deploy.yml` | Validados e usados para subir API + SQL Server nos jobs Linux |
| PDF técnico | 6 páginas extraídas e renderizadas sem corte, sobreposição ou glifos quebrados |
| Varredura de chaves privadas, tokens GitHub/AWS e arquivos `.env` | Nenhum segredo real ou `.env` encontrado |
| `dotnet restore/build/test` | Aprovado com SDK .NET 8.0.426: restore concluído, build Release com 0 erros/0 avisos e 5/5 testes xUnit aprovados |
| Build da imagem e publicação | Aprovados no GitHub Actions; imagem pública no GHCR com tags `latest` e `608c0ff...` |
| Staging | Aprovado em 55s; API + SQL Server e `/health` HTTP 200 |
| Produção | Aprovada em 58s depois de staging; API + SQL Server e `/health` HTTP 200 |

Validação comprovada em <https://github.com/henry-gois/cidadeEsgInteligente/actions/runs/38491269652>. A imagem está em <https://github.com/henry-gois/cidadeEsgInteligente/pkgs/container/cidadeesginteligente>. Os deployments padrão são reais e efêmeros: permanecem ativos durante cada job e são removidos ao final.

## Comandos para repetir a validação

```bash
dotnet restore EsgEnergy.sln
dotnet build EsgEnergy.sln --configuration Release --no-restore
dotnet test EsgEnergy.sln --configuration Release --no-build
docker compose --env-file .env.example config --quiet
docker compose up --build -d --wait --wait-timeout 180
curl --fail http://localhost:8080/health
docker compose ps
docker compose down
```

O último comando preserva o volume. Não use `docker compose down -v` sem intenção explícita de apagar o banco local.

## Correções aplicadas na auditoria

- Os scripts locais passaram a fornecer `.env.example` na validação do Compose.
- O job de CI passou a validar tanto o Compose local quanto o de deploy antes de restaurar/compilar a solução.
- SSH e SCP passaram a exigir explicitamente a host key registrada em `SSH_KNOWN_HOSTS`.
- O deploy remoto passou a remover o login temporário do GHCR também quando `pull` ou `up` falhar.
- O pipeline ganhou deployments efêmeros padrão para staging e produção, eliminando a necessidade de servidor próprio para a demonstração.
- A migration inicial recebeu os tipos SQL explícitos nos `InsertData`; isso corrigiu a falha encontrada no primeiro deploy com SQL Server real.

## Evidências anexadas

1. GitHub Actions com restore, build e 5/5 testes aprovados e artefato TRX disponível.
2. GHCR mostrando a imagem com tag imutável igual ao SHA do commit e tag `latest`.
3. Staging aprovado com a etapa de prontidão e `/health` HTTP 200.
4. Produção aprovada depois de staging, com `/health` HTTP 200.

As capturas originais e seus detalhes estão em `docs/evidencias/`.
