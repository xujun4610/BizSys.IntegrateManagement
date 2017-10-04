using BizSys.IntegrateManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.UServiceTest
{
    public abstract class BaseUnitTest
    {
        private Criteria _cri;

        public Criteria cri
        {
            get
            {
                if (_cri == null)
                {
                    return new Criteria()
                    {
                        __type = "Criteria",
                        ResultCount = 30,
                        isDbFieldName = false,
                        BusinessObjectCode = null,
                        Conditions = new List<Conditions>()
                        {
                      //       new Conditions(){
                      //  Alias = "ItemCode",
                      //  CondVal = "a00001",
                      //  Operation = "co_EQUAL"
                      //}
                        },
                        Sorts = new List<Sorts>(){
                                    //new Sorts(){
                                    //     __type="Sort",
                                    //     Alias="DocEntry",
                                    //     SortType="st_Ascending"
                                    //}
                            },
                        ChildCriterias = new List<ChildCriterias>()
                        {

                        },
                        NotLoadedChildren = false,
                        Remarks = null
                    };
                }
                else
                {
                    return _cri;
                }
            }
            set
            {
                _cri = value;
            }
        }
    }
}
