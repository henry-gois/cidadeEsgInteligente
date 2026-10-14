# Evidências reais coletadas

Capturas originais da execução `#2` do GitHub Actions, commit `608c0ff98b957e5cabcfc00107c6951bb8c40537`, realizada em 13/10/2026. Nenhuma captura contém senhas, tokens ou chaves privadas.

1. `pipeline-sucesso.png` — fluxo completo aprovado: build/testes, imagem Docker, staging e produção.
2. `ghcr-imagem-publicada.png` — pacote público GHCR com as tags `latest` e SHA imutável.
3. `staging-health.png` — job de staging aprovado, incluindo a etapa "Subir staging e validar prontidão".
4. `producao-health.png` — job de produção aprovado, incluindo a etapa "Subir produção e validar prontidão".

Execução: <https://github.com/henry-gois/cidadeEsgInteligente/actions/runs/38491269652>

Imagem: <https://github.com/henry-gois/cidadeEsgInteligente/pkgs/container/cidadeesginteligente>

Os health checks usam `curl --fail` contra `/health`; os jobs só ficam verdes quando a API responde e o banco SQL Server está acessível. O resumo de cada job registrou `HTTP 200 - {"status":"healthy"}`.
