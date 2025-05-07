using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace WebServicesGestionIncapacidades.Core.utilities
{
    public class documentConverterClass
    {
        public string convertImgToPdf(string imgBase64, string rutaGuardado, string nombreDoc)
        {
            //rutaGuardado = "C:\\Users\\uinformatica6.GIGHA\\OneDrive - GIGHA SAS - JIRO SAS\\Escritorio\\documentos";
            //validar y orientar imagen  
            byte[] imgBytes;
            imgBytes = Convert.FromBase64String(imgBase64);

            using (var metaStream = new MemoryStream(imgBytes))
            {
                var info = SixLabors.ImageSharp.Image.Identify(metaStream);
                if (info == null)
                    return ("Formato de imagen no reconocido.");

                // Ejemplo de validación de resolución mínima  
                if (info.Width < 700 || info.Height < 500)
                    return ($"Resolución mínima 700×500 requerida. La imagen es {info.Width}×{info.Height}.");
            }

            string rutaImagen = string.Empty;
            string nombreFile = $"{nombreDoc}";
            using (var image = SixLabors.ImageSharp.Image.Load(imgBytes))
            {
                image.Mutate(x => x.AutoOrient());

                string fileName = $"{nombreFile}.jpg";
                string folder = rutaGuardado;
                Directory.CreateDirectory(folder);
                string outputPath = Path.Combine(folder, fileName);

                // jpg  
                image.SaveAsJpegAsync(outputPath, new JpegEncoder { Quality = 85 });
                rutaImagen = folder + "\\" + fileName;
            }

            //convertir a pdf  
            var document = new PdfDocument();
            var page = document.AddPage();

            using (var imageStream = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
            {
                var xImage = XImage.FromFile(rutaImagen);

                page.Width = xImage.PixelWidth * 72 / xImage.HorizontalResolution;
                page.Height = xImage.PixelHeight * 72 / xImage.VerticalResolution;

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                }

                // Guardar pdf  
                string pdfPath = Path.Combine(rutaGuardado, $"{nombreFile}.pdf");
                document.Save(pdfPath);

                return nombreFile + ".pdf";
            }
        }
    }
}
