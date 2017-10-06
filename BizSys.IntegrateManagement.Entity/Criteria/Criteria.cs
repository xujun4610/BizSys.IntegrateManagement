using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity
{


    public class Criteria
    {
        public string __type { get; set; }
        public int ResultCount { get; set; }
        public List<Conditions> Conditions { get; set; }
        public bool isDbFieldName { get; set; }
        public string BusinessObjectCode { get; set; }
        public List<Sorts> Sorts { get; set; }
        public List<ChildCriterias> ChildCriterias { get; set; }
        public bool NotLoadedChildren { get; set; }
        public string Remarks { get; set; }
    }

    public class Conditions
    {
        /// <summary>
        /// 条件的字段（属性）名
        /// </summary>
        public string Alias { set; get; }	//The alias name of a database field.	For example, for a WHERE clause CardName = "Joe", the Alias property would be CardName.
        /// <summary>
        /// 几个闭括号“）”
        /// </summary>
        public int BracketCloseNum { set; get; } //The number of closing brackets in the condition.
                                                 /// <summary>
                                                 /// 几个开括号“（”
                                                 /// </summary>
        public int BracketOpenNum { set; get; } //The number of opening brackets in the condition.
                                                /// <summary>
                                                /// 比较的字段（属性）名
                                                /// CardName = CntctPrson
                                                /// CntctPrson 是ComparedAlias
                                                /// </summary>
        public string ComparedAlias { set; get; }   //The alias name of a database field with which to compare the field specified in the Alias property.	For example, for a WHERE clause CardName = CntctPrsn, the Alias property would be CardName and the ComparedAlias property would be CntctPrson.
                                                    /// <summary>
                                                    /// 为其他表的字段时使用，？
                                                    /// </summary>
        public string CompareFields { set; get; }   //Indicates whether the value with which to compare the field specified by the Alias property field is another database field (specified in the CompareFields property) or a constant (specified in the CondVal property).
                                                    /// <summary>
                                                    /// 比较的值，用于between运算
                                                    /// </summary>
        public string CondEndVal { set; get; }	//A second constant for use when the comparison operation is BETWEEN.For example, for a WHERE clause Balance BETWEEN 1000 and 3000, the CondVal property would be 1000 and the CondEndVal property would be 3000.
        /// <summary>
        /// 比较的值
        /// </summary>
        public string CondVal { set; get; }	//The constant with which to compare the field specified in the Alias property.For example, for a WHERE clause CardName = "Joe", the CondVal property would be Joe.
        ///// <summary>
        ///// 比较方法
        ///// </summary>
        public string Operation { set; get; }	//The comparison operation for the condition.	For example, for a WHERE clause CardName = "Joe", the Operation property would be co_EQUAL.
       
        public string Relationship { get; set; }
        ///// <summary>
        ///// 和后续条件关系
        ///// </summary>
        //ConditionRelationship Relationship { set; get; }	//Indicates the logical relationship of the current condition to the following condition in the collection.
        public bool UseResult { set; get; }	//For future use.     
        /// <summary>
        /// 数字类型的字段
        /// </summary>
        public bool NumericAlias { set; get; }
        /// <summary>
        /// 查询条件备注
        /// </summary>
        public string Remarks { set; get; }
    }


    public class Sorts
    {
        public string __type { get; set; }
        public string Alias { get; set; }
        public string SortType { get; set; }
    }

    public class ChildCriterias
    {
    }
}
