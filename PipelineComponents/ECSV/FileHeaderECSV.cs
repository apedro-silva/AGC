using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Expand.Switch.SwitchServices
{
    public class FileHeaderECSV
    {
        private int length;
        private string tipreg = "0";
        private string aplic = "M";
        private string fich = new string(' ', 4);
        private string verifich = "00";
        private string codban = "0045";
        private string cpd = "1";
        private string idfich = new string(' ', 11);
        private string idfichant = new string(' ', 11);
        private string datavalor = new string(' ', 8);
        private string codmodeda = "024";

        public int Length
        {
            get { return length; }
        }

        public string FICH
        {
            get { return fich; }
            set { fich = value; }
        }

        public string VERIFICH
        {
            get { return verifich; }
            set { verifich = value; }
        }

        public string IDFICH
        {
            get { return idfich; }
            set { idfich = value; }
        }

        public string IDFICHANT
        {
            get { return idfichant; }
            set { idfichant = value; }
        }

        public string DATAVALOR
        {
            get { return datavalor; }
            set { datavalor = value; }
        }

        public FileHeaderECSV(int length)
        {
            this.length = length;
        }

        public char[] GetHeader()
        {
            char[] buffer = new char[length];

            tipreg.ToCharArray().CopyTo(buffer, 0);
            aplic.ToCharArray().CopyTo(buffer, 1);
            fich.ToCharArray().CopyTo(buffer, 2);
            verifich.ToCharArray().CopyTo(buffer, 6);
            codban.ToCharArray().CopyTo(buffer, 8);
            cpd.ToCharArray().CopyTo(buffer, 12);
            idfich.ToCharArray().CopyTo(buffer, 13);
            idfichant.ToCharArray().CopyTo(buffer, 24);
            datavalor.ToCharArray().CopyTo(buffer, 35);
            codmodeda.ToCharArray().CopyTo(buffer, 43);
            
            return buffer;
        }
    }
}
