using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EnrollRequest
    {
        [Key, Required]
        public Guid RequestId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string School { get; set; }

        [Required]
        public DateTime RegisterTime { get; set; }

        [Required]
        public string SelfIntroduction { get; set; }

        [Required]
        public byte[] IPAddressData { get; set; }

        private IPAddress _IPAddress;
        [NotMapped]
        public IPAddress IPAddress
        {
            get
            {
                if (_IPAddress == null)
                    _IPAddress = new IPAddress(IPAddressData);
                return _IPAddress;
            }
            set
            {
                IPAddressData = value.GetAddressBytes();
                _IPAddress = value;
            }
        }

    }
}