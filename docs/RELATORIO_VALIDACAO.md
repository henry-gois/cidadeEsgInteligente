# Relatório de validação

Data da auditoria: 13 de outubro de 2026.

## Resultado neste ambiente

| Verificação | Resultado |
|---|---|
| Inventário de código, migrations, testes, Docker, CI/CD e documentação | Concluído |
| Sintaxe de `.github/workflows/ci-cd.yml` | Aprovada por parser YAML; execução no GitHub pendente |
| Sintaxe de `docker-compose.yml` e `deploy/docker-compose.deploy.yml` | Aprovada por parser YAML; normalização com Docker pendente |
| PDF técnico | 6 páginas extraídas e renderizadas sem corte, sobreposição ou glifos quebrados |
| Varredura de chaves privadas, tokens GitHub/AWS e arquivos `.env` | Nenhum segredo real ou `.env` encontrado |
| `dotnet restore/build/test` | Aprovado com SDK .NET 8.0.426: restore concluído, build Release com 0 erros/0 avisos e 5/5 testes xUnit aprovados |
| `docker compose config`, build da imagem e `/health` | Não executado: Docker não está instalado no ambiente de auditoria |
| GitHub Actions, GHCR e deploy | Repositório público informado; execução ainda pendente do primeiro push. O modo padrão usa staging/produção efêmeros sem servidor próprio |

Os itens marcados como pendentes não devem ser apresentados como aprovados. A validação .NET abaixo já foi executada localmente; repita-a no GitHub Actions e execute as etapas Docker antes da entrega. Anexe somente capturas reais.

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

## Evidências externas ainda necessárias

1. GitHub Actions com restore, build e testes aprovados e o artefato TRX disponível.
2. GHCR mostrando a imagem com a tag imutável igual ao SHA do commit.
3. Staging respondendo HTTP 200 em `/health`.
4. Produção respondendo HTTP 200 em `/health` depois do sucesso de staging.

Os nomes e o local sugeridos para as capturas estão em `docs/evidencias/README.md`.
