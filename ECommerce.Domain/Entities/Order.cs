using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        //public Guid CustomerId { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public string OrderCode { get; set; }

        /// <summary>
        /// 0=Pending, 1=Completed, 2=Failed, 3=Processing, 4=Shipped, 5=Delivered
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Kargo şirketi adı (Yurtiçi Kargo, Aras Kargo vb.)
        /// </summary>
        public string? CargoCompany { get; set; }

        /// <summary>
        /// Kargo takip numarası
        /// </summary>
        public string? TrackingNumber { get; set; }

        public Basket Basket { get; set; }
        //public ICollection<Product> Products { get; set; }
        //public Customer Customer { get; set; }
        public CompletedOrder CompletedOrder { get; set; }
    }
}
