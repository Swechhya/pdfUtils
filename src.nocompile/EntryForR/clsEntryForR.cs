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


namespace EntryForR
{
    interface IEntryForR
    {
        void HighLightText(String[] searchText);
        //void ExtractImage(String DirPath);
    }
    public class clsEntryForR:IEntryForR
    {
        String m_DirPath;
        String m_PDFName;
        String m_filename;
        //String m_searchText;
        public clsEntryForR()
        {

        }

        public clsEntryForR(String DirPath, String PDFName)//, string searchText)
        {
            m_DirPath = DirPath;
            m_PDFName = PDFName;
            m_filename = Path.Combine(m_DirPath, m_PDFName);
            //m_searchText = searchText;
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
                arr_t = new MyTextExtractionStrategy[r.NumberOfPages, searchText.Length];
                //Create an instance of our strategy
                // var t = new LocationTextExtractionStrategyEx(searchText, 1056f);
                for (int i = 0; i < r.NumberOfPages; i++)
                {
                    for (int j = 0; j < searchText.Length; j++)
                    {
                        arr_t[i, j] = new MyTextExtractionStrategy(searchText[j], 1056f);
                        var ex = PdfTextExtractor.GetTextFromPage(r, i + 1, arr_t[i, j]);
                    }

                }

                //int pageNo = 1;
                //while (pageNo < r.NumberOfPages)
                //{
                //    var t =  arr_t[pageNo - 1]
                //    var ex = PdfTextExtractor.GetTextFromPage(r, pageNo++, t);
                //}

            }

            //Bind a reader and stamper to our test PDF
            PdfReader reader = new PdfReader(m_filename);

            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    //Create a rectangle for the highlight. NOTE: Technically this isn't us(ed but it helps with the quadpoint calculation
                    //iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(60.6755f, 749.172f, 94.0195f, 735.3f);
                    // int count = 0;
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

                                //Create our hightlight
                                PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

                                //Set the color
                                highlight.Color = BaseColor.YELLOW;

                                //Add the annotation
                                stamper.AddAnnotation(highlight, i + 1);
                            }


                        }

                    }

                    //foreach (var t in arr_t) 
                    //{
                    //    count += 1;
                    //    foreach (var p in t.m_SearchResultsList)
                    //    {
                    //        Rectangle rect = p.rect;
                    //        //Create an array of quad points based on that rectangle. NOTE: The order below doesn't appear to match the actual spec but is what Acrobat produces
                    //        float[] quad = { rect.Left, rect.Bottom, rect.Right, rect.Bottom, rect.Left, rect.Top, rect.Right, rect.Top };

                    //        //Create our hightlight
                    //        PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

                    //        //Set the color
                    //        highlight.Color = BaseColor.YELLOW;

                    //        //Add the annotation
                    //        stamper.AddAnnotation(highlight, count);
                    //    }
                    //}


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
                //Create an instance of our strategy
                // var t = new LocationTextExtractionStrategyEx(searchText, 1056f);
                for (int i = 0; i < r.NumberOfPages; i++)
                {
                    //for (int j = 0; j < searchText.Length; j++)
                    //{
                        arr_t[i, 0] = new MyTextExtractionStrategy(searchText, 1056f);
                        var ex = PdfTextExtractor.GetTextFromPage(r, i + 1, arr_t[i, 0]);
                    //}

                }

                //int pageNo = 1;
                //while (pageNo < r.NumberOfPages)
                //{
                //    var t =  arr_t[pageNo - 1]
                //    var ex = PdfTextExtractor.GetTextFromPage(r, pageNo++, t);
                //}

            }

            //Bind a reader and stamper to our test PDF
            PdfReader reader = new PdfReader(m_filename);

            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    //Create a rectangle for the highlight. NOTE: Technically this isn't us(ed but it helps with the quadpoint calculation
                    //iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(60.6755f, 749.172f, 94.0195f, 735.3f);
                    // int count = 0;
                    for (int i = 0; i < Pages; i++)
                    {
                        //for (int j = 0; j < searchText.Length; j++)
                        //{
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


                       // }

                    }

                                  }
            }


        }

        //public void ExtractIamges(String DirPath)
        //{

        //}

    }
}
