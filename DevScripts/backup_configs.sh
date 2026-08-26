#!/bin/bash

# ============================================================
# Script de Backup - appsettings.json e cloudagents.json
# Cria cópias com timestamp na pasta DevScripts/backups/
# ============================================================

# Diretório base do projeto (relativo ao DevScripts)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SOURCE_DIR="$PROJECT_ROOT/Wolfish.Maia"
BACKUP_DIR="$SCRIPT_DIR/backups"

# Arquivos para backup
FILES=("appsettings.json" "cloudagents.json")

# Timestamp para o nome do backup
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")

echo ""
echo "============================================"
echo "  Backup de Configurações - Wolfish.Maia"
echo "============================================"
echo ""
echo "  Origem:  $SOURCE_DIR"
echo "  Destino: $BACKUP_DIR"
echo "  Data:    $(date +"%d/%m/%Y %H:%M:%S")"
echo ""

# Cria o diretório de backups se não existir
mkdir -p "$BACKUP_DIR"

# Contador de arquivos copiados
COPIED=0
ERRORS=0

for FILE in "${FILES[@]}"; do
    SOURCE_FILE="$SOURCE_DIR/$FILE"
    
    if [ -f "$SOURCE_FILE" ]; then
        # Nome do backup: appsettings_2026-08-24_13-48-00.json
        BASENAME="${FILE%.json}"
        BACKUP_FILE="$BACKUP_DIR/${BASENAME}_${TIMESTAMP}.json"
        
        cp "$SOURCE_FILE" "$BACKUP_FILE"
        
        if [ $? -eq 0 ]; then
            SIZE=$(stat --printf="%s" "$BACKUP_FILE" 2>/dev/null || stat -f%z "$BACKUP_FILE" 2>/dev/null)
            echo "  ✅ $FILE → $(basename "$BACKUP_FILE") ($SIZE bytes)"
            ((COPIED++))
        else
            echo "  ❌ Erro ao copiar $FILE"
            ((ERRORS++))
        fi
    else
        echo "  ⚠️  Arquivo não encontrado: $FILE"
        ((ERRORS++))
    fi
done

echo ""

# Mostra resumo dos backups existentes
BACKUP_COUNT=$(ls -1 "$BACKUP_DIR"/*.json 2>/dev/null | wc -l)
echo "────────────────────────────────────────────"
echo "  Resumo: $COPIED arquivo(s) copiado(s), $ERRORS erro(s)"
echo "  Total de backups na pasta: $BACKUP_COUNT arquivo(s)"
echo "────────────────────────────────────────────"

# Limpeza automática: mantém apenas os últimos 20 backups de cada arquivo
MAX_BACKUPS=20
for FILE in "${FILES[@]}"; do
    BASENAME="${FILE%.json}"
    PATTERN="$BACKUP_DIR/${BASENAME}_*.json"
    COUNT=$(ls -1 $PATTERN 2>/dev/null | wc -l)
    
    if [ "$COUNT" -gt "$MAX_BACKUPS" ]; then
        REMOVE_COUNT=$((COUNT - MAX_BACKUPS))
        echo ""
        echo "  🧹 Limpando $REMOVE_COUNT backup(s) antigo(s) de $FILE..."
        ls -1t $PATTERN | tail -n "$REMOVE_COUNT" | xargs rm -f
    fi
done

echo ""

if [ $ERRORS -eq 0 ]; then
    echo "  ✅ Backup concluído com sucesso!"
else
    echo "  ⚠️  Backup concluído com $ERRORS erro(s)."
fi

echo ""
