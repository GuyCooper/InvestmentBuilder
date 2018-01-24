﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderService
{
    internal class CashFlowModel
    {
        public IEnumerable<ReceiptTransaction> Receipts { get; set; }
        public IEnumerable<PaymentTransaction> Payments { get; set; }
        public double ReceiptsTotal { get; set; }
        public double PaymentsTotal { get; set; }
        public string ValuationDate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanBuild { get; set; }
    }

    internal class CashFlowManager
    {
        private AccountService _accountService;
        private IClientDataInterface _clientData;
        private CashAccountTransactionManager _cashTransactionManager;

        public CashFlowManager(AccountService accountService, IClientDataInterface clientData,
            CashAccountTransactionManager cashTransactionManager)
        {
            _accountService = accountService;
            _clientData = clientData;
            _cashTransactionManager = cashTransactionManager;
        }

        public IEnumerable<CashFlowModel> GetCashFlowModel(UserSession userSession, string sDateFrom)
        {
            var token = _accountService.GetUserAccountToken(userSession, null);
            var dtDateEarliest = string.IsNullOrEmpty(sDateFrom) ? userSession.ValuationDate : DateTime.Parse(sDateFrom);
            var dtDateLatest = userSession.ValuationDate;
            var dtDateNext = dtDateLatest;

            var finished = false;
            while (!finished)
            {
                var dtDateFrom = _clientData.GetPreviousAccountValuationDate(token, dtDateNext);

                double dReceiptTotal, dPaymentTotal;
                var cashFlowModel = new CashFlowModel();
                cashFlowModel.Receipts = _cashTransactionManager.GetReceiptTransactions(token, dtDateNext, dtDateFrom, out dReceiptTotal);
                cashFlowModel.Payments = _cashTransactionManager.GetPaymentTransactions(token, dtDateNext, out dPaymentTotal);
                cashFlowModel.ReceiptsTotal = dReceiptTotal;
                cashFlowModel.PaymentsTotal = dPaymentTotal;
                cashFlowModel.ValuationDate = dtDateNext.ToString("yyyy-MM-dd"); //ISO 8601

                cashFlowModel.CanEdit = dtDateNext == dtDateLatest;
                cashFlowModel.CanBuild = cashFlowModel.CanEdit && cashFlowModel.ReceiptsTotal > 0 && cashFlowModel.ReceiptsTotal == cashFlowModel.PaymentsTotal;

                if (dtDateFrom.HasValue == false)
                {
                    finished = true;
                }
                else
                {
                    dtDateNext = dtDateFrom.Value;
                    if (dtDateFrom <= dtDateEarliest)
                    {
                        finished = true;
                    }
                }

                yield return cashFlowModel;
            }
        }

        public IEnumerable<string> GetReceiptParamTypes()
        {
            return _cashTransactionManager.GetTransactionTypes(_cashTransactionManager.ReceiptMnemomic);
        }

        public IEnumerable<string> GetPaymentParamTypes()
        {
            return _cashTransactionManager.GetTransactionTypes(_cashTransactionManager.PaymentMnemomic);
        }
    }
}
