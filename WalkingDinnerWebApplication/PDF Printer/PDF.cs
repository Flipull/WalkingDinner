using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
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
            const int cutFontSize = 8;

            XFont fontHeader = new XFont("Arial Narrow", headerFontSize, XFontStyle.Bold);
            XFont fontSubHeader = new XFont("Arial Narrow", subHeaderFontSize, XFontStyle.BoldItalic);
            XFont fontItalic = new XFont("Arial Narrow", normalFontSize, XFontStyle.Italic);
            XFont fontNormal = new XFont("Arial Narrow", normalFontSize, XFontStyle.Regular);
            XFont fontCuttingLine = new XFont("Arial", cutFontSize, XFontStyle.Regular);

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
                helper.CreatePage();

                // Draw Gang#
                XUnit top = helper.GetLinePosition( headerFontSize + 5, headerFontSize );
                helper.Gfx.DrawString($"Gang #{gangNummer}", fontHeader, XBrushes.Black, left, top, XStringFormats.TopLeft);

                foreach (var groep in gang.Groepen)
                {
                    // Do not place Hostname at bottom of page if there is no space for at least one guest
                    top = helper.GetLinePosition(subHeaderFontSize + 3, subHeaderFontSize + 3 + cutFontSize + 2 + normalFontSize + 2 + normalFontSize);
                    // Draw Host name
                    helper.Gfx.DrawString($"Host: {groep.Host.Naam}", fontSubHeader, XBrushes.Black, left, top, XStringFormats.TopLeft);

                    // TODO: Draw cutting line
                    top = helper.GetLinePosition(cutFontSize + 2, cutFontSize);
                    helper.Gfx.DrawString("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - knip hier - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ",
                        fontCuttingLine, XBrushes.Black, left, top, XStringFormats.TopLeft);

                    foreach (var gast in groep.Gasten)
                    {
                        // Do not place duo name at bottom of page if there is no space for destination
                        top = helper.GetLinePosition(normalFontSize + 2, normalFontSize + 2 + normalFontSize);
                        // Draw Duo name
                        helper.Gfx.DrawString($"{gast.Naam}", fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
                        // Draw Duo destination
                        top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                        helper.Gfx.DrawString($"Adres voor gang {gangNummer}: {groep.Host.Straat} {groep.Host.Huisnummer}, {groep.Host.PostCode}, {groep.Host.Stad}", fontItalic, XBrushes.Black, left, top, XStringFormats.TopLeft);

                        // TODO: Draw cutting line
                        top = helper.GetLinePosition(cutFontSize + 2, cutFontSize);
                        helper.Gfx.DrawString("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - knip hier - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ",
                            fontCuttingLine, XBrushes.Black, left, top, XStringFormats.TopLeft);
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

            public void CreatePage()
            {
                Page = _document.AddPage();
                Page.Size = PdfSharp.PageSize.A4;
                Gfx = XGraphics.FromPdfPage(Page);
                _currentPosition = _topPosition;
            }
        }
    }
}