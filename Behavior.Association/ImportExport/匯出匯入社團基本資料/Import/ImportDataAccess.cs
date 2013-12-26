using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartSchool.Feature.Class;
using SmartSchool.Feature.Teacher;
using FISCA.DSAUtil;
using JHSchool.Legacy.ImportSupport;
using JHSchool.Feature.Legacy;
using FISCA.Authentication;

namespace JHSchool.Association
{
    internal class ImportDataAccess : IDataAccess
    {
        public XmlElement GetImportFieldList()
        {
            return CallEmptyRequestService("SmartSchool.Course.BulkProcessAssociation.GetImportFieldList");
        }

        public XmlElement GetValidateFieldRule()
        {
            return CallEmptyRequestService("SmartSchool.Course.BulkProcessAssociation.GetValidateFieldRule");
        }

        public XmlElement GetUniqueFieldData()
        {
            return CallEmptyRequestService("SmartSchool.Course.BulkProcessAssociation.GetUniqueFieldData");
        }

        public XmlElement GetShiftCheckList(params string[] fieldNameList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldNameList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Course.BulkProcessAssociation.GetShiftCheckList";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        public void InsertImportData(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Course.BulkProcessAssociation.InsertImportCourse", new DSRequest(data));
        }

        public XmlElement InsertImportDataAssociation(XmlElement data)
        {
            return DSAServices.CallService("SmartSchool.Course.BulkProcessAssociation.InsertImportCourse", new DSRequest(data)).GetContent().BaseElement;
        }

        public void UpdateImportData(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Course.BulkProcessAssociation.UpdateImportCourse", new DSRequest(data));
        }

        public XmlElement UpdateImportDataAssociation(XmlElement data)
        {
            return DSAServices.CallService("SmartSchool.Course.BulkProcessAssociation.UpdateImportCourse", new DSRequest(data)).GetContent().BaseElement;
        }

        public void AddCourseTeachers(XmlElement request)
        {
            EditCourse.AddCourseTeacher(new DSXmlHelper(request));
        }

        public void RemoveCourseTeachers(XmlElement request)
        {
            EditCourse.RemoveCourseTeachers(new DSXmlHelper(request));
        }

        public XmlElement GetCourseTeachers(IEnumerable<string> fieldNameList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldNameList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Course.BulkProcessAssociation.GetCourseTeachers";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        private static XmlElement CallEmptyRequestService(string serviceName)
        {
            string strServiceName = serviceName;
            DSResponse rsp = DSAServices.CallService(serviceName, new DSRequest());

            if (rsp.GetContent() == null)
                throw new Exception("服務未回傳任何欄位資訊。(" + strServiceName + ")");

            return rsp.GetContent().BaseElement;
        }
    }
}
