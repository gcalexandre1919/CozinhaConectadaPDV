# 📁 Organização do Projeto - Cozinha Conectada

## ✅ O que foi reorganizado

### 🗑️ Arquivos removidos (duplicados)
- Scripts duplicados da raiz do projeto
- Pasta `SistemaPDV/` que continha duplicatas
- Documentação espalhada

### 📂 Nova estrutura de pastas

```
📂 CozinhaConectada/
├── 📂 API/                    # API REST do sistema
├── 📂 Core/                   # Lógica de negócio e DTOs
├── 📂 Infrastructure/         # Acesso a dados e repositórios
├── 📂 Web/                    # Interface web Blazor
├── 📂 scripts/                # Scripts SQL organizados
│   ├── criar-banco-sqlserver.sql
│   ├── criar-banco.bat
│   ├── criar-tabelas.sql
│   └── limpar-dados-teste.sql
├── 📂 tools/                  # Ferramentas e scripts PowerShell
│   ├── criar-dados-exemplo.ps1
│   ├── executar-simples.ps1
│   ├── executar-sistema.ps1
│   ├── executar.ps1
│   ├── gerenciador-dados.html
│   └── test-api.ps1
├── 📂 docs/                   # Documentação completa
│   ├── GUIA_HEIDISQL.md
│   ├── INSTRUCOES_EXECUCAO.md
│   └── README-PDV.md
├── 🔧 executar-projeto.ps1    # Script principal com menu
├── 🔧 executar-sqlserver.ps1  # Script específico SQL Server
├── 📄 README.md              # Documentação principal
└── 📄 SistemaPDV.sln         # Solution do Visual Studio
```

## 🚀 Como usar agora

### Script Principal
```powershell
.\executar-projeto.ps1
```
- Menu interativo com todas as opções
- Execução de componentes específicos
- Gerenciamento de banco de dados

### Scripts Específicos
```powershell
.\executar-projeto.ps1 -Acao api        # Apenas API
.\executar-projeto.ps1 -Acao web        # Apenas Web
.\executar-projeto.ps1 -Acao sqlserver  # Com SQL Server
```

## 🎯 Benefícios da reorganização

1. **📁 Estrutura clara:** Cada tipo de arquivo em sua pasta apropriada
2. **🔄 Sem duplicatas:** Eliminação de arquivos duplicados
3. **📚 Documentação centralizada:** Tudo na pasta `docs/`
4. **🛠️ Ferramentas organizadas:** Scripts úteis na pasta `tools/`
5. **💾 Scripts SQL centralizados:** Todos em `scripts/`
6. **🎮 Menu único:** Script principal com todas as opções

## 📋 Próximos passos recomendados

1. **Testar o sistema:** Execute `.\executar-projeto.ps1` para verificar se tudo funciona
2. **Atualizar .gitignore:** Considere usar o `.gitignore-new` criado
3. **Revisar documentação:** Verifique se os caminhos nos documentos estão corretos
4. **Configurar CI/CD:** Com a estrutura organizada, fica mais fácil automatizar

---
✅ Projeto reorganizado com sucesso!
