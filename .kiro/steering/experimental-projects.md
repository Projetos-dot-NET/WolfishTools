---
inclusion: manual
---

# Projetos Experimentais — Rita, Cadu, Llama, Gemini, MCP

Estes projetos estão em fase de idealização/experimento. Não têm SLA de estabilidade.

## Wolfish.Rita — Armazenamento Vetorial

**Propósito:** Base de dados vetorial local usando SQLite + EF Core.

- `DocumentRecord` — entidade com `TextContent` (string) e `Embedding` (float[], serializado como JSON no SQLite)
- `AppDbContext` — DbContext base, configurável por herança (padrão: `app.db` local)
- `RetrievalCount` — contador de recuperações, usado pelo Cadu para evitar garbage collection de registros relevantes

**Uso típico:**
```
Rita popula o banco → Cadu consulta via CaduDbContext (aponta para ../Wolfish.Rita/app.db)
```

## Wolfish.Cadu — Agente RAG Local

**Propósito:** Agente corporativo que combina busca semântica (embeddings Nomic) + geração de texto (Qwen2.5/LLamaSharp) em modo RAG.

**Fluxo:**
1. Conecta ao banco da Rita
2. Carrega modelo embedder (Nomic GGUF) para busca semântica
3. Carrega modelo LLM (Qwen2.5 GGUF) para geração
4. Loop de chat: pergunta → embedding → cosine similarity → top-3 chunks → prompt injetado → resposta streaming

**Modelos esperados (caminhos locais — não commitar):**
- Embedder: `nomic-embed-text-v1.5.Q8_0.gguf`
- LLM: `qwen2.5-1.5b-instruct-q8_0.gguf`

**GPU:** Suporte futuro via `LLamaSharp.Backend.Cuda12` (atualmente CPU).

## Wolfish.Llama — Wrapper LLamaSharp

**Propósito:** Abstração reutilizável para modelos Llama locais.

- `LlamaService` — inicialização e inferência
- `LlamaHistory` — persistência de histórico de conversas
- `HistoryMessage` — DTO de mensagem

## Wolfish.Gemini — Wrapper Google Gemini

**Propósito:** Integração com a API do Google Gemini (alternativa cloud aos provedores OpenAI-compatíveis).

## Wolfish.ServerMcp / Wolfish.AgentClientMcp — Model Context Protocol

**Propósito:** Implementação do protocolo MCP para comunicação entre agentes.
- `ServerMcp` — expõe ferramentas/recursos via MCP
- `AgentClientMcp` — cliente que consome servidores MCP

## Regras para Projetos Experimentais

1. Caminhos de modelos GGUF são **locais** — nunca hardcodar caminhos de outros usuários
2. `app.db` da Rita não deve ser commitado (dados reais)
3. Ao evoluir Rita/Cadu para produto, seguir o mesmo padrão de versionamento do Maia
4. LLamaSharp só entra em projetos experimentais — **nunca no Maia**
