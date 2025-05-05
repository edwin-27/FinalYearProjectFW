namespace FinalYearProject.Models
{

    public class order
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public string inv_email { get; set; }
        public string order_status { get; set; }
        public DateTime? despatch_date { get; set; }
        public string inv_firstname { get; set; }
        public string inv_surname { get; set; }
        public string tracking_url { get; set; }

        //public order(int id_, string order_id_, string inv_email_, string order_status_, DateTime despatch_date_, string inv_firstname_, string inv_surname_)
        //{
        //    this.id = id_;
        //    this.order_id = order_id_;
        //    this.inv_email = inv_email_;
        //    this.order_status = order_status_;
        //    this.despatch_date = despatch_date_;
        //    this.inv_firstname = inv_firstname_;
        //    this.inv_surname = inv_surname_;
        //}
    }

    public class OrderHandler
    {
        public bool GetOrderStatus()
        {
            bool ret = false;
            string sql;
            sql = "select id, order_id, order_status, despatch_date from order where order_id = @order_id and inv_email = @inv_email";


            return ret;
        }
    }
}
