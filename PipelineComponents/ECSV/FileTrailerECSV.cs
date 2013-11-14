using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Expand.Switch.SwitchServices
{
    public class FileTrailerECSV
    {
        private int length;
        private string tipreg;
        private string totreg;
        private string totdeb;
        private string totcred;

        public int Length
        {
            get { return length; }
        }

        public string TOTREG
        {
            get { return totreg; }
            set { totreg = value; }
        }

        public string TOTDEB
        {
            get { return totdeb; }
            set { totdeb = value; }
        }

        public string TOTCRED
        {
            get { return totcred; }
            set { totcred = value; }
        }

        public FileTrailerECSV(int length)
        {
            this.length = length;
            tipreg = "9";
            totreg = new string('0', 8);
            totdeb = new string('0', 16);
            totcred = new string('0', 16);
        }

        public char[] GetTrailer()
        {
            char[] buffer = new char[length];

            tipreg.ToCharArray().CopyTo(buffer, 0);
            totreg.ToCharArray().CopyTo(buffer, 1);
            totdeb.ToCharArray().CopyTo(buffer, 9);
            totcred.ToCharArray().CopyTo(buffer, 25);

            return buffer;
        }
    }
}
