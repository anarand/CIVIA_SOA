# Execução automática do roteiro de testes

Data: 2026-11-15 23:05:02 | API: http://localhost:5080/api | Resultado: 45 de 45 com o status esperado.

### 01 Criar usuária Ana
`POST /usuarios` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "Ana Teste", "email": "ana.teste@civia.dev", "tipoDeficiencia": "BAIXA_VISAO"}`
Resposta: `{"id":1,"nome":"Ana Teste","email":"ana.teste@civia.dev","tipoDeficiencia":"BAIXA_VISAO","criadoEm":"2026-11-16T02:05:00.9114726Z"}`

### 02 Criar usuário Bruno
`POST /usuarios` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "Bruno Teste", "email": "bruno.teste@civia.dev", "tipoDeficiencia": "SURDEZ"}`
Resposta: `{"id":2,"nome":"Bruno Teste","email":"bruno.teste@civia.dev","tipoDeficiencia":"SURDEZ","criadoEm":"2026-11-16T02:05:01.1101642Z"}`

### 03 Email já cadastrado
`POST /usuarios` | esperado **409** | obtido **409** | OK
Requisição: `{"nome": "Ana Outra", "email": "ANA.TESTE@civia.dev", "tipoDeficiencia": "CEGUEIRA"}`
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.10","title":"Conflito com o estado atual","status":409,"detail":"Já existe um usuário com o email ana.teste@civia.dev.","traceId":"00-1638fc1a640f5f09cb548cd29af18b43-387b3b8473dbbede-00"}`

### 04 Dados inválidos
`POST /usuarios` | esperado **400** | obtido **400** | OK
Requisição: `{"nome": "A", "email": "email-invalido", "tipoDeficiencia": "DALTONISMO"}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios","errors":{"tipoDeficiencia":["Valor inválido ou em formato incorreto."]}}`

### 05 Listar usuários
`GET /usuarios` | esperado **200** | obtido **200** | OK
Resposta: `[{"id":1,"nome":"Ana Teste","email":"ana.teste@civia.dev","tipoDeficiencia":"BAIXA_VISAO","criadoEm":"2026-11-16T02:05:00.9114726Z"},{"id":2,"nome":"Bruno Teste","email":"bruno.teste@civia.dev","tipoDeficiencia":"SURDEZ","criadoEm":"2026-11-16T02:05:01.1101642Z"}]`

### 06 Consultar usuária
`GET /usuarios/1` | esperado **200** | obtido **200** | OK
Resposta: `{"id":1,"nome":"Ana Teste","email":"ana.teste@civia.dev","tipoDeficiencia":"BAIXA_VISAO","criadoEm":"2026-11-16T02:05:00.9114726Z"}`

### 07 Usuário inexistente
`GET /usuarios/999` | esperado **404** | obtido **404** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Recurso não encontrado","status":404,"detail":"Usuário 999 não encontrado.","traceId":"00-f061419b9427d65bc16114f64005930c-1891d1660ed00b14-00"}`

### 08 Atualizar usuário
`PUT /usuarios/2` | esperado **200** | obtido **200** | OK
Requisição: `{"nome": "Bruno Teste Silva", "email": "bruno.teste@civia.dev", "tipoDeficiencia": "PERDA_AUDITIVA"}`
Resposta: `{"id":2,"nome":"Bruno Teste Silva","email":"bruno.teste@civia.dev","tipoDeficiencia":"PERDA_AUDITIVA","criadoEm":"2026-11-16T02:05:01.1101642Z"}`

### 09 Preferências ainda não configuradas
`GET /usuarios/1/preferencias` | esperado **404** | obtido **404** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Recurso não encontrado","status":404,"detail":"O usuário 1 ainda não configurou suas preferências.","traceId":"00-dde79a3c53354535c82289a685720603-6a1de84cda8fd1ee-00"}`

### 10 Salvar preferências
`PUT /usuarios/1/preferencias` | esperado **200** | obtido **200** | OK
Requisição: `{"canalPrincipal": "AUDIO", "vibracaoAtiva": true, "altoContraste": false, "priorizarElevador": true}`
Resposta: `{"usuarioId":1,"canalPrincipal":"AUDIO","vibracaoAtiva":true,"altoContraste":false,"priorizarElevador":true,"atualizadoEm":"2026-11-16T02:05:01.3866977Z"}`

### 11 Consultar preferências
`GET /usuarios/1/preferencias` | esperado **200** | obtido **200** | OK
Resposta: `{"usuarioId":1,"canalPrincipal":"AUDIO","vibracaoAtiva":true,"altoContraste":false,"priorizarElevador":true,"atualizadoEm":"2026-11-16T02:05:01.3866977Z"}`

### 12 Preferências incompletas
`PUT /usuarios/1/preferencias` | esperado **400** | obtido **400** | OK
Requisição: `{"canalPrincipal": "AUDIO"}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/preferencias","errors":{"altoContraste":["Informe se o alto contraste está ativo."],"vibracaoAtiva":["Informe se a vibração está ativa."],"priorizarElevador":["Informe se o elevador deve ser priorizado."]}}`

### 13 Cadastrar estação
`POST /usuarios/1/locais` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "Estação da Luz", "tipo": "ESTACAO", "endereco": "Praça da Luz, 1 - Luz, São Paulo - SP", "latitude": -23.5345, "longitude": -46.6345}`
Resposta: `{"id":1,"usuarioId":1,"nome":"Estação da Luz","tipo":"ESTACAO","endereco":"Praça da Luz, 1 - Luz, São Paulo - SP","latitude":-23.5345,"longitude":-46.6345,"criadoEm":"2026-11-16T02:05:01.4230717Z"}`

### 14 Cadastrar casa
`POST /usuarios/1/locais` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "Casa", "tipo": "CASA", "endereco": "Rua Fictícia, 100 - São Paulo - SP"}`
Resposta: `{"id":2,"usuarioId":1,"nome":"Casa","tipo":"CASA","endereco":"Rua Fictícia, 100 - São Paulo - SP","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.459014Z"}`

### 15 Coordenada inválida
`POST /usuarios/1/locais` | esperado **400** | obtido **400** | OK
Requisição: `{"nome": "Trabalho", "tipo": "TRABALHO", "endereco": "Av. Paulista, 1000", "latitude": 120}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/locais","errors":{"latitude":["A latitude deve estar entre -90 e 90."]}}`

### 16 Listar locais
`GET /usuarios/1/locais` | esperado **200** | obtido **200** | OK
Resposta: `[{"id":2,"usuarioId":1,"nome":"Casa","tipo":"CASA","endereco":"Rua Fictícia, 100 - São Paulo - SP","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.459014Z"},{"id":1,"usuarioId":1,"nome":"Estação da Luz","tipo":"ESTACAO","endereco":"Praça da Luz, 1 - Luz, São Paulo - SP","latitude":-23.5345,"longitude":-46.6345,"criadoEm":"2026-11-16T02:05:01.4230717Z"}]`

### 17 Atualizar local
`PUT /usuarios/1/locais/1` | esperado **200** | obtido **200** | OK
Requisição: `{"nome": "Estação da Luz (CPTM)", "tipo": "ESTACAO", "endereco": "Praça da Luz, 1 - Luz, São Paulo - SP", "latitude": -23.5345, "longitude": -46.6345}`
Resposta: `{"id":1,"usuarioId":1,"nome":"Estação da Luz (CPTM)","tipo":"ESTACAO","endereco":"Praça da Luz, 1 - Luz, São Paulo - SP","latitude":-23.5345,"longitude":-46.6345,"criadoEm":"2026-11-16T02:05:01.4230717Z"}`

### 18 Local de outro usuário
`GET /usuarios/2/locais/1` | esperado **404** | obtido **404** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Recurso não encontrado","status":404,"detail":"Local 1 não encontrado para o usuário 2.","traceId":"00-7b7f39f0fa0b82b4d08a4b3bc3879139-a69f417b259d344c-00"}`

### 19 Excluir local
`DELETE /usuarios/1/locais/2` | esperado **204** | obtido **204** | OK

### 20 Cadastrar contato ativo
`POST /usuarios/1/contatos` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "Maria Teste", "telefone": "+5511999990000", "relacao": "Irmã", "ativo": true}`
Resposta: `{"id":1,"usuarioId":1,"nome":"Maria Teste","telefone":"+5511999990000","relacao":"Irmã","ativo":true,"criadoEm":"2026-11-16T02:05:01.5860848Z"}`

### 21 Cadastrar contato inativo
`POST /usuarios/1/contatos` | esperado **201** | obtido **201** | OK
Requisição: `{"nome": "João Teste", "telefone": "11988887777", "relacao": "Amigo", "ativo": false}`
Resposta: `{"id":2,"usuarioId":1,"nome":"João Teste","telefone":"11988887777","relacao":"Amigo","ativo":false,"criadoEm":"2026-11-16T02:05:01.613729Z"}`

### 22 Telefone inválido
`POST /usuarios/1/contatos` | esperado **400** | obtido **400** | OK
Requisição: `{"nome": "Carlos Teste", "telefone": "123", "relacao": "Vizinho", "ativo": true}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/contatos","errors":{"telefone":["O telefone deve ter de 10 a 15 dígitos, com DDD, e pode começar com +."]}}`

### 23 Listar contatos
`GET /usuarios/1/contatos` | esperado **200** | obtido **200** | OK
Resposta: `[{"id":2,"usuarioId":1,"nome":"João Teste","telefone":"11988887777","relacao":"Amigo","ativo":false,"criadoEm":"2026-11-16T02:05:01.613729Z"},{"id":1,"usuarioId":1,"nome":"Maria Teste","telefone":"+5511999990000","relacao":"Irmã","ativo":true,"criadoEm":"2026-11-16T02:05:01.5860848Z"}]`

### 24 Atualizar contato
`PUT /usuarios/1/contatos/2` | esperado **200** | obtido **200** | OK
Requisição: `{"nome": "João Teste", "telefone": "11988887777", "relacao": "Amigo de trabalho", "ativo": false}`
Resposta: `{"id":2,"usuarioId":1,"nome":"João Teste","telefone":"11988887777","relacao":"Amigo de trabalho","ativo":false,"criadoEm":"2026-11-16T02:05:01.613729Z"}`

### 25 Pedir ajuda a um contato
`POST /usuarios/1/solicitacoes` | esperado **201** | obtido **201** | OK
Requisição: `{"tipo": "AJUDA", "contatoConfiancaId": 1, "mensagem": "Não encontro a entrada da plataforma 3 na Estação da Luz."}`
Resposta: `{"id":1,"usuarioId":1,"status":"ABERTA","tipo":"AJUDA","contatoConfiancaId":1,"mensagem":"Não encontro a entrada da plataforma 3 na Estação da Luz.","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.6750238Z","atualizadoEm":"2026-11-16T02:05:01.6750238Z"}`

### 26 AJUDA sem contato
`POST /usuarios/1/solicitacoes` | esperado **400** | obtido **400** | OK
Requisição: `{"tipo": "AJUDA"}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/solicitacoes","errors":{"contatoConfiancaId":["Uma solicitação do tipo AJUDA precisa de um contato de confiança."]}}`

### 27 AJUDA para contato inativo
`POST /usuarios/1/solicitacoes` | esperado **400** | obtido **400** | OK
Requisição: `{"tipo": "AJUDA", "contatoConfiancaId": 2}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/solicitacoes","errors":{"contatoConfiancaId":["O contato 2 está inativo e não pode receber solicitações."]}}`

### 28 Acionar SOS
`POST /usuarios/1/solicitacoes` | esperado **201** | obtido **201** | OK
Requisição: `{"tipo": "SOS", "latitude": -23.5345, "longitude": -46.6345}`
Resposta: `{"id":2,"usuarioId":1,"status":"ABERTA","tipo":"SOS","contatoConfiancaId":null,"mensagem":null,"latitude":-23.5345,"longitude":-46.6345,"criadoEm":"2026-11-16T02:05:01.8069044Z","atualizadoEm":"2026-11-16T02:05:01.8069044Z"}`

### 29 SOS sem localização
`POST /usuarios/1/solicitacoes` | esperado **400** | obtido **400** | OK
Requisição: `{"tipo": "SOS"}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/usuarios/1/solicitacoes","errors":{"latitude":["Um SOS precisa da última localização conhecida (latitude e longitude)."],"longitude":["Um SOS precisa da última localização conhecida (latitude e longitude)."]}}`

### 30 Iniciar atendimento
`PATCH /usuarios/1/solicitacoes/1/status` | esperado **200** | obtido **200** | OK
Requisição: `{"status": "EM_ATENDIMENTO"}`
Resposta: `{"id":1,"usuarioId":1,"status":"EM_ATENDIMENTO","tipo":"AJUDA","contatoConfiancaId":1,"mensagem":"Não encontro a entrada da plataforma 3 na Estação da Luz.","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.6750238Z","atualizadoEm":"2026-11-16T02:05:01.89058Z"}`

### 31 Concluir atendimento
`PATCH /usuarios/1/solicitacoes/1/status` | esperado **200** | obtido **200** | OK
Requisição: `{"status": "CONCLUIDA"}`
Resposta: `{"id":1,"usuarioId":1,"status":"CONCLUIDA","tipo":"AJUDA","contatoConfiancaId":1,"mensagem":"Não encontro a entrada da plataforma 3 na Estação da Luz.","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.6750238Z","atualizadoEm":"2026-11-16T02:05:01.8976188Z"}`

### 32 Reabrir solicitação concluída
`PATCH /usuarios/1/solicitacoes/1/status` | esperado **409** | obtido **409** | OK
Requisição: `{"status": "ABERTA"}`
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.10","title":"Conflito com o estado atual","status":409,"detail":"Não é possível mudar o status de CONCLUIDA para ABERTA.","traceId":"00-e9a16c31042dd45592343bdbababdc8e-1d280c250ec8530a-00"}`

### 33 Listar solicitações abertas
`GET /usuarios/1/solicitacoes?status=ABERTA` | esperado **200** | obtido **200** | OK
Resposta: `[{"id":2,"usuarioId":1,"status":"ABERTA","tipo":"SOS","contatoConfiancaId":null,"mensagem":null,"latitude":-23.5345,"longitude":-46.6345,"criadoEm":"2026-11-16T02:05:01.8069044Z","atualizadoEm":"2026-11-16T02:05:01.8069044Z"}]`

### 34 Consultar solicitação
`GET /usuarios/1/solicitacoes/1` | esperado **200** | obtido **200** | OK
Resposta: `{"id":1,"usuarioId":1,"status":"CONCLUIDA","tipo":"AJUDA","contatoConfiancaId":1,"mensagem":"Não encontro a entrada da plataforma 3 na Estação da Luz.","latitude":null,"longitude":null,"criadoEm":"2026-11-16T02:05:01.6750238Z","atualizadoEm":"2026-11-16T02:05:01.8976188Z"}`

### 35 Excluir contato com histórico
`DELETE /usuarios/1/contatos/1` | esperado **409** | obtido **409** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.10","title":"Conflito com o estado atual","status":409,"detail":"O contato 1 possui solicitações registradas. Desative-o com ativo = false.","traceId":"00-d1082eab50bcaa8308eb7ce22fb5b570-cedba5e3c3b56c02-00"}`

### 36 Excluir contato sem histórico
`DELETE /usuarios/1/contatos/2` | esperado **204** | obtido **204** | OK

### 37 Cadastrar mudança de plataforma
`POST /alertas` | esperado **201** | obtido **201** | OK
Requisição: `{"titulo": "Mudança de plataforma na Linha 7", "descricao": "Embarque transferido da plataforma 2 para a plataforma 3 até as 18h.", "tipo": "MUDANCA_PLATAFORMA", "local": "Estação da Luz", "ativo": true}`
Resposta: `{"id":1,"titulo":"Mudança de plataforma na Linha 7","descricao":"Embarque transferido da plataforma 2 para a plataforma 3 até as 18h.","tipo":"MUDANCA_PLATAFORMA","local":"Estação da Luz","ativo":true,"criadoEm":"2026-11-16T02:05:01.9816952Z","atualizadoEm":"2026-11-16T02:05:01.9816952Z"}`

### 38 Cadastrar elevador indisponível
`POST /alertas` | esperado **201** | obtido **201** | OK
Requisição: `{"titulo": "Elevador em manutenção", "descricao": "Elevador do acesso norte indisponível.", "tipo": "ELEVADOR_INDISPONIVEL", "local": "Estação Sé", "ativo": true}`
Resposta: `{"id":2,"titulo":"Elevador em manutenção","descricao":"Elevador do acesso norte indisponível.","tipo":"ELEVADOR_INDISPONIVEL","local":"Estação Sé","ativo":true,"criadoEm":"2026-11-16T02:05:02.0084668Z","atualizadoEm":"2026-11-16T02:05:02.0084668Z"}`

### 39 Alerta inválido
`POST /alertas` | esperado **400** | obtido **400** | OK
Requisição: `{"titulo": "x", "tipo": "OBRA"}`
Resposta: `{"title":"Dados inválidos","status":400,"instance":"/api/alertas","errors":{"ativo":["Informe se o alerta está ativo."],"local":["O local é obrigatório."],"titulo":["O título deve ter entre 3 e 120 caracteres."],"descricao":["A descrição é obrigatória."]}}`

### 40 Filtrar alertas ativos por tipo
`GET /alertas?tipo=MUDANCA_PLATAFORMA&ativo=true` | esperado **200** | obtido **200** | OK
Resposta: `[{"id":1,"titulo":"Mudança de plataforma na Linha 7","descricao":"Embarque transferido da plataforma 2 para a plataforma 3 até as 18h.","tipo":"MUDANCA_PLATAFORMA","local":"Estação da Luz","ativo":true,"criadoEm":"2026-11-16T02:05:01.9816952Z","atualizadoEm":"2026-11-16T02:05:01.9816952Z"}]`

### 41 Encerrar alerta
`PUT /alertas/1` | esperado **200** | obtido **200** | OK
Requisição: `{"titulo": "Mudança de plataforma na Linha 7", "descricao": "Embarque normalizado na plataforma 2.", "tipo": "MUDANCA_PLATAFORMA", "local": "Estação da Luz", "ativo": false}`
Resposta: `{"id":1,"titulo":"Mudança de plataforma na Linha 7","descricao":"Embarque normalizado na plataforma 2.","tipo":"MUDANCA_PLATAFORMA","local":"Estação da Luz","ativo":false,"criadoEm":"2026-11-16T02:05:01.9816952Z","atualizadoEm":"2026-11-16T02:05:02.0428387Z"}`

### 42 Excluir alerta
`DELETE /alertas/2` | esperado **204** | obtido **204** | OK

### 43 Alerta excluído
`GET /alertas/2` | esperado **404** | obtido **404** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Recurso não encontrado","status":404,"detail":"Alerta 2 não encontrado.","traceId":"00-e26c57ffaebc256be99f8fc55b261433-4bc2e8c9f38b1d0e-00"}`

### 44 Excluir usuário Bruno
`DELETE /usuarios/2` | esperado **204** | obtido **204** | OK

### 45 Usuário excluído
`GET /usuarios/2` | esperado **404** | obtido **404** | OK
Resposta: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Recurso não encontrado","status":404,"detail":"Usuário 2 não encontrado.","traceId":"00-49531e201caed08e18b410eb241c7aab-5141c6e737c95ef7-00"}`

