using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FISCA.DSAUtil;

namespace ClubProofReport
{
    class RecordObj
    {
        public String StudentID, SchoolYear, Semester, Content, ClubName, Score, Effort, Text;

        public RecordObj(String StudentID, String SchoolYear, String Semester, String Content)
        {
            this.StudentID = StudentID;
            this.SchoolYear = SchoolYear;
            this.Semester = Semester;
            this.Content = Content;
            setValue();
        }

        private void setValue()
        {
            XmlElement xml = DSXmlHelper.LoadXml(Content);
            foreach (XmlElement elem in xml.SelectNodes("//Item"))
            {
                this.ClubName = elem.GetAttribute("AssociationName");
                this.Score = elem.GetAttribute("Score");
                this.Effort = elem.GetAttribute("Effort");
                this.Text = elem.GetAttribute("Text");
            }
        }

    }
}
