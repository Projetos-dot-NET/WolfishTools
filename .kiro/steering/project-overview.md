# WolfishTools — Visão Geral da Solução

## O que é

WolfishTools é um ecossistema de ferramentas .NET 10 voltado para automação de terminal, integração com modelos de linguagem (LLMs) e recuperação de informações. O repositório contém um produto publicado no NuGet (`wolfish.maia`) e outros projetos em idealização ou em fase de biblioteca de suporte.

## Hierarquia dos Projetos

### ✅ Produto Publicado

| Projeto | Tipo | Status |
|---------|------|--------|
| `Wolfish.Maia` | CLI tool (`dotnet tool install --global wolfish.maia`) | **Publicado no NuGet** |

### 📚 Bibliotecas de Suporte (usadas pelo Wolfish.Maia)

| Projeto | Papel |
|---------|-------|
| `Wolfish.ChatAgent` | Abstração de histórico de conversa e agente OpenAI-compatível |
| `Wolfish.Shared` | DTOs compartilhados (`CloudAgent`, `LlmProvider`, `LlamaSettings`) |
| `Wolfish.Commands` | Carregamento de comandos via JSON, helpers de terminal e GitHub Issues |
| `Wolfish.CloudAgents` | Modelos de configuração de agentes cloud (não dependência direta do Maia hoje) |

### 🧪 Projetos em Idealização / Experimento

| Projeto | Conceito |
|---------|---------- |
| `Wolfish.Rita` | Armazenamento vetorial local (SQLite + EF Core + embeddings float[]) — RITA = Retrieval of Informational Texts & Archives |
| `Wolfish.Cadu` | Agente RAG corporativo que consome a base da Rita e usa LLamaSharp para inferência local — C.A.D.U. = Computational Accelerator for Development and Utilities |
| `Wolfish.Llama` | Wrapper para modelos Llama via LLamaSharp (histórico + serviço) |
| `Wolfish.Gemini` | Wrapper para Google Gemini |
| `Wolfish.ServerMcp` | Servidor MCP (Model Context Protocol) |
| `Wolfish.AgentClientMcp` | Cliente MCP para agentes |
| `Wolfish.Core` | Core library placeholder |

## Regras Fundamentais

1. **Wolfish.Maia é o produto principal** — qualquer mudança que afete o pacote NuGet deve ser tratada com cuidado redobrado (versionamento semântico, backward compatibility).
2. **Bibliotecas de suporte** (`Wolfish.ChatAgent`, `Wolfish.Shared`, `Wolfish.Commands`) são dependências internas e podem evoluir junto com o Maia.
3. **Projetos em idealização** não têm SLA de estabilidade — experimentos são bem-vindos.
4. **O Maia não depende de LLamaSharp diretamente** — inferência local é responsabilidade do Cadu/Llama. O Maia usa apenas provedores cloud (OpenAI-compatible endpoints).
5. **Configuração por JSON** — `cloudagents.json` e `appsettings.json` são o mecanismo de configuração do Maia. Nunca commitar API keys.

## Linguagem e Plataforma

- **Linguagem:** C# / .NET 10 (multi-target: net8.0, net10.0, net11.0)
- **SO alvo:** Linux e Windows
- **Paradigma:** CLI-first, sem UI gráfica

## Glossário de Acrônimos

| Sigla | Expansão | Descrição |
|-------|----------|-----------|
| **MAIA** | *MAIA Automated Integrated Assistant* (acrônimo recursivo) | CLI principal, publicada no NuGet |
| **RITA** | *Retrieval of Informational Texts & Archives* | Armazenamento vetorial local |
| **C.A.D.U.** | *Computational Accelerator for Development and Utilities* | Agente RAG corporativo local |
| **GEO** | *módulo de interação kernel de baixo nível* | Módulo futuro para interações de sistema operacional de baixo nível |
| **LLM** | *Large Language Model* | Modelo de linguagem grande (GPT, Gemini, Qwen, etc.) |
| **MCP** | *Model Context Protocol* | Protocolo para comunicação entre agentes de IA |

## Processo de Desenvolvimento Orientado a Spec

O WolfishTools adota spec-driven development. Para qualquer nova feature ou mudança:

1. **Criar/atualizar spec** — escrever o documento de requisitos ou design antes do código
2. **Derivar implementação** — escrever código que satisfaça a spec (a spec é a fonte da verdade)
3. **Validar** — rodar testes e verificar que o comportamento bate com a spec
4. **Versionar junto** — commitar spec e implementação na mesma unidade de trabalho
5. **CI** — um step de CI pode fazer lint da spec e falhar o build se divergir do código

As specs vivem em `.kiro/specs/{feature-name}/` e os steering files em `.kiro/steering/`.
