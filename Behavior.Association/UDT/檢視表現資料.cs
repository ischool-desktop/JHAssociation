
using FISCA.UDT;
namespace JHSchool.Association.UDT
{
    public partial class 檢視活動表現紀錄 : FISCA.Presentation.Controls.BaseForm
    {
        public 檢視活動表現紀錄()
        {
            InitializeComponent();

            AccessHelper helper = new AccessHelper();

            dataGridViewX1.DataSource = helper.Select<活動表現紀錄>();
        }
    }
}