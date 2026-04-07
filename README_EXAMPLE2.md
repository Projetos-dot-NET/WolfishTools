# 🐺 Wolfish.Maia (v1.5)

**Wolfish.Maia** é uma plataforma de automação de sistemas e assistência inteligente desenvolvida em **C#** para ambientes **Linux**. O projeto vai além de um simples CLI, estruturando-se como um ecossistema de módulos especializados que integram automação de terminal, IA local acelerada por hardware e serviços de contexto (MCP).

---

## 🏗️ Arquitetura do Sistema (Ecossistema Wolfish)

O Wolfish.Maia é composto por quatro pilares fundamentais, cada um com identidades e responsabilidades técnicas simétricas em Português e Inglês:

### 🧠 MAIA (O Cérebro)
> **EN:** *MAIA Automated Integrated Assistant* > **PT:** *MAIA Assistente Integrada Automatizada*

A **MAIA** é a orquestradora central. Ela gerencia a lógica de decisão, a interface com o usuário e a comunicação com os agentes de IA (como Qwen ou Llama). É a inteligência que decide *o que* deve ser executado.

### 📜 RITA (A Memória)
> **EN:** *Retrieval of Informational Texts & Archives* > **PT:** *Recuperação de Informações e Textos em Arquivos*

A **RITA** atua como um servidor **MCP (Model Context Protocol)**. Sua função é pesquisar em documentações e arquivos, fornecendo o contexto necessário para que a MAIA responda com precisão. É a base de conhecimento do sistema.

### 🚀 CADU (O Músculo Computacional)
> **EN:** *Computational Accelerator for Development & Utilities* > **PT:** *Computação Acelerada para Desenvolvimento e Utilidades*

O **CADU** é o módulo de **Aceleração por GPU**. Enquanto o sistema pode operar via CPU para acessibilidade, o CADU é acionado para transferir o processamento pesado dos LLMs para a placa de vídeo (via CUDA), garantindo alta performance e baixa latência na execução da IA.

### ⚙️ GEO (A Execução)
> **EN:** *Generator of Execution & Operations* > **PT:** *Gerador de Execução e Operações*

A **GEO** é o braço operacional de baixo nível. Ela lida com a manipulação do `ProcessStartInfo`, execução de comandos `sudo` via Standard Input (`-S`) e interação direta com o Kernel Linux para realizar as alterações no sistema.

---

## 🛠️ Fluxo de Trabalho Inteligente

1. **Input:** O usuário solicita uma tarefa complexa à **MAIA**.
2. **Contexto:** A **MAIA** consulta a **RITA** via MCP para entender os parâmetros técnicos da tarefa.
3. **Processamento:** Se uma GPU NVIDIA for detectada, o **CADU** assume o processamento do modelo de linguagem para gerar a solução rapidamente.
4. **Ação:** A **GEO** executa os comandos de terminal necessários com os privilégios devidos.



---

## 🚀 Requisitos e Tecnologias

* **Linguagem:** C# (.NET 8/9)
* **OS:** Linux (Ubuntu/Debian preferencialmente)
* **Aceleração:** NVIDIA GPU com suporte a CUDA (Gerenciado pelo módulo CADU).
* **IA Local:** Suporte a modelos via LLamaSharp / GGUF.

---

## 📝 Licença
Distribuído sob a licença MIT. Veja o arquivo `LICENSE` para detalhes.

---
*Wolfish.Maia: Automação Inteligente, Aceleração Nativa.*