# 🐺 WolfishTools

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.txt)
[![NuGet](https://img.shields.io/nuget/v/wolfish.maia.svg?label=wolfish.maia)](https://www.nuget.org/packages/wolfish.maia)
[![Downloads](https://img.shields.io/nuget/dt/wolfish.maia.svg)](https://www.nuget.org/packages/wolfish.maia)

WolfishTools é um ecossistema de ferramentas .NET para desenvolvedores, focado em automação de terminal, integração com modelos de linguagem (LLMs) e recuperação de informações. O repositório agrupa um CLI publicado no NuGet, bibliotecas de suporte e projetos experimentais em idealização.

---

## Projetos

### ✅ Wolfish.Maia — CLI publicado no NuGet

> **MAIA** — *MAIA Automated Integrated Assistant* (acrônimo recursivo)

O produto principal. Um assistente de terminal leve instalável com um único comando, sem dependência de servidores locais. Conecta-se a qualquer provedor de LLM compatível com a API OpenAI (OpenRouter, LM Studio, GitHub Copilot, Gemini, etc.) e automatiza tarefas do sistema operacional via comandos extensíveis por JSON.

```bash
dotnet tool install --global wolfish.maia
```

[→ Documentação completa do Maia](Wolfish.Maia/README.md)

---

### 📚 Bibliotecas de Suporte

Estas libs são as dependências internas do Maia. Evoluem junto com o produto principal.

#### Wolfish.ChatAgent

Abstração de agente de chat sobre APIs OpenAI-compatible. Oferece:
- `OpenAiAgent` — envio de mensagens com streaming (`IAsyncEnumerable`)
- `AgentHistory` — persistência de histórico de conversas em JSON local

#### Wolfish.Shared

DTOs compartilhados entre projetos:
- `CloudAgent` — representa um agente configurado (nome, modelo, provider, history mode)
- `LlmProvider` — configuração de endpoint + API key
- `LlamaSettings` — schema de configuração para integração com modelos Llama locais

#### Wolfish.Commands

Infraestrutura de comandos do Maia:
- `WolfishCommand` — carrega e executa comandos via `TerminalCommands.json`
- `AgentCommand` — helper para comandos de agentes
- `IssueManagerCommand` — utilitário para criar GitHub Issues direto do terminal
- `TerminalCommandDto` / `StepCommand` — DTOs para descrição de comandos

#### Wolfish.CloudAgents

Modelos de configuração para agentes cloud. Espelha parcialmente o `Wolfish.Shared`; será a fonte única de verdade em versões futuras.

---

### 🧪 Projetos Experimentais

Em idealização e desenvolvimento. Sem SLA de estabilidade.

#### Wolfish.Rita — A Memória

> **RITA** — *Retrieval of Informational Texts & Archives*

Armazenamento vetorial local usando SQLite + EF Core. Persiste documentos com seus embeddings (`float[]`) para recuperação semântica. Base de conhecimento para o Cadu.

- `DocumentRecord` — entidade com conteúdo textual e embedding vetorial
- `AppDbContext` — DbContext configurável por herança

#### Wolfish.Cadu — O Acelerador

> **C.A.D.U.** — *Computational Accelerator for Development & Utilities*

Agente RAG corporativo local. Combina busca semântica por cosine similarity (via embeddings Nomic) com geração de texto via LLamaSharp (Qwen2.5 GGUF). Opera em CPU; suporte a GPU NVIDIA via CUDA planejado.

**Fluxo:** pergunta → embedding → top-3 chunks da Rita → prompt injetado → resposta streaming via LLM local.

#### Wolfish.Llama — Wrapper LLamaSharp

Abstração reutilizável para modelos Llama locais via LLamaSharp:
- `LlamaService` — inicialização e inferência
- `LlamaHistory` — persistência de histórico

#### Wolfish.Gemini — Wrapper Google Gemini

Integração com a API do Google Gemini. Alternativa cloud para os projetos experimentais.

#### Wolfish.ServerMcp / Wolfish.AgentClientMcp — Model Context Protocol

> **MCP** — *Model Context Protocol*

Servidor e cliente MCP para comunicação entre agentes de IA. O `ServerMcp` expõe ferramentas e recursos; o `AgentClientMcp` os consome.

#### Wolfish.Core

Biblioteca núcleo, atualmente em estágio de placeholder. Destinada a conter funcionalidades fundamentais compartilhadas por toda a solução.

---

## Arquitetura da Solução

```
maia (CLI)
├── Wolfish.ChatAgent    ← streaming + histórico
├── Wolfish.Commands     ← execução de comandos via JSON
└── Wolfish.Shared       ← DTOs

Projetos Experimentais
├── Wolfish.Rita         ← vetor store (SQLite)
├── Wolfish.Cadu         ← RAG local (LLamaSharp + Rita)
├── Wolfish.Llama        ← wrapper LLamaSharp
├── Wolfish.Gemini       ← wrapper Gemini API
├── Wolfish.ServerMcp    ← servidor MCP
└── Wolfish.AgentClientMcp ← cliente MCP
```

O Maia não depende de LLamaSharp, EF Core ou projetos experimentais. A separação é intencional para manter o pacote NuGet leve.

---

## Glossário

| Sigla | Expansão | Papel no ecossistema |
|-------|----------|----------------------|
| MAIA | *MAIA Automated Integrated Assistant* | CLI principal (cérebro) |
| RITA | *Retrieval of Informational Texts & Archives* | Memória vetorial |
| C.A.D.U. | *Computational Accelerator for Development & Utilities* | Aceleração computacional / RAG local |
| GEO | *Generator of Execution & Operations* | Execução de baixo nível no kernel (planejado) |
| MCP | *Model Context Protocol* | Protocolo de comunicação entre agentes |

---

## Tecnologias

- **Linguagem:** C# / .NET 10 (multi-target: net8.0, net10.0, net11.0)
- **Distribuição:** NuGet (`dotnet tool`)
- **LLM cloud:** qualquer endpoint OpenAI-compatible
- **LLM local:** LLamaSharp + modelos GGUF (Qwen2.5, Nomic Embed)
- **Persistência:** SQLite via EF Core (projetos experimentais)
- **Plataformas:** Linux e Windows

---

## Contribuindo

Contribuições são bem-vindas. Para mudanças no Maia (produto publicado), abra uma issue antes para alinhar o escopo. Para projetos experimentais, PRs diretos são encorajados.

1. Fork o repositório
2. Crie uma branch (`feature/minha-feature`)
3. Commit e abra um Pull Request

---

## Licença

MIT — veja [LICENSE.txt](LICENSE.txt).

---

*Wolfish Studio — [wolfishstudio.github.io](https://wolfishstudio.github.io/tools/pages/home.html)*
