using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SearchTextBoundary
{
    //Copied from http://stackoverflow.com/a/33014401/5894457 and modified 
    public class MyTextExtractionStrategy : LocationTextExtractionStrategy
    {
        private List<MyTextExtractionStrategy.ExtendedTextChunk> m_DocChunks = new List<ExtendedTextChunk>();
        private List<MyTextExtractionStrategy.LineInfo> m_LinesTextInfo = new List<LineInfo>();
        public List<SearchResult> m_SearchResultsList = new List<SearchResult>();
        private String m_SearchText;
        public const float PDF_PX_TO_MM = 0.3528f;
        public float m_PageSizeY;


        public MyTextExtractionStrategy(String sSearchText, float fPageSizeY)
            : base()
        {
            this.m_SearchText = sSearchText;
            this.m_PageSizeY = fPageSizeY;
        }

        private void searchText()
        {
            foreach (LineInfo aLineInfo in m_LinesTextInfo)
            {
                int iIndex = aLineInfo.m_Text.IndexOf(m_SearchText);
                if (iIndex != -1)
                {
                    TextRenderInfo aFirstLetter = aLineInfo.m_LineCharsList.ElementAt(iIndex);
                    TextRenderInfo aLastLetter = aLineInfo.m_LineCharsList.ElementAt(iIndex + m_SearchText.Length);
                    SearchResult aSearchResult = new SearchResult(aFirstLetter, aLastLetter, m_PageSizeY);
                    this.m_SearchResultsList.Add(aSearchResult);
                }
            }
        }

        private void groupChunksbyLine()
        {
            MyTextExtractionStrategy.ExtendedTextChunk textChunk1 = null;
            MyTextExtractionStrategy.LineInfo textInfo = null;
            foreach (MyTextExtractionStrategy.ExtendedTextChunk textChunk2 in this.m_DocChunks)
            {
                if (textChunk1 == null)
                {
                    textInfo = new MyTextExtractionStrategy.LineInfo(textChunk2);
                    this.m_LinesTextInfo.Add(textInfo);
                }
                else if (textChunk2.sameLine(textChunk1))
                {
                    textInfo.appendText(textChunk2);
                }
                else
                {
                    textInfo = new MyTextExtractionStrategy.LineInfo(textChunk2);
                    this.m_LinesTextInfo.Add(textInfo);
                }
                textChunk1 = textChunk2;
            }
        }

        public override string GetResultantText()
        {
            groupChunksbyLine();
            searchText();
            //In this case the return value is not useful
            return "";
        }

        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment baseline = renderInfo.GetBaseline();
            //Create ExtendedChunk
            ExtendedTextChunk aExtendedChunk = new ExtendedTextChunk(renderInfo.GetText(), baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth(), renderInfo.GetCharacterRenderInfos().ToList());
            this.m_DocChunks.Add(aExtendedChunk);
        }

        public class ExtendedTextChunk
        {
            public string m_text;
            private Vector m_startLocation;
            private Vector m_endLocation;
            private Vector m_orientationVector;
            private int m_orientationMagnitude;
            private int m_distPerpendicular;
            private float m_charSpaceWidth;
            public List<TextRenderInfo> m_ChunkChars;


            public ExtendedTextChunk(string txt, Vector startLoc, Vector endLoc, float charSpaceWidth, List<TextRenderInfo> chunkChars)
            {
                this.m_text = txt;
                this.m_startLocation = startLoc;
                this.m_endLocation = endLoc;
                this.m_charSpaceWidth = charSpaceWidth;
                this.m_orientationVector = this.m_endLocation.Subtract(this.m_startLocation).Normalize();
                this.m_orientationMagnitude = (int)(Math.Atan2((double)this.m_orientationVector[1], (double)this.m_orientationVector[0]) * 1000.0);
                this.m_distPerpendicular = (int)this.m_startLocation.Subtract(new Vector(0.0f, 0.0f, 1f)).Cross(this.m_orientationVector)[2];
                this.m_ChunkChars = chunkChars;

            }


            public bool sameLine(MyTextExtractionStrategy.ExtendedTextChunk textChunkToCompare)
            {
                return this.m_orientationMagnitude == textChunkToCompare.m_orientationMagnitude && this.m_distPerpendicular == textChunkToCompare.m_distPerpendicular;
            }


        }

        public class SearchResult
        {
            //public int iPosX;
            //public int iPosY;
            //public int Page;
            public iTextSharp.text.Rectangle rect;

            public SearchResult(TextRenderInfo aFirstCharcter, TextRenderInfo aLastCharcter, float fPageSizeY)
            {
                //Get position of upperLeft coordinate
                //Vector vTopLeft = aFirstCharcter.GetAscentLine().GetStartPoint();
                Vector vbottomLeft = aFirstCharcter.GetDescentLine().GetStartPoint();
                Vector vtopRight = aLastCharcter.GetAscentLine().GetEndPoint();
                //PosX
                //float fPosX = vTopLeft[Vector.I1];

                //PosY
                //float fPosY = vTopLeft[Vector.I2];

                //Transform to mm and get y from top of page
                //iPosX = Convert.ToInt32(fPosX * PDF_PX_TO_MM);
                //iPosY = Convert.ToInt32((fPageSizeY - fPosY) * PDF_PX_TO_MM);

                rect = new iTextSharp.text.Rectangle(vbottomLeft[Vector.I1],
                    vbottomLeft[Vector.I2], vtopRight[Vector.I1], vtopRight[Vector.I2]);

            }
        }

        public class LineInfo
        {
            public string m_Text;
            public List<TextRenderInfo> m_LineCharsList;

            public LineInfo(MyTextExtractionStrategy.ExtendedTextChunk initialTextChunk)
            {
                this.m_Text = initialTextChunk.m_text;
                this.m_LineCharsList = initialTextChunk.m_ChunkChars;
            }

            public void appendText(MyTextExtractionStrategy.ExtendedTextChunk additionalTextChunk)
            {
                m_LineCharsList.AddRange(additionalTextChunk.m_ChunkChars);
                this.m_Text += additionalTextChunk.m_text;
            }
        }
    }
}
