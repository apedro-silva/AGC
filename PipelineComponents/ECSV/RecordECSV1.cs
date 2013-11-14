using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Expand.Switch.SwitchServices
{
    class RecordECSV1
    {
        private int length;
        private string tipreg = "1";
        private string conta = new string('0', 15);
        private string sinal1 = new string(' ', 1);
        private string svesp = new string('0', 13);
        private string dtscont = new string(' ', 8);
        private string sinal2 = new string(' ', 1);
        private string scont = new string(' ', 13);
        
        public int Length
        {
            get { return length; }
        }

        public string CONTA
        {
            get { return conta; }
            set { conta = value; }
        }

        public string SINAL1
        {
            get { return sinal1; }
            set { sinal1 = value; }
        }

        public string SVESP
        {
            get { return svesp; }
            set { svesp = value; }
        }

        public string DTSCONT
        {
            get { return dtscont; }
            set { dtscont = value; }
        }

        public string SINAL2
        {
            get { return sinal2; }
            set { sinal2 = value; }
        }

        public string SCONT
        {
            get { return scont; }
            set { scont = value; }
        }

        public RecordECSV1(int length)
        {
            this.length = length;
        }

        public char[] GetECSVRecord1()
        {
            char[] buffer = new char[length];

            tipreg.ToCharArray().CopyTo(buffer, 0);
            conta.ToCharArray().CopyTo(buffer, 1);
            sinal1.ToCharArray().CopyTo(buffer, 16);
            svesp.ToCharArray().CopyTo(buffer, 17);
            dtscont.ToCharArray().CopyTo(buffer, 30);
            sinal2.ToCharArray().CopyTo(buffer, 38);
            scont.ToCharArray().CopyTo(buffer, 39);
            
            return buffer;
        }
    }
}
