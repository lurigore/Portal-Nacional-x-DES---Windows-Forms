using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Portal_Nacional_x_DES
{
    public partial class Form1 : Form
    {
        static string XML_FOLDER = @""; // Pasta onde estão os XMLs
        static string imTomadorGlobal = "";
        static List<string> chaveSubstituida = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();
            registros1.Text = "Total: " + listBox1.Items.Count.ToString();
        }

        private void processButton1_Click(object sender, EventArgs e)
        {
            ProcessarPasta(XML_FOLDER);
        }

        public void ChooseFolder()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Selecione uma pasta";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    XML_FOLDER = folderBrowserDialog.SelectedPath;
                    textBox1.Text = XML_FOLDER;
                    string[] arquivos = Directory.GetFiles(XML_FOLDER);
                    listBox1.Items.Clear();
                    foreach (string arquivo in arquivos)
                    {
                        if (arquivo.EndsWith(".xml"))
                        {
                            listBox1.Items.Add(Path.GetFileName(arquivo));
                        }
                    }
                }
            }
        }

        public void ProcessarPasta(string xmlFolder)
        {
            if (!Directory.Exists(xmlFolder))
            {
                MessageBox.Show("Selecione uma pasta", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var arquivosXml = Directory.GetFiles(xmlFolder, "*.xml");
                if (arquivosXml.Length == 0)
                {
                    MessageBox.Show("Nenhum arquivo .xml encontrado na pasta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var registrosH = new Dictionary<string, string>();
                    var registrosR = new Dictionary<string, string>();
                    var listaRelatorio = new List<Relatorio>();
                    var index = new List<int>();
                    foreach (var path in arquivosXml)
                    {
                        try
                        {
                            var (h, r, cnpj, dados) = GerarRegistros(path);
                            if (!registrosH.ContainsKey(cnpj))
                                registrosH[cnpj] = h;
                            registrosR.Add(Path.GetFileName(path).Substring(0, Path.GetFileName(path).Length -4), r);
                            listaRelatorio.Add(dados);
                            foreach(string chave in chaveSubstituida)
                            {
                                if(chave != "")
                                {
                                    if(registrosR.Keys.ToList().IndexOf(chave) >= 0)
                                    {
                                        index.Add(registrosR.Keys.ToList().IndexOf(chave));
                                    }
                                    if (chave != "" && registrosR.ContainsKey(chave))
                                    {
                                        MessageBox.Show($"Nota {chave} desconsiderada: Nota Substituída",
                                            "Nota Substituída", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        registrosR.Remove(chave);
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao processar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    foreach(int posicao in index)
                    {
                        listaRelatorio.RemoveAt(posicao);
                    }

                    string arquivoSaida = Path.Combine(xmlFolder, "1 - ARQUIVO PARA IMPORTAÇÃO - DES.txt");

                    string pastaLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                    Directory.CreateDirectory(pastaLog);
                    string relatorio = Path.Combine(pastaLog, $"relatorio_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                    using (StreamWriter sw = new StreamWriter(relatorio))
                    {
                        sw.WriteLine("- /// RELATÓRIO DE NOTAS PROCESSADAS /// -");
                        sw.WriteLine($"Tomador: {listaRelatorio.FirstOrDefault()?.tomador} | {listaRelatorio.FirstOrDefault()?.cnpjTomador} \n");
                        sw.WriteLine("Número da Nota | Nome do Emitente | Estado | Município | Valor | Alíquota\n");
                        foreach (var item in listaRelatorio)
                        {
                            sw.WriteLine($"{item.numeroNota} | {item.nomeEmitente} | {item.estado} | {item.municipio} | {item.valor} | {item.aliquota}");
                        }
                        sw.WriteLine($"\nTotal de Notas Processadas: {listaRelatorio.Count}");
                    }
                    Process.Start(new ProcessStartInfo(relatorio) { UseShellExecute = true });
                    File.WriteAllLines(arquivoSaida, registrosH.Values.Concat(registrosR.Values));
                }
            }

        }

        public (string registroH, string registroR, string cnpjTomador, Relatorio dados) GerarRegistros(string xmlPath)
        {
            try
            {

                XDocument doc = XDocument.Load(xmlPath);
                XNamespace nf = "http://www.sped.fazenda.gov.br/nfse";

                var toma = doc.Descendants(nf + "toma").FirstOrDefault();
                var emit = doc.Descendants(nf + "emit").FirstOrDefault();
                var prest = doc.Descendants(nf + "prest").FirstOrDefault();
                var regime = doc.Descendants(nf + "regTrib").FirstOrDefault();
                string opcaoSimples = prest?.Descendants(nf + "opSimpNac").FirstOrDefault()?.Value ?? "";
                var infDPS = doc.Descendants(nf + "infDPS").FirstOrDefault();
                var valores = doc.Descendants(nf + "valores").FirstOrDefault();
                var valorServ = doc.Descendants(nf + "vServPrest").FirstOrDefault();
                var localIncid = doc.Descendants(nf + "cLocIncid").FirstOrDefault();
                var trib = doc.Descendants(nf + "tribMun").FirstOrDefault();
                var serv = doc.Descendants(nf + "cServ").FirstOrDefault();
                var subst = doc.Descendants(nf + "subst").FirstOrDefault();

                //possibilidades
                string[] sociedadeCGC = {
                    "19179789000118",
                    "65165649000108"
                };
                string[] construcao = {
                    "070201",
                    "070202",
                    "070203",
                    "070204",
                    "070205",
                    "070206",
                    "070207",
                    "070208",
                    "070209",
                    "0702010"
                };
                string[] propaganda = {
                    "170601",
                    "170602",
                    "170603",
                    "170604",
                    "170605",
                    "170606",
                    "170607",
                    "170608",
                    "170609",
                    "170610"

                };

                string chaveSubst = subst?.Element(nf + "chSubstda")?.Value ?? "";
                chaveSubstituida.Add(chaveSubst);


                string nomeArquivo = Path.GetFileName(xmlPath);

                string imXml = toma?.Element(nf + "IM")?.Value ?? "";
                if (imTomadorGlobal == "")
                {
                    imTomadorGlobal = imXml;

                }
                string imTomador = imTomadorGlobal;
                if (imTomador == "")
                {
                    MessageBox.Show("Inscrição Municipal do Tomador não encontrada. Por favor, insira manualmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    while (true)
                    {
                        imTomadorGlobal = Interaction.InputBox("Insira a Inscrição Municipal do Tomador (Sem pontuação):", "Inscrição Municipal");

                        if (imTomadorGlobal.Length == 11)
                            break;

                        MessageBox.Show("Inscrição Municipal Inválida.",
                        "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (imXml != "" && imXml != imTomadorGlobal)
                {
                    imTomadorGlobal = imXml;
                }
                showInscrição.Text = "Inscrição Municipal: " + imTomadorGlobal;


                string cnpjTomador = toma?.Element(nf + "CNPJ")?.Value ?? "";
                string xNomeTomador = toma?.Element(nf + "xNome")?.Value ?? "";

                string dataAtual = DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss");
                string versaoSistema = "VERSÃO301 BUILD152";

                string registroH = $"H|{dataAtual}||{versaoSistema}|{imTomadorGlobal}|{cnpjTomador}||{xNomeTomador}|{xNomeTomador}|||0|2|2|2|||2|2|null";

                // === Registro R ===
                string dhEmissao = infDPS?.Element(nf + "dhEmi")?.Value ?? "";
                string dataEmissao = "";
                if (DateTime.TryParse(dhEmissao, null, DateTimeStyles.AdjustToUniversal, out DateTime dt))
                    dataEmissao = dt.ToString("ddMMyyyy");


                string serie = "0";
                string numeroNF = doc.Descendants(nf + "nNFSe").FirstOrDefault()?.Value ?? "";
                string valorTotal = valorServ?.Element(nf + "vServ")?.Value ?? "";

                var tomaEnd = toma?.Descendants(nf + "endNac").FirstOrDefault();
                var prestEnd = emit?.Descendants(nf + "enderNac").FirstOrDefault();

                bool isMei = (opcaoSimples == "2");
                /*
                DES - 
                1 - Simples Nacional
                2 - Não Optante
                3 - MEI

                Portal Nacional - 
                1 - Não Optante
                2 - MEI
                3 - Simples Nacional
                */
                string opcao = opcaoSimples switch
                {
                    "3" => "1",
                    "2" => "3",
                    "1" => "2",
                    _ => "2"
                };
                string modelo = isMei ? "28" : "5";

                string situacaoResponsabilidade = "1";
                string codServ = serv?.Element(nf + "cTribNac")?.Value ?? "";
                foreach (string servico in construcao)
                {
                    situacaoResponsabilidade = (codServ == servico) ? "3" : "1";
                    break;
                }
                foreach (string servico in propaganda)
                {
                    situacaoResponsabilidade = (codServ == servico) ? "5" : "1";
                    break;
                }


                string ufEmitente = prestEnd?.Element(nf + "UF")?.Value ?? "";
                string codMunEmitente = prestEnd?.Element(nf + "cMun")?.Value ?? "";
                string localIncidencia = localIncid?.Value ?? "";

                string aliquotaIss = trib?.Element(nf + "pAliq")?.Value ?? "0.00";
                bool isRetido = aliquotaIss != "0.00";
                string retencao = isRetido ? "1" : "2";

                string motivoNaoRetencao = "1";
                if (regime?.Element(nf + "regEspTrib")?.Value == "6")
                {
                    motivoNaoRetencao = "6";
                }
                else
                {
                    if (isMei)
                    {
                        motivoNaoRetencao = "14";
                    }
                    else if (!isMei && !isRetido)
                    {
                        motivoNaoRetencao = "1";
                        string semPrefixo = numeroNF.Substring(2);
                        numeroNF = "2025" + semPrefixo.TrimStart('0');
                    }
                    else if (isRetido)
                    {
                        motivoNaoRetencao = "16";
                        string semPrefixo = numeroNF.Substring(2);
                        numeroNF = "2025" + semPrefixo.TrimStart('0');
                    }
                    foreach (string cadastro in sociedadeCGC)
                    {
                        if (emit?.Element(nf + "CNPJ")?.Value == cadastro)
                        {
                            motivoNaoRetencao = "6";
                        }
                    }
                }


                var camposR = new List<string>
            {
                "R",
                dataEmissao,
                dataEmissao,
                modelo,
                serie,
                "",
                situacaoResponsabilidade,
                motivoNaoRetencao,
                localIncidencia,
                retencao,
                numeroNF,
                valorTotal,
                valorTotal,
                aliquotaIss,
                opcao,
                "",
                emit?.Element(nf + "CNPJ")?.Value ?? "",
                "",
                emit?.Element(nf + "xNome")?.Value ?? "",
                prestEnd?.Element(nf + "xLgr")?.Value ?? "",
                prestEnd?.Element(nf + "nro")?.Value ?? "",
                "",
                prestEnd?.Element(nf + "xBairro")?.Value ?? "",
                codMunEmitente,
                "1058",
                prestEnd?.Element(nf + "CEP")?.Value ?? "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                localIncidencia,
                localIncidencia,
                "1058",
                "",
                "",
                ""
            };

                string registroR = string.Join("|", camposR);

                var nota = new Relatorio
                {
                    tomador = xNomeTomador,
                    cnpjTomador = cnpjTomador,
                    numeroNota = numeroNF,
                    nomeEmitente = emit?.Element(nf + "xNome")?.Value ?? "",
                    estado = ufEmitente,
                    municipio = codMunEmitente,
                    valor = valorTotal,
                    aliquota = aliquotaIss
                };

                return (registroH, registroR, cnpjTomador, nota);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception($"Erro ao processar {xmlPath}: {ex.Message}");
            }
        }

        public class Relatorio
        {
            public string? tomador { get; set; }
            public string? cnpjTomador { get; set; }
            public string? numeroNota { get; set; }
            public string? nomeEmitente { get; set; }
            public string? estado { get; set; }
            public string? municipio { get; set; }
            public string? valor { get; set; }
            public string? aliquota { get; set; }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void logButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log"));
        }
    }

}

