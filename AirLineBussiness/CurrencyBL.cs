using AirLine.Models;
using AirLineData;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AirLineBussiness
{
    public class CurrencyBL
    {
        // Read currency Moke Data 
        public List<CurrencyModel> ReadFile()
        {

            string filePath = HttpContext.Current.Request.MapPath("~/File/Currency.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/catalog/data");

            List<CurrencyModel> currencys = new List<CurrencyModel>();

            foreach (XmlNode node in nodes)
            {
                CurrencyModel currency = new CurrencyModel();

                currency.ID = Convert.ToInt32(node.SelectSingleNode("CurrencyId").InnerText);
                currency.Name = node.SelectSingleNode("Name").InnerText;
                currency.Country = node.SelectSingleNode("Country").InnerText;
                currency.Value = Convert.ToInt32(node.SelectSingleNode("Value").InnerText);

                currencys.Add(currency);
            }

            return currencys;
        }

        public List<CurrencyModel> ReadCurrencyFromDB()
        {
            try { 
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ExchangeRate, CurrencyModel>();
            });


            List<CurrencyModel> currencyModelList = new List<CurrencyModel>();

            using (CurrancyEntities entities = new CurrancyEntities())
            {
               var currencyList = (from customer in entities.ExchangeRates select customer).ToList();

                foreach (var item in currencyList)
                {
                    CurrencyModel model = Mapper.Map<ExchangeRate, CurrencyModel>(item);
                    currencyModelList.Add(model);
                }
            }

            return currencyModelList;
            }
            catch (Exception EX)
            {
                return null;
            }
        }

        public CurrencyModel ReadCurrencyFromDB(int ID)
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<ExchangeRate, CurrencyModel>();
            //});

            CurrencyModel model = new CurrencyModel();
            using (CurrancyEntities entities = new CurrancyEntities())
            {
                var currency = (from customer in entities.ExchangeRates.Where(p => p.ID == ID) select customer).SingleOrDefault();
                model = Mapper.Map<ExchangeRate, CurrencyModel>(currency); 
            }

            return model;
        }
        public ExchangeRate SaveCurrency(CurrencyModel currencyModel)
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<CurrencyModel,ExchangeRate>();
            //});
            ExchangeRate newRate = new ExchangeRate();
            using (CurrancyEntities entities = new CurrancyEntities())
            {
                
                newRate = Mapper.Map<CurrencyModel, ExchangeRate>(currencyModel);
                entities.ExchangeRates.Add(newRate);
                int result = entities.SaveChanges();

                if (result == 1)
                    return newRate;
            }
                return newRate;
        }

        public CurrencyModel UpdateCurrency(CurrencyModel currencyModel)
        {

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CurrencyModel, ExchangeRate>();
            });

            //using (AirLineEntities entities = new AirLineEntities())
            //{
            //    ExchangeRate exchangeRate = (from c in entities.ExchangeRates
            //                         where c.ID == currencyModel.ID
            //                         select c).FirstOrDefault();
            //    exchangeRate = Mapper.Map<CurrencyModel, ExchangeRate>(currencyModel);
            //    entities.Configuration.ValidateOnSaveEnabled = false;

            //    exchangeRate.ID = currencyModel.ID;
            //    entities.Entry(exchangeRate).State = System.Data.Entity.EntityState.Modified;
            //    entities.SaveChanges();

            //    entities.Configuration.ValidateOnSaveEnabled = true;
            //    int result = entities.SaveChanges();
                
            //    if (result == 1)
            //        return currencyModel;
            //}

           // return currencyModel;

            ExchangeRate exchangeRate;
            using (CurrancyEntities entities = new CurrancyEntities())
            {
                exchangeRate = (from c in entities.ExchangeRates where c.ID == currencyModel.ID select c).FirstOrDefault();
            }
            exchangeRate = exchangeRate = Mapper.Map<CurrencyModel, ExchangeRate>(currencyModel);

            using (CurrancyEntities entities = new CurrancyEntities())
            {
                entities.Configuration.ValidateOnSaveEnabled = false;
                entities.Entry(exchangeRate).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();

                entities.Configuration.ValidateOnSaveEnabled = true;
                int result = entities.SaveChanges();

                if (result == 1)
                    return currencyModel;
            }

            return currencyModel;


        }

        public bool DeleteCurrency(CurrencyModel currencyModel)
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<CurrencyModel, ExchangeRate>();
            //});

            using (CurrancyEntities entities = new CurrancyEntities())
            {
                ExchangeRate exchangeRate = new ExchangeRate();
                entities.Configuration.ValidateOnSaveEnabled = false;

                exchangeRate.ID = currencyModel.ID;
                entities.Entry(exchangeRate).State = System.Data.Entity.EntityState.Deleted;
                entities.SaveChanges();

                entities.Configuration.ValidateOnSaveEnabled = true;
                int result = entities.SaveChanges();

                return true;
            }
        }
    }
}
