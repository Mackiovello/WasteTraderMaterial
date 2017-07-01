﻿using Simplified.Ring3;
using Starcounter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WasteTrader.Database;
using WasteTrader.Helpers;
using WasteTrader.ViewModels;

namespace WasteTrader.Api
{
    public class PartialHandlers : IHandler
    {
        public static string partialPrefix = "/Waste2Value/partial/";
        private static readonly string SELECT_WASTE_BY_ENTRYTIME =
            $"SELECT w FROM {typeof(Waste)} w ORDER BY w.{nameof(Waste.EntryTime)} DESC";
        private static readonly string SELECT_WASTE = $"SELECT w FROM {typeof(Waste)} w";
        private static readonly string SELECT_SELLWASTE = $"SELECT w FROM {typeof(SellWaste)} w";
        private static readonly string SELECT_BUYWASTE = $"SELECT w FROM {typeof(BuyWaste)} w";

        protected HandlerOptions internalOption = new HandlerOptions { SelfOnly = true };

        public void Register()
        {
            Handle.GET(partialPrefix + "Hitta", () =>
            {
                var page = new FindPage();
                page.WasteEntries.Data = Db.SQL<Waste>(SELECT_WASTE_BY_ENTRYTIME).Where(w => w.Active);
                return page;
            }, internalOption);

            Handle.GET(partialPrefix + "drawer", () =>
            {
                return new Drawer() { Authorized = SystemUser.GetCurrentSystemUser() != null };
            }, internalOption);

            Handle.GET(partialPrefix + "Home", () => new HomePage(), internalOption);

            Handle.GET(partialPrefix + "header", () => new Header(), internalOption);

            Handle.GET(partialPrefix + "Registrera", () => new Json(), internalOption);

            Handle.GET(partialPrefix + "sell", () => new SellPage(), internalOption);

            Handle.GET(partialPrefix + "user/{?}", (string username) => 
            {
                return new UserPage()
                {
                    Data = Client.GetClientFromUsername(username)
                };
            }, internalOption);

            Handle.GET(partialPrefix + "waste/{?}", (string objectId) =>
            {
                Waste waste = Db.FromId<Waste>(objectId);

                return new WastePage() { Data = waste};
            }, internalOption);
        }
    }
}
