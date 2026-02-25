using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.Components
{
    /// <summary>
    /// Textbox that allows right clicking for quick deletion
    /// </summary>
    internal class QuickTextBox : TextBox
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            CheckAndPerformTrim(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            CheckAndPerformTrim(e);
        }

        /// <summary>
        /// Checks if conditions are correct to initiate trim, and if so begins it
        /// </summary>
        /// <param name="e"></param>
        private void CheckAndPerformTrim(MouseEventArgs e)
        {
            //Check if this is a left click and ALT is held
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Alt) == Keys.Alt)
                PerformTrim(SelectionStart);
        }

        /// <summary>
        /// Trims the words before the specified index
        /// </summary>
        /// <param name="charIndex"></param>
        private void PerformTrim(int charIndex)
        {
            //Capture text
            string text = Text;

            //Abort out if < 0 or text length is empty
            if (charIndex < 0 || text.Length == 0)
                return;

            //Constrain index to be <= length; Equals is intentional to prevent the indexof function from failing
            if (charIndex >= text.Length)
            {
                //Constrain
                charIndex = text.Length;
            } else if (text[charIndex] == ' ')
            {
                //Simply add 1 to also remove the space
                charIndex++;
            } else
            {
                //Find the index of the nearest space
                charIndex = text.LastIndexOf(' ', charIndex);

                //Add 1 to also remove the space
                charIndex++;
            }

            //If the index >= length, clear it
            if (charIndex >= text.Length)
                Text = string.Empty;
            else if (charIndex > 0) // If greater than zero, trim
                Text = text.Substring(charIndex, text.Length - charIndex);
        }
    }
}
