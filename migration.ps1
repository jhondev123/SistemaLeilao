<#
.SYNOPSIS
    CLI de Migrations para o Sistema de Leilão.
.DESCRIPTION
    Uso: .\migration.ps1 [comando] [nome]
#>

param(
    [ValidateSet("add", "update", "remove", "list")]
    [string]$Action = "help",
    
    [string]$Name = ""
)

# Configuração de caminhos baseada na sua estrutura
$INFRA_PROJ = "SistemaLeilao.Infrastructure"
$API_PROJ = "SistemaLeilao.API"

function Show-Banner {
    Write-Host "`n---------------------------------------------------" -ForegroundColor Cyan
    Write-Host "   🔨 SISTEMA DE LEILÃO - EF CORE CLI" -ForegroundColor Cyan
    Write-Host "---------------------------------------------------" -ForegroundColor Cyan
}

function Run-Command {
    param($Cmd)
    Write-Host "`n🚀 Executando: $Cmd" -ForegroundColor Yellow
    Invoke-Expression $Cmd
}

Show-Banner

switch ($Action) {
    "add" {
        if ([string]::IsNullOrWhiteSpace($Name)) {
            Write-Host "❌ ERRO: Você precisa fornecer um nome para a migration." -ForegroundColor Red
            Write-Host "Exemplo: .\migration.ps1 add NomeDaMigration" -ForegroundColor Gray
            break
        }
        Run-Command "dotnet ef migrations add $Name --project $INFRA_PROJ --startup-project $API_PROJ"
    }

    "update" {
        Run-Command "dotnet ef database update --project $INFRA_PROJ --startup-project $API_PROJ"
        Write-Host "`n✅ Banco de dados atualizado com sucesso!" -ForegroundColor Green
    }

    "remove" {
        Write-Host "⚠️  Removendo a última migration ainda não aplicada..." -ForegroundColor Yellow
        Run-Command "dotnet ef migrations remove --project $INFRA_PROJ --startup-project $API_PROJ"
    }

    "list" {
        Run-Command "dotnet ef migrations list --project $INFRA_PROJ --startup-project $API_PROJ"
    }

    "help" {
        Write-Host "Comandos disponíveis:" -ForegroundColor White
        Write-Host "  add [nome]  - Cria uma nova migration"
        Write-Host "  update      - Aplica as migrations ao banco de dados"
        Write-Host "  remove      - Remove a última migration (se não aplicada)"
        Write-Host "  list        - Lista todas as migrations"
        Write-Host "`nExemplo: .\leilao.ps1 add InitialCreate" -ForegroundColor Gray
    }
}

Write-Host "`n---------------------------------------------------`n" -ForegroundColor Cyan