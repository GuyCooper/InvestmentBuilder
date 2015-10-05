using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderWeb.Models;

namespace InvestmentBuilderWeb.Services
{
    public class InvestmentRecordBuilderService
    {
        private List<InvestmentRecordModel> _records;

        public InvestmentRecordBuilderService()
        {
            _records = new List<InvestmentRecordModel>
            {
                new InvestmentRecordModel("dealhub", 254, 14.54),
                new InvestmentRecordModel("blobby inc", 26, 34.87),
                new InvestmentRecordModel("argyll investments", 95, 165.23)
            };
        }

        public IEnumerable<InvestmentRecordModel> GetRecords()
        {
            return _records;
        }

        public void AddRecord(InvestmentRecordModel record)
        {
            if (record != null)
            {
                _records.Add(record);
            }
        }

        public void DeleteRecord(InvestmentRecordModel record)
        {
            if (record != null)
            {
                _records = _records.Where(x => x.Name != record.Name).ToList();
            }
        }
    }
}