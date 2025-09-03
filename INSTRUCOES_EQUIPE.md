# 🚀 INSTRUÇÕES PARA A EQUIPE - SISTEMA PDV v1.0.0

## 📋 **COMO TESTAR O SISTEMA**

### 🔄 **1. Clonar/Atualizar o Repositório**
```bash
# Se ainda não tem o projeto:
git clone https://github.com/gcalexandre1919/CozinhaConectadaPDV.git
cd CozinhaConectadaPDV

# Se já tem, apenas atualize:
git pull origin main
```

### ⚡ **2. Execução Rápida (30 segundos)**
```powershell
# Execute apenas este comando:
.\start-pdv.ps1

# Ou para ver todas as opções:
.\start-pdv.ps1 -Help
```

### 🌐 **3. Acessar o Sistema**
- **Interface Web**: http://localhost:5000
- **API REST**: http://localhost:5001  
- **Swagger API**: http://localhost:5001/swagger

### 🧪 **4. Cenários de Teste Essenciais**

#### ✅ **Teste 1: Login e Autenticação**
1. Acesse http://localhost:5000
2. Clique em "Login" 
3. Use credenciais de teste ou registre novo usuário
4. Verifique se navegação funciona após login

#### ✅ **Teste 2: Gestão de Clientes**
1. Vá em "Clientes" no menu
2. Adicione um novo cliente
3. **TESTE PRINCIPAL**: Clique no botão "Criar Pedido" 
4. Verifique se navega direto para criação de pedido

#### ✅ **Teste 3: Configuração de Impressoras**
1. Acesse "Gestão" → "Impressoras"
2. Cadastre impressoras por área:
   - Nome: "Impressora Cozinha", Área: "Cozinha"
   - Nome: "Impressora Bar", Área: "Bar"
3. Salve as configurações

#### ✅ **Teste 4: Associação Categoria-Impressora** ⭐
1. Acesse "Gestão" → "Categorias"
2. Para cada categoria, associe uma impressora:
   - Bebidas → Impressora Bar
   - Pratos Principais → Impressora Cozinha
3. **FUNCIONALIDADE CHAVE IMPLEMENTADA!**

#### ✅ **Teste 5: Impressão Multi-área Automática** 🎯
1. Vá em "Pedidos" → "Novo Pedido"
2. Selecione um cliente
3. Adicione produtos de categorias diferentes
4. **TESTE CRÍTICO**: Verifique se ao adicionar item:
   - Sistema imprime automaticamente na área correta
   - Bebidas vão para impressora do bar
   - Pratos vão para impressora da cozinha

#### ✅ **Teste 6: Fluxo Completo**
1. Cliente → Criar Pedido → Adicionar Produtos → Impressão Automática
2. Verifique se todo o workflow funciona integrado

### 🐛 **5. Reportar Problemas**

#### 📝 **Se encontrar bugs:**
```markdown
**Título**: [DESCRIÇÃO CURTA DO PROBLEMA]

**Passos para reproduzir:**
1. Primeiro passo
2. Segundo passo 
3. Terceiro passo

**Resultado esperado:**
O que deveria acontecer

**Resultado atual:**
O que realmente acontece

**Screenshots/Logs:**
[Cole aqui se possível]

**Ambiente:**
- OS: Windows/Linux/Mac
- Browser: Chrome/Firefox/Edge
- .NET Version: [execute `dotnet --version`]
```

### 📊 **6. Funcionalidades para Validar**

#### ✅ **Core Features (Obrigatório testar)**
- [ ] Login/Logout funciona
- [ ] CRUD de Clientes completo
- [ ] CRUD de Produtos e Categorias
- [ ] **Associação Categoria → Impressora** ⭐
- [ ] **Impressão automática por área** ⭐
- [ ] Criação e gestão de pedidos
- [ ] Botão "Criar Pedido" em clientes

#### ✅ **Advanced Features (Testar se possível)**
- [ ] Relatórios de vendas
- [ ] Busca e filtros
- [ ] Upload de imagens de produtos
- [ ] API endpoints no Swagger

### 🔧 **7. Troubleshooting**

#### ❌ **Problema: Script não executa**
```powershell
# Solução:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\start-pdv.ps1
```

#### ❌ **Problema: Porta em uso**
```powershell
# Solução:
.\start-pdv.ps1 -Clean
```

#### ❌ **Problema: Build falha**
```powershell
# Solução:
dotnet --version  # Verificar se é .NET 8
dotnet clean SistemaPDV.sln
dotnet build SistemaPDV.sln
```

### 📞 **8. Contato e Feedback**

#### 💬 **Para discussões técnicas:**
- Abra Issues no GitHub
- Use labels: `bug`, `enhancement`, `question`

#### 🎯 **Focar nos testes:**
1. **Impressão multi-área** (funcionalidade principal)
2. **Workflow cliente → pedido** (otimização implementada)
3. **Interface geral** (usabilidade)

---

## 🎉 **VERSÃO ATUAL: v1.0.0**

### ✨ **Status: 100% FUNCIONAL**
- Todas as funcionalidades principais implementadas
- Sistema pronto para produção
- Arquitetura limpa e organizada

### 🚀 **Próximos Passos:**
1. **Testes da equipe** (esta etapa)
2. **Correções de bugs** (se encontrados)
3. **Melhorias de UX** (baseado no feedback)
4. **Deploy em produção**

**Boa sorte nos testes! 🧪✨**
