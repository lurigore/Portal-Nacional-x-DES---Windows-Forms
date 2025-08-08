# Portal Nacional x DES

Os arquivos para instalação do conversor Portal Nacional X DES - Versão Alpha estão disponíveis no P:\Fiscal\Arquivos de Apoio\Conversor Portal Nacional X DES, e em algumas máquinas também será necessário instalar o framework .NET 8.0.

Ao iniciar, selecione a pasta desejada contendo os XML's baixados via Portal Nacional. Certifique-se de que o total de arquivos é coerente com a quantidade de XML e clique em Processar.

Ao término do processamento, o programa abrirá um relatório contendo todas as notas que foram processadas, e também será gerado um novo arquivo .txt na mesma pasta dos XML's com nome de "1 - Arquivo para Importação - DES". Use este arquivo para importar na DES.

Abaixo as regras de validação:

Modelo:
NFS que o prestador é de BH e não é optante pelo MEI será lançada como "5 - NFS-e";
Para os demais casos, será lançado como "28 - NFS-e Nacional".

Campo Situação Especial:

NFS com serviço de Construção Civil (7.02) serão lançadas como "1.3 - Construção Civil";

NFS com serviço de Publicidade e Propaganda (17.06) serão lançadas como "1.5 - Publicidade e Propaganda";

Para os demais casos, a situação especial será "1.1 - Exclusivamente prestação de serviço".

Motivo de não retenção:

NFS sem retenção de ISS em BH e outros municípios serão lançadas como "2.1 - Não retido";

NFS emitidas pela CGC / CGCON serão lançadas como "2.6 - SPL";

NFS emitidas por optante pelo MEI serão lançadas como "2.14 - MEI";

NFS com retenção de ISS para BH serão lançadas como "2.16 - ISSQN Retido".

Local de Incidência / Prestação:

Para lançamento do local de incidência/prestação será sempre considerado o local de incidência destacado na NFS.


Observações Importantes:
O programa somente não processará notas substituídas caso conste na mesma pasta o XML da nota substituta, ao contrário, ela será importada;
O programa não valida notas canceladas. Caso as mesmas esteja na pasta serão processadas normalmente sem aviso.
