winget install GitHub.cli

gh auth login

gh pr create

gh pr create --title "Minha Feature" --body "Adiciona X e Y" --base master --head developer

gh repo clone OWNER/REPO
# Ex: gh repo clone microsoft/vscode

gh pr list

gh pr checkout 123  # Onde 123 é o número do PR

gh release create v1.0.0 --notes "Lançamento oficial"