# 🐺 Wolfish.Maia (v1.5)

**Wolfish.Maia** é uma ferramenta de linha de comando (CLI) desenvolvida em **C#** para automação de tarefas e gerenciamento de sistemas no ecossistema **Linux**. O projeto utiliza uma arquitetura modular baseada em "Assistentes Especializados" para garantir que a automação seja inteligente, segura e persistente.

---

## 🏗️ Arquitetura do Sistema (Ecossistema Maia)

O coração do Wolfish.Maia é dividido em três módulos principais, cada um com responsabilidades bilíngues (Português/Inglês) e funções técnicas específicas:

### 🧠 MAIA
> **EN:** *MAIA Automated Integrated Assistant* > **PT:** *MAIA Assistente Integrada Automatizada*

A **MAIA** atua como o **Cérebro** da aplicação. Ela é responsável pela orquestração de alto nível, lógica de decisão e interface direta com o usuário. É a MAIA quem define o fluxo de trabalho e valida as intenções do operador.

### 📜 RITA
> **EN:** *Retrieval of Informational Texts & Archives* > **PT:** *Recuperação de Informações, Textos e Arquivos*

A **RITA** é a **Memória** do sistema. Ela gerencia a persistência de dados, leitura de arquivos de configuração, logs de depuração e histórico de comandos. Com a RITA, o estado da aplicação é preservado entre diferentes sessões.

### ⚙️ GEO
> **EN:** *Generator of Execution & Operations* > **PT:** *Gerador de Execução e Operações*

A **GEO** é o **Músculo** do projeto. Especializada na comunicação de baixo nível com o Kernel Linux, ela gerencia a manipulação do `ProcessStartInfo` e a execução de comandos que exigem privilégios elevados (`sudo`). É a GEO quem lida com o redirecionamento de streams e entrada de senhas de forma segura.

---

## 🛠️ Tecnologias Utilizadas

* **Runtime:** .NET 8 / .NET 9
* **Linguagem:** C#
* **Ambiente Alvo:** Linux (Ubuntu, Debian, Fedora, etc.)
* **Bibliotecas Chave:** * `System.Diagnostics.Process` para integração com o Kernel.
    * `CliWrap` (opcional/recomendado) para automação de terminal.

---

## 🚀 Como Executar

### Pré-requisitos
* .NET SDK instalado.
* Permissões de execução no terminal Linux.

### Instalação
Clone o repositório e compile o projeto:
```bash
git clone [https://github.com/seu-usuario/wolfish-maia.git](https://github.com/seu-usuario/wolfish-maia.git)
cd wolfish-maia
dotnet build