namespace Civia.Api.Models;

// Os nomes seguem exatamente os valores do contrato (docs/openapi.yaml), em UPPER_SNAKE_CASE.
// Assim o mesmo texto aparece no JSON, nos parâmetros de query e nas colunas do banco.

public enum TipoDeficiencia { CEGUEIRA, BAIXA_VISAO, SURDEZ, PERDA_AUDITIVA }

public enum CanalComunicacao { AUDIO, VIBRACAO, LEGENDA, VISUAL }

public enum TipoLocal { CASA, TRABALHO, ESTACAO, OUTRO }

public enum TipoSolicitacao { AJUDA, SOS }

public enum StatusSolicitacao { ABERTA, EM_ATENDIMENTO, CONCLUIDA, CANCELADA }

public enum TipoAlerta { OBRA, ELEVADOR_INDISPONIVEL, ALAGAMENTO, MUDANCA_PLATAFORMA, OUTRO }
