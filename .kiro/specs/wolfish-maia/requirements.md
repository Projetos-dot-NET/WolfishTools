# Requirements Document

## Introduction

Wolfish.Maia (`maia`) é uma ferramenta CLI publicada no NuGet como `dotnet tool`. Seu propósito é ser um assistente de terminal leve que combina automação de comandos do sistema operacional com integração opcional a provedores LLM cloud (OpenAI-compatible). O produto precisa funcionar de forma fluida em Linux e Windows, sem dependência de modelos locais.

## Glossary

| Termo | Definição |
|-------|-----------|
| Quick Shot | Comando de 1 argumento despachado via `CommandRegistry` |
| Clean Shot | Comando de 2 argumentos despachado via `WolfishCommand` + JSON |
| Burst | Comando `ask` com 3+ argumentos para interação com agentes LLM |
| Agent | Configuração de um LLM (nome, provider, modelo, system prompt, modo de histórico) definida em `cloudagents.json` |
| Provider | Configuração de endpoint + API key de um serviço LLM em `appsettings.json` |
| History Mode | Modo de memória do agente: `none`, `self`, `global` |

---

## Requirements

### Requirement 1: Despacho de Comandos por Camadas

**User Story:** Como usuário do terminal, quero que o Maia identifique automaticamente o tipo de comando pela quantidade de argumentos, para que eu não precise aprender uma sintaxe complexa.

#### Acceptance Criteria

1. WHEN o usuário executa `maia` sem argumentos, THEN o sistema SHALL exibir a mensagem de ajuda (equivalente a `maia help`).
2. WHEN o usuário executa `maia <verb>` com exatamente 1 argumento, THEN o sistema SHALL buscar o verbo no `CommandRegistry` e executá-lo se encontrado.
3. WHEN o usuário executa `maia <verb> <target>` com exatamente 2 argumentos e o verbo não for encontrado no `CommandRegistry`, THEN o sistema SHALL buscar no `WolfishCommand` via `TerminalCommands.json`.
4. WHEN o usuário executa `maia ask <agent> <text...>` com 3 ou mais argumentos e o primeiro argumento for `ask`, THEN o sistema SHALL despachar para o `AskCommand`.
5. WHEN nenhuma camada resolve o comando, THEN o sistema SHALL exibir `"Unknown command! Try Again!"` sem lançar exceção.

#### Correctness Properties

- **P1.1 (Completeness):** Para qualquer entrada com 1, 2 ou 3+ args, existe exatamente uma camada responsável por tentar resolver.
- **P1.2 (No-Throw):** O programa nunca termina com exceção não tratada independente dos argumentos fornecidos.

---

### Requirement 2: Comandos Quick Shot

**User Story:** Como desenvolvedor, quero comandos de atalho de 1 argumento para ações frequentes como ver ajuda, listar comandos e informações do sistema.

#### Acceptance Criteria

1. WHEN o usuário executa `maia help`, THEN o sistema SHALL exibir a lista de comandos disponíveis com descrição.
2. WHEN o usuário executa `maia welcome`, THEN o sistema SHALL exibir uma mensagem de boas-vindas com o nome MAIA e versão.
3. WHEN o usuário executa `maia list`, THEN o sistema SHALL listar todos os comandos registrados no `CommandRegistry` e no `TerminalCommands.json`.
4. WHEN o usuário executa `maia info`, THEN o sistema SHALL exibir informações do sistema (OS, .NET runtime, diretório base).
5. WHEN o usuário executa `maia home`, THEN o sistema SHALL exibir o diretório base da instalação da ferramenta.
6. WHEN o usuário executa `maia config`, THEN o sistema SHALL exibir ou guiar a configuração dos arquivos `cloudagents.json` e `appsettings.json`.
7. EACH Quick Shot command SHALL complete in under 500ms (sem I/O externo).

---

### Requirement 3: Integração com Agentes LLM via `ask`

**User Story:** Como usuário, quero enviar perguntas a agentes LLM configurados localmente, para que eu possa usar diferentes modelos e providers sem mudar o comando.

#### Acceptance Criteria

1. WHEN o usuário executa `maia ask <agentName> <question...>`, THEN o sistema SHALL carregar o agente de nome `agentName` do `cloudagents.json`.
2. WHEN o agente é encontrado, THEN o sistema SHALL carregar o provider correspondente do `appsettings.json` usando o campo `ProviderName`.
3. WHEN o provider não for encontrado, THEN o sistema SHALL exibir mensagem de erro indicando o provider ausente sem lançar exceção.
4. WHEN o agente não for encontrado, THEN o sistema SHALL exibir `"Agent '<name>' not found."`.
5. WHEN o usuário usa `maia ask all <question...>`, THEN o sistema SHALL enviar a pergunta para todos os agentes configurados em sequência.
6. WHEN o agente tem `History: "self"`, THEN o sistema SHALL carregar e salvar o histórico em `history-{agentName}.json` no diretório corrente.
7. WHEN o agente tem `History: "global"`, THEN o sistema SHALL carregar todos os arquivos `history-*.json` do diretório corrente antes de enviar.
8. WHEN o agente tem `History: "none"`, THEN o sistema SHALL não persistir nem carregar histórico algum.
9. WHEN a resposta do LLM é recebida via streaming, THEN o sistema SHALL escrever a resposta completa em um arquivo `ask-{agentName}-{timestamp}.md` no diretório corrente.
10. WHEN o modelo utilizado é identificado na resposta, THEN o sistema SHALL incluir o nome do modelo no arquivo de saída no formato `> **Modelo utilizado:** \`{model}\``.
11. IF a chamada ao LLM falhar, THEN o sistema SHALL exibir `"Error: {mensagem}"` sem encerrar o processo com código de erro diferente de zero.

#### Correctness Properties

- **P3.1 (Agent Resolution):** `SearchAgentByName(name, agents)` retorna `null` se e somente se nenhum agente tem `Name` igual a `name` (case-insensitive).
- **P3.2 (History Isolation):** Modo `none` nunca lê nem escreve arquivos de histórico.
- **P3.3 (Output Persistence):** Toda resposta bem-sucedida gera exatamente um arquivo `.md` por agente.

---

### Requirement 4: Configuração por Arquivo JSON

**User Story:** Como usuário avançado, quero configurar agentes e providers via arquivos JSON sem recompilar a ferramenta, para ter flexibilidade na escolha de modelos e endpoints.

#### Acceptance Criteria

1. WHEN o `cloudagents.json` não existe no diretório base da ferramenta, THEN o sistema SHALL lançar erro informativo indicando o arquivo ausente.
2. WHEN o `appsettings.json` não existe, THEN o sistema SHALL lançar erro informativo.
3. WHEN um provider tem `ApiKey` vazio ou placeholder, THEN o sistema SHALL permitir a execução mas o LLM pode recusar a requisição.
4. WHEN o `cloudagents.json` contém agentes duplicados (mesmo `Name`), THEN o sistema SHALL usar o primeiro encontrado.
5. THE `cloudagents.json` distribuído com o pacote SHALL conter apenas valores placeholder (sem API keys, sem endpoints reais).
6. THE `appsettings.json` distribuído com o pacote SHALL conter apenas estrutura template.

#### Correctness Properties

- **P4.1 (Config Immutability):** A leitura de configuração não modifica os arquivos JSON.
- **P4.2 (No Secret Leak):** Nenhuma API key presente no `appsettings.json` é impressa em stdout ou stderr.

---

### Requirement 5: Compatibilidade de Plataforma

**User Story:** Como usuário Linux ou Windows, quero que o Maia funcione igualmente bem em ambas as plataformas, para que eu possa usar a ferramenta no meu ambiente preferido.

#### Acceptance Criteria

1. THE tool SHALL be published targeting `net8.0`, `net10.0`, and `net11.0`.
2. WHEN executado no Linux, THEN o sistema SHALL usar `TerminalCommands.Linux.json` para os Clean Shots específicos de plataforma.
3. WHEN executado no Windows, THEN o sistema SHALL usar `TerminalCommands.Windows.json`.
4. ALL file path operations SHALL use `Path.Combine` or equivalent cross-platform APIs, never hardcoded separators.
5. THE output `.md` files SHALL use UTF-8 encoding.

#### Correctness Properties

- **P5.1 (Cross-platform paths):** Nenhum path hardcodado com `\` ou `/` — sempre via `Path.Combine`.

---

### Requirement 6: Empacotamento e Distribuição NuGet

**User Story:** Como usuário, quero instalar o Maia com um único comando `dotnet tool install`, para que a instalação seja simples e sem dependências externas além do .NET SDK.

#### Acceptance Criteria

1. THE `Wolfish.Maia.csproj` SHALL have `<PackAsTool>true</PackAsTool>` and `<ToolCommandName>maia</ToolCommandName>`.
2. WHEN `dotnet pack -c Release` is executed, THEN a `.nupkg` file SHALL be generated in `./nupkg`.
3. THE package SHALL include `README.md` and `LICENSE.txt` at the package root.
4. THE package SHALL include all JSON list files (`TerminalCommands.json`, `cloudagents.json`, etc.) as content copied to the output directory.
5. WHEN a new version is released, THEN `Version.props` SHALL be updated before packing.
6. THE package SHALL NOT include PDB files or debug symbols in Release configuration.

#### Correctness Properties

- **P6.1 (Installability):** O pacote gerado pode ser instalado via `dotnet tool install --global` sem erros adicionais.
- **P6.2 (Version Consistency):** A versão no `.nupkg` corresponde ao valor em `Version.props`.
