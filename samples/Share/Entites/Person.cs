using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Share.Entites
{
    public class Person : FullAuditedEntity<int>
    {

        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }

    }
}
