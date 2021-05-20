using System;
using System.Windows.Forms;

namespace Lommeregner_til_aflevering
{
    //Class lavet af Visul Studio
    public partial class LommeregnerForm : Form
    {
        public LommeregnerForm()
        {
            InitializeComponent();
        }

        private void FrontPage_Load(object sender, EventArgs e)
        {

        }

        //Hvis "Claculate" knappen trykkes på kaldes Calculate()
        private void CalculateButton_Click(object _, EventArgs e)
        {
            Calculate();
        }
        //Skriver resultatet af regnestykket med teksten fra "InputText". Hvis ikke regnestykket var rigtigt skrives fejl, hvis ikke programmet crasher.
        private void Calculate()
        {
            TextBox textBox = this.Controls.Find("InputText", false)[0] as TextBox;
            Label resultText = this.Controls.Find("Result", false)[0] as Label;
            string result = CalculatorParser.ValueAsString(textBox.Text, out bool succes);
            if (succes)
                resultText.Text = result;
            else
                resultText.Text = "Fejl";
        }
        //Hvis der trykkes enter i inputfeltet kaldes Calculate()
        private void InputText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Calculate();
            }
        }
    }
}
