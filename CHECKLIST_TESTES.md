# ✅ CHECKLIST DE TESTES - SISTEMA PDV v1.0.0

## 🎯 **PARA CADA MEMBRO DA EQUIPE**

### 📝 **Informações do Testador**
- **Nome**: ________________________
- **Data do Teste**: ________________
- **OS/Browser**: ___________________
- **Versão .NET**: __________________

---

## 🧪 **TESTES OBRIGATÓRIOS**

### ✅ **1. Instalação e Setup**
- [ ] Clone/pull do repositório funcionou
- [ ] `.\start-pdv.ps1` executou sem erros
- [ ] API carregou em http://localhost:5001
- [ ] Web carregou em http://localhost:5000
- [ ] Swagger disponível em http://localhost:5001/swagger

### ✅ **2. Autenticação**
- [ ] Página de login aparece
- [ ] Consegue registrar novo usuário
- [ ] Consegue fazer login
- [ ] Logout funciona
- [ ] Redirecionamento após login funciona

### ✅ **3. Gestão de Clientes**
- [ ] Lista de clientes carrega
- [ ] Consegue criar novo cliente
- [ ] Consegue editar cliente existente
- [ ] **CRÍTICO**: Botão "Criar Pedido" está presente
- [ ] **CRÍTICO**: Botão "Criar Pedido" navega corretamente

### ✅ **4. Gestão de Impressoras** ⭐
- [ ] Acessa página de impressoras
- [ ] Consegue cadastrar nova impressora
- [ ] Campo "Área" está funcionando
- [ ] Lista de impressoras exibe corretamente

### ✅ **5. Gestão de Categorias** ⭐
- [ ] Acessa página de categorias
- [ ] Consegue criar nova categoria
- [ ] **FUNCIONALIDADE CHAVE**: Dropdown de impressoras aparece
- [ ] **FUNCIONALIDADE CHAVE**: Consegue associar categoria à impressora
- [ ] Associação é salva corretamente

### ✅ **6. Gestão de Produtos**
- [ ] Lista de produtos carrega
- [ ] Consegue criar novo produto
- [ ] Dropdown de categorias funciona
- [ ] Upload de imagem funciona (se implementado)

### ✅ **7. Sistema de Pedidos**
- [ ] Acessa página de novo pedido
- [ ] Dropdown de clientes funciona
- [ ] Consegue adicionar produtos ao pedido
- [ ] Lista de itens do pedido atualiza
- [ ] Cálculo do total funciona

### ✅ **8. Impressão Multi-área** 🎯 **[FUNCIONALIDADE PRINCIPAL]**
- [ ] **PREPARAÇÃO**: Criou pelo menos 2 impressoras (ex: Cozinha, Bar)
- [ ] **PREPARAÇÃO**: Associou categorias às impressoras
- [ ] **TESTE**: Ao adicionar item ao pedido, sistema tenta imprimir
- [ ] **VERIFICAÇÃO**: Logs mostram tentativa de impressão na área correta
- [ ] **VERIFICAÇÃO**: Bebidas vão para impressora do bar
- [ ] **VERIFICAÇÃO**: Comidas vão para impressora da cozinha

### ✅ **9. Relatórios**
- [ ] Página de relatórios carrega
- [ ] Filtros de data funcionam
- [ ] Dados são exibidos corretamente

### ✅ **10. API (Swagger)**
- [ ] Swagger UI carrega
- [ ] Consegue executar endpoints
- [ ] Autenticação na API funciona

---

## 🐛 **BUGS ENCONTRADOS**

### 🔴 **Bug #1**
- **Tela/Função**: _________________________
- **Descrição**: ___________________________
- **Passos para reproduzir**: _______________
- **Severidade**: [ ] Crítico [ ] Alto [ ] Médio [ ] Baixo

### 🔴 **Bug #2**
- **Tela/Função**: _________________________
- **Descrição**: ___________________________
- **Passos para reproduzir**: _______________
- **Severidade**: [ ] Crítico [ ] Alto [ ] Médio [ ] Baixo

### 🔴 **Bug #3**
- **Tela/Função**: _________________________
- **Descrição**: ___________________________
- **Passos para reproduzir**: _______________
- **Severidade**: [ ] Crítico [ ] Alto [ ] Médio [ ] Baixo

---

## 💡 **SUGESTÕES DE MELHORIA**

### ✨ **Interface/UX**
- _____________________________________
- _____________________________________
- _____________________________________

### ⚡ **Performance**
- _____________________________________
- _____________________________________

### 🔧 **Funcionalidades**
- _____________________________________
- _____________________________________

---

## 📊 **RESUMO FINAL**

### 🎯 **Funcionalidades Críticas**
- [ ] **Impressão multi-área funciona**: SIM / NÃO
- [ ] **Workflow cliente → pedido funciona**: SIM / NÃO
- [ ] **Sistema geral está estável**: SIM / NÃO

### 📈 **Nota Geral do Sistema**
- [ ] **Excelente** (9-10) - Pronto para produção
- [ ] **Bom** (7-8) - Pequenos ajustes necessários
- [ ] **Regular** (5-6) - Precisa de melhorias
- [ ] **Ruim** (0-4) - Requer revisão significativa

### 💬 **Comentários Finais**
```
__________________________________________________
__________________________________________________
__________________________________________________
__________________________________________________
```

### ✅ **Status do Teste**
- [ ] **APROVADO** - Sistema pronto para produção
- [ ] **APROVADO COM RESSALVAS** - Pequenos ajustes
- [ ] **REPROVADO** - Precisa de correções importantes

---

**Assinatura do Testador**: ________________________

**Data**: ______________

---

## 📋 **INSTRUÇÕES PARA ENTREGA**

1. **Preencha** todo o checklist
2. **Reporte bugs** via GitHub Issues
3. **Envie** este checklist preenchido para o lead do projeto
4. **Indique** se o sistema está pronto para produção

**Obrigado por ajudar a validar o Sistema PDV! 🚀**
