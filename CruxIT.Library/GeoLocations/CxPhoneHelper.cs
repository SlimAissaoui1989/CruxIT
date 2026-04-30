using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.GeoLocations
{
    public static class CxPhoneHelper
    {
        private static PhoneNumberUtil? _phoneNumberUtil;

        private static PhoneNumberUtil PhoneNumberUtil
        {
            get
            {
                if (_phoneNumberUtil == null)
                    _phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                return _phoneNumberUtil;
            }
        }

        public static string GetPhoneNumberNationalFormat(string phoneNumber, string? regionCode = "")
        {
            PhoneNumber number = PhoneNumberUtil.Parse(phoneNumber, regionCode);
            return PhoneNumberUtil.Format(number, PhoneNumberFormat.NATIONAL);
        }

        public static string GetPhoneNumberInterNationalFormat(string phoneNumber, string? regionCode = "")
        {
            PhoneNumber number = PhoneNumberUtil.Parse(phoneNumber, regionCode);
            return PhoneNumberUtil.Format(number, PhoneNumberFormat.INTERNATIONAL);
        }

        public static string GetPhoneNumber_RFC_3966_Format(string phoneNumber, string? regionCode = "")
        {
            PhoneNumber number = PhoneNumberUtil.Parse(phoneNumber, regionCode);
            return PhoneNumberUtil.Format(number, PhoneNumberFormat.E164);
        }

        public static string GetPhoneNumber_E_146_Format(string phoneNumber, string? regionCode = "")
        {
            PhoneNumber number = PhoneNumberUtil.Parse(phoneNumber, regionCode);
            return PhoneNumberUtil.Format(number, PhoneNumberFormat.RFC3966);
        }
    }
}
