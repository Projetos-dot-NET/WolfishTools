Claro! Dominar o terminal (shell) é uma das habilidades que mais aumentam a produtividade de um desenvolvedor. Abaixo está uma lista dos comandos mais úteis, divididos por categoria, com exemplos práticos.

---

### 1. Navegação no Sistema de Arquivos

Esses são os comandos básicos para se mover pelo seu computador.

*   **`pwd` (Print Working Directory)**: Mostra o caminho completo do diretório em que você está.
    ```bash
    pwd
    # Saída: /home/usuario/projetos/meu-app
    ```

*   **`ls` (List)**: Lista os arquivos e diretórios no local atual.
    *   **Dica de ouro:** Use `ls -la` para ver uma lista detalhada (`-l`), incluindo arquivos ocultos (`-a`).
    ```bash
    ls -la
    # drwxr-xr-x  4 usuario staff  128 20 dez 10:30 .
    # drwxr-xr-x 10 usuario staff  320 19 dez 17:00 ..
    # -rw-r--r--  1 usuario staff  351 20 dez 10:30 package.json
    # drwxr-xr-x  5 usuario staff  160 20 dez 10:30 src
    ```

*   **`cd` (Change Directory)**: Muda para outro diretório.
    ```bash
    # Entra no diretório 'src'
    cd src

    # Volta um nível
    cd ..

    # Vai para o diretório 'home' do usuário
    cd ~
    # ou apenas
    cd

    # Volta para o último diretório que você estava
    cd -
    ```

### 2. Manipulação de Arquivos e Diretórios

Criar, mover, copiar e apagar coisas.

*   **`touch`**: Cria um arquivo vazio.
    ```bash
    touch index.js
    ```

*   **`mkdir` (Make Directory)**: Cria um novo diretório.
    *   **Dica:** Use `mkdir -p` para criar um caminho completo de uma só vez.
    ```bash
    # Cria a estrutura 'src/components' mesmo que 'src' não exista
    mkdir -p src/components
    ```

*   **`cp` (Copy)**: Copia arquivos ou diretórios.
    ```bash
    # Copia um arquivo
    cp Dockerfile Dockerfile.bak

    # Copia uma pasta inteira (recursivamente)
    cp -r src/ build/
    ```

*   **`mv` (Move)**: Move ou renomeia um arquivo/diretório.
    ```bash
    # Renomeando um arquivo
    mv old-name.txt new-name.txt

    # Movendo um arquivo para outro diretório
    mv index.js src/
    ```

*   **`rm` (Remove)**: Apaga arquivos. **Use com MUITO cuidado, não tem lixeira!**
    ```bash
    # Apaga um arquivo
    rm temp.log

    # Apaga um diretório e todo o seu conteúdo (recursivo e forçado)
    rm -rf old_project/
    ```

### 3. Visualização e Edição de Conteúdo

Ler o conteúdo de arquivos sem abrir um editor completo.

*   **`cat` (Concatenate)**: Mostra o conteúdo de um arquivo na tela.
    ```bash
    cat package.json
    ```

*   **`less`**: Visualiza arquivos grandes de forma paginada. É mais eficiente que o `cat` para arquivos de log, por exemplo. (Use `q` para sair).
    ```bash
    less server.log
    ```

*   **`head` e `tail`**: Mostram, respectivamente, o início e o fim de um arquivo.
    *   **Dica de ouro:** `tail -f` é essencial para monitorar logs em tempo real.
    ```bash
    # Mostra as 10 últimas linhas de um log
    tail server.log

    # Mostra as 50 últimas linhas
    tail -n 50 server.log

    # Fica "assistindo" o arquivo e mostra novas linhas conforme aparecem
    tail -f application.log
    ```

### 4. Busca e Pesquisa

Encontrar arquivos e texto dentro de arquivos.

*   **`grep` (Global Regular Expression Print)**: Busca por um padrão de texto dentro de arquivos. É um dos comandos mais poderosos.
    ```bash
    # Procura a palavra "error" no arquivo de log
    grep "error" server.log

    # Procura recursivamente (-r) ignorando maiúsculas/minúsculas (-i)
    grep -ri "api_key" .
    ```

*   **`find`**: Encontra arquivos e diretórios com base em nome, tipo, data, etc.
    ```bash
    # Encontra todos os arquivos com a extensão .js no diretório atual e subdiretórios
    find . -name "*.js"

    # Encontra todos os diretórios chamados 'node_modules' e os apaga
    find . -name "node_modules" -type d -exec rm -rf {} +
    ```

### 5. Processos e Rede

Gerenciar o que está rodando na sua máquina.

*   **`ps` (Process Status)**: Lista os processos em execução.
    *   **Dica:** `ps aux` mostra todos os processos de todos os usuários de forma detalhada.
    ```bash
    ps aux

    # Combine com grep para encontrar um processo específico (ex: um servidor Node.js)
    ps aux | grep "node"
    ```

*   **`kill`**: Encerra um processo usando seu ID (PID).
    ```bash
    # Primeiro, encontre o PID com 'ps'
    # Depois, encerre o processo (ex: PID 12345)
    kill 12345

    # Se não funcionar, force o encerramento (sinal 9)
    kill -9 12345
    ```

*   **`curl`**: Ferramenta para fazer requisições HTTP. Essencial para testar APIs.
    ```bash
    # Faz uma requisição GET simples
    curl https://api.github.com/users/octocat

    # Faz um POST com dados JSON
    curl -X POST -H "Content-Type: application/json" -d '{"name":"novo-repo"}' https://api.github.com/user/repos
    ```

*   **`netstat` ou `ss`**: Mostra informações sobre as conexões de rede e portas abertas.
    ```bash
    # Lista todas as portas TCP (-t) e UDP (-u) que estão "ouvindo" (-l)
    netstat -tuln | grep LISTEN
    ```

### 6. "Dicas de Ouro" - Multiplicando o Poder

Estes não são comandos, mas conceitos que combinam os comandos acima.

*   **Pipe `|`**: Encadeia comandos. A saída de um comando se torna a entrada do próximo.
    ```bash
    # Pega a lista de processos, filtra as linhas que contêm "nginx"
    ps aux | grep "nginx"
    ```

*   **Redirecionamento `>` e `>>`**: Salva a saída de um comando em um arquivo.
    ```bash
    # '>' SOBRESCREVE o arquivo
    ls -l > lista_de_arquivos.txt

    # '>>' ADICIONA ao final do arquivo
    echo "Nova linha de log" >> app.log
    ```

*   **`sudo`**: Executa um comando com privilégios de administrador (superuser).
    ```bash
    # Instala um pacote que precisa de permissão de root
    sudo apt-get install nginx
    ```

*   **`&&`**: Executa o segundo comando apenas se o primeiro for bem-sucedido.
    ```bash
    # Executa os testes e, se passarem, faz o deploy
    npm test && npm run deploy
    ```

*   **`history`**: Mostra o histórico de comandos que você digitou.
    *   **Dica:** Use `Ctrl + R` para fazer uma busca reversa no seu histórico. É um salva-vidas!

*   **`alias`**: Cria atalhos (apelidos) para comandos longos.
    ```bash
    # Crie um atalho 'll' para 'ls -la'
    alias ll='ls -la'
    # Agora é só digitar 'll'
    ```

### Ferramentas Adicionais Indispensáveis

*   **`git`**: Sistema de controle de versão. Todo desenvolvedor precisa saber o básico: `git clone`, `git add`, `git commit`, `git push`, `git pull`, `git status`, `git branch`.
*   **`docker` e `docker-compose`**: Para criar e gerenciar ambientes de desenvolvimento em contêineres.
*   **`jq`**: Um processador de JSON para a linha de comando. Fantástico para formatar e extrair dados de respostas de API.
    ```bash
    curl 'https://api.github.com/users/octocat' | jq '.name'
    # Saída: "The Octocat"
    ```

Pratique esses comandos no seu dia a dia e você verá sua velocidade e confiança no desenvolvimento aumentarem drasticamente