using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using SearchTextBoundary;
using System.Drawing.Imaging;
using ExtractImage;


namespace EntryForR
{
    public class clsEntryForR
    {
        String m_DirPath;
        String m_PDFName;
        String m_filename;

        public clsEntryForR()
        {

        }

        public clsEntryForR(String DirPath, String PDFName)//, string searchText)
        {
            m_DirPath = DirPath;
            m_PDFName = PDFName;
            m_filename = Path.Combine(m_DirPath, m_PDFName);
        }


        public void HighLightText(String[] searchText)
        {
            //Create a new file from our test file with highlighting
            string highLightFile = Path.Combine(m_DirPath, "Highlighted.pdf");

            MyTextExtractionStrategy[,] arr_t = null;
            int Pages = 0;
            //Parse page 1 of the document above
            using (var r = new PdfReader(m_filename))
            {
                Pages = r.NumberOfPages;
                //Create an array of our strategy
                arr_t = new MyTextExtractionStrategy[r.NumberOfPages, searchText.Length];

                for (int i = 0; i < r.NumberOfPages; i++)
                {
                    for (int j = 0; j < searchText.Length; j++)
                    {
                        arr_t[i, j] = new MyTextExtractionStrategy(searchText[j], 1056f);
                        var ex = PdfTextExtractor.GetTextFromPage(r, i + 1, arr_t[i, j]);
                    }

                }

            }

            //Bind a reader and stamper to our test PDF
            PdfReader reader = new PdfReader(m_filename);

            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    for (int i = 0; i < Pages; i++)
                    {
                        for (int j = 0; j < searchText.Length; j++)
                        {
                            var t = arr_t[i, j];
                            foreach (var p in t.m_SearchResultsList)
                            {
                                Rectangle rect = p.rect;
                                //Create an array of quad points based on that rectangle. NOTE: The order below doesn't appear to match the actual spec but is what Acrobat produces
                                float[] quad = { rect.Left, rect.Bottom, rect.Right, rect.Bottom, rect.Left, rect.Top, rect.Right, rect.Top };

                                //Create our highlight
                                PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

                                //Set the color
                                highlight.Color = BaseColor.YELLOW;

                                //Add the annotation
                                stamper.AddAnnotation(highlight, i + 1);
                            }


                        }

                    }
                }
            }
        }


        public void HighLightText(String searchText)
        {
            //Create a new file from our test file with highlighting
            string highLightFile = Path.Combine(m_DirPath, "Highlighted.pdf");

            MyTextExtractionStrategy[,] arr_t = null;
            int Pages = 0;
            //Parse page 1 of the document above
            using (var r = new PdfReader(m_filename))
            {
                Pages = r.NumberOfPages;
                arr_t = new MyTextExtractionStrategy[r.NumberOfPages, 1];

                for (int i = 0; i < r.NumberOfPages; i++)
                {
                    arr_t[i, 0] = new MyTextExtractionStrategy(searchText, 1056f);
                    var ex = PdfTextExtractor.GetTextFromPage(r, i + 1, arr_t[i, 0]);
                }

            }

            //Bind a reader and stamper to our test PDF
            PdfReader reader = new PdfReader(m_filename);

            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {

                    for (int i = 0; i < Pages; i++)
                    {
                        var t = arr_t[i, 0];
                        foreach (var p in t.m_SearchResultsList)
                        {
                            Rectangle rect = p.rect;
                            //Create an array of quad points based on that rectangle. NOTE: The order below doesn't appear to match the actual spec but is what Acrobat produces
                            float[] quad = { rect.Left, rect.Bottom, rect.Right, rect.Bottom, rect.Left, rect.Top, rect.Right, rect.Top };

                            //Create our hightlight
                            PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

                            //Set the color
                            highlight.Color = BaseColor.YELLOW;

                            //Add the annotation
                            stamper.AddAnnotation(highlight, i + 1);
                        }

                    }

                }
            }


        }

        public int ExtractImages(int[] Page, int ExtractionType)
        {
            var rs = new PdfReader(m_filename);
            PdfDictionary pg;// = rs.GetPageN(15);
            string path = "";
            var imgext = new ImageExtraction();
            int nImages = 0;
            using (var r = new PdfReader(m_filename))
            {
                switch (ExtractionType)
                {
                    case 1://When pages are not specified, extract images from the entire pdf
                           //Find images in all the pages
                        {
                            for (int i = 1; i <= r.NumberOfPages; i++)
                            {
                                pg = rs.GetPageN(i);
                                var images = imgext.GetImagesFromPdf(pg, rs);
                                for (int cnt = 0; cnt < images.Count; cnt++)
                                {
                                    path = Path.Combine(m_DirPath, String.Format(@"{0}_{1}.png", i, cnt + 1));
                                    images[cnt].Save(path);
                                    nImages++;
                                }
                            }
                            break;
                        }


                    case 2: //When the more than 1 page is specified
                        {
                            for (int i = 0; i < Page.Length; i++)
                            {
                                pg = rs.GetPageN(Page[i]);
                                var images = imgext.GetImagesFromPdf(pg, rs);
                                for (int cnt = 0; cnt < images.Count; cnt++)
                                {
                                    path = Path.Combine(m_DirPath, String.Format(@"{0}_{1}.png", Page[i], cnt + 1));
                                    images[cnt].Save(path);
                                    nImages++;
                                }

                            }
                            break;
                        }


                    case 3: //When only 1 page is specified
                        {
                            pg = rs.GetPageN(Page[0]);
                            var images = imgext.GetImagesFromPdf(pg, rs);
                            for (int cnt = 0; cnt < images.Count; cnt++)
                            {
                                path = Path.Combine(m_DirPath, String.Format(@"{0}_{1}.png", Page[0], cnt + 1));
                                images[cnt].Save(path);
                                nImages++;
                            }
                            break;
                        }
                }
            }

            return nImages;

        }


        public int GetNumPages()
        {
            var r = new PdfReader(m_filename);
            return r.NumberOfPages;
        }

    }
}
