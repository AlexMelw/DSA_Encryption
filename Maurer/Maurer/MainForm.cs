namespace Maurer
{
    //https://code.msdn.microsoft.com/windowsapps/Maurers-MaurerAlgorithm-for-3422d1b2
    /*
     This applications uses Maurer's algorithm for provable prime generation to create true primes of a given bit-length.
     This is an implementation of 4.62 MaurerAlgorithm found in the Handbook of Applied Cryptography by Alfred Menezes among others.
    */

    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        #region CONSTRUCTORS

        public MainForm()
        {
            InitializeComponent();
            textBox1.Text =
                "Article: https://code.msdn.microsoft.com/windowsapps/Maurers-MaurerAlgorithm-for-3422d1b2" +
                Environment.NewLine;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int keySize = int.Parse(textBox2.Text);

                if (keySize < 7)
                    throw new ArgumentException("k is too small must be >= 7");

                int seed = int.Parse(textBox3.Text);
                MaurerAlgorithm algo = MaurerAlgorithm.Instance;
                BigInteger n = algo.ProvablePrime(keySize);

                textBox1.Text =
                    "Article: https://code.msdn.microsoft.com/windowsapps/Maurers-MaurerAlgorithm-for-3422d1b2" +
                    Environment.NewLine;
                textBox1.Text += n + "\r\n";
                textBox1.Text += Math.Round(BigInteger.Log10(n)).ToString("F0") + "\r\n";
                textBox1.Text += Math.Round(algo.Log2(n)).ToString("F0") + "\r\n";

                Stack<int> bits = new Stack<int>();

                while (n > 0)
                {
                    bits.Push((int)(n & 1));
                    n >>= 1;
                }

                //for (int i = 0; i < bits.Count; i++)
                //    textBox1.Text += bits[i].ToString();

                foreach (int t in bits)
                    textBox1.Text += t.ToString();

                textBox1.Text += "\t" + bits.Count + "\r\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}