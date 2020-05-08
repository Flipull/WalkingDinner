using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication.PDF_Printer
{
    public static class PDF
    {
        public static PdfDocument Print(EventSchema schema)
        {
            PdfDocument document = new PdfDocument();
            PdfPage topPage = document.AddPage();
            XGraphics gfxTopPage = XGraphics.FromPdfPage(topPage);

            const int headerFontSize = 24;
            const int subHeaderFontSize = 18;
            const int normalFontSize = 12;

            XFont fontHeader = new XFont("Arial Narrow", headerFontSize, XFontStyle.BoldItalic);
            XFont fontSubHeader = new XFont("Arial Narrow", subHeaderFontSize, XFontStyle.BoldItalic);
            XFont fontNormal = new XFont("Arial Narrow", normalFontSize, XFontStyle.Regular);

            gfxTopPage.DrawString($"{schema.Naam}", fontHeader, XBrushes.Black,
                                  new XRect(0, 200, topPage.Width, 0), XStringFormats.Center);
            gfxTopPage.DrawString($"{schema.VerzamelDatum} - {schema.VerzamelAdres}", fontSubHeader, XBrushes.Black,
                                  new XRect(0, 225, topPage.Width, 0), XStringFormats.Center);

            LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(29.7 - 2.5));
            XUnit left = XUnit.FromCentimeter(2.5);

            int gangNummer = 0;

            foreach(var gang in schema.Gangen)
            {
                gangNummer++;
                bool isHeader = true;

                // Print Gang#

                foreach(var groep in gang.Groepen)
                {
                    // Print Host name

                    foreach(var gast in groep.Gasten)
                    {
                        // Print Duos that ate at this host's place + next location for this duo to head to

                        // Do not place duo name at bottom of page if there is no space for destination
                        XUnit top = helper.GetLinePosition(isHeader ? headerFontSize + 5 : normalFontSize + 2, isHeader ? headerFontSize + 5 + normalFontSize : normalFontSize);

                        helper.Gfx.DrawString(isHeader ? "Sed massa libero, semper a nisi nec" : "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                            isHeader ? fontHeader : fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
                    }
                }
            }

            return document;
        }

        public class LayoutHelper
        {
            private readonly PdfDocument _document;
            private readonly XUnit _topPosition;
            private readonly XUnit _bottomMargin;
            private XUnit _currentPosition;

            public LayoutHelper(PdfDocument document, XUnit topPosition, XUnit bottomMargin)
            {
                _document = document;
                _topPosition = topPosition;
                _bottomMargin = bottomMargin;
                // Set a value outside the page - a new page will be created on the first request.
                _currentPosition = bottomMargin + 10000;
            }

            public XUnit GetLinePosition(XUnit requestedHeight)
            {
                return GetLinePosition(requestedHeight, -1f);
            }

            public XUnit GetLinePosition(XUnit requestedHeight, XUnit requiredHeight)
            {
                XUnit required = requiredHeight == -1f ? requestedHeight : requiredHeight;
                if (_currentPosition + required > _bottomMargin)
                    CreatePage();
                XUnit result = _currentPosition;
                _currentPosition += requestedHeight;
                return result;
            }

            public XGraphics Gfx { get; private set; }
            public PdfPage Page { get; private set; }

            void CreatePage()
            {
                Page = _document.AddPage();
                Page.Size = PdfSharp.PageSize.A4;
                Gfx = XGraphics.FromPdfPage(Page);
                _currentPosition = _topPosition;
            }
        }
    }
}