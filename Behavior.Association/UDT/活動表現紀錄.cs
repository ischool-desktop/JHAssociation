using FISCA.UDT;

namespace JHSchool.Association.UDT
{
    [TableName("活動表現紀錄")]
    public class 活動表現紀錄 : FISCA.UDT.ActiveRecord
    {
        [Field(Field="學生編號",Indexed=true)]
        public int 學生編號 {get ;set;}
        [Field(Field = "細項")]
        public string 細項 { get; set;}
        [Field(Field = "類別")]
        public string 類別 { get; set;}
        [Field(Field = "單位")]
        public string 單位 { get; set;}
        [Field(Field = "學年度")]
        public string 學年度 { get; set;}
        [Field(Field = "學期")]
        public string 學期 { get; set;}
    }
}