using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zulassungssoftware_Webservice.helper
{
    public class ResultMessages
    {
        private string sucessed;
        private string error;
        private RegistrationNeuzulassung  registration;
        private ReadmissionWiederzulassung readmission;
        private DeregistrationAbmeldung deregistration;
        private RemarkUmkennzeichnung remark;

        public RemarkUmkennzeichnung Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        public DeregistrationAbmeldung Deregistration
        {
            get { return deregistration; }
            set { deregistration = value; }
        }
        public RegistrationNeuzulassung Registration
        {
            get { return registration; }
            set { registration = value; }
        }
        public ReadmissionWiederzulassung Readmission
        {
            get { return readmission; }
            set { readmission = value; }
        }
        public string Sucessed
        {
            get { return sucessed; }
            set { sucessed = value; }
        }
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

    }
}