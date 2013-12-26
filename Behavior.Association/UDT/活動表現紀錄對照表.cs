using FISCA.UDT;

namespace JHSchool.Association.UDT
{
    [TableName("活動表現紀錄對照表")]
    public class 活動表現紀錄對照表 : FISCA.UDT.ActiveRecord
    {
        [Field(Field="細項")]
        public string 細項 { get; set; }
        [Field(Field = "類別")]
        public string 類別 { get; set;}
        [Field(Field = "數量")]
        public int 數量 { get; set;}
    }
}