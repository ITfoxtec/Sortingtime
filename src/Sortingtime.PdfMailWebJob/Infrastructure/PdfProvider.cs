using Sortingtime.Infrastructure;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TuesPechkin;

namespace Sortingtime.PdfMailWebJob.Infrastructure
{
    class PdfProvider
    {
        static IConverter converter =
            new ThreadSafeConverter(
                new PdfToolset(
                    new Win32EmbeddedDeployment(
                        new TempFolderDeployment())));

        public static void CreatePdfAsync(MemoryStream reportPdfStream, MemoryStream pdfRepostHtmlStream, CancellationToken cancellationToken)
        {
            //var htmlReader = new StreamReader(pdfRepostHtmlStream);
            //var html = await htmlReader.ReadToEndAsync();

            //await Task.Factory.StartNewPersistedCulture(() =>
            //{
                var document = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        ProduceOutline = true,
                        //DocumentTitle = "My Website",
                        PaperSize = PaperKind.A4,
                        //PaperSize = new PechkinPaperSiz
                        Margins =
                        {
                            Unit = Unit.Centimeters,
                            Left = 0.3,
                            Right = 0.3,
                            Top = 1.0,
                            Bottom = 1.0,
                        }
                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            WebSettings = new WebSettings
                            {
                                DefaultEncoding = "UTF-8",
                            },

                            //HtmlText = Encoding.UTF8.GetString(html),
                            HtmlText = new StreamReader(pdfRepostHtmlStream).ReadToEnd(),
                            
                            //HeaderSettings = new HeaderSettings{CenterText = "I'm a header!"},
                            FooterSettings = new FooterSettings{ FontSize = 9, RightText = "[page]"} // CenterText = "I'm a footer!",
                        }
                    }
                };

                byte[] result = converter.Convert(document);

                var write = new BinaryWriter(reportPdfStream);
                write.Write(result);
                reportPdfStream.Position = 0;

            //}, cancellationToken);
        }
    }
}
