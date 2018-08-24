using RandomUtils.Log;
/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using vApus.Gui.Properties;

namespace vApus.Gui {
    public partial class AuthorGrid : UserControl {
        #region Fields
        private List<XmlNode> _authors;
        #endregion

        #region Constructors
        public AuthorGrid() {
            InitializeComponent();

            dataGrid.AutoGenerateColumns = false;

            dataGrid.RowTemplate.Height = 60;

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += AuthorGrid_HandleCreated;
        }
        #endregion

        #region Functions
        private void AuthorGrid_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(Resources.Authors);

            _authors = new List<XmlNode>(xmlDocument.FirstChild.ChildNodes.Count);
            foreach (XmlNode authordNode in xmlDocument.FirstChild.ChildNodes)
                _authors.Add(authordNode);

            dataGrid.DataSource = _authors;
            dataGrid.CellPainting += dataGrid_CellPainting;
        }

        private void dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
            if (e.RowIndex >= 0) {
                Graphics g = e.Graphics;
                var authorElement = _authors[e.RowIndex] as XmlElement;

                if (e.ColumnIndex == 0) {
                    if (colImage.Width == colImage.MinimumWidth) {
                        colImage.Width = e.CellBounds.Height;
                        return;
                    }

                    e.Paint(e.ClipBounds, e.PaintParts);

                    try {
                        var image = Resources.ResourceManager.GetObject(authorElement.GetAttribute("name").Replace(' ', '_')) as Image;

                        int x = e.CellBounds.Left + e.CellStyle.Padding.Left;
                        int y = e.CellBounds.Top + e.CellStyle.Padding.Top;
                        int width = e.CellBounds.Width - (e.CellStyle.Padding.Left + e.CellStyle.Padding.Right);
                        int height = e.CellBounds.Height - (e.CellStyle.Padding.Top + e.CellStyle.Padding.Bottom);

                        g.DrawImage(image, x, y, width, height);
                        e.Handled = true;
                    } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed painting an author image.", ex, new object[] { sender, e });
                    }
                } else if (e.ColumnIndex == 1) {
                    // Draw Merged Cell
                    bool selected = ((e.State & DataGridViewElementStates.Selected) ==
                                     DataGridViewElementStates.Selected);
                    Color fcolor = (selected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor);
                    Color bcolor = (selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor);
                    var font = new Font(e.CellStyle.Font.FontFamily, 9f, FontStyle.Bold);

                    // Get size information
                    Size size = TextRenderer.MeasureText(e.Graphics, authorElement.GetAttribute("name"), font);

                    // Note that this always aligns top, right
                    // Also this should use the ClipBounds but that is not currently working
                    int x = e.CellBounds.Left + e.CellStyle.Padding.Left;
                    int y = e.CellBounds.Top + e.CellStyle.Padding.Top;
                    int width = e.CellBounds.Width - (e.CellStyle.Padding.Left + e.CellStyle.Padding.Right);
                    int height = size.Height + (e.CellStyle.Padding.Top + e.CellStyle.Padding.Bottom);

                    // Draw background
                    g.FillRectangle(new SolidBrush(bcolor), e.CellBounds);

                    // Draw first line
                    TextRenderer.DrawText(e.Graphics,
                                          authorElement.GetAttribute("name") + " (" +
                                          authorElement.GetAttribute("period") + ")", font,
                                          new Rectangle(x, y, width, height), fcolor,
                                          TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);

                    // Use grey for second line if not selected
                    if (!selected)
                        fcolor = Color.Gray;

                    // Reset font and y location
                    font = e.CellStyle.Font;
                    y = y + height + 2;

                    TextRenderer.DrawText(e.Graphics, authorElement.GetAttribute("email"), font,
                                          new Rectangle(x, y, width, height), fcolor,
                                          TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);

                    // Let them know we handled it
                    e.Handled = true;
                } else {
                    DataGridViewCell cell = dataGrid[e.ColumnIndex, e.RowIndex];
                    cell.Tag = "http://www.linkedin.com/in/" + authorElement.GetAttribute("linkedInID");
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e) {
            if (dataGrid.SelectedCells.Count > 0)
                if (dataGrid.SelectedCells[0].ColumnIndex == 0)
                    dataGrid[1, dataGrid.SelectedCells[0].RowIndex].Selected = true;
        }

        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 2) {
                DataGridViewCell cell = dataGrid[e.ColumnIndex, e.RowIndex];
                Process.Start(cell.Tag as string);
            }
        }

        private void dataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 2)
                dataGrid.Cursor = Cursors.Hand;
        }

        private void dataGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            dataGrid.Cursor = Cursors.Default;
        }
        #endregion
    }
}